Imports System.Text
Imports System.Web

Namespace Web

	Partial Public Class Url
		Implements ICloneable

		#Region " Public Constants "

			''' <summary></summary>
			Public Const QUERY_START As Char = "?"c

			''' <summary></summary>
			Public Const QUERY_DELINEATOR As Char= "&"c

			''' <summary></summary>
			Public Const QUERY_OPERATOR As Char = "="c

			''' <summary></summary>
			Public Const PATH_DELINEATOR As Char = "/"c

			''' <summary></summary>
			Public Const BOOKMARK_DELINEATOR As Char = "#"c

		#End Region

		#Region " Private Variables "

			' This stops the Web Tree View Query Strings screwing things up!
			Private REMOVED_QUERYVARIABLES As String() = New String() {"_wtv"}

		#End Region

		#Region " Public ReadOnly Properties "

			Public ReadOnly Property Build_Full() As String
				Get
					Return BuildUrl(Base)
				End Get
			End Property

			Public ReadOnly Property Build_Relative() As String
				Get
					Return BuildUrl(Relative)
				End Get
			End Property

		#End Region

		#Region " Private Methods "

			Private Function BuildUrl( _
				ByVal urlBase As String _
			) As String

				Dim url_Builder As New StringBuilder

				url_Builder.Append(urlBase)

				AddQueries(url_Builder)

				If Not Bookmark = Nothing AndAlso Bookmark.Length > 0 Then
					If Not Bookmark.StartsWith(BOOKMARK_DELINEATOR) Then url_Builder.Append(BOOKMARK_DELINEATOR)
					url_Builder.Append(Bookmark.TrimEnd)
				End If

				Return url_Builder.ToString

			End Function

			Private Sub AddQueries( _
				ByRef url_Builder As StringBuilder _
			)

				If Queries.Count > 0 Then

					url_Builder.Append(QUERY_START)

					For Each dict As DictionaryEntry In Queries
						url_Builder.Append(dict.Key).Append(QUERY_OPERATOR) _
							.Append(dict.Value).Append(QUERY_DELINEATOR)
					Next

					url_Builder.Remove(url_Builder.Length - 1, 1)

				End If

			End Sub

		#End Region

		#Region " Public Methods "

			Public Function Update_Params( _
				ByVal ParamArray parametersAndValues As DictionaryEntry() _
			) As Url

				If Not parametersAndValues Is Nothing Then
					For i As Integer = 0 To parametersAndValues.Length - 1
						Update_Param(CStr(parametersAndValues(i).Key), CStr(parametersAndValues(i).Value))
					Next
				End If

				Return Me

			End Function

			Public Function Update_Param( _
				ByVal parameter As String, _
				ByVal value As String _
			) As Url

				If Not String.IsNullOrEmpty(value) AndAlso Not Server Is Nothing Then _
					value = Server.UrlEncode(value)

				If Queries.Contains(parameter) Then

					Queries.Item(parameter) = value

				Else

					Queries.Add(parameter, value)

				End If

				Return Me

			End Function

			Public Function Remove_Param( _
				ByVal parameter As String _
			) As Url

				If Queries.Contains(parameter) Then Queries.Remove(parameter)

				Return Me

			End Function

			Public Function Clear_Params() As Url

				Queries.Clear()

				Return Me

			End Function

			Public Sub ChangePage( _
				ByVal newPage As String _
			)

				If Not newPage = Nothing AndAlso newPage.Length > 0 Then

					m_Relative = ChangePage(Relative, newPage)
					m_Base = ChangePage(Base, newPage)

				End If

			End Sub

			Public Sub TransferToPage( _
				ByRef context As HttpContext _
			)

				context.Server.Transfer(Build_Relative)

			End Sub

			Public Sub RedirectToPage( _
				ByVal context As HttpContext _
			)

				context.Response.Redirect(Build_Relative, True)

			End Sub

			Public Overrides Function ToString() As String

				Return Relative

			End Function

		#End Region

		#Region " Private Shared Methods "

			Private Shared Function ChangePage( _
				ByVal url As String, _
				ByVal newPage As String _
			) As String

				Dim parts() As String = url.Split(PATH_DELINEATOR)
				Dim oldPage As String = parts(parts.Length - 1)

				Dim pos As Integer = url.LastIndexOf(oldPage)

				url = url.Remove(pos, oldPage.Length)
				url = url.Insert(pos, newPage)

				Return url

			End Function

		#End Region

		#Region " Public Shared Parse Methods "

			Public Shared Function Parse( _
				ByVal value As HttpContext _
			) As Url

				Return Parse(value, Nothing)

			End Function

			Public Shared Function Parse( _
				ByVal value As HttpContext, _
				ByVal ParamArray parameters As DictionaryEntry() _
			) As Url

				Return Parse(value.Request.RawUrl, value.Server, parameters)

			End Function

			Public Shared Function Parse( _
				ByVal value As String _
			) As Url

				Return Parse(value, Nothing, Nothing)

			End Function

			Public Shared Function Parse( _
				ByVal value As String, _
				ByVal server As HttpServerUtility, _
				ByVal ParamArray parameters As DictionaryEntry() _
			) As Url

				If String.IsNullOrEmpty(value) Then

						Throw New ArgumentException("Value must be present", "value")

				Else

					Dim _Url As New Url()

					_Url.m_Server = server

					If value.Contains(QUERY_START) Then
						_Url.m_Base = value.Split(QUERY_START)(0)
					Else
						_Url.m_Base = value
					End If

					If _Url.Base.Contains(PATH_DELINEATOR) Then
						_Url.m_Relative = _Url.Base.Split(PATH_DELINEATOR)(_Url.Base.Split(PATH_DELINEATOR).Length - 1)
					Else
						_Url.m_Relative = _Url.Base
					End If

					If value.Contains(QUERY_START) Then

						Dim _Queries As String() = value.Split(QUERY_START)(1).Split(QUERY_DELINEATOR)

						If Not _Queries Is Nothing AndAlso _Queries.Length > 0 Then

							For i As Integer = 0 To _Queries.Length - 1

								Dim query_field As String = _Queries(i).Split(QUERY_OPERATOR)(0)
								Dim query_value As String = _Queries(i).Split(QUERY_OPERATOR)(1)

								Dim ignoreQueryField As Boolean = False

								For j As Integer = 0 To _Url.REMOVED_QUERYVARIABLES.Length - 1

									If query_field.ToLower.StartsWith(_Url.REMOVED_QUERYVARIABLES(j)) Then

										ignoreQueryField = True
										Exit For

									End If

								Next

								If Not ignoreQueryField Then

									If _Url.Queries.Contains(query_field) Then

										_Url.Queries(query_field) = query_value

									Else

										_Url.Queries.Add(query_field, query_value)

									End If

								End If

							Next

						End If

					End If

					_Url.Update_Params(parameters)

					Return _Url

				End If

			End Function

		#End Region

		#Region " Public Shared Methods "

			Public Shared Function BuildSimpleUrl( _
				ByVal basePage As String, _
				ByVal ParamArray parameters() As DictionaryEntry _
			) As String

				If parameters Is Nothing OrElse parameters.Length = 0 Then

					Return basePage

				ElseIf parameters.Length = 1 Then

					Return basePage & QUERY_START.ToString() & parameters(0).Key.ToString() & QUERY_OPERATOR & parameters(0).Value.ToString()

				Else

					Dim url_Builder As New System.Text.StringBuilder()

					url_Builder.Append(basePage).Append(QUERY_START)

					For i As Integer = 0 To parameters.Length - 1

						url_Builder.Append(parameters(i).Key)
						url_Builder.Append(QUERY_OPERATOR)
						url_Builder.Append(parameters(i).Value)
						If Not i = parameters.Length - 1 Then url_Builder.Append(QUERY_DELINEATOR)

					Next

					Return url_Builder.ToString

				End If

			End Function

			Public Shared Function BuildSimpleUrl( _
				ByVal basePage As String, _
				ByVal bookmark As String, _
				ByVal ParamArray parameters() As DictionaryEntry _
			) As String

				Return BuildSimpleUrl(basePage, parameters) & BOOKMARK_DELINEATOR & bookmark

			End Function

			Public Shared Function GetNewUrl( _
				ByVal context As HttpContext, _
				ByVal page As String _
			) As String

				Dim currentUrl As Uri = context.Request.Url
				Dim baseUrl As String = currentUrl.AbsoluteUri.Substring(0, currentUrl.AbsoluteUri.LastIndexOf("/"))
				If baseUrl.EndsWith(PATH_DELINEATOR) Then

					Return baseUrl & page

				Else

					If page.StartsWith(PATH_DELINEATOR) Then
						Return baseUrl & page
					Else
						Return baseUrl & PATH_DELINEATOR & page
					End If

				End If

			End Function

			Public Shared Function GetCurrentPage( _
				ByVal context As HttpContext _
			) As String

				Dim currentUrl As Uri = context.Request.Url
				Return currentUrl.LocalPath.Substring(currentUrl.LocalPath.LastIndexOf(PATH_DELINEATOR))

			End Function

		#End Region

		#Region " ICloneable Implementation "

			Public Function Clone() As Object Implements System.ICloneable.Clone

				Return New Url( _
					Base, _
					Relative, _
					Bookmark, _
					CType(Queries.Clone, System.Collections.Hashtable) _
				)

			End Function

		#End Region

	End Class

End Namespace