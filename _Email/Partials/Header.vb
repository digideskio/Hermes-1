Imports System.Net

Namespace Email

	Public Partial Class Header

		#Region " Public Methods "

			Public Overrides Function ToString() As String

				Dim header_Builder As New System.Text.StringBuilder()

				Select Case Type

					Case HeaderType.Meta_Name

						If Not String.IsNullOrEmpty(Name) AndAlso Not String.IsNullOrEmpty(Value) Then _
							header_Builder.AppendFormat("<meta name=""{0}"" content=""{1}"" />", Name, Value)

					Case HeaderType.Meta_HTTP_Equiv

						If Not String.IsNullOrEmpty(Name) AndAlso Not String.IsNullOrEmpty(Value) Then _
							header_Builder.AppendFormat("<meta http-equiv=""{0}"" content=""{1}"" />", Name, Value)

					Case HeaderType.Base

						If Not String.IsNullOrEmpty(Name) OrElse Not String.IsNullOrEmpty(Value) Then

							header_Builder.Append("<base ")

							If Not String.IsNullOrEmpty(Name) Then _
								header_Builder.AppendFormat("target=""{0}""", Name)

							If Not String.IsNullOrEmpty(Value) Then _
								header_Builder.AppendFormat("href=""{0}""", Value)

							header_Builder.Append(" />")

						End If

					Case HeaderType.StylesheetLink

						If Not String.IsNullOrEmpty(Name) OrElse Not String.IsNullOrEmpty(Value) Then _
							header_Builder.AppendFormat("<link rel=""stylesheet"" type=""{0}"" href=""{1}"" />", Name, Value)

					Case HeaderType.ScriptLink

						If Not String.IsNullOrEmpty(Value) Then

							Dim script_Type As String = Name
							If String.IsNullOrEmpty(script_Type) Then script_Type = "text/javascript"
							header_Builder.AppendFormat("<script type=""{0}"" src=""{1}""", script_Type, Value)

						End If

					Case HeaderType.Script

						If Not String.IsNullOrEmpty(Value) Then

							Dim script_Type As String = Name
							If String.IsNullOrEmpty(script_Type) Then script_Type = "text/javascript"
							header_Builder.AppendFormat("<script type=""{0}"">{1}</script>", script_Type, Value)

						End If

					Case HeaderType.Style

						If Not String.IsNullOrEmpty(Name) AndAlso Not String.IsNullOrEmpty(Value) Then _
							header_Builder.AppendFormat("<style type=""{0}"">{1}</style>", Name, Value)

					Case HeaderType.Title

						If Not String.IsNullOrEmpty(Value) Then _
							header_Builder.AppendFormat("<title>{0}</title>", Value)

				End Select

				Return header_Builder.ToString()

			End Function

		#End Region

	End Class

End Namespace