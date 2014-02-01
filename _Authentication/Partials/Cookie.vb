Imports Hermes.Cryptography.Cipher

Namespace Authentication

	Public Partial Class Cookie

		#Region " Public Shared Constants "

			' -- Cookie Details --
			Public Const COOKIE_AUTH_PAIRNAME_AUTH As String = "AUTH"

			Public Const COOKIE_AUTH_PAIRNAME_ID As String = "ID"

			Public Const COOKIE_AUTH_PAIRNAME_VERSION As String = "VERSION"

			Public Const COOKIE_AUTH_PAIRNAME_EXPIRES As String = "EXPIRES"

			Public Const COOKIE_AUTH_PAIRNAME_USERNAME As String = "USERNAME"

			Public Const COOKIE_AUTH_PAIRNAME_DISPLAY As String = "DISPLAY"

			Public Const COOKIE_AUTH_PAIRNAME_ROLES As String = "ROLES"

			Public Const COOKIE_AUTH_PAIRNAME_AUTHPROVIDER As String = "AUTH_PROVIDER"

			Public Const COOKIE_AUTH_PAIRNAME_EMAIL As String = "EMAIL"
			' --------------------

		#End Region

		#Region " Public Cookie Handling Methods "

			Public Sub Read( _
				ByVal value As String _
			)

				RawValue = value
				Dim cookie_Values As String() = New String(){}
				If Not String.IsNullOrEmpty(RawValue) Then cookie_Values = RawValue.Split(";"C)

				If cookie_Values.Length >= 1 AndAlso cookie_Values(0) = COOKIE_AUTH_PAIRNAME_AUTH Then Authenticated = True

				If cookie_Values.Length > 1 Then

					For i As Integer = 1 To cookie_Values.Length - 1

						If cookie_Values(i).Split("="C)(0) = COOKIE_AUTH_PAIRNAME_EXPIRES Then _
							Expires = DateTime.Parse(cookie_Values(i).Split("="C)(1))

						If cookie_Values(i).Split("="C)(0) = COOKIE_AUTH_PAIRNAME_USERNAME Then _
							Username = cookie_Values(i).Split("="C)(1)

						If cookie_Values(i).Split("="C)(0) = COOKIE_AUTH_PAIRNAME_DISPLAY Then _
							Details = cookie_Values(i).Split("="C)(1)

						If cookie_Values(i).Split("="C)(0) = COOKIE_AUTH_PAIRNAME_ID Then _
							System.Guid.TryParse(cookie_Values(i).Split("="C)(1), Id)

						If cookie_Values(i).Split("="C)(0) = COOKIE_AUTH_PAIRNAME_VERSION Then _
							System.Int32.TryParse(cookie_Values(i).Split("="C)(1), Version)

						If cookie_Values(i).Split("="C)(0) = COOKIE_AUTH_PAIRNAME_AUTHPROVIDER Then _
							Authentication_Provider = cookie_Values(i).Split("="C)(1)

						If cookie_Values(i).Split("="C)(0) = COOKIE_AUTH_PAIRNAME_ROLES Then

							Dim user_Roles As String = cookie_Values(i).Split("="C)(1)
							If Not String.IsNullOrEmpty(user_Roles) Then
								If user_Roles.Contains("|") Then Roles = user_Roles.Split("|"C) Else Roles = New String() {user_Roles}
							End If

						End If

						If cookie_Values(i).Split("="C)(0) = COOKIE_AUTH_PAIRNAME_EMAIL Then _
							Email_Address = cookie_Values(i).Split("="C)(1)

					Next

				End If

			End Sub

			Public Function Write() As String

				Dim cookie_Builder As New System.Text.StringBuilder()

				If Authenticated Then cookie_Builder.Append(COOKIE_AUTH_PAIRNAME_AUTH).Append(";")

				cookie_Builder.Append(COOKIE_AUTH_PAIRNAME_ID).Append("=").Append(Id).Append(";")

				cookie_Builder.Append(COOKIE_AUTH_PAIRNAME_VERSION).Append("=").Append(Version).Append(";")

				cookie_Builder.Append(COOKIE_AUTH_PAIRNAME_EXPIRES).Append("=").Append(Expires.ToString()).Append(";")

				cookie_Builder.Append(COOKIE_AUTH_PAIRNAME_USERNAME).Append("=").Append(Username).Append(";")

				cookie_Builder.Append(COOKIE_AUTH_PAIRNAME_AUTHPROVIDER).Append("=").Append(Authentication_Provider).Append(";")

				If Not String.IsNullOrEmpty(Details) Then _
					cookie_Builder.Append(COOKIE_AUTH_PAIRNAME_DISPLAY).Append("=").Append(Details).Append(";")

				If Not String.IsNullOrEmpty(Email_Address) Then _
					cookie_Builder.Append(COOKIE_AUTH_PAIRNAME_EMAIL).Append("=").Append(Email_Address).Append(";")

				If Not Roles Is Nothing AndAlso Roles.Length > 0 Then
					cookie_Builder.Append(COOKIE_AUTH_PAIRNAME_ROLES).Append("=")
					For i As Integer = 0 TO Roles.Length - 1
						If i > 0 Then cookie_Builder.Append("|")
						cookie_Builder.Append(Roles(i))
					Next
					cookie_Builder.Append(";")
				End If

				cookie_Builder.Remove(cookie_Builder.Length - 1, 1) ' Remove the final semi-colon

				RawValue = cookie_Builder.ToString()

				Return RawValue

			End Function

		#End Region

		#Region " Public Display Methods "

			Public Function Show_Details() As String

				Dim return_String As New System.Text.StringBuilder()

				If Not Authenticated Then

					return_String.AppendLine("No Authenticated User")

				Else

					return_String.AppendFormat("Authenticated: {0} // As: {1} // Details: {2} // Until: {3} // By: {4}", _
						Authenticated.ToString(), Username, Details, Expires.ToString(), Authentication_Provider)

					If Not String.IsNullOrEmpty(Email_Address) Then return_String.AppendFormat(" // Email: {0}", Email_Address)

					If Not Roles Is Nothing AndAlso Roles.Length > 0 Then

						For i As Integer = 0 To Roles.Length - 1
							return_String.AppendFormat(" // Role: {0}", Roles(i))
						Next

					End If

					return_String.AppendFormat(" // Value: {0}", RawValue)

				End If

				Return return_String.ToString()

			End Function

		#End Region

		#Region " Public Shared Methods "

			Public Shared Function Create( _
				ByVal value As String _
			) As Cookie

				Dim ret_Val As New Cookie()
				ret_Val.Read(value)
				Return ret_Val

			End Function

		#End Region

	End Class

End Namespace
