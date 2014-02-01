Namespace Files

	Public Interface IContainerDocumentProvider

		#Region " Properties "

			Property Name As String

			Property Support_Interaction As Boolean

			Property Roles As String()

		#End Region

		#Region " Configuration Methods "

			Function Configure( _
				ByVal values As List(Of String) _
			) As Boolean

		#End Region

		#Region " Provider Methods "

			Function Get_Containers( _
				ByVal parent As Container _
			) As Container()

			Function Get_Container( _
				ByVal name As String, _
				ByVal parent As Container _
			) As Container

			Function Get_Documents( _
				ByVal parent As Container _
			) As Document()

			Function Get_Documents( _
				ByVal container_Search As String _
			) As Document()

			Function Get_Document( _
				ByVal name As String, _
				ByVal parent As Container _
			) As Document

			Function Get_Data( _
				ByVal file As Document _
			) As Byte()

		#End Region

		#Region " Interaction Methods "
		
			Function Create_Container( _
				ByVal name As String, _
				ByVal parent As Container _
			) As Boolean

			Function Delete_Container( _
				ByVal name As String, _
				ByVal parent As Container _
			) As Boolean

			Function Rename_Container( _
				ByVal name As String, _
				ByVal rename_To As String, _
				ByVal parent As Container _
			) As Boolean

		#End Region

	End Interface

End Namespace