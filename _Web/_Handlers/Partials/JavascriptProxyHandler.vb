Imports System.Collections.Generic
Imports System.Text
Imports System.Web
Imports System.IO
Imports System.Reflection
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Web.UI
Imports System.Web.Script.Services

Namespace Web.Handlers

	Partial Public Class JavascriptProxyHandler
		Implements IHttpHandler

		#Region "IHttpHandler Implementation "

			Private ReadOnly Property IHttpHandler_IsReusable() As Boolean _
				Implements IHttpHandler.IsReusable
				Get
					Return False
				End Get
			End Property

			Private Sub IHttpHandler_ProcessRequest( _
				ByVal context As HttpContext _
			) Implements IHttpHandler.ProcessRequest

				context.Response.ContentType = "application/x-javascript"
				context.Response.Write(Value)

			End Sub

		#End Region

	End Class

End Namespace