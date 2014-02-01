Namespace Authentication

	''' <summary></summary>
	''' <autogenerated>Generated from a T4 template. Modifications will be lost, if applicable use a partial class instead.</autogenerated>
	''' <generator-date>01/02/2014 12:14:53</generator-date>
	''' <generator-functions>1</generator-functions>
	''' <generator-source>Hermes_\_Authentication\Generated\Cookie.tt</generator-source>
	''' <generator-template>Text-Templates\Classes\VB_Object.tt</generator-template>
	''' <generator-version>1</generator-version>
	<System.CodeDom.Compiler.GeneratedCode("Hermes_\_Authentication\Generated\Cookie.tt", "1")> _
	Partial Public Class Cookie
		Inherits System.Object
		Implements System.IComparable
		Implements System.IFormattable

		#Region " Public Constructors "

			''' <summary>Default Constructor</summary>
			Public Sub New()

				MyBase.New()

				m_Roles = New System.String() {}
			End Sub

			''' <summary>Parametered Constructor (1 Parameters)</summary>
			Public Sub New( _
				ByVal _Id As System.Guid _
			)

				MyBase.New()

				Id = _Id

				m_Roles = New System.String() {}
			End Sub

			''' <summary>Parametered Constructor (2 Parameters)</summary>
			Public Sub New( _
				ByVal _Id As System.Guid, _
				ByVal _Authenticated As System.Boolean _
			)

				MyBase.New()

				Id = _Id
				Authenticated = _Authenticated

				m_Roles = New System.String() {}
			End Sub

			''' <summary>Parametered Constructor (3 Parameters)</summary>
			Public Sub New( _
				ByVal _Id As System.Guid, _
				ByVal _Authenticated As System.Boolean, _
				ByVal _Username As System.String _
			)

				MyBase.New()

				Id = _Id
				Authenticated = _Authenticated
				Username = _Username

				m_Roles = New System.String() {}
			End Sub

			''' <summary>Parametered Constructor (4 Parameters)</summary>
			Public Sub New( _
				ByVal _Id As System.Guid, _
				ByVal _Authenticated As System.Boolean, _
				ByVal _Username As System.String, _
				ByVal _Details As System.String _
			)

				MyBase.New()

				Id = _Id
				Authenticated = _Authenticated
				Username = _Username
				Details = _Details

				m_Roles = New System.String() {}
			End Sub

			''' <summary>Parametered Constructor (5 Parameters)</summary>
			Public Sub New( _
				ByVal _Id As System.Guid, _
				ByVal _Authenticated As System.Boolean, _
				ByVal _Username As System.String, _
				ByVal _Details As System.String, _
				ByVal _Roles As System.String() _
			)

				MyBase.New()

				Id = _Id
				Authenticated = _Authenticated
				Username = _Username
				Details = _Details
				Roles = _Roles

			End Sub

			''' <summary>Parametered Constructor (6 Parameters)</summary>
			Public Sub New( _
				ByVal _Id As System.Guid, _
				ByVal _Authenticated As System.Boolean, _
				ByVal _Username As System.String, _
				ByVal _Details As System.String, _
				ByVal _Roles As System.String(), _
				ByVal _Expires As System.DateTime _
			)

				MyBase.New()

				Id = _Id
				Authenticated = _Authenticated
				Username = _Username
				Details = _Details
				Roles = _Roles
				Expires = _Expires

			End Sub

			''' <summary>Parametered Constructor (7 Parameters)</summary>
			Public Sub New( _
				ByVal _Id As System.Guid, _
				ByVal _Authenticated As System.Boolean, _
				ByVal _Username As System.String, _
				ByVal _Details As System.String, _
				ByVal _Roles As System.String(), _
				ByVal _Expires As System.DateTime, _
				ByVal _Version As System.Int32 _
			)

				MyBase.New()

				Id = _Id
				Authenticated = _Authenticated
				Username = _Username
				Details = _Details
				Roles = _Roles
				Expires = _Expires
				Version = _Version

			End Sub

			''' <summary>Parametered Constructor (8 Parameters)</summary>
			Public Sub New( _
				ByVal _Id As System.Guid, _
				ByVal _Authenticated As System.Boolean, _
				ByVal _Username As System.String, _
				ByVal _Details As System.String, _
				ByVal _Roles As System.String(), _
				ByVal _Expires As System.DateTime, _
				ByVal _Version As System.Int32, _
				ByVal _Authentication_Provider As System.String _
			)

				MyBase.New()

				Id = _Id
				Authenticated = _Authenticated
				Username = _Username
				Details = _Details
				Roles = _Roles
				Expires = _Expires
				Version = _Version
				Authentication_Provider = _Authentication_Provider

			End Sub

			''' <summary>Parametered Constructor (9 Parameters)</summary>
			Public Sub New( _
				ByVal _Id As System.Guid, _
				ByVal _Authenticated As System.Boolean, _
				ByVal _Username As System.String, _
				ByVal _Details As System.String, _
				ByVal _Roles As System.String(), _
				ByVal _Expires As System.DateTime, _
				ByVal _Version As System.Int32, _
				ByVal _Authentication_Provider As System.String, _
				ByVal _Email_Address As System.String _
			)

				MyBase.New()

				Id = _Id
				Authenticated = _Authenticated
				Username = _Username
				Details = _Details
				Roles = _Roles
				Expires = _Expires
				Version = _Version
				Authentication_Provider = _Authentication_Provider
				Email_Address = _Email_Address

			End Sub

			''' <summary>Parametered Constructor (10 Parameters)</summary>
			Public Sub New( _
				ByVal _Id As System.Guid, _
				ByVal _Authenticated As System.Boolean, _
				ByVal _Username As System.String, _
				ByVal _Details As System.String, _
				ByVal _Roles As System.String(), _
				ByVal _Expires As System.DateTime, _
				ByVal _Version As System.Int32, _
				ByVal _Authentication_Provider As System.String, _
				ByVal _Email_Address As System.String, _
				ByVal _RawValue As System.String _
			)

				MyBase.New()

				Id = _Id
				Authenticated = _Authenticated
				Username = _Username
				Details = _Details
				Roles = _Roles
				Expires = _Expires
				Version = _Version
				Authentication_Provider = _Authentication_Provider
				Email_Address = _Email_Address
				RawValue = _RawValue

			End Sub

		#End Region

		#Region " Class Plumbing/Interface Code "

			#Region " IComparable Implementation "

				#Region " Public Methods "

					''' <summary>Comparison Method</summary>
					Public Overridable Function IComparable_CompareTo( _
						ByVal value As System.Object _
					) As System.Int32 Implements System.IComparable.CompareTo

						Dim typed_Value As Cookie = TryCast(value, Cookie)

						If typed_Value Is Nothing Then

							Throw New ArgumentException(String.Format("Value is not of comparable type: {0}", value.GetType.Name), "Value")

						Else

							Dim return_Value As Integer = 0

							Return return_Value

						End If

					End Function

				#End Region

			#End Region

			#Region " IFormattable Implementation "

				#Region " Public Constants "

					''' <summary>Public Shared Reference to the Name of the Property: AsString</summary>
					''' <remarks></remarks>
					Public Const PROPERTY_ASSTRING As String = "AsString"

				#End Region

				#Region " Public Properties "

					''' <summary></summary>
					''' <remarks></remarks>
					Public ReadOnly Property AsString() As System.String
						Get
							Return Me.ToString()
						End Get
					End Property

				#End Region

				#Region " Public Shared Methods "

					Public Shared Function ToString_default( _
						ByVal Username As System.String _
					) As String

						Return String.Format( _
							"{0}", _
							Username)

					End Function

				#End Region

				#Region " Public Methods "

					Public Overloads Overrides Function ToString() As String

						Return Me.ToString(String.Empty, Nothing)

					End Function

					Public Overloads Function ToString( _
						ByVal format As String _
					) As String

						If String.IsNullOrEmpty(format) OrElse String.Compare(format, "default", True) = 0 Then

							Return ToString_default( _
								Username _
							)

						End If

						Return String.Empty

					End Function

					Public Overloads Function ToString( _
						ByVal format As String, _
						ByVal formatProvider As System.IFormatProvider _
					) As String Implements System.IFormattable.ToString

						If String.IsNullOrEmpty(format) OrElse String.Compare(format, "default", True) = 0 Then	

							Return ToString_default( _
								Username _
							)

						End If

						Return String.Empty

					End Function

				#End Region

			#End Region

		#End Region

		#Region " Public Constants "

			''' <summary>Public Shared Reference to the Name of the Property: Id</summary>
			''' <remarks></remarks>
			Public Const PROPERTY_ID As String = "Id"

			''' <summary>Public Shared Reference to the Name of the Property: Authenticated</summary>
			''' <remarks></remarks>
			Public Const PROPERTY_AUTHENTICATED As String = "Authenticated"

			''' <summary>Public Shared Reference to the Name of the Property: Username</summary>
			''' <remarks></remarks>
			Public Const PROPERTY_USERNAME As String = "Username"

			''' <summary>Public Shared Reference to the Name of the Property: Details</summary>
			''' <remarks></remarks>
			Public Const PROPERTY_DETAILS As String = "Details"

			''' <summary>Public Shared Reference to the Name of the Property: Roles</summary>
			''' <remarks></remarks>
			Public Const PROPERTY_ROLES As String = "Roles"

			''' <summary>Public Shared Reference to the Name of the Property: Expires</summary>
			''' <remarks></remarks>
			Public Const PROPERTY_EXPIRES As String = "Expires"

			''' <summary>Public Shared Reference to the Name of the Property: Version</summary>
			''' <remarks></remarks>
			Public Const PROPERTY_VERSION As String = "Version"

			''' <summary>Public Shared Reference to the Name of the Property: Authentication_Provider</summary>
			''' <remarks></remarks>
			Public Const PROPERTY_AUTHENTICATION_PROVIDER As String = "Authentication_Provider"

			''' <summary>Public Shared Reference to the Name of the Property: Email_Address</summary>
			''' <remarks></remarks>
			Public Const PROPERTY_EMAIL_ADDRESS As String = "Email_Address"

			''' <summary>Public Shared Reference to the Name of the Property: RawValue</summary>
			''' <remarks></remarks>
			Public Const PROPERTY_RAWVALUE As String = "RawValue"

		#End Region

		#Region " Private Variables "

			''' <summary>Private Data Storage Variable for Property: Id</summary>
			''' <remarks></remarks>
			Private m_Id As System.Guid

			''' <summary>Private Data Storage Variable for Property: Authenticated</summary>
			''' <remarks></remarks>
			Private m_Authenticated As System.Boolean

			''' <summary>Private Data Storage Variable for Property: Username</summary>
			''' <remarks></remarks>
			Private m_Username As System.String

			''' <summary>Private Data Storage Variable for Property: Details</summary>
			''' <remarks></remarks>
			Private m_Details As System.String

			''' <summary>Private Data Storage Variable for Property: Roles</summary>
			''' <remarks></remarks>
			Private m_Roles As System.String()

			''' <summary>Private Data Storage Variable for Property: Expires</summary>
			''' <remarks></remarks>
			Private m_Expires As System.DateTime

			''' <summary>Private Data Storage Variable for Property: Version</summary>
			''' <remarks></remarks>
			Private m_Version As System.Int32

			''' <summary>Private Data Storage Variable for Property: Authentication_Provider</summary>
			''' <remarks></remarks>
			Private m_Authentication_Provider As System.String

			''' <summary>Private Data Storage Variable for Property: Email_Address</summary>
			''' <remarks></remarks>
			Private m_Email_Address As System.String

			''' <summary>Private Data Storage Variable for Property: RawValue</summary>
			''' <remarks></remarks>
			Private m_RawValue As System.String

		#End Region

		#Region " Public Properties "

			''' <summary>Provides Access to the Property: Id</summary>
			''' <remarks></remarks>
			Public Property Id() As System.Guid
				Get
					Return m_Id
				End Get
				Set(value As System.Guid)
					m_Id = value
				End Set
			End Property

			''' <summary>Provides Access to the Property: Authenticated</summary>
			''' <remarks></remarks>
			Public Property Authenticated() As System.Boolean
				Get
					Return m_Authenticated
				End Get
				Set(value As System.Boolean)
					m_Authenticated = value
				End Set
			End Property

			''' <summary>Provides Access to the Property: Username</summary>
			''' <remarks></remarks>
			Public Property Username() As System.String
				Get
					Return m_Username
				End Get
				Set(value As System.String)
					m_Username = value
				End Set
			End Property

			''' <summary>Provides Access to the Property: Details</summary>
			''' <remarks></remarks>
			Public Property Details() As System.String
				Get
					Return m_Details
				End Get
				Set(value As System.String)
					m_Details = value
				End Set
			End Property

			''' <summary>Provides Access to the Property: Roles</summary>
			''' <remarks></remarks>
			Public Property Roles() As System.String()
				Get
					Return m_Roles
				End Get
				Set(value As System.String())
					m_Roles = value
				End Set
			End Property

			''' <summary>Provides Access to the Property: Expires</summary>
			''' <remarks></remarks>
			Public Property Expires() As System.DateTime
				Get
					Return m_Expires
				End Get
				Set(value As System.DateTime)
					m_Expires = value
				End Set
			End Property

			''' <summary>Provides Access to the Property: Version</summary>
			''' <remarks></remarks>
			Public Property Version() As System.Int32
				Get
					Return m_Version
				End Get
				Set(value As System.Int32)
					m_Version = value
				End Set
			End Property

			''' <summary>Provides Access to the Property: Authentication_Provider</summary>
			''' <remarks></remarks>
			Public Property Authentication_Provider() As System.String
				Get
					Return m_Authentication_Provider
				End Get
				Set(value As System.String)
					m_Authentication_Provider = value
				End Set
			End Property

			''' <summary>Provides Access to the Property: Email_Address</summary>
			''' <remarks></remarks>
			Public Property Email_Address() As System.String
				Get
					Return m_Email_Address
				End Get
				Set(value As System.String)
					m_Email_Address = value
				End Set
			End Property

			''' <summary>Provides Access to the Property: RawValue</summary>
			''' <remarks></remarks>
			Public Property RawValue() As System.String
				Get
					Return m_RawValue
				End Get
				Set(value As System.String)
					m_RawValue = value
				End Set
			End Property

		#End Region

	End Class

End Namespace