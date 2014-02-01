Imports System.Reflection
Imports System.Web
Imports W = System.Web.Configuration.WebConfigurationManager

Namespace Authentication

	Public Class Config

		#Region " Public Shared Constants "

			' -- Configuration Key Names --
			Public Const CONFIGURATION_AUTH_DOMAIN As String = "Hermes.Auth.Domain"

			Public Const CONFIGURATION_AUTH_KEY As String = "Hermes.Auth.Key"

			Public Const CONFIGURATION_AUTH_EXPIRATION As String = "Hermes.Auth.Expiration"

			Public Const CONFIGURATION_AUTH_PROVIDERS As String = "Hermes.Auth.Provider"

			Public Const CONFIGURATION_AUTH_PATH As String = "Hermes.Auth.Providers.Path"

			Public Const CONFIGURATION_LOG_PATH As String = "Hermes.Auth.Log.Path"

			Public Const CONFIGURATION_HASH_PATH As String = "Hermes.Auth.Hash.Path"
			' -----------------------------

		#End Region

		#Region " Post Construction "

			Private m_LogEnabled As Boolean = False

			Private ReadOnly Property _Log As Boolean
				Get
					If m_LogEnabled AndAlso m_Log Is Nothing Then _
						m_Log = New System.IO.StreamWriter(String.Format(Log_Path, "config", Guid.NewGuid.ToString()), True)
					Return m_LogEnabled
				End Get
			End Property

			Private m_Log As System.IO.StreamWriter

			Private Sub Post_Construction()

				If _Log Then

					m_Log.WriteLine("-- Log Started --")
					m_log.WriteLine(String.Format("Config Created: {0}", DateTime.Now.ToString()))
					m_log.Flush()

				End If

			End Sub

			Private Sub Close_Log()

				If _Log Then

					m_log.WriteLine(String.Format("Config Disposed: {0}", DateTime.Now.ToString()))
					m_log.Flush()
					m_Log.Close()

				End If

			End Sub

		#End Region

		#Region " Private Methods "

			Private Function Initialise_Log_Path() As String

				Dim log_Path As String = W.AppSettings(CONFIGURATION_LOG_PATH)

				If Not String.IsNullOrEmpty(log_Path) Then m_LogEnabled = True

				Return log_Path

			End Function

			Private Function Initialise_Cookie_Domain() As String

				If _Log Then m_Log.WriteLine("Starting Cookie Domain Configuration")

				Dim configured_Domain As String = W.AppSettings(CONFIGURATION_AUTH_DOMAIN)

				If _Log Then
					m_log.WriteLine(String.Format("Cookie Domain: {0}", configured_Domain))
					m_log.Flush()
				End If

				' TODO: This doesn't work with .co.uk Domains!
				If String.IsNullOrEmpty(configured_Domain) Then

					If HttpContext.Current Is Nothing Then Throw New Exception() ' TODO: Useful Exception Message Here
					Dim server_Host As String = HttpContext.Current.Request.Url.Host
					Do While ((Not server_Host.IndexOf(".") = server_Host.LastIndexOf(".")) AndAlso _
						(server_Host.IndexOf(".") + 2 < server_Host.SubString(server_Host.IndexOf(".") + 1).IndexOf(".")))
						server_Host = server_Host.SubString(server_Host.IndexOf(".") + 1)
					Loop
					If _Log Then
						m_log.WriteLine(String.Format("Server Host: {0}", server_Host))
						m_log.Flush()
					End If
					Return server_Host
					
				Else
					Return configured_Domain
				End If

			End Function

			Private Function Initialise_Cookie_Name() As String

				Return Cookie_Domain.Replace(".", "_").ToUpper() & "_AUTH"

			End Function

			Private Function Initialise_Cookie_ExpiresInDays() As System.Int32

				If _Log Then m_Log.WriteLine("Starting Cookie Expires Configuration")

				Dim settings_Parsed As System.Int32, settings_UnParsed As String

				settings_UnParsed = W.AppSettings(CONFIGURATION_AUTH_EXPIRATION)

				If _Log Then
					m_log.WriteLine(String.Format("Cookie Expires: {0}", settings_UnParsed))
					m_log.Flush()
				End If

				If Not String.IsNullOrEmpty(settings_UnParsed) AndAlso Int32.TryParse(settings_UnParsed, settings_Parsed) Then Return settings_Parsed Else Return 7

			End Function

			Private Function Initialise_Cookie_EncryptionKey() As System.String

				If _Log Then m_Log.WriteLine("Starting Cookie Key Configuration")

				Dim configured_Key As String = W.AppSettings(CONFIGURATION_AUTH_KEY)

				If _Log Then
					m_log.WriteLine(String.Format("Cookie Key: {0}", configured_Key))
					m_log.Flush()
				End If

				If String.IsNullOrEmpty(configured_Key) Then Throw New Exception() ' TODO: Useful Exception Message Here

				Return configured_Key

			End Function

			Private Function Initialise_Providers_RelativePath() As System.String

				If _Log Then m_Log.WriteLine("Starting Providers Relative Path Configuration")

				Dim relative_Path As String = W.AppSettings(CONFIGURATION_AUTH_PATH)

				If _Log Then
					m_log.WriteLine(String.Format("Relative Path: {0}", relative_Path))
					m_log.Flush()
				End If

				If String.IsNullOrEmpty(relative_Path) Then relative_Path = "bin"

				Return relative_Path
			
			End Function

			Private Function Initialise_AuthenticationProviders() As IAuthenticationProvider()

				If _Log Then m_Log.WriteLine("Starting Providers Configuration")

				Dim all_Providers As New System.Collections.Generic.List(Of IAuthenticationProvider)()

				Dim all_Settings As System.Collections.Specialized.NameValueCollection = W.AppSettings

				If _Log Then m_log.WriteLine(String.Format("Provider Config Count: {0}", all_Settings.Count))

				For Each single_Setting As String In all_Settings.AllKeys

					If single_Setting.ToLower.StartsWith(CONFIGURATION_AUTH_PROVIDERS.ToLower & ".") Then

						Dim provider_Name As String = single_Setting.SubString(CONFIGURATION_AUTH_PROVIDERS.Length + 1)

						If _Log Then m_log.WriteLine(String.Format("Configuring Provider: {0}", provider_Name))

						Dim provider_Details As New List(Of String)(all_Settings(single_Setting).Split(New Char() {"|"c}, StringSplitOptions.RemoveEmptyEntries))

						If provider_Details.Count > 0 Then

							Dim provider_Type As System.Type = Config.Load_Type(provider_Details(0), Providers_RelativePath)

							If Not provider_Type Is Nothing Then

								If _Log Then m_log.WriteLine(String.Format("Got Provider Type: {0}", provider_Type.FullName))

								provider_Details.RemoveAt(0)
								Dim provider_Class As IAuthenticationProvider = CType(Activator.CreateInstance(provider_Type), IAuthenticationProvider)
								provider_Class.Name = provider_Name
								If provider_Class.Configure(provider_Details) Then all_Providers.Add(provider_Class)

							End If

						End If

					End If

				Next

				If _Log Then
					m_log.WriteLine(String.Format("Provider Return Count: {0}", all_Providers.Count))
					m_log.Flush()
				End If
				Return all_Providers.ToArray()

			End Function

			Private Function Initialise_Server_HashPath() As String

				If _Log Then m_Log.WriteLine("Starting Server Hash Path Configuration")

				Dim configured_Path As String = W.AppSettings(CONFIGURATION_HASH_PATH)

				If _Log Then
					If String.IsNullOrEmpty(configured_Path) Then
						m_log.WriteLine("No Hash Path Configured, Disabling Hash Checking")
					Else
						m_log.WriteLine(String.Format("Hash Path: {0}", configured_Path))
					End If
					m_log.Flush()
				End If

				If Not String.IsNullOrEmpty(configured_Path) AndAlso Not System.IO.Directory.Exists(configured_Path) Then Throw New Exception() ' TODO: Useful Exception Message Here

				Return configured_Path

			End Function
		
		#End Region

		#Region " Public Methods "

			Public Function Provider( _
				ByVal provider_Name As String _
			) As IAuthenticationProvider

				Dim ret_Provider As IAuthenticationProvider = Nothing

				For i As Integer = 0 To Providers.Length - 1

					If String.Compare(provider_Name, Providers(i).Name) = 0 Then
						ret_Provider =  Providers(i)
						Exit For
					End If

				Next

				If Not ret_Provider Is Nothing Then

					Return ret_Provider

				Else

					Throw New Exception()

				End If

			End Function
				
		#End Region

		#Region " Public Shared Methods "

			Public Shared Function Load_Type( _
				ByVal full_Name As String, _
				Optional ByVal relative_Path As String = Nothing, _
				Optional ByVal log As System.IO.StreamWriter = Nothing _
			) As System.Type

				Dim return_Type As Type = Nothing

				Dim current_Path As String = Nothing
				If String.IsNullOrEmpty(relative_Path) Then
					current_Path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase
				Else
					current_Path = IO.Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, relative_Path)
				End If

				If Not log Is Nothing Then log.WriteLine(String.Format("Using App Domain/Files Based At {0} To Look For {1}", current_Path, full_Name))

				Dim dll_Files As String() = IO.Directory.GetFiles(current_Path, "*.dll")
				Dim best_Match As Assembly = Nothing

				If Not log Is Nothing  Then log.WriteLine(String.Format("Found {0} Dll Files", dll_Files.Length))

				For i As Integer = 0 To dll_Files.Length - 1

					If Not log Is Nothing  Then log.WriteLine(String.Format("Checking {0}", dll_Files(i)))

					Dim simple_Name As String = dll_Files(i).ToLower().SubString(dll_Files(i).LastIndexOf("\") + 1)
					simple_Name = simple_Name.SubString(0, simple_Name.IndexOf(".dll"))

					If Not log Is Nothing  Then log.WriteLine(String.Format("Comparing {0} To {1}", full_Name, simple_Name))

					If full_Name.ToLower().StartsWith(simple_Name) Then
						If Not log Is Nothing  Then log.WriteLine(String.Format("MATCHED: Attempting to Load {0}", dll_Files(i)))
						Try
							best_Match = Assembly.LoadFrom(dll_Files(i))
							Exit For
						Catch ex As Exception
							If Not log Is Nothing  Then log.WriteLine(String.Format("FAILED TO LOAD: {0}", ex.ToString()))
						End Try
					End If

				Next

				If Not best_Match Is Nothing Then

					If Not log Is Nothing  Then log.WriteLine(String.Format("Checking (Best Match) Loaded Assembly {0}", best_Match.FullName))

					return_Type = best_Match.GetType(full_Name, False)

				End If

				If return_Type Is Nothing Then

					For Each single_Assembly As Assembly In AppDomain.CurrentDomain.GetAssemblies()

						If Not log Is Nothing  Then log.WriteLine(String.Format("Checking Assembly {0}", single_Assembly.FullName))

						return_Type = single_Assembly.GetType(full_Name, False)

						If Not return_Type Is Nothing Then Exit For

					Next

				End If

				If return_Type Is Nothing Then

					For i As Integer = 0 To dll_Files.Length - 1

						Try
							Dim single_Assembly As Assembly = Assembly.LoadFrom(dll_Files(i))

							If Not log Is Nothing  Then log.WriteLine(String.Format("Checking Loaded Assembly {0}", single_Assembly.FullName))

							return_Type = single_Assembly.GetType(full_Name, False)

							If Not return_Type Is Nothing Then Exit For

						Catch
						End Try

					Next

				End If

				Return return_Type

			End Function

		#End Region

		#Region " Public Properties "

			Public ReadOnly Property Check_Hashes() As Boolean
				Get
					Return Not String.IsNullOrEmpty(Server_HashPath)
				End Get
			End Property

		#End Region

	End Class

End Namespace