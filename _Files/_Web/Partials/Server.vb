Imports Hermes.Files
Imports Hermes.Cryptography.Cipher
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Web

Namespace Files.Web

	Public Class Server

		#Region " Private Shared Constants "

			' -- Query String Key Names --
			Private Const QUERY_PROVIDER As String = "p"
			
			Private Const QUERY_CONTAINERS As String = "c"
			
			Private Const QUERY_DOCUMENT As String = "d"

		#End Region

		#Region " Public Shared Constants "

			' -- Configuration Key Names --
			Public Const CONFIGURATION_PROVIDERS As String = "Hermes.Files.Providers"

		#End Region

		#Region " Private Variables "

		#End Region

		#Region " Post Construction "

			Private ReadOnly Property _Log As Boolean
				Get
					If m_Log Is Nothing AndAlso Not String.IsNullOrEmpty(Settings.Log_Path) Then _
						m_Log = New System.IO.StreamWriter(String.Format(Settings.Log_Path, "server", Guid.NewGuid.ToString()), True)
					Return Not String.IsNullOrEmpty(Settings.Log_Path)
				End Get
			End Property

			Private m_Log As System.IO.StreamWriter

			Private Sub Post_Construction()

				If _Log Then

					m_Log.WriteLine("-- Log Started --")
					m_log.WriteLine(String.Format("Server Created: {0}", DateTime.Now.ToString()))
					m_log.WriteLine(String.Format("Configuration Has {0} Providers", Settings.Providers.Length))
					m_log.Flush()

				End If

			End Sub

			Private Sub Close_Log()

				If _Log

					m_log.WriteLine(String.Format("Server Disposed: {0}", DateTime.Now.ToString()))
					m_log.Flush()
					m_Log.Close()

				End If

			End Sub

		#End Region

		#Region " Private Methods "

			Private Function Get_Container( _
				ByVal provider As IContainerDocumentProvider, _
				ByVal names As System.String() _
			) As Container

				Dim return_Container As Container = Nothing

				For i As Integer = 0 To names.Length - 1

					If Not String.IsNullOrEmpty(names(i)) Then _
						return_Container = provider.Get_Container(names(i), return_Container)

				Next

				Return return_Container

			End Function

		#End Region

		#Region " Friend Methods "

		#End Region

		#Region " Public Methods "

			Public Function Get_Containers( _
				ByVal provider As System.String, _
				Optional ByVal parents As System.String() = Nothing _
			) As Container()

				If Not SV.Authenticated Then Throw New UnauthorizedAccessException()

				Dim return_List As New List(Of Container)

				Dim _provider As IContainerDocumentProvider = Settings.Provider(provider)

				If Settings.Authorised(SV, _provider.Roles) Then

					Dim parent_Container As Container = Nothing
					If Not parents Is Nothing AndAlso parents.length > 0 Then _
						parent_Container = Get_Container(_provider, parents)

					return_List.AddRange(_provider.Get_Containers(parent_Container))

				Else

					Throw New UnauthorizedAccessException()
					
				End If

				Return return_List.ToArray()

			End Function

			''' <summary></summary>
			''' param name="provider">The Name of the provider to be used</summary>
			''' <param name="parent">The Parent Container</summary>
			''' <returns></returns>
			Public Function Get_Documents( _
				ByVal provider As System.String, _
				Optional Byval parents As System.String() = Nothing _
			) As Document()

				If Not SV.Authenticated Then Throw New UnauthorizedAccessException()

				Dim return_List As New List(Of Document)

				Dim _provider As IContainerDocumentProvider = Settings.Provider(provider)

				If Settings.Authorised(SV, _provider.Roles) Then

					Dim parent_Container As Container = Nothing

					If Not parents Is Nothing AndAlso parents.Length = 1 AndAlso parents(0).StartsWith("%%") Then

						return_List.AddRange(_provider.Get_Documents(parents(0).SubString(2).Replace(".", "")))

					Else

						If Not parents Is Nothing AndAlso parents.length > 0 Then _
							parent_Container = Get_Container(_provider, parents)

						return_List.AddRange(_provider.Get_Documents(parent_Container))

					End If
					
				Else

					Throw New UnauthorizedAccessException()

				End If

				Return return_List.ToArray()

			End Function

			Public Function Get_Data( _
				ByVal provider As System.String, _
				Byval parents As System.String(), _
				ByVal name As System.String _
			) As Byte()

				If Not SV.Authenticated Then Throw New UnauthorizedAccessException()

				Dim _provider As IContainerDocumentProvider = Settings.Provider(provider)

				If Settings.Authorised(SV, _provider.Roles) Then

					Dim parent_Container As Container = Nothing
					If Not parents Is Nothing AndAlso parents.length > 0 Then _
						parent_Container = Get_Container(_provider, parents)

					If _Log Then
						If parent_Container Is Nothing Then m_log.WriteLine("NULL Container") Else m_log.WriteLine("Fine Container")
						m_log.Flush()
					End If

					Dim _document As Document = _provider.Get_Document(name, parent_Container)

					If _Log Then
						If _document Is Nothing Then m_log.WriteLine("NULL Document") Else m_log.WriteLine("Fine Document")
						m_log.Flush()
					End If

					If Not _document Is Nothing Then Return _provider.Get_Data(_document)
					
				Else

					Throw New UnauthorizedAccessException()

				End If

				Return Nothing

			End Function

		#End Region

	End Class

End Namespace