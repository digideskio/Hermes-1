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

	Public Class HttpOptionsHandler
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
			End Sub

		#End Region

	End Class

End Namespace