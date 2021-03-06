Namespace Files.Web

	''' <summary></summary>
	''' <autogenerated>Generated from a T4 template. Modifications will be lost, if applicable use a partial class instead.</autogenerated>
	''' <generator-date>01/02/2014 12:17:53</generator-date>
	''' <generator-functions>1</generator-functions>
	''' <generator-source>Hermes_\_Files\_Web\Generated\Server.tt</generator-source>
	''' <generator-template>Text-Templates\Classes\VB_Object.tt</generator-template>
	''' <generator-version>1</generator-version>
	<System.CodeDom.Compiler.GeneratedCode("Hermes_\_Files\_Web\Generated\Server.tt", "1")> _
	Partial Public Class Server
		Inherits System.Object
		Implements System.IDisposable

		#Region " Public Constructors "

			''' <summary>Default Constructor</summary>
			Public Sub New()

				MyBase.New()

				m_Settings = New Config
				m_SV = New Hermes.Authentication.Server
				Post_Construction()

			End Sub

			''' <summary>Parametered Constructor (1 Parameters)</summary>
			Public Sub New( _
				ByVal _Settings As Config _
			)

				MyBase.New()

				m_Settings = _Settings

				m_SV = New Hermes.Authentication.Server
				Post_Construction()

			End Sub

			''' <summary>Parametered Constructor (2 Parameters)</summary>
			Public Sub New( _
				ByVal _Settings As Config, _
				ByVal _SV As Hermes.Authentication.Server _
			)

				MyBase.New()

				m_Settings = _Settings
				SV = _SV

				Post_Construction()

			End Sub

		#End Region

		#Region " Class Plumbing/Interface Code "

			#Region " IDisposable Implementation "

				#Region " Private Variables "

					''' <summary></summary>
					''' <remarks></remarks>
					Private IDisposable_DisposedCalled As System.Boolean

				#End Region

				#Region " Public Methods "

					Public Sub IDisposable_Dispose() Implements IDisposable.Dispose

						If Not IDisposable_DisposedCalled Then

							IDisposable_DisposedCalled = True
						End If

					End Sub

				#End Region

			#End Region

		#End Region

		#Region " Public Constants "

			''' <summary>Public Shared Reference to the Name of the Property: Settings</summary>
			''' <remarks></remarks>
			Public Const PROPERTY_SETTINGS As String = "Settings"

			''' <summary>Public Shared Reference to the Name of the Property: SV</summary>
			''' <remarks></remarks>
			Public Const PROPERTY_SV As String = "SV"

		#End Region

		#Region " Private Variables "

			''' <summary>Private Data Storage Variable for Property: Settings</summary>
			''' <remarks></remarks>
			Private m_Settings As Config

			''' <summary>Private Data Storage Variable for Property: SV</summary>
			''' <remarks></remarks>
			Private m_SV As Hermes.Authentication.Server

		#End Region

		#Region " Public Properties "

			''' <summary>Provides Access to the Property: Settings</summary>
			''' <remarks></remarks>
			Public ReadOnly Property Settings() As Config
				Get
					Return m_Settings
				End Get
			End Property

			''' <summary>Provides Access to the Property: SV</summary>
			''' <remarks></remarks>
			Public Property SV() As Hermes.Authentication.Server
				Get
					Return m_SV
				End Get
				Set(value As Hermes.Authentication.Server)
					m_SV = value
				End Set
			End Property

		#End Region

	End Class

End Namespace