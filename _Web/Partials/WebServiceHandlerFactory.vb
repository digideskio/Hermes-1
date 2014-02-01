Imports System.Activator
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

	Partial Public Class WebServiceHandlerFactory
		Implements IHttpHandlerFactory
		
		#Region " Private Constants "

			' -- Headers & Requests --
			Private Const HTTP_OPTIONS As String = "OPTIONS"

			Private Const HEADER_MODIFIED As String = "If-Modified-Since"
			' ------------------------

			' -- Handlers & Factories --
			Private Const SCRIPT_FACTORY As String = "System.Web.Script.Services.ScriptHandlerFactory"

			Private Const REST_FACTORY As String = "System.Web.Script.Services.RestHandlerFactory"

			Private Const REST_HANDLER As String = "System.Web.Script.Services.RestHandler"
			' --------------------------

			' -- Methods --
			Private Const METHOD_RESTREQUEST As String = "IsRestRequest"

			Private Const METHOD_PROXYSCRIPT As String = "GetClientProxyScript"

			Private Const METHOD_CREATE As String = "CreateHandler"

			Private Const METHOD_GET As String = "CoreGetHandler"
			' -------------

			' -- Types --
			Private Const TYPE_WSDATA As String = "System.Web.Script.Services.WebServiceData"

			Private Const TYPE_WSPROXYGEN As String = "System.Web.Script.Services.WebServiceClientProxyGenerator"

			Private Const TYPE_PROXYGEN As String = "System.Web.Script.Services.ClientProxyGenerator"
			' -----------

		#End Region

		#Region " Private Shared Methods "

			Private Shared Function GetAssembly_LastModifiedTime( _
				ByVal assembly As Assembly _
			) As DateTime

				Dim lastWriteTime As DateTime = File.GetLastWriteTime(New Uri(assembly.GetName().CodeBase).LocalPath)
				Return New DateTime(lastWriteTime.Year, lastWriteTime.Month, lastWriteTime.Day, lastWriteTime.Hour, lastWriteTime.Minute, 0)

			End Function

		#End Region

		#Region " IHttpHandlerFactory Implementation "

			Public Function GetHandler( _
				ByVal context As HttpContext, _
				ByVal requestType As String, _
				ByVal url As String, _
				ByVal pathTranslated As String _
			) As IHttpHandler _
			Implements IHttpHandlerFactory.GetHandler

				Try

					Dim aspNet_permission As New AspNetHostingPermission(AspNetHostingPermissionLevel.Minimal)
					aspNet_permission.Demand() ' Exception / Security

					Dim WebServiceType As Type = Hermes.Authentication.Config.Load_Type(Path.GetFileNameWithoutExtension(pathTranslated), "bin")

					If WebServiceType Is Nothing Then

						' Handle as Normal Script Handler
						Dim ScriptHandlerFactory As IHttpHandlerFactory = DirectCast(System.Activator.CreateInstance(Ajax_Assembly.[GetType](SCRIPT_FACTORY)), IHttpHandlerFactory)
						HandlerFactory = ScriptHandlerFactory
						Return DirectCast(ScriptHandlerFactory.GetHandler(context, requestType, url, pathTranslated), IHttpHandler)

					End If

					If requestType = HTTP_OPTIONS Then

						Return DirectCast(New HttpOptionsHandler(), IHttpHandler)

					Else

						Dim JsHandlerFactory As IHttpHandlerFactory = DirectCast(CreateInstance(Ajax_Assembly.[GetType](REST_FACTORY)), IHttpHandlerFactory)
						Dim IsScriptRequestMethod As MethodInfo = JsHandlerFactory.[GetType]().GetMethod(METHOD_RESTREQUEST, BindingFlags.[Static] Or BindingFlags.NonPublic)

						If CBool(IsScriptRequestMethod.Invoke(Nothing, New Object() {context})) Then

							HandlerFactory = JsHandlerFactory

							Dim IsJavascriptDebug As Boolean = (String.Compare(context.Request.PathInfo, "/jsdebug", StringComparison.OrdinalIgnoreCase) = 0) ' JS Debug Proxy
							Dim IsJavascript As Boolean = (String.Compare(context.Request.PathInfo, "/js", StringComparison.OrdinalIgnoreCase) = 0) ' JS Request

							If IsJavascript OrElse IsJavascriptDebug Then

								Dim WebServiceDataConstructor As ConstructorInfo = Ajax_Assembly.[GetType](TYPE_WSDATA).GetConstructor(BindingFlags.NonPublic Or _
									BindingFlags.Instance, Nothing, New Type() {GetType(Type), GetType(Boolean)}, Nothing)
								Dim WebServiceClientProxyGeneratorConstructor As ConstructorInfo = Ajax_Assembly.[GetType](TYPE_WSPROXYGEN). _
									GetConstructor(BindingFlags.NonPublic Or BindingFlags.Instance, Nothing, New Type() {GetType(String), GetType(Boolean)}, Nothing)
								Dim GetClientProxyScriptMethod As MethodInfo = Ajax_Assembly.[GetType](TYPE_PROXYGEN).GetMethod(METHOD_PROXYSCRIPT, BindingFlags.NonPublic Or _
									BindingFlags.Instance, Nothing, New Type() {Ajax_Assembly.[GetType](TYPE_WSDATA)}, Nothing)
								Dim javascript_Value As String = DirectCast(GetClientProxyScriptMethod.Invoke(WebServiceClientProxyGeneratorConstructor.Invoke( _
									New [Object]() {url, IsJavascriptDebug}), New [Object]() {WebServiceDataConstructor.Invoke(New Object() {WebServiceType, False})}), String)

								Dim assembly_ModifiedDate As DateTime = GetAssembly_LastModifiedTime(WebServiceType.Assembly)
								Dim modified_Since As String = context.Request.Headers(HEADER_MODIFIED)

								If Not String.IsNullOrEmpty(modified_Since) Then

									Dim parsed_modified_Since As DateTime

									If DateTime.TryParse(modified_Since, parsed_modified_Since) AndAlso (parsed_modified_Since >= assembly_ModifiedDate) Then
										context.Response.StatusCode = &H130
										Return Nothing
									End If

								End If

								If Not IsJavascriptDebug AndAlso (assembly_ModifiedDate.ToUniversalTime() < DateTime.UtcNow) Then

									Dim cache As HttpCachePolicy = context.Response.Cache
									cache.SetCacheability(HttpCacheability.[Public])
									cache.SetLastModified(assembly_ModifiedDate)

								End If
								Return DirectCast(New JavascriptProxyHandler(javascript_Value), IHttpHandler)

							Else

								Dim JavascriptHandler As IHttpHandler = DirectCast(CreateInstance(Ajax_Assembly.[GetType](REST_HANDLER)), IHttpHandler)
								Dim WebServiceDataConstructor As ConstructorInfo = Ajax_Assembly.[GetType](TYPE_WSDATA).GetConstructor(BindingFlags.NonPublic Or _
									BindingFlags.Instance, Nothing, New Type() {GetType(Type), GetType(Boolean)}, Nothing)
								Dim CreateHandlerMethod As MethodInfo = JavascriptHandler.[GetType]().GetMethod(METHOD_CREATE, BindingFlags.NonPublic Or _
									BindingFlags.[Static], Nothing, New Type() {Ajax_Assembly.[GetType](TYPE_WSDATA), GetType(String)}, Nothing)
								Return DirectCast(CreateHandlerMethod.Invoke(JavascriptHandler, New [Object]() {WebServiceDataConstructor.Invoke(New Object() {WebServiceType, False}), context.Request.PathInfo.Substring(1)}), IHttpHandler)

							End If

						Else

							HandlerFactory = New System.Web.Services.Protocols.WebServiceHandlerFactory()
							Dim CoreGetHandlerMethod As MethodInfo = HandlerFactory.[GetType]().GetMethod(METHOD_GET, BindingFlags.Instance Or BindingFlags.NonPublic)
							Return DirectCast(CoreGetHandlerMethod.Invoke(HandlerFactory, New Object() {WebServiceType, context, context.Request, context.Response}), IHttpHandler)

						End If

					End If

				Catch e As TargetInvocationException

					Throw e.InnerException
					
				End Try

			End Function

			Public Sub ReleaseHandler( _
				ByVal handler As IHttpHandler _
			) Implements IHttpHandlerFactory.ReleaseHandler

				If Not HandlerFactory Is Nothing Then HandlerFactory.ReleaseHandler(handler)

			End Sub

		#End Region

    End Class

End Namespace