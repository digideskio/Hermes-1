Imports System.Collections.Generic
Imports System.IO
Imports System.ServiceModel
Imports System.Web
Imports System.Web.Script.Services
Imports System.Web.Services
Imports System.Web.Services.Protocols
 
Namespace Files.Web

	<WebService(Namespace:="urn:uuid:aa7c691b-48a1-4bbc-9976-99732ca2d5ff", Description:="Hermes Files Services"), _
	ServiceBehavior(Namespace:="urn:uuid:aa7c691b-48a1-4bbc-9976-99732ca2d5ff"), WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1), ScriptService()> _
	Public Class Service
		Inherits WebService
	 
		#Region " Private Variables "

			Private SV As New Hermes.Files.Web.Server()
			Private LS As New Hermes.Authentication.Server()

		#End Region

		#Region " Public Web-Service Methods "

			''' <summary>Method to Get Containers</summary>
			''' <param name="provider">The Name of the provider to be used</summary>
			''' <param name="parent">The Parent Container</summary>
			''' <returns></returns>
			<WebMethod( _
				MessageName:="Get_Containers", _
				Description:="Get Containers", _
				CacheDuration:=0, _
				EnableSession:=True, _
				BufferResponse:=True _
			), _
			ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
			Public Function Get_Containers( _
				ByVal provider As System.String, _
				Byval parent As System.String _
			) As Hermes.Files.Container()

				Try

					Return SV.Get_Containers(provider, parent.Split("|"C))

				Catch uex As UnauthorizedAccessException

					context.Response.StatusCode = System.Net.HttpStatusCode.Unauthorized

				Catch ex As Exception

					context.Response.StatusCode = System.Net.HttpStatusCode.InternalServerError

				End Try

				Return New Hermes.Files.Container() {}

			End Function

			''' <summary>Method to Get Documents</summary>
			''' <param name="provider">The Name of the provider to be used</summary>
			''' <param name="parent">The Parent Container</summary>
			''' <returns></returns>
			<WebMethod( _
				MessageName:="Get_Documents", _
				Description:="Get Documents", _
				CacheDuration:=0, _
				EnableSession:=True, _
				BufferResponse:=True _
			), _
			ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
			Public Function Get_Documents( _
				ByVal provider As System.String, _
				Byval parent As System.String _
			) As Hermes.Files.Document()

				Try

					Return SV.Get_Documents(provider, parent.Split("|"C))

				Catch uex As UnauthorizedAccessException

					context.Response.StatusCode = System.Net.HttpStatusCode.Unauthorized

				Catch ex As Exception

					context.Response.StatusCode = System.Net.HttpStatusCode.InternalServerError

				End Try

				Return New Hermes.Files.Document() {}

			End Function

		#End Region

	End Class

End Namespace