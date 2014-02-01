Imports System.Collections.Specialized
Imports System.Drawing
Imports System.Drawing.ColorTranslator
Imports System.IO.File
Imports System.Net
Imports System.Net.Sockets
Imports System.Text.RegularExpressions
Imports System.Web
Imports System.Web.Caching
Imports System.Xml

Namespace Files.Web

	Public Class File
		Implements System.Web.IHttpHandler

		#Region " Public Constants "

			Public Const QUERYSTRING_PROVIDER As String = "p"

			Public Const QUERYSTRING_CONTAINER As String = "c"

			Public Const QUERYSTRING_FILE As String = "f"

		#End Region

		#Region " IHttpHandler Implementation "

			Public ReadOnly Property IsReusable() As Boolean _
			Implements System.Web.IHttpHandler.IsReusable
				Get
					Return False
				End Get
			End Property

			Public Sub ProcessRequest( _
				ByVal context As System.Web.HttpContext _
			) Implements System.Web.IHttpHandler.ProcessRequest

				Try

					Dim provider_Name As String = context.Request.QueryString(QUERYSTRING_PROVIDER)
					Dim path_Name As String = context.Request.QueryString(QUERYSTRING_CONTAINER)
					If path_Name Is Nothing Then path_Name = ""
					Dim file_Name As String = context.Request.QueryString(QUERYSTRING_FILE)
					Dim ws As New Web.Server()
					Dim file_Data As Byte() = ws.Get_Data(provider_Name, path_Name.Split("|"C), file_Name)

					If Not file_Data Is Nothing AndAlso file_Data.Length > 0 Then	

						context.Response.Cache.SetExpires(DateTime.Now.ToUniversalTime().AddHours(1))
						context.Response.Cache.SetMaxAge(New TimeSpan(1, 0, 0))
						context.Response.Cache.SetETag(file_Data.GetHashCode().ToString()) ' TODO: Fix this // http://en.wikipedia.org/wiki/HTTP_ETag
						context.Response.Cache.SetCacheability(System.Web.HttpCacheability.Private)
						context.Response.Cache.SetValidUntilExpires(True)
						context.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches)

						If file_Name.EndsWith(".pdf") Then
							context.Response.AddHeader("Content-Type", "application/pdf")
						Else
							context.Response.AddHeader("Content-Type", "application/octet-stream") 
						End If

						context.Response.AddHeader("Content-Disposition", "attachment; filename=""" & file_Name & """")
						context.Response.AddHeader("Content-Length", file_Data.Length.ToString())
						context.Response.BinaryWrite(file_Data)
						
					End If

				Catch uex As UnauthorizedAccessException

					context.Response.StatusCode = System.Net.HttpStatusCode.Unauthorized

				Catch ex As Exception

					context.Response.StatusCode = System.Net.HttpStatusCode.InternalServerError
					context.Response.Write(ex.ToString)

				Finally

					context.Response.Flush
					context.Response.End

				End Try

			End Sub

		#End Region

	End Class

End Namespace