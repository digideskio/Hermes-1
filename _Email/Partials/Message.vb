Imports System.Collections
Imports System.Net

Namespace Email

	Public Partial Class Message
		Implements IDisposable

		#Region " Protected Constants "

			Protected Friend Const AUTOGEN_PRECEDENCE As String = "Precedence"

			Protected Friend Const AUTOGEN_AUTOSUBMITTED As String = "Auto-Submitted"

			Protected Friend Const AUTOGEN_SUPPRESSAUTOREPONSE As String = "X-Auto-Response-Suppress"

			Protected Friend Const SENDER As String = "X-Mailer"

		#End Region

		#Region " Public Properties "

			Public ReadOnly Property SubjectLength As System.Int32
				Get
					If Not String.IsNullOrEmpty(Subject) Then
						Return Subject.Length
					Else
						Return 0
					End If
				End Get
			End Property

			Public ReadOnly Property BodyLength As System.Int32
				Get
					If Not Body_Text Is Nothing Then
						Return Body_Text.Length
					Else
						Return 0
					End If
				End Get
			End Property

		#End Region

		#Region " Public Methods "

			Public Function AppendHeader( _
				ByVal header_Type As HeaderType, _
				ByVal header_Name As System.String, _
				ByVal header_Value As System.String _
			) As Message

				Headers.Add(New Header(header_Type, header_Name, header_Value))

				Return Me

			End Function

			Public Function AppendResource( _
				ByVal resource_File As String, _
				ByVal resource_Name As String, _
				Optional ByVal resource_Type As String = Nothing _
			) As Message

				If Not IO.File.Exists(resource_File) Then

					Throw New ArgumentException("Resource File Must Exist", "resource_File")

				Else

					Dim message_Resource As Mail.LinkedResource
					If Not String.IsNullOrEmpty(resource_Type) Then
						message_Resource = New Mail.LinkedResource(resource_File, resource_Type)
					Else
						message_Resource = New Mail.LinkedResource(resource_File)
					End If
					message_Resource.ContentId = resource_Name
					Resources.Add(message_Resource)
					Return Me

				End If

			End Function

			Public Function AppendResource( _
				ByVal resource_Stream As IO.Stream, _
				ByVal resource_Name As String, _
				Optional ByVal resource_Type As String = Nothing _
			) As Message

				If Not resource_Stream.CanRead Then

					Throw New ArgumentException("Resource Stream Must be Readable", "resource_Stream")

				Else

					Dim message_Resource As Mail.LinkedResource
					If Not String.IsNullOrEmpty(resource_Type) Then
						message_Resource = New Mail.LinkedResource(resource_Stream, resource_Type)
					Else
						message_Resource = New Mail.LinkedResource(resource_Stream)
					End If
					message_Resource.ContentId = resource_Name
					Resources.Add(message_Resource)
					Return Me

				End If

			End Function

			Public Function AppendAttachment( _
				ByVal attachment_File As String, _
				Optional ByVal attachment_Name As String = Nothing _
			) As Message

				If Not IO.File.Exists(attachment_File) Then

					Throw New ArgumentException("Attachment File Must Exist", "attachment_File")

				Else

					Dim message_Attachment As New Mail.Attachment(attachment_File)
					If Not String.IsNullOrEmpty(attachment_Name) Then _
						message_Attachment.Name = attachment_Name
					Attachments.Add(message_Attachment)
					Return Me

				End If

			End Function

			Public Function AppendAttachment( _
				ByVal attachment_Stream As IO.Stream, _
				Optional ByVal attachment_Name As String = "Attachment" _
			) As Message

				If Not attachment_Stream.CanRead Then

					Throw New ArgumentException("Attachment Stream Must be Readable", "attachment_Stream")

				Else

					Attachments.Add(New Mail.Attachment(attachment_Stream, attachment_Name))
					Return Me

				End If

			End Function

			Public Function Append( _
				ByVal value As String _
			) As Message

				Body_Text.Append(value)
				Return Me

			End Function

			Public Function AppendLine() As Message

				Body_Text.AppendLine()
				Return Me

			End Function

			Public Function AppendLine( _
				ByVal value As String _
			) As Message

				Body_Text.AppendLine(value)
				Return Me

			End Function

			Public Function AppendFormat( _
				ByVal format As String, _
				ByVal arg0 As Object _
			) As Message

				Body_Text.AppendFormat(format, arg0)
				Return Me

			End Function

			Public Function AppendFormat( _
				ByVal format As String, _
				ByVal arg0 As Object, _
				ByVal arg1 As Object _
			) As Message

				Body_Text.AppendFormat(format, arg0, arg1)
				Return Me

			End Function

			Public Function AppendFormat( _
				ByVal format As String, _
				ByVal arg0 As Object, _
				ByVal arg1 As Object, _
				ByVal arg2 As Object _
			) As Message

				Body_Text.AppendFormat(format, arg0, arg1, arg2)
				Return Me

			End Function

			Public Function AppendFormat( _
				ByVal format As String, _
				ParamArray args As Object() _
			) As Message

				Body_Text.AppendFormat(format, args)
				Return Me

			End Function

			Public Function AppendNamedFormat( _
				ByVal format As String, _
				ByVal args As System.Collections.Generic.Dictionary(Of System.String, System.Object) _
			) As Message

				Body_Text.Append(Message.NamedFormat(format, args))
				Return Me

			End Function

			Public Function ToMailMessage() As Mail.MailMessage

				Dim return_Message As New Mail.MailMessage()

				If AutoGenerated Then

					return_Message.Headers.Add(Message.AUTOGEN_PRECEDENCE, "bulk")
					return_Message.Headers.Add(Message.AUTOGEN_AUTOSUBMITTED, "auto-generated")
					return_Message.Headers.Add(Message.AUTOGEN_SUPPRESSAUTOREPONSE, "All")
					' -- Final Header is Exchange Only --
					' None | DR (delivery receipts) | DR (non-delivery receipts) | RN (read notifications)
					' NRN (not read notifications) | OOF (out of office) | AutoReply | All

				End If

				return_Message.Subject = Subject
				return_Message.BodyEncoding = Body_Encoder

				If Body_Format = BodyType.Html Then

					' -- Trim Off HTML Start/Ends --
					Dim Body_String As String = Body_Text.ToString().Trim()
					If Body_String.StartsWith("<html>", System.StringComparison.OrdinalIgnoreCase) Then _
							Body_String = Body_String.Remove(0, 6).Trim()

					If Body_String.EndsWith("</html>", System.StringComparison.OrdinalIgnoreCase) Then _
						Body_String = Body_String.Remove(Body_String.Length - 7, 7).Trim()

					If Body_String.StartsWith("<body>", System.StringComparison.OrdinalIgnoreCase) Then _
						Body_String = Body_String.Remove(0, 6).Trim()

					If Body_String.EndsWith("</body>", System.StringComparison.OrdinalIgnoreCase) Then _
						Body_String = Body_String.Remove(Body_String.Length - 7, 7).Trim()

					Dim Body_Builder As New System.Text.StringBuilder()
					Body_Builder.AppendLine("<html>")

					' Insert Headers Here
					If Not Headers Is Nothing AndAlso Headers.Count > 0 Then

						Body_Builder.AppendLine("<head>")

						For i As Integer = 0 To Headers.Count - 1

							Dim header_Value As String = Headers(i).ToString()
							If Not String.IsNullOrEmpty(header_Value) Then _
								Body_Builder.AppendLine(header_Value)

						Next

						Body_Builder.AppendLine("</head>")

					End If

					Body_Builder.AppendLine("<body>")
					Body_Builder.AppendLine(Body_String)
					If Not String.IsNullOrEmpty(Footer) Then _
						Body_Builder.Append(Footer)
					Body_Builder.AppendLine("</body>")
					Body_Builder.AppendLine("</html>")

					If Not Resources Is Nothing AndAlso Resources.Count > 0 Then

						Dim view_Builder As Mail.AlternateView = Mail.AlternateView.CreateAlternateViewFromString( _
							Body_Builder.ToString(), New System.Net.Mime.ContentType("text/html"))

						For i As Integer = 0 To Resources.Count - 1

							view_Builder.LinkedResources.Add(Resources(i))

						Next

						return_Message.AlternateViews.Add(view_Builder)

					Else

						return_Message.IsBodyHtml = True
						return_Message.Body = Body_Builder.ToString()

					End If

				ElseIf Body_Format = BodyType.Text

					If Not String.IsNullOrEmpty(Footer) Then
						return_Message.Body = New System.Text.StringBuilder( _
							Body_Text.ToString()).AppendLine().Append(Footer).ToString()
					Else
						return_Message.Body = Body_Text.ToString()
					End If

				End If

				If Not Attachments Is Nothing AndAlso Attachments.Count > 0 Then

					For i As Integer = 0 To Attachments.Count - 1

						return_Message.Attachments.Add(Attachments(i))

					Next

				End If

				Return return_Message

			End Function

		#End Region

		#Region " IDisposable Implementation "

			Private disposed As Boolean = False

			Protected Overridable Overloads Sub Dispose( _
				ByVal disposing As Boolean _
			)
				If Not disposed Then
					If disposing Then
						If Not Attachments Is Nothing Then
							For i As Integer = 0 TO Attachments.Count - 1
								Attachments(i).Dispose()
							Next
						End If
						If Not Resources Is Nothing Then
							For i As Integer = 0 TO Resources.Count - 1
								Resources(i).Dispose()
							Next
						End If
					End If
					disposed = True
				End If
			End Sub

			Public Overloads Sub Dispose() Implements IDisposable.Dispose
				Dispose(True)
				GC.SuppressFinalize(Me)
			End Sub

		#End Region

		#Region " Public Shared Methods "

			Public Shared Function NamedFormat( _
				ByVal format As String, _
				ByVal args As System.Collections.Generic.Dictionary(Of System.String, System.Object) _
			) As String

				Dim ary_FormatObjects(args.Count - 1) As Object

				Dim key_Integer As Int32 = 0

				For each key As String In args.Keys

					format = format.Replace("{" & key & "}", "{" & key_Integer.ToString() & "}")
					ary_FormatObjects(key_Integer) = args(key)
					key_Integer += 1

				Next

				Return String.Format(format, ary_FormatObjects)

			End Function

			Public Shared Function LoadTextFromFile( _
				ByVal text_File As String _
			) As String

				Dim return_String As String = Nothing

				If Not IO.File.Exists(text_File) Then

					Throw New ArgumentException("Text File Must Exist", "text_File")

				Else

					Dim text_Stream As IO.FileStream = Nothing

					Try

						text_Stream = New IO.FileStream(text_File, IO.FileMode.Open, IO.FileAccess.Read)

						return_String = LoadTextFromStream(text_Stream)

					Catch ex As Exception

						Throw New ArgumentException("Text File Must Be Accessible", "text_File", ex)

					Finally

						If Not text_Stream Is Nothing Then text_Stream.Close()

					End Try

				End If

				Return return_String

			End Function

			Public Shared Function LoadTextFromStream( _
				ByVal text_Stream As IO.Stream _
			) As String

				If text_Stream.CanRead Then

					Dim text_Reader As New IO.StreamReader(text_Stream)
					Return text_Reader.ReadToEnd()

				Else

					Throw New ArgumentException("Text Stream Must Be Readable", "text_Reader")

				End If

			End Function

		#End Region

	End Class

End Namespace