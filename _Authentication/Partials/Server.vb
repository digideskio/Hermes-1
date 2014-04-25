Imports Hermes.Cryptography.Cipher
Imports System.Collections.Generic
Imports System.Web
Imports W = System.Web.Configuration.WebConfigurationManager

Namespace Authentication

	Public Class Server

		#Region " Private Shared Constants "

			Private Const COOKIE_URL_FORMAT As String = "{0}"

		#End Region

		#Region " Public Shared Constants "

			' -- Cookie Details --
			Public Const COOKIE_CURRENT_VERSION As Int32 = 3
			' --------------------

		#End Region

		#Region " Post Construction "

			Private ReadOnly Property _Log As Boolean
				Get
					If m_Log Is Nothing AndAlso Not String.IsNullOrEmpty(Settings.Log_Path) Then _
						m_Log = New System.IO.StreamWriter(String.Format(Settings.Log_Path, "server", Guid.NewGuid.ToString()), True)
					Return Not String.IsNullOrEmpty(Settings.Log_Path)
				End Get
			End Property

			Private m_Log As System.IO.StreamWriter

			Private Sub Post_Construction()

				If _Log Then

					m_Log.WriteLine("-- Log Started --")
					m_log.WriteLine(String.Format("Server Created: {0}", DateTime.Now.ToString()))
					m_log.WriteLine(String.Format("Configuration Has {0} Providers", Settings.Providers.Length))
					m_log.Flush()

				End If

			End Sub

			Private Sub Close_Log()

				If _Log

					m_log.WriteLine(String.Format("Server Disposed: {0}", DateTime.Now.ToString()))
					m_log.Flush()
					m_Log.Close()

				End If

			End Sub

		#End Region

		#Region " Private Login Processing Functions "

			Private Function Perform_Login( _
				ByVal _request As Request, _
				ByRef _response As Response _
			) As Boolean

				For i As Integer = 0 To Settings.Providers.Length - 1

					If _Log

						m_log.WriteLine(String.Format("Checking Auth with Provider: {0}", Settings.Providers(i).Name))
						m_log.Flush()

					End If

					If Settings.Providers(i).Authenticate(_request, _response) Then
						_response.Provider = Settings.Providers(i).Name
						Return True
					End If

				Next

				Return False

			End Function

			Private Sub Remove_Cookie( _
				Optional ByVal cookie_Id As Guid = Nothing _
			)

				Dim cookie As New HttpCookie(Settings.Cookie_Name)
				cookie.Expires = DateTime.Now.AddYears(-1)
				cookie.Domain = String.Format(COOKIE_URL_FORMAT, Settings.Cookie_Domain)
				HttpContext.Current.Response.Cookies.Remove(Settings.Cookie_Name)
				HttpContext.Current.Response.Cookies.Add(cookie)

				' -- Remove Cookie Hash from File -- '
				If Not cookie_Id = Nothing AndAlso Settings.Check_Hashes Then
					Dim hash_File As String = System.IO.Path.Combine( _
						Settings.Server_HashPath, cookie_Id.ToString("N"))
					If System.IO.File.Exists(hash_File) Then System.IO.File.Delete(hash_File)
				End If
				' -- Remove Cookie Hash from File -- '

			End Sub

			Private Sub Add_Cookie( _
				ByVal logon_Cookie As Cookie, _
				ByVal persist_Login As Boolean _
			)

				Dim cookie As New HttpCookie(Settings.Cookie_Name)
				Dim l_cookie_Value As String = logon_Cookie.Write()
				cookie.Value = Encrypt_Cookie(l_cookie_Value)
				If persist_Login Then cookie.Expires = logon_Cookie.Expires
				cookie.Domain = String.Format(COOKIE_URL_FORMAT, Settings.Cookie_Domain)

				' -- Write Cookie Hash to File -- '
				If Settings.Check_Hashes Then
					Dim cookie_Hash As String = Generate_Hash(l_cookie_Value)
					System.IO.File.WriteAllText(System.IO.Path.Combine( _
						Settings.Server_HashPath, logon_Cookie.Id.ToString("N")), cookie_Hash)
				End If
				' -- Write Cookie Hash to File -- '

				HttpContext.Current.Response.Cookies.Add(cookie)

			End Sub

		#End Region

		#Region " Public Login Processing Methods "

			Public Function CheckCookieDomain( _
				ByVal cookie As HttpCookie _
			) As HttpCookie

				If cookie.Name = Settings.Cookie_Name Then cookie.Domain = String.Format(COOKIE_URL_FORMAT, Settings.Cookie_Domain)
				Return cookie
			
			End Function

			''' <summary></summary>
			''' <param name="username"></summary>
			''' <param name="password"></summary>
			''' <returns></returns>
			Public Function Login( _
				ByVal username As System.String, _
				Byval password As System.String, _
				ByVal Optional persist_Login As System.Boolean = False _
			) As System.Boolean

				If _Log Then m_log.WriteLine(String.Format("Auth Request Made for User: {0}", username))

				Dim _request As New Request(username, password, Settings.Cookie_Domain, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.UserAgent, persist_Login, _
					HttpContext.Current.Request.Url.Host)
				Dim _response As New Response()

				If Perform_Login(_request, _response) Then

					If _Log Then m_log.WriteLine(String.Format("Auth Success for User: {0} Using Provider: {1}", username, _response.Provider))

					Dim date_Expiration As DateTime
					If persist_Login Then
						date_Expiration = DateTime.Now.AddDays(Settings.Cookie_ExpiresInDays)
					Else
						date_Expiration = DateTime.Now()
					End If

					Add_Cookie(New Cookie(Guid.NewGuid(), True, _request.Username, _response.Details, _response.Roles, date_Expiration, _
						COOKIE_CURRENT_VERSION, _response.Provider, _response.Email_Address), persist_Login)

					If _Log Then m_log.Flush()
					Return True

				Else

					If _Log Then

						m_log.WriteLine(String.Format("Auth Failed for User: {0}", username))
						m_log.Flush()
					
					End If

					Return False

				End If

			End Function

			''' <summary></summary>
			''' <param name="username"></summary>
			''' <param name="password"></summary>
			''' <returns></returns>
			Public Function Login( _
				ByVal username As System.String, _
				ByVal password As System.String, _
				ByRef token As System.String, _
				ByVal Optional persist_Login As System.Boolean = False _
			) As System.Boolean

				If _Log Then m_log.WriteLine(String.Format("Auth Request Made for User: {0}", username))

				Dim _request As New Request(username, password, Settings.Cookie_Domain, HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.UserAgent, persist_Login, _
					HttpContext.Current.Request.Url.Host)
				Dim _response As New Response()

				If Perform_Login(_request, _response) Then

					If _Log Then m_log.WriteLine(String.Format("Auth Success for User: {0} Using Provider: {1}", username, _response.Provider))

					Dim date_Expiration As DateTime
					If persist_Login Then
						date_Expiration = DateTime.Now.AddDays(Settings.Cookie_ExpiresInDays)
					Else
						date_Expiration = DateTime.Now()
					End If

					Dim logon_Cookie As New Cookie(Guid.NewGuid(), True, _request.Username, _response.Details, _response.Roles, date_Expiration, _
						COOKIE_CURRENT_VERSION, _response.Provider, _response.Email_Address)
					Dim l_cookie_Value As String = logon_Cookie.Write()
					token = Encrypt_Cookie(l_cookie_Value)

					' -- Write Cookie Hash to File -- '
					If Settings.Check_Hashes Then
						Dim cookie_Hash As String = Generate_Hash(l_cookie_Value)
						System.IO.File.WriteAllText(System.IO.Path.Combine( _
							Settings.Server_HashPath, logon_Cookie.Id.ToString("N")), cookie_Hash)
					End If
					' -- Write Cookie Hash to File -- '

					If _Log Then m_log.Flush()
					Return True

				Else

					If _Log Then

						m_log.WriteLine(String.Format("Auth Failed for User: {0}", username))
						m_log.Flush()
					
					End If

					Return False

				End If

			End Function

			Public Function Logout() As Boolean

				If Not HttpContext.Current.Request.Cookies(Settings.Cookie_Name) Is Nothing Then

					Dim current_Cookie As Cookie = Get_Cookie()

					If Not current_Cookie Is Nothing AndAlso current_Cookie.Authenticated AndAlso Not String.IsNullOrEmpty(current_Cookie.Authentication_Provider) Then

						If _Log Then m_log.WriteLine(String.Format("Logout Request Made for User: {0}", current_Cookie.Username))

						Settings.Provider(current_Cookie.Authentication_Provider).DeAuthenticate(New Request(current_Cookie.Username, String.Empty, Settings.Cookie_Domain, _
							HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.UserAgent, False, HttpContext.Current.Request.Url.Host))

					End If

					Remove_Cookie(current_Cookie.Id)

					If _Log Then m_log.Flush()
					Return True

				Else

					Return False

				End If

			End Function

		#End Region

		#Region " Public Logging Methods "

			Public Function Log( _
				ByVal _event As Hermes.Authentication.Event _
			) As Boolean

				If Not HttpContext.Current.Request.Cookies(Settings.Cookie_Name) Is Nothing Then

					Dim current_Cookie As Cookie = Get_Cookie()

					If Not current_Cookie Is Nothing AndAlso current_Cookie.Authenticated AndAlso Not String.IsNullOrEmpty(current_Cookie.Authentication_Provider) Then

						If _Log Then m_log.WriteLine(String.Format("Log Request Made for User: {0}", current_Cookie.Username))

						Settings.Provider(current_Cookie.Authentication_Provider).Log(New Request(current_Cookie.Username, String.Empty, Settings.Cookie_Domain, _
							HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.UserAgent, False, HttpContext.Current.Request.Url.Host), _event)

					End If

					If _Log Then m_log.Flush()
					Return True

				Else

					Return False

				End If

			End Function

		#End Region

		#Region " Public Status Methods "

			Public Function Status() As String

				Dim return_String As New System.Text.StringBuilder()

				Dim current_Cookie As Cookie = Get_Cookie()

				If Not current_Cookie Is Nothing Then return_String.Append(current_Cookie.Show_Details())

				If Not HttpContext.Current Is Nothing AndAlso Not HttpContext.Current.Session Is Nothing Then

					If return_String.Length > 0 Then return_String.Append(" // ")
					return_String.AppendFormat("Session: {0} // App: {1}", HttpContext.Current.Session("SESSION_STARTED"), HttpContext.Current.Session("APP_STARTED"))

				End If

				Return return_String.ToString()

			End Function

		#End Region

		#Region " Public Cookie Handling Methods "

			Public Function Get_Cookie() As Cookie

				Dim _cookie As HttpCookie = HttpContext.Current.Request.Cookies(Settings.Cookie_Name)

				If Not _cookie Is Nothing Then

					Dim ret_Cookie As Cookie = Cookie.Create(Decrypt_Cookie(_cookie.Value))
					
					If ret_Cookie.Authenticated Then '

						If ret_Cookie.Version < COOKIE_CURRENT_VERSION Then

							Remove_Cookie(ret_Cookie.Id)

						Else

							If Settings.Check_Hashes Then

								' -- Read Cookie Hash from File -- '
								Dim hash_File As String = System.IO.Path.Combine( _
									Settings.Server_HashPath, ret_Cookie.Id.ToString("N"))

								If System.IO.File.Exists(hash_File) Then

									Dim cookie_Hash As String = Generate_Hash(ret_Cookie.Write())

									If String.Compare(System.IO.File.ReadAllText(hash_File), cookie_Hash) = 0 Then _
										Return ret_Cookie

								End If
								' -- Read Cookie Hash from File -- '

							Else

								Return ret_Cookie

							End If

						End If

					Else

						' Seems like Decryption May have failed
						Remove_Cookie()

					End If

				End If

				Return Nothing

			End Function

			Public Function Get_Cookie( _
				ByVal cookie_As_Token As String _
			) As Cookie

				Dim ret_Cookie As Cookie = Cookie.Create(Decrypt_Cookie(cookie_As_Token))
					
				If ret_Cookie.Authenticated AndAlso ret_Cookie.Version = COOKIE_CURRENT_VERSION Then

					If Settings.Check_Hashes Then

						' -- Read Cookie Hash from File -- '
						Dim hash_File As String = System.IO.Path.Combine( _
							Settings.Server_HashPath, ret_Cookie.Id.ToString("N"))

						If System.IO.File.Exists(hash_File) Then

							Dim cookie_Hash As String = Generate_Hash(ret_Cookie.Write())

							If String.Compare(System.IO.File.ReadAllText(hash_File), cookie_Hash) = 0 Then _
								Return ret_Cookie

						End If
						' -- Read Cookie Hash from File -- '

					Else

						Return ret_Cookie

					End If

				End If

				Return Nothing

			End Function

		#End Region

		#Region " Friend Cookie Handling Methods "

			Friend Function Encrypt_Cookie( _
				ByVal value As String _
			) As String

				Return Encrypt(value, Settings.Cookie_EncryptionKey & "_" & _
					Generate_Hash(HttpContext.Current.Request.UserAgent))

			End Function

			Friend Function Decrypt_Cookie( _
				ByVal value As String _
			) As String

				Dim ret_Val As String = Nothing

				Try
					ret_Val = Decrypt(value, Settings.Cookie_EncryptionKey & "_" & _
						Generate_Hash(HttpContext.Current.Request.UserAgent))
				Catch ex As Exception
				End Try

				Return ret_Val

			End Function

		#End Region

		#Region " Public Token Properties "

		#End Region

		#Region " Public Cookie Properties "

			Public ReadOnly Property Authenticated() As System.Boolean
				Get
					Dim current_Cookie As Cookie = Get_Cookie()
					Return Not current_Cookie Is Nothing AndAlso current_Cookie.Authenticated
				End Get
			End Property

			Public ReadOnly Property Authenticated( _
				ByVal _provider As System.String _
			) As System.Boolean
				Get
					Dim current_Cookie As Cookie = Get_Cookie()
					Return Not current_Cookie Is Nothing AndAlso current_Cookie.Authenticated _
						AndAlso String.Compare(current_Cookie.Authentication_Provider, _provider, True) = 0
				End Get
			End Property

			Public ReadOnly Property Roles() As System.String()
				Get
					Dim current_Cookie As Cookie = Get_Cookie()
					If Not current_Cookie Is Nothing AndAlso current_Cookie.Authenticated AndAlso Not current_Cookie.Roles Is Nothing Then
						Return current_Cookie.Roles
					Else
						Return New String() {}
					End If
				End Get
			End Property

			Public ReadOnly Property IsInRole( _
				ByVal role As String _
			) As System.Boolean
				Get
					Dim current_Cookie As Cookie = Get_Cookie()
					If Not current_Cookie Is Nothing AndAlso current_Cookie.Authenticated AndAlso Not current_Cookie.Roles Is Nothing Then
						For i As Integer = 0 To current_Cookie.Roles.Length - 1
							If String.Compare(current_Cookie.Roles(i), role, True) = 0 Then Return True
						Next
					End If
					Return False
				End Get
			End Property

			Public ReadOnly Property IsInAnyRole( _
				ByVal roles As String() _
			) As System.Boolean
				Get
					For i As Integer = 0 To roles.Length - 1
						If IsInRole(roles(i)) Then Return True
					Next
					Return False
				End Get
			End Property

			Public ReadOnly Property Username() As System.String
				Get
					Dim current_Cookie As Cookie = Get_Cookie()
					If Not current_Cookie Is Nothing AndAlso current_Cookie.Authenticated Then
						Return current_Cookie.Username
					Else
						Return String.Empty
					End If
				End Get
			End Property

			Public ReadOnly Property Details() As System.String
				Get
					Dim current_Cookie As Cookie = Get_Cookie()
					If Not current_Cookie Is Nothing AndAlso current_Cookie.Authenticated Then
						Return current_Cookie.Details
					Else
						Return String.Empty
					End If
				End Get
			End Property

			Public ReadOnly Property Expires() As System.DateTime
				Get
					Dim current_Cookie As Cookie = Get_Cookie()
					If Not current_Cookie Is Nothing AndAlso current_Cookie.Authenticated Then
						Return current_Cookie.Expires
					Else
						Return New DateTime
					End If
				End Get
			End Property

			Public ReadOnly Property Email_Address() As System.String
				Get
					Dim current_Cookie As Cookie = Get_Cookie()
					If Not current_Cookie Is Nothing AndAlso current_Cookie.Authenticated Then
						Return current_Cookie.Email_Address
					Else
						Return String.Empty
					End If
				End Get
			End Property

		#End Region

	End Class

End Namespace