#If TESTING Then

Imports Hermes.Email
Imports NUnit.Framework

Namespace Testing.Email
	<TestFixture(Description:="Tests Email Message Functionality")> _
	Public Class MessageTests

		<Test(Description:="Shared Footer Loading Method Testing"), _
			ExpectedException(GetType(System.ArgumentException), _
			ExpectedMessage:="Text File Must Exist" & vbCrLf & "Parameter name: text_File")> _
		Public Sub Footer_Creation_Test_1()

			Message.LoadTextFromFile(String.Empty)

		End Sub

		<Test(Description:="Shared Footer Loading Method Testing")> _
		Public Sub Footer_Creation_Test_2()

			Dim test_Strings As String() = New String() { _
				"This is a test." & vbCrLf & "This is the second line.", _
				"This is a test. This is the second line." _
			}

			For i As Integer = 0 To test_Strings.Length - 1

				Dim new_TestGuid As Guid = Guid.NewGuid()

				Dim test_File As String = IO.Path.Combine(Environment.CurrentDirectory, _
					String.Format("{0}.txt", new_TestGuid.ToString("D")))

				Dim test_Writer As New IO.StreamWriter(test_File)

				test_Writer.Write(test_Strings(i))
				test_Writer.Flush()
				test_Writer.Close()

				Assert.AreEqual(test_Strings(i), Message.LoadTextFromFile(test_File))

				IO.File.Delete(test_File)

			Next

		End Sub

		<Test(Description:="Message Creation Method Testing")> _
		Public Sub Message_Creation_Test_1()

			Dim string_1 As String = "Test Message"
			Dim string_2 As String = String.Format( _
				"<html>{1}<body>{1}{0}{1}</body>{1}</html>{1}", string_1, VbCrLf)

			Dim message_1 As New Message()
			message_1.Append(string_1)
			Assert.IsFalse(message_1.ToMailMessage.IsBodyHtml)
			Assert.AreEqual(string_1, message_1.ToMailMessage.Body)

			Dim message_2 As New Message
			message_2.Body_Format = BodyType.Html
			message_2.Append(string_1)
			Assert.IsTrue(message_2.ToMailMessage.IsBodyHtml)
			Assert.AreEqual(string_2, message_2.ToMailMessage.Body)

			Dim message_3 As New Message
			message_3.Body_Format = BodyType.Html
			message_3.Append(string_2)
			Assert.IsTrue(message_3.ToMailMessage.IsBodyHtml)
			Assert.AreEqual(string_2, message_3.ToMailMessage.Body)

		End Sub

		<Test(Description:="Message Creation Method Testing")> _
		Public Sub Message_Creation_Test_2()

			Dim string_1 As String = "Test Message"
			Dim string_2 As String = "Test Resource"

			Dim new_TestGuid As Guid = Guid.NewGuid()

			Dim test_File As String = IO.Path.Combine(Environment.CurrentDirectory, _
				String.Format("{0}.txt", new_TestGuid.ToString("D")))

			Dim test_Writer As New IO.StreamWriter(test_File)
			test_Writer.Write(string_2)
			test_Writer.Flush()
			test_Writer.Close()

			Dim message_1 As New Message
			message_1.Body_Format = BodyType.Html
			message_1.Append(string_1)
			message_1.AppendResource(test_File, "test")
			Assert.AreEqual(1, message_1.ToMailMessage.AlternateViews.Count)
			Assert.AreEqual(1, message_1.ToMailMessage.AlternateViews(0).LinkedResources.Count)

			message_1.Dispose()

			IO.File.Delete(test_File)

		End Sub

		<Test(Description:="Message Formatting Method Testing")> _
		Public Sub Message_Formatting_Test_1()

			Dim string_1 As String = "This is a Test Message. {format-1} should go here, and here ({format-1}). The second object will be here:{1234}."
			Dim string_2 As Integer = 6745646
			Dim string_2_Key As String = "format-1"
			Dim string_3 As String = "Testing Input"
			Dim string_3_Key As String = "1234"

			Dim test_Objects As New System.Collections.Generic.Dictionary(Of String, Object)
			test_Objects.Add(string_2_Key, string_2)
			test_Objects.Add(string_3_Key, string_3)

			Dim new_TestGuid As Guid = Guid.NewGuid()

			Dim test_File As String = IO.Path.Combine(Environment.CurrentDirectory, _
				String.Format("{0}.txt", new_TestGuid.ToString("D")))

			Dim test_Writer As New IO.StreamWriter(test_File)
			test_Writer.Write(string_1)
			test_Writer.Flush()
			test_Writer.Close()

			Dim message_1 As New Message
			message_1.Body_Format = BodyType.Text
			message_1.AppendNamedFormat(Message.LoadTextFromFile(test_File), test_Objects)

			Assert.AreEqual(string_1.Replace("{" & string_2_Key & "}", string_2.ToString()).Replace("{" & string_3_Key & "}", string_3), message_1.ToMailMessage.Body)

			message_1.Dispose()

			IO.File.Delete(test_File)

		End Sub

	End Class

End Namespace

#End If