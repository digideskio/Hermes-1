Imports System
Imports System.Collections.Specialized
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Collections.Generic
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web.Script.Serialization
Imports W = System.Web.Configuration.WebConfigurationManager

Namespace Authentication

	Public Class JWT_SSO
		Implements System.Web.IHttpHandler

		#Region " Public Shared Constants "

			' -- Configuration Key Names --
			Public Const CONFIGURATION_SSO_URL As String = "Hermes.Auth.SSO.{0}.Url"

			Public Const CONFIGURATION_SSO_KEY As String = "Hermes.Auth.SSO.{0}.Key"

			Public Const CONFIGURATION_SSO_REDIRECT As String = "Hermes.Auth.SSO.{0}.RedirectQueryKey"

			Public Const CONFIGURATION_SSO_JWT As String = "Hermes.Auth.SSO.{0}.JWTQueryKey"
			' -----------------------------

		#End Region

		#Region " Private Properties "

			Private ReadOnly Property Url( _
				ByVal app As String _
			) As String
				Get
					Return W.AppSettings( _
						String.Format(CONFIGURATION_SSO_URL, app) _
					)
				End Get
			End Property

			Private ReadOnly Property Key( _
				ByVal app As String _
			) As String
				Get
					Return W.AppSettings( _
						String.Format(CONFIGURATION_SSO_KEY, app) _
					)
				End Get
			End Property

			Private ReadOnly Property Redirect_Param( _
				ByVal app As String _
			) As String
				Get
					Return W.AppSettings( _
						String.Format(CONFIGURATION_SSO_REDIRECT, app) _
					)
				End Get
			End Property

			Private ReadOnly Property JWT_Param( _
				ByVal app As String _
			) As String
				Get
					Return W.AppSettings( _
						String.Format(CONFIGURATION_SSO_JWT, app) _
					)
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
			
				Dim SV = New Hermes.Authentication.Server()

				If SV.Authenticated Then

					Dim app As String = context.Request.Path.TrimStart("/"c).ToUpper()
					Dim redirect_To As String = context.Request.QueryString(Redirect_Param(app))
					Dim jwt_Algorithm As JwtHashAlgorithm = JwtHashAlgorithm.HS256

					Dim ts As TimeSpan = (DateTime.UtcNow - New DateTime(1970, 1, 1))
					Dim timestamp As Integer = CInt(ts.TotalSeconds)

					Dim sso_Header As New Dictionary(Of String, Object)
					sso_Header.Add("alg", jwt_Algorithm.ToString)
					sso_Header.Add("typ", "JWT")

					Dim sso_Payload As New Dictionary(Of String, Object)
					sso_Payload.Add("iat", timestamp)
					sso_Payload.Add("jti", System.Guid.NewGuid().ToString())
					If Not String.IsNullOrEmpty(SV.Details) Then
						sso_Payload.Add("name", SV.Details)
					Else
						sso_Payload.Add("name", SV.Username)
					End If
					sso_Payload.Add("email", SV.Email_Address)

					Dim sso_Token As String = JWT_SSO.Encode( _
						sso_Header, sso_Payload, Encoding.UTF8.GetBytes(Key(app)), JwtHashAlgorithm.HS256)

					Dim sso_Redirect As String = String.Format( _
						Url(app), JWT_Param(app), sso_Token, Redirect_Param(app), redirect_To)

					If Not String.IsNullOrEmpty(SV.Email_Address) Then _
						context.Response.Redirect(sso_Redirect)

				End If

				context.Response.Flush
				context.Response.End

			End Sub

		#End Region

		#Region " Private Shared Methods "

			Private Shared Function Base64_UrlEncode( _
				ByVal input As Byte() _
			) As String

				Dim output = Convert.ToBase64String(input)
				output = output.Split("="C)(0) ' Remove any trailing '='s
				output = output.Replace("+"C, "-"C) ' 62nd char of encoding
				output = output.Replace("/"C, "_"C) ' 63rd char of encoding
				Return output

			End Function

			Private Shared Function Base64_UrlDecode( _
				ByVal input As String _
			) As Byte()

				Dim output As String = input
			
				output = output.Replace("-"C, "+"C) ' 62nd char of encoding
				output = output.Replace("_"C, "/"C) ' 63rd char of encoding

				Select Case (output.Length Mod 4) ' Pad with trailing '='s
			
					Case 0 
						Exit Select ' No pad chars in this case
					Case 2
						output += "==" ' Two pad chars
						Exit Select
					Case 3
						output += "=" ' One pad char
						Exit Select
					Case Else
						Throw New System.Exception("Illegal base64url string!")
				
				End Select

				Return Convert.FromBase64String(output)

			End Function

			Private Shared Function Sign_Bytes( _
				ByVal key As Byte(), _
				ByVal algorithm As JwtHashAlgorithm, _
				ByVal data_To_Sign As Byte() _
			) As Byte()

				If algorithm = JwtHashAlgorithm.HS256 Then

					Return New HMACSHA256(key).ComputeHash(data_To_Sign)

				ElseIf algorithm = JwtHashAlgorithm.HS384 Then

					Return New HMACSHA384(key).ComputeHash(data_To_Sign)

				ElseIf algorithm = JwtHashAlgorithm.HS512 Then

					Return New HMACSHA512(key).ComputeHash(data_To_Sign)

				Else

					Return New HMACSHA256(key).ComputeHash(data_To_Sign)

				End If

			End Function

		#End Region

		#Region " Public Shared Methods "

			''' <summary>
			''' Creates a JWT given a payload, the signing key, and the algorithm to use.
			''' </summary>
			''' <param name="payload">An arbitrary payload (must be serializable to JSON via <see cref="System.Web.Script.Serialization.JavaScriptSerializer"/>).</param>
			''' <param name="key">The key bytes used to sign the token.</param>
			''' <param name="algorithm">The hash algorithm to use.</param>
			''' <returns>The generated JWT.</returns>
			Public Shared Function Encode( _
				ByVal header As Object, _
				ByVal payload As Object, _
				ByVal key As Byte(), _
				ByVal algorithm As JwtHashAlgorithm _
			) As String

				Dim serializer As New JavaScriptSerializer()

				Dim segments = New List(Of String)

				Dim headerBytes As Byte() = Encoding.UTF8.GetBytes(serializer.Serialize(header))
				Dim payloadBytes As Byte() = Encoding.UTF8.GetBytes(serializer.Serialize(payload))

				segments.Add(Base64_UrlEncode(headerBytes))
				segments.Add(Base64_UrlEncode(payloadBytes))

				Dim stringToSign = String.Join(".", segments.ToArray())

				Dim bytesToSign = Encoding.UTF8.GetBytes(stringToSign)

				Dim signature As Byte() = Sign_Bytes(key, algorithm, bytesToSign)

				segments.Add(Base64_UrlEncode(signature))

				Return String.Join(".", segments.ToArray())

			End Function

			''' <summary>
			''' Given a JWT, decode it and return the JSON payload.
			''' </summary>
			''' <param name="token">The JWT.</param>
			''' <param name="key">The key bytes that were used to sign the JWT.</param>
			''' <param name="verify">Whether to verify the signature (default is true).</param>
			''' <returns>A string containing the JSON payload.</returns>
			''' <exception cref="SignatureVerificationException">Thrown if the verify parameter was true and the signature was NOT valid or if the JWT was signed with an unsupported algorithm.</exception>
			Public Shared Function Decode( _
				ByVal token As String, _
				ByVal key As Byte(), _
				Optional ByVal verify As Boolean = True, _
				Optional serializer As JavaScriptSerializer = Nothing _
			) As String

				If serializer Is Nothing Then serializer = New JavaScriptSerializer()

				Dim parts = token.Split("."C)
				Dim header = parts(0)
				Dim payload = parts(1)
				Dim crypto As Byte() = Base64_UrlDecode(parts(2))

				Dim headerJson = Encoding.UTF8.GetString(Base64_UrlDecode(header))
				Dim headerData = serializer.Deserialize(Of Dictionary(Of String, Object))(headerJson)
				Dim payloadJson = Encoding.UTF8.GetString(Base64_UrlDecode(payload))

				If verify Then

					Dim bytesToSign = Encoding.UTF8.GetBytes(String.Concat(header, ".", payload))
					Dim _algorithm = DirectCast(headerData("alg"), String)
					Dim algorithm As JwtHashAlgorithm

					Select Case _algorithm
						Case "HS256"
							algorithm = JwtHashAlgorithm.HS256
						Case "HS384"
							algorithm = JwtHashAlgorithm.HS384
						Case "HS512"
							algorithm = JwtHashAlgorithm.HS512
						Case Else
							Throw New SignatureVerificationException("Algorithm not supported.")
					End Select

					Dim signature As Byte() = Sign_Bytes(key, algorithm, bytesToSign)

					Dim decodedCrypto = Convert.ToBase64String(crypto)
					Dim decodedSignature = Convert.ToBase64String(signature)

					If decodedCrypto <> decodedSignature Then Throw New SignatureVerificationException( _
						String.Format("Invalid signature. Expected {0} got {1}", decodedCrypto, decodedSignature))

				End If

				Return payloadJson

			End Function

			Public Shared Function Decode_To_JSON( _
				ByVal token As String, _
				ByVal key As Byte(), _
				Optional ByVal verify As Boolean = True _
			) As Object

				Dim serializer As New JavaScriptSerializer()
				Dim json_Payload As String = Decode(token, key, verify, serializer)
				Return serializer.Deserialize(Of Dictionary(Of String, Object))(json_Payload)

			End Function

		#End Region

	End Class

	Public Class SignatureVerificationException
		Inherits Exception

		Public Sub New( _
			ByVal message As String _
		)
		
			MyBase.New(message)

		End Sub

	End Class
    
End Namespace