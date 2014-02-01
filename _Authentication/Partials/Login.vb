Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls

Namespace Authentication

	Public Class Login
		Implements System.Web.IHttpHandler

		#Region " Private Constants "

			Private Const VALUE_SIGN_IN As String = "Sign In"

		#End Region

		#Region " Public Constants "

			Public Const CONTROL_LOGIN As String = "txt_Login"

			Public Const CONTROL_USERNAME As String = "txt_Username"
		
			Public Const CONTROL_PASSWORD As String = "txt_Password"

			Public Const CONTROL_PERSIST As String = "chk_Persist"

			Public Const QUERYSTRING_LOGIN As String = "l"

			Public Const QUERYSTRING_USERNAME As String = "u"

			Public Const QUERYSTRING_PASSWORD As String = "p"

			Public Const QUERYSTRING_PERSIST As String = "s"

			Public Const QUERYSTRING_REDIRECT As String = "r"

		#End Region

		#Region " Private Properties "

			Private ReadOnly Property Ctrl_Login( _
					ByVal context As System.Web.HttpContext _
				) As String
				Get
					If String.IsNullOrEmpty(context.Request.QueryString(QUERYSTRING_LOGIN)) Then
						Return CONTROL_LOGIN
					Else
						Return context.Request.QueryString(QUERYSTRING_LOGIN)
					End If
				End Get
			End Property

			Private ReadOnly Property Ctrl_Username( _
					ByVal context As System.Web.HttpContext _
				) As String
				Get
					If String.IsNullOrEmpty(context.Request.QueryString(QUERYSTRING_USERNAME)) Then
						Return CONTROL_USERNAME
					Else
						Return context.Request.QueryString(QUERYSTRING_USERNAME)
					End If
				End Get
			End Property

			Private ReadOnly Property Ctrl_Password( _
					ByVal context As System.Web.HttpContext _
				) As String
				Get
					If String.IsNullOrEmpty(context.Request.QueryString(QUERYSTRING_PASSWORD)) Then
						Return CONTROL_PASSWORD
					Else
						Return context.Request.QueryString(QUERYSTRING_PASSWORD)
					End If
				End Get
			End Property

			Private ReadOnly Property Ctrl_Persist( _
					ByVal context As System.Web.HttpContext _
				) As String
				Get
					If String.IsNullOrEmpty(context.Request.QueryString(QUERYSTRING_PERSIST)) Then
						Return CONTROL_PERSIST
					Else
						Return context.Request.QueryString(QUERYSTRING_PERSIST)
					End If
				End Get
			End Property

		#End Region

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
			
				Dim _success As System.Boolean

				If context.Request.Form(Ctrl_Login(context)) = VALUE_SIGN_IN AndAlso _
					Not String.IsNullOrEmpty(context.Request.Form(Ctrl_Username(context))) AndAlso _
					Not String.IsNullOrEmpty(context.Request.Form(Ctrl_Password(context))) Then

					Dim persist_Login As System.Boolean = False

					If Not context.Request.Form(Ctrl_Persist(context)) Is Nothing Then persist_Login = True

					_success = New Hermes.Authentication.Server().Login(context.Request.Form(Ctrl_Username(context)), _
						context.Request.Form(Ctrl_Password(context)), persist_Login)

				Else

					_success = New Hermes.Authentication.Server().Logout()

				End If

				context.Response.Redirect( _
					Hermes.Web.Url.Parse(context.Request.QueryString(QUERYSTRING_REDIRECT)).Build_Full, True _
				)

			End Sub

		#End Region

	End Class

End Namespace