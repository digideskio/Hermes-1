Imports System
Imports System.Collections.Specialized
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls

Namespace Web.Handlers

	Public Class BrowserDetailsHandler
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

				With context

					.Response.Clear

					.Response.Write(String.Format("ID: {0}<br/><br/>", .Request.Browser.Id))
					.Response.Write(String.Format("Browser: {0}<br/><br/>", .Request.Browser.Browser))
					.Response.Write(String.Format("Version: {0}<br/><br/>", .Request.Browser.Version))
					.Response.Write(String.Format("Major Version: {0}<br/><br/>", .Request.Browser.MajorVersion))
					.Response.Write(String.Format("Minor Version: {0}<br/><br/>", .Request.Browser.MinorVersion))
					.Response.Write(String.Format("Platform: {0}<br/><br/>", .Request.Browser.Platform))

					For i As Integer = 0 To .Request.Browser.Browsers.Count - 1

						If String.Compare(CStr(.Request.Browser.Browsers(i)).ToLower(), CStr(.Request.Browser.Browser).ToLower()) <> 0 Then
							.Response.Write(String.Format("Browsers {1}: {0}<br/><br/>", .Request.Browser.Browsers(i), i))
						End If

					Next

					If Not String.IsNullOrEmpty(.Request.Browser.MobileDeviceManufacturer) AndAlso _
						String.Compare("Unknown", .Request.Browser.MobileDeviceManufacturer) <> 0 Then _
							.Response.Write(String.Format("Mobile Device Manufacturer: {0}<br/><br/>", .Request.Browser.MobileDeviceManufacturer))

					If Not String.IsNullOrEmpty(.Request.Browser.MobileDeviceModel) AndAlso _
						String.Compare("Unknown", .Request.Browser.MobileDeviceModel) <> 0 Then _
							.Response.Write(String.Format("Mobile Device Model: {0}<br/><br/>", .Request.Browser.MobileDeviceModel))

					.Response.Write(String.Format("Screen Char Height: {0}<br/><br/>", .Request.Browser.ScreenCharactersHeight))
					.Response.Write(String.Format("Screen Char Width: {0}<br/><br/>", .Request.Browser.ScreenCharactersWidth))
					.Response.Write(String.Format("Screen Pixels Height: {0}<br/><br/>", .Request.Browser.ScreenPixelsHeight))
					.Response.Write(String.Format("Screen Pixels Width: {0}<br/><br/>", .Request.Browser.ScreenPixelsWidth))

					.Response.Write(String.Format("User Agent: {0}<br/><br/>", .Request.UserAgent))

					.Response.Flush
					.Response.End

				End With

			End Sub

		#End Region

	End Class

End Namespace