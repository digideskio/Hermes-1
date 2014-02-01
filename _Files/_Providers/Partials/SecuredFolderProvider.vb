Imports Hermes.Files
Imports System.IO
Imports System.Linq

Namespace Files.Providers

	Public Partial Class SecuredFolderProvider
		Implements IContainerDocumentProvider

		#Region " Private Methods "

			Private Function Root_Paths() As String()

				Dim paths As New List(Of String)

				For Each role As String in SV.Roles

					paths.Add(String.Format(Path_Filter, role))

				Next

				Return paths.ToArray()

			End Function

			Private Function Check_Parent( _
				ByVal parent As Container _
			) As Container

				Dim _parent As Container = parent

				Do Until (_parent Is Nothing OrElse String.IsNullOrEmpty(_parent.Name))

					If _parent.Parent Is Nothing Then

						For Each path As String In Root_Paths

							If path.StartsWith("*") AndAlso path.EndsWith("*") AndAlso _parent.Name.Contains(path.Substring(1, path.Length - 2)) Then

								Return parent

							ElseIf path.StartsWith("*") AndAlso _parent.Name.EndsWith(path.Substring(1)) Then

								Return parent

							ElseIf path.EndsWith("*") AndAlso _parent.Name.StartsWith(path.Substring(0, path.Length - 1)) Then

								Return parent

							End If

						Next

						Throw New UnauthorizedAccessException()

					Else

						_parent = _parent.Parent

					End If

				Loop

				Return parent

			End Function

		#End Region

		#Region " IContainerDocumentProvider Implementation "

			Public Function Configure( _
				ByVal values As List(Of String) _
			) As Boolean _
			Implements IContainerDocumentProvider.Configure

				If values.Count > 0 Then

					Root_Path = values(0)

					If values.Count > 1 Then _
						Path_Filter = values(1)

					If values.Count > 2 Then _
						Sub_Path_Filter = values(2)

					If values.Count > 3 AndAlso Not String.IsNullOrEmpty(values(3)) Then
						If values(3).IndexOf(";") > 0 Then
							File_Filters = values(3).Split(New Char() {";"c}, StringSplitOptions.RemoveEmptyEntries)
						Else
							File_Filters = New String(){values(3)}
						End If
					Else
						File_Filters = New String(){}
					End If

					If values.Count > 4 AndAlso Not String.IsNullOrEmpty(values(4)) Then
						If values(4).IndexOf(";") > 0 Then
							Required_Roles = values(4).Split(New Char() {";"c}, StringSplitOptions.RemoveEmptyEntries)
						Else
							Required_Roles = New String(){values(4)}
						End If
					Else
						Required_Roles = New String(){Web.Config.WILDCARD}
					End If

					If values.Count > 5 AndAlso Not String.IsNullOrEmpty(values(5)) Then

						Dim parsed_Sort_Type As SortType

						If [Enum].TryParse(values(5), True, parsed_Sort_Type) Then Sort_Type = parsed_Sort_Type

					End If

					Return True

				Else

					Return False

				End If

			End Function

			Public Function Get_Containers( _
				ByVal parent As Container _
			) As Container() _
			Implements IContainerDocumentProvider.Get_Containers

				Dim root_Directory As New DirectoryInfo(FileSystemProvider.Get_Root(Check_Parent(parent), Me.Root_Path))

				If parent Is Nothing OrElse String.IsNullOrEmpty(parent.Name) Then

					Dim return_List As New List(Of Container)

					For Each path As String In Root_Paths

						For Each child As DirectoryInfo In FileSystemProvider.Sort_Directory_Array(root_Directory.GetDirectories(path, SearchOption.TopDirectoryOnly), Me.Sort_Type)

							return_List.Add(New Container(child.Name, parent))

						Next

					Next
					
					Return return_List.ToArray()

				ElseIf Not String.IsNullOrEmpty(Sub_Path_Filter) AndAlso Not Sub_Path_Filter = "*" AndAlso parent.Parent Is Nothing Then

					Dim return_List As New List(Of Container)
					
					For Each child As DirectoryInfo In FileSystemProvider.Sort_Directory_Array(root_Directory.GetDirectories(Sub_Path_Filter), Me.Sort_Type)

						return_List.Add(New Container(child.Name, parent))

					Next
					
					Return return_List.ToArray()

				Else

					Return FileSystemProvider.Get_Containers(root_Directory, parent, Sort_Type)

				End If

			End Function

			Public Function Get_Container( _
				ByVal name As String, _
				ByVal parent As Container _
			) As Container _
			Implements IContainerDocumentProvider.Get_Container
				
				Return FileSystemProvider.Get_Container(name, _
					FileSystemProvider.Get_Root(Check_Parent(parent), Me.Root_Path), parent _
				)

			End Function

			Public Function Get_Documents( _
				ByVal parent As Container _
			) As Document() _
			Implements IContainerDocumentProvider.Get_Documents

				' TODO: THIS ISN'T RIGHT // WHY???
				Return FileSystemProvider.Get_Documents( _
					New DirectoryInfo(FileSystemProvider.Get_Root(Check_Parent(parent), Me.Root_Path)), File_Filters, parent, Me.Name, Me.Sort_Type _
				)

			End Function

			Public Function Get_Documents( _
				ByVal container_Search As String _
			) As Document() _
			Implements IContainerDocumentProvider.Get_Documents

				Dim root_Directory As New DirectoryInfo(FileSystemProvider.Get_Root(Check_Parent(Nothing), Me.Root_Path))

				Dim return_List As New List(Of Document)

				For Each path As String In Root_Paths

					For Each child As DirectoryInfo In FileSystemProvider.Sort_Directory_Array(root_Directory.GetDirectories(path, SearchOption.TopDirectoryOnly), Me.Sort_Type)

						return_List.AddRange(FileSystemProvider.Search_Documents(child, root_Directory, File_Filters, container_Search, Me.Name, Me.Sort_Type))

					Next

				Next
					
				Return return_List.ToArray()

			End Function

			Public Function Get_Document( _
				ByVal name As String, _
				ByVal parent As Container _
			) As Document _
			Implements IContainerDocumentProvider.Get_Document

				Return FileSystemProvider.Get_Document(name, _
					FileSystemProvider.Get_Root(Check_Parent(parent), Me.Root_Path), parent, Me.Name _
				)
				
			End Function

			Public Function Get_Data( _
				ByVal file As Document _
			) As Byte() _
			Implements IContainerDocumentProvider.Get_Data
				
				Return FileSystemProvider.Get_Data( _
					file.Name, _
					FileSystemProvider.Get_Root(Check_Parent(file.Parent), Root_Path), _
					File_Filters, SV _
				)
				
			End Function

			Public Function Create_Container( _
				ByVal name As String, _
				ByVal parent As Container _
			)  As Boolean _
			Implements IContainerDocumentProvider.Create_Container

				Throw New NotImplementedException

			End Function

			Public Function Delete_Container( _
				ByVal name As String, _
				ByVal parent As Container _
			)  As Boolean _
			Implements IContainerDocumentProvider.Delete_Container

				Throw New NotImplementedException

			End Function

			Public Function Rename_Container( _
				ByVal name As String, _
				ByVal rename_To As String, _
				ByVal parent As Container _
			)  As Boolean _
			Implements IContainerDocumentProvider.Rename_Container

				Throw New NotImplementedException
				
			End Function

		#End Region

	End Class

End Namespace