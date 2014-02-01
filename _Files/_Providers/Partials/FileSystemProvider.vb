Imports Hermes.Files
Imports System.IO
Imports System.Linq

Namespace Files.Providers

	Public Partial Class FileSystemProvider
		Implements IContainerDocumentProvider

		#Region " Private Shared Methods "

			Private Shared Function Descendent_Documents( _
				ByVal root As DirectoryInfo, _
				ByVal value As DirectoryInfo, _
				ByVal filters As System.String(), _
				ByVal provider_Name As String, _
				Optional ByVal _sort As SortType = SortType.Last_Modified _
			) As Document()

				Dim return_List As New List(Of Document)

				return_List.AddRange(Get_Documents(value, filters, Make_Container(root, value), provider_Name, _sort))

				For Each child As DirectoryInfo in Sort_Directory_Array(value.GetDirectories(), _sort)

					return_List.AddRange(Descendent_Documents(root, child, filters, provider_Name, _sort))

				Next

				Return return_List.ToArray()

			End Function

		#End Region

		#Region " Friend Shared Methods "

			Friend Shared Function Sort_File_Array( _
				ByRef value As FileInfo(), _
				ByVal _sort As SortType _
			) As System.Linq.IOrderedEnumerable(Of FileInfo)

				If _sort = SortType.Last_Modified Then

					Return value.OrderByDescending(Function(f) f.LastWriteTime)

				ElseIf _sort = SortType.By_Name Then

					Return value.OrderBy(Function(f) f.Name)

				ElseIf _sort = SortType.By_Name_Desc Then

					Return value.OrderByDescending(Function(f) f.Name)

				Else

					Return value.OrderByDescending(Function(f) f.LastWriteTime)

				End If

			End Function
 
			Friend Shared Function Sort_Directory_Array( _
				ByRef value As DirectoryInfo(), _
				ByVal _sort As SortType _
			) As System.Linq.IOrderedEnumerable(Of DirectoryInfo)

				If _sort = SortType.Last_Modified Then

					Return value.OrderByDescending(Function(d) d.LastWriteTime)

				ElseIf _sort = SortType.By_Name Then

					Return value.OrderBy(Function(d) d.Name)

				ElseIf _sort = SortType.By_Name_Desc Then

					Return value.OrderByDescending(Function(d) d.Name)

				Else

					Return value.OrderByDescending(Function(d) d.LastWriteTime)

				End If

			End Function

			Friend Shared Function Make_Container( _
				ByVal parent As DirectoryInfo, _
				ByVal child As DirectoryInfo _
			) As Container

				If Not parent Is Nothing AndAlso Not child Is Nothing

					Dim path_Delta As String = child.FullName.Substring(parent.FullName.Length)

					If Not String.IsNullOrEmpty(path_Delta) Then

						Dim ret_Value As Container = Nothing
						Dim current_Value As Container = Nothing

						Dim path_Deltas As String() = path_Delta.Split(New Char() {"\"C}, StringSplitOptions.RemoveEmptyEntries)
						For i As Integer = path_Deltas.Length - 1 To 0 Step -1
							Dim l_Container = New Container(path_Deltas(i))
							If ret_Value Is Nothing Then
								ret_Value = l_Container
							Else
								current_Value.Parent = l_Container
							End If
							current_Value = l_Container
						Next

						Return ret_Value

					End If

				End If

				Return Nothing

			End Function

			Friend Shared Function Get_Root( _
				ByVal parent As Container, _
				ByVal root As String _
			) As String

				Dim _parent As String = Nothing
				While Not parent Is Nothing
					If String.IsNullOrEmpty(_parent) Then
						_parent = parent.Name
					Else
						_parent = System.IO.Path.Combine(parent.Name, _parent)
					End If
					parent = parent.Parent
				End While

				If String.IsNullOrEmpty(_parent) Then
					Return root
				Else
					Return Path.Combine(root, _parent)
				End If

			End Function

			Friend Shared Function Can_Download( _
				ByVal file As String, _
				ByVal filters As String() _
			) As Boolean

				If Not String.IsNullOrEmpty(file) Then

					If Not filters Is Nothing AndAlso filters.Length > 0 Then

						Dim file_Extension As String = file.Substring(file.LastIndexOf(".") + 1)

						If String.IsNullOrEmpty(file_Extension) Then file_Extension = "" Else file_Extension = file_Extension.ToLower()

						For Each filter As String In filters

							Dim root_Ext As String = filter.Split("."C)(1)
							If root_Ext  = Web.Config.WILDCARD OrElse file_Extension.EndsWith(root_Ext.ToLower()) Then
								Return True
								Exit For
							End If

						Next

						Return False

					Else

						Return True

					End If

				End If

				Return False

			End Function

			Friend Shared Function Get_Containers( _
				ByVal parent As DirectoryInfo, _
				ByVal _parent As Container, _
				Optional ByVal _sort As SortType = SortType.Last_Modified _
			) As Container()

				Dim return_List As New List(Of Container)

				For Each child As DirectoryInfo in Sort_Directory_Array(parent.GetDirectories(), _sort)

					return_List.Add(New Container(child.Name, _parent))

				Next

				Return return_List.ToArray()

			End Function

			Friend Shared Function Get_Container( _
				ByVal name As String, _
				ByVal parent As String, _
				ByVal _parent As Container _
			) As Container

				Dim named_Directory As DirectoryInfo

				If String.IsNullOrEmpty(name) Then
					named_Directory = New DirectoryInfo(parent)
				Else
					named_Directory = New DirectoryInfo(Path.Combine(parent, name))
				End If

				If named_Directory.Exists Then
					Return New Container(named_Directory.Name, _parent)
				Else
					Return Nothing
				End If

			End Function

			Friend Shared Function Get_Documents( _
				ByVal parent As DirectoryInfo, _
				ByVal filters As System.String(), _
				ByVal _parent As Container, _
				ByVal provider_Name As String, _
				Optional ByVal _sort As SortType = SortType.Last_Modified _
			) As Document()

				Dim return_List As New List(Of Document)

				If filters Is Nothing OrElse filters.Length = 0 OrElse _
					(filters.Length = 1 AndAlso String.IsNullOrEmpty(filters(0))) Then

					For Each child As FileInfo In Sort_File_Array(parent.GetFiles(), _sort)

						return_List.Add(New Document(child.Name, child.Extension, child.LastWriteTime, provider_Name, _parent))

					Next

				Else

					For i As Integer = 0 To filters.Length - 1

						For Each child As FileInfo In Sort_File_Array(parent.GetFiles(filters(i)), _sort)

							return_List.Add(New Document(child.Name, child.Extension, child.LastWriteTime, provider_Name, _parent))

						Next

					Next

				End If

				Return return_List.ToArray()

			End Function

			Friend Shared Function Search_Documents( _
				ByVal parent As DirectoryInfo, _
				ByVal root As DirectoryInfo, _
				ByVal filters As System.String(), _
				ByVal _parent_Search As String, _
				ByVal provider_Name As String, _
				Optional ByVal _sort As SortType = SortType.Last_Modified _
			) As Document()

				Dim return_List As New List(Of Document)

				For Each value As DirectoryInfo In Sort_Directory_Array(parent.GetDirectories(_parent_Search, SearchOption.AllDirectories), _sort)

					return_List.AddRange(Descendent_Documents(root, value, filters, provider_Name, _sort))

				Next

				Return return_List.ToArray()

			End Function

			Friend Shared Function Get_Document( _
				ByVal name As String, _
				ByVal parent As String, _
				ByVal _parent As Container, _
				ByVal provider_Name As String _
			) As Document

				If Not String.IsNullOrEmpty(name) Then

					Dim named_File As FileInfo = New FileInfo(Path.Combine(parent, name))

					If named_File.Exists Then _
						Return New Document(named_File.Name, named_File.Extension, named_File.LastWriteTime, provider_Name, _parent)
						
				End If

				Return Nothing

			End Function

			Friend Shared Function Get_Data( _
				ByVal name As String, _
				ByVal parent As String, _
				ByVal filters As String(), _
				ByVal _sv As Hermes.Authentication.Server _
			) As Byte()

				Dim file_Path As String = System.IO.Path.Combine(parent, name)

				If Not FileSystemProvider.Can_Download(file_Path, filters) Then Throw New UnauthorizedAccessException()

				If System.IO.File.Exists(file_Path) Then

					_sv.Log(New Hermes.Authentication.Event("Download", String.Format("Document {0} Downloaded From {1}", name, parent)))

					Dim file_Stream As FileStream = Nothing

					Try
						file_Stream = System.IO.File.OpenRead(file_Path)
						Dim file_Length As Integer = CInt(file_Stream.Length)

						Dim file_Data(file_Length - 1) As Byte
						file_Stream.Read(file_Data, 0, file_Length)
						file_Stream.Close()
						file_Stream = Nothing

						Return file_Data

					Catch ex As Exception
						If Not file_Stream Is Nothing Then file_Stream.Close()
					End Try

				End If

				Return Nothing

			End Function

		#End Region

		#Region " IContainerDocumentProvider Implementation "

			Public Function Configure( _
				ByVal values As List(Of String) _
			) As Boolean Implements IContainerDocumentProvider.Configure

				If values.Count > 0 Then

					Root_Path = values(0)

					If values.Count > 1 AndAlso Not String.IsNullOrEmpty(values(1)) Then
						If values(1).IndexOf(";") > 0 Then
							File_Filters = values(1).Split(New Char() {";"c}, StringSplitOptions.RemoveEmptyEntries)
						Else
							File_Filters = New String(){values(1)}
						End If
					Else
						File_Filters = New String(){}
					End If

					If values.Count > 2 AndAlso Not String.IsNullOrEmpty(values(2)) Then
						If values(2).IndexOf(";") > 0 Then
							Required_Roles = values(2).Split(New Char() {";"c}, StringSplitOptions.RemoveEmptyEntries)
						Else
							Required_Roles = New String(){values(2)}
						End If
					Else
						Required_Roles = New String(){Web.Config.WILDCARD}
					End If

					If values.Count > 3 AndAlso Not String.IsNullOrEmpty(values(3)) Then

						Dim parsed_Sort_Type As SortType

						If [Enum].TryParse(values(3), True, parsed_Sort_Type) Then Sort_Type = parsed_Sort_Type

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

				Return FileSystemProvider.Get_Containers( _
					New DirectoryInfo(FileSystemProvider.Get_Root(parent, Me.Root_Path)), parent, Me.Sort_Type _
				)

			End Function

			Public Function Get_Container( _
				ByVal name As String, _
				ByVal parent As Container _
			) As Container _
			Implements IContainerDocumentProvider.Get_Container

				Return FileSystemProvider.Get_Container(name, _
					FileSystemProvider.Get_Root(parent, Me.Root_Path), parent _
				)

			End Function

			Public Function Get_Documents( _
				ByVal parent As Container _
			) As Document() _
			Implements IContainerDocumentProvider.Get_Documents

				Return FileSystemProvider.Get_Documents( _
					New DirectoryInfo(FileSystemProvider.Get_Root(parent, Me.Root_Path)), File_Filters, parent, Me.Name, Me.Sort_Type _
				)

			End Function

			Public Function Get_Documents( _
				ByVal container_Search As String _
			) As Document() _
			Implements IContainerDocumentProvider.Get_Documents

				Dim parent As DirectoryInfo = New DirectoryInfo(FileSystemProvider.Get_Root(Nothing, Me.Root_Path))
				Return FileSystemProvider.Search_Documents(parent, parent, File_Filters, container_Search, Me.Name, Me.Sort_Type)

			End Function

			Public Function Get_Document( _
				ByVal name As String, _
				ByVal parent As Container _
			) As Document _
			Implements IContainerDocumentProvider.Get_Document

				Return FileSystemProvider.Get_Document(name, _
					FileSystemProvider.Get_Root(parent, Me.Root_Path), parent, Me.Name _
				)

			End Function

			Public Function Get_Data( _
				ByVal file As Document _
			) As Byte() _
			Implements IContainerDocumentProvider.Get_Data

				Return FileSystemProvider.Get_Data( _
					file.Name, _
					FileSystemProvider.Get_Root(file.Parent, Root_Path), _
					File_Filters, SV _
				)

			End Function

			Public Function Create_Container( _
				ByVal name As String, _
				ByVal parent As Container _
			) As Boolean _
			Implements IContainerDocumentProvider.Create_Container

				Throw New NotImplementedException

			End Function

			Public Function Delete_Container( _
				ByVal name As String, _
				ByVal parent As Container _
			) As Boolean _
			Implements IContainerDocumentProvider.Delete_Container

				Throw New NotImplementedException

			End Function

			Public Function Rename_Container( _
				ByVal name As String, _
				ByVal rename_To As String, _
				ByVal parent As Container _
			) As Boolean _
			Implements IContainerDocumentProvider.Rename_Container

				Throw New NotImplementedException

			End Function

		#End Region

	End Class

End Namespace