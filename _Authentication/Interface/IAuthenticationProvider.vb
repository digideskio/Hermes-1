Namespace Authentication

	Public Interface IAuthenticationProvider

		#Region " Properties "

			Property Name As String

		#End Region

		#Region " Methods "

			Function Configure( _
				ByVal values As List(Of String) _
			) As Boolean

			Function Authenticate( _
				ByVal _request As Request, _
				ByRef _response As Response _
			) As Boolean

			Sub DeAuthenticate( _
				ByVal _request As Request _
			)

			Sub Log( _
				ByVal _request As Request, _
				ByVal _event As Hermes.Authentication.Event _
			)

		#End Region

	End Interface

End Namespace