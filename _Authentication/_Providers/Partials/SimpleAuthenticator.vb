Imports Hermes.Authentication
Imports System.Configuration.ConfigurationManager

Namespace Authentication.Providers

	Partial Public Class SimpleAuthenticator
		Implements IAuthenticationProvider

		#Region " Public Methods "

			Public Function Configure( _
				ByVal values As List(Of String) _
			) As Boolean _
			Implements IAuthenticationProvider.Configure

				If values.Count > 1 Then

					Username = values(0)
					Password = values(1)

					If values.Count > 2 Then
						Roles = values(2).Split(New Char() {";"c}, StringSplitOptions.RemoveEmptyEntries)
					Else
						Roles = New String(){}
					End If

					If values.Count > 3 Then _
						Email_Address = values(3)

					If values.Count > 4 Then _
						Details = values(4)

					If String.IsNullOrEmpty(Password) Then Return False Else Return True

				Else

					Return False

				End If

			End Function

			Function Authenticate( _
				ByVal _request As Request, _
				ByRef _response As Response _
			) As Boolean _
			Implements IAuthenticationProvider.Authenticate

				If String.Compare(_request.Username, Username, StringComparison.InvariantCultureIgnoreCase) = 0 Then

					If _request.Password = Password Then

						If _response Is Nothing Then _response = New Response()
						If Not String.IsNullOrEmpty(Details) Then
							_response.Details = Details
						Else
							_response.Details = Username + " [TESTING]"
						End If
						If Not String.IsNullOrEmpty(Email_Address) Then _
							_response.Email_Address = Email_Address
						_response.Roles = Roles

						Return True

					End If

				End If

				Return False

			End Function

			Public Sub DeAuthenticate( _
				ByVal _request As Request _
			) Implements IAuthenticationProvider.DeAuthenticate

				' Do Nothing

			End Sub

			Public Sub Log( _
				ByVal _request As Request, _
				ByVal _event As Hermes.Authentication.Event _
			) Implements IAuthenticationProvider.Log

				' Do Nothing

			End Sub
			
		#End Region

	End Class

End Namespace