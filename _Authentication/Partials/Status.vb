Imports System
Imports System.Collections.Specialized
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls

Namespace Authentication

	Public Class Status
		Implements System.Web.IHttpHandler

		#Region " IHttpHandler Implementation "

			Public ReadOnly Property IsReusable() As Boolean _
			Implements System.Web.IHttpHandler.IsReusable
				Get
					Return True
				End Get
			End Property

			Public Sub ProcessRequest( _
				ByVal context As System.Web.HttpContext _
			) Implements System.Web.IHttpHandler.ProcessRequest
			
				Dim SV = New Hermes.Authentication.Server()

				Dim auth_Builder As New System.Text.StringBuilder()

				auth_Builder.AppendLine("<div>")

				If SV.Authenticated Then
					auth_Builder.AppendLine(String.Format("<h3>{0}</h3>", "Hermes Authenticated"))

					auth_Builder.AppendLine("<ul>")

					auth_Builder.AppendLine(String.Format("<li>{0}: {1}</li>", "Username", SV.Username))
					auth_Builder.AppendLine(String.Format("<li>{0}: {1}</li>", "Details", SV.Details))
					auth_Builder.AppendLine(String.Format("<li>{0}: {1}</li>", "Expires", SV.Expires.ToString()))
					auth_Builder.AppendLine(String.Format("<li>{0}: {1}</li>", "Email", SV.Email_Address))

					Dim role_Index As Integer = 0
					For Each role As String In SV.Roles

						role_Index += 1
						auth_Builder.AppendLine(String.Format("<li>Role {0}: {1}</li>", role_Index, role))

					Next

					auth_Builder.AppendLine("</ul>")
				Else
					auth_Builder.AppendLine(String.Format("<h3>{0}</h3>", "Hermes NOT Authenticated"))
				End If

				auth_Builder.AppendLine("</div>")

				context.Response.Write(auth_Builder.ToString())

				For i As Integer = 0 To context.Request.Cookies.Count - 1

					Dim cookie_Builder As New System.Text.StringBuilder()

					cookie_Builder.AppendLine("<div>")
					cookie_Builder.AppendLine(String.Format("<h3>{0}</h3>", context.Request.Cookies(i).Name))
					cookie_Builder.AppendLine("<ul>")

					cookie_Builder.AppendLine(String.Format("<li>{0}: {1}</li>", "Domain", context.Request.Cookies(i).Domain))
					cookie_Builder.AppendLine(String.Format("<li>{0}: {1}</li>", "Path", context.Request.Cookies(i).Path))
					cookie_Builder.AppendLine(String.Format("<li>{0}: {1}</li>", "Expires", context.Request.Cookies(i).Expires))
					cookie_Builder.AppendLine(String.Format("<li>{0}: {1}</li>", "Secure", context.Request.Cookies(i).Secure))
					cookie_Builder.AppendLine(String.Format("<li>{0}: {1}</li>", "HttpOnly", context.Request.Cookies(i).HttpOnly))
					cookie_Builder.AppendLine(String.Format("<li>{0}: {1}</li>", "HasKeys", context.Request.Cookies(i).HasKeys))
					cookie_Builder.AppendLine(String.Format("<li>{0}: {1}</li>", "Value", context.Request.Cookies(i).Value))

					If context.Request.Cookies(i).HasKeys AndAlso Not context.Request.Cookies(i).Values Is Nothing AndAlso context.Request.Cookies(i).Values.Count > 0 Then	

						cookie_Builder.AppendLine("<li>")
						cookie_Builder.AppendLine(String.Format("<h3>{0}</h3>", "Values"))
						cookie_Builder.AppendLine("<ul>")

						Dim cookie_Values As New NameValueCollection(context.Request.Cookies(i).Values)
						For Each name As String In cookie_Values.AllKeys
							Dim values As String() = cookie_Values.GetValues(name)

							For k As Integer = 0 To values.Length - 1

								cookie_Builder.AppendLine(String.Format("<li>{0} [Value {1}]: {2}</li>", name, (k + 1), values(k)))

							Next

						Next

						cookie_Builder.AppendLine("</ul>")
						cookie_Builder.AppendLine("</li>")

					End If

					cookie_Builder.AppendLine("</ul>")
					cookie_Builder.AppendLine("</div>")

					context.Response.Write(cookie_Builder.ToString())

				Next

				context.Response.Flush
				context.Response.End

			End Sub

		#End Region

	End Class

End Namespace