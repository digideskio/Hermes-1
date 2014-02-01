Imports System.Collections.Generic
Imports System.IO
Imports System.ServiceModel
Imports System.Web
Imports System.Web.Script.Services
Imports System.Web.Services
Imports System.Web.Services.Protocols
 
Namespace Authentication

	<WebService(Namespace:="urn:uuid:5a67470d-95e7-4bea-93b4-381f591147d1", Description:="Hermes Authentication Services"), _
	ServiceBehavior(Namespace:="urn:uuid:5a67470d-95e7-4bea-93b4-381f591147d1"), WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1), ScriptService()> _
	Public Class Service
		Inherits WebService
	 
		#Region " Private Variables "

			Private SV As New Hermes.Authentication.Server()

		#End Region

		#Region " Public Web-Service Methods "

			''' <summary></summary>
			''' <param name="provider">The Name of the provider to be used</summary>
			''' <param name="parent">The Parent Container</summary>
			''' <returns></returns>
			<WebMethod( _
				MessageName:="Get_Display_Name", _
				Description:="Get_Display_Name", _
				CacheDuration:=0, _
				EnableSession:=False, _
				BufferResponse:=True _
			), _
			ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
			Public Function Get_Display_Name() As String

				Return SV.Details

			End Function

			''' <summary></summary>
			''' <param name="provider">The Name of the provider to be used</summary>
			''' <param name="parent">The Parent Container</summary>
			''' <returns></returns>
			<WebMethod( _
				MessageName:="Get_User_Name", _
				Description:="Get_User_Name", _
				CacheDuration:=0, _
				EnableSession:=False, _
				BufferResponse:=True _
			), _
			ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
			Public Function Get_User_Name() As String

				Return SV.Username

			End Function

			''' <summary></summary>
			''' <param name="provider">The Name of the provider to be used</summary>
			''' <param name="parent">The Parent Container</summary>
			''' <returns></returns>
			<WebMethod( _
				MessageName:="Get_Persist_Days", _
				Description:="Get_Persist_Days", _
				CacheDuration:=0, _
				EnableSession:=False, _
				BufferResponse:=True _
			), _
			ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
			Public Function Get_Persist_Days() As Int32

				Return SV.Settings.Cookie_ExpiresInDays()

			End Function

			''' <summary></summary>
			''' <param name="provider">The Name of the provider to be used</summary>
			''' <param name="parent">The Parent Container</summary>
			''' <returns></returns>
			<WebMethod( _
				MessageName:="Login", _
				Description:="Login", _
				CacheDuration:=0, _
				EnableSession:=False, _
				BufferResponse:=True _
			), _
			ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
			Public Function Login(
				ByVal username As System.String, _
				ByVal password As System.String, _
				ByVal persist As Boolean _
			) As Boolean

				Return New Hermes.Authentication.Server().Login(username, password, persist)

			End Function
		
			''' <summary></summary>
			''' <param name="provider">The Name of the provider to be used</summary>
			''' <param name="parent">The Parent Container</summary>
			''' <returns></returns>
			<WebMethod( _
				MessageName:="Logout", _
				Description:="Logout", _
				CacheDuration:=0, _
				EnableSession:=False, _
				BufferResponse:=True _
			), _
			ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
			Public Function Logout() As Boolean

				Return New Hermes.Authentication.Server().Logout()

			End Function

		#End Region

	End Class

End Namespace