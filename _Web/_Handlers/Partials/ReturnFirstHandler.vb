Imports System
Imports System.Collections.Specialized
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls

Namespace Web.Handlers

	Public Class ReturnFirstHandler
		Implements System.Web.IHttpHandler

		#Region " Public Constants "

			Public Const QUERY_BASE As String = "b"

			Public Const QUERY_INDEX As String = "i"

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

				With context

					Dim base_Name = .Request.QueryString(QUERY_BASE)
					base_Name = base_Name.Replace("\","").Replace("/","").Replace(".","")

					Dim directory As New IO.DirectoryInfo(.Server.MapPath(base_Name))

					Dim files As IO.FileInfo() = directory.GetFiles("*.html", IO.SearchOption.TopDirectoryOnly)

					If Not files Is Nothing AndAlso files.Length > 0 Then

						Dim fileNames(files.Length - 1) As String

						For i As Integer = 0 To files.Length - 1

							fileNames(i) = files(i).FullName

						Next

						Array.Sort(fileNames)

						Dim requested_Index As System.Int32 = 0
						Integer.TryParse(.Request.QueryString(QUERY_INDEX), requested_Index)

						While requested_Index < (0 - (fileNames.Length - 1))
							requested_Index += (fileNames.Length - 1)
						End While

						While requested_Index > (fileNames.Length - 1)
							requested_Index -= (fileNames.Length - 1)
						End While

						If requested_Index < 0 Then requested_Index = fileNames.Length + requested_Index

						Dim requested_File As String = fileNames((fileNames.Length - 1) - requested_Index)

						If System.IO.File.Exists(requested_File) Then

							.Response.Clear
							.Response.ContentType = "text/html"
							.Response.TransmitFile(requested_File)
							.Response.Flush
							.Response.End

						End If

					End If

				End With

			End Sub

		#End Region

	End Class

End Namespace