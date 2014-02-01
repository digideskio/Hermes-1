Imports Hermes.Files
Imports System.Reflection
Imports System.Web
Imports W = System.Web.Configuration.WebConfigurationManager

Namespace Files.Web

	Public Partial Class Config

		#Region " Public Shared Constants "

			' -- Configuration Key Names --
			Public Const CONFIGURATION_FILE_PROVIDERS As String = "Hermes.Files.Provider."

			Public Const CONFIGURATION_FILE_PATH As String = "Hermes.Files.Providers.Path"

			Public Const CONFIGURATION_LOG_PATH As String = "Hermes.Files.Log.Path"

			Public Const CONFIGURATION_REQUIRES_AUTH_PROVIDERS As String = "Hermes.Files.Requires.Authentication.Providers"
			' -----------------------------

		#End Region

		#Region " Friend Constants "

			Friend Const WILDCARD = "*"

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

			Private Function Initialise_RequiresAuthenticationProviders() As System.String()

				If _Log Then m_Log.WriteLine("Starting Requires Auth Configuration")

				Dim requires_AuthProviders As String = W.AppSettings(CONFIGURATION_REQUIRES_AUTH_PROVIDERS)

				If _Log Then
					m_log.WriteLine(String.Format("Requires Authentication Providers: {0}", requires_AuthProviders))
					m_log.Flush()
				End If

				If String.IsNullOrEmpty(requires_AuthProviders) Then ' No Need for Any Authentication Provider (Open)
					Return Nothing
				ElseIf requires_AuthProviders = "*" Then ' Allow any valid Authentication
					Return New String() {}
				Else
					Return requires_AuthProviders.Split(";"C) ' Only Allow Specifically Named Authentication Providers
				End If

			End Function

			Private Function Initialise_Providers_RelativePath() As System.String

				If _Log Then m_Log.WriteLine("Starting Providers Relative Path Configuration")

				Dim relative_Path As String = W.AppSettings(CONFIGURATION_FILE_PATH)

				If _Log Then
					m_log.WriteLine(String.Format("Relative Path: {0}", relative_Path))
					m_log.Flush()
				End If

				If String.IsNullOrEmpty(relative_Path) Then relative_Path = "bin"

				Return relative_Path
			
			End Function

			Private Function Initialise_Providers() As IContainerDocumentProvider()

				If _Log Then m_Log.WriteLine("Starting Providers Configuration")

				Dim all_Providers As New System.Collections.Generic.List(Of IContainerDocumentProvider)()

				Dim all_Settings As System.Collections.Specialized.NameValueCollection = W.AppSettings

				If _Log Then m_log.WriteLine(String.Format("Provider Config Count: {0}", all_Settings.Count))

				For Each single_Setting As String In all_Settings.AllKeys

					If single_Setting.ToLower.StartsWith(CONFIGURATION_FILE_PROVIDERS.ToLower) Then

						Dim provider_Name As String = single_Setting.SubString(CONFIGURATION_FILE_PROVIDERS.Length)

						If _Log Then m_log.WriteLine(String.Format("Configuring Provider: {0}", provider_Name))

						Dim provider_Details As New List(Of String)(all_Settings(single_Setting).Split(New Char() {"|"c}, StringSplitOptions.RemoveEmptyEntries))

						If provider_Details.Count > 0 Then

							Dim provider_Type As System.Type = Hermes.Authentication.Config.Load_Type(provider_Details(0), Providers_RelativePath)

							If Not provider_Type Is Nothing Then

								If _Log Then m_log.WriteLine(String.Format("Got Provider Type: {0}", provider_Type.FullName))

								provider_Details.RemoveAt(0)
								Dim provider_Class As IContainerDocumentProvider = CType(Activator.CreateInstance(provider_Type), IContainerDocumentProvider)
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

		#End Region

		#Region " Public Methods "

			Public Function Authorised( _
				ByRef _auth As Hermes.Authentication.Server, _
				ByVal required_Roles As String() _
			) As Boolean

				Dim ret_Val As Boolean = False

				' -- Check Against Authentication Provider -- '
				If RequiresAuthenticationProviders Is Nothing Then

					ret_Val = True

				ElseIf RequiresAuthenticationProviders.Length = 0 Then


					ret_Val = _auth.Authenticated

				Else

					For i As Integer = 0 To RequiresAuthenticationProviders.Length - 1
						If _auth.Authenticated(RequiresAuthenticationProviders(i)) Then ret_Val = True
					Next

				End If

				' -- Check Roles -- '
				If ret_Val

					If required_Roles Is Nothing OrElse required_Roles.Length = 0 _
						OrElse (required_Roles.Length = 1 AndAlso String.IsNullOrEmpty(required_Roles(0))) Then

						ret_Val = False

					ElseIf Not (required_Roles.Length = 1 AndAlso required_Roles(0) = WILDCARD)

						ret_Val = _auth.IsInAnyRole(required_Roles)

					End If

				End If

				Return ret_Val

			End Function

			Public Function Provider( _
				ByVal provider_Name As String _
			) As IContainerDocumentProvider

				Dim ret_Provider As IContainerDocumentProvider = Nothing

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

	End Class

End Namespace