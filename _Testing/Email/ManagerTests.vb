#If TESTING Then

Imports Hermes.Email
Imports NUnit.Framework

Namespace Testing.Email
	<TestFixture(Description:="Tests Email Manager Functionality")> _
	Public Class ManagerTests

		<Test(Description:="Shared Email Address Creation Method Testing"), _
			ExpectedException(GetType(System.ArgumentException), _
			ExpectedMessage:="Email Address must be present and Valid" & vbCrLf & "Parameter name: address")> _
		Public Sub Address_Creation_Test_1()

			Manager.CreateAddress(String.Empty, String.Empty)

		End Sub

		<Test(Description:="Shared Email Address Creation Method Testing"), _
			ExpectedException(GetType(System.ArgumentException), _
			ExpectedMessage:="Email Address must be present and Valid" & vbCrLf & "Parameter name: address")> _
		Public Sub Address_Creation_Test_2()

			Manager.CreateAddress("me@example", String.Empty)

		End Sub

		<Test(Description:="Shared Email Address Creation Method Testing"), _
			ExpectedException(GetType(System.ArgumentException), _
			ExpectedMessage:="Email Address must be present and Valid" & vbCrLf & "Parameter name: address")> _
		Public Sub Address_Creation_Test_3()

			Manager.CreateAddress("me@example.con", String.Empty)

		End Sub

		<Test(Description:="Shared Email Address Creation Method Testing")> _
		Public Sub Address_Creation_Test_4()

			Dim test_Address As String = "me@example.com"
			Dim test_Display As String = "Test Display"

			Dim address_1 As System.Net.Mail.MailAddress = Manager.CreateAddress(test_Address, test_Display)

			Assert.IsNotNull(address_1)

			Assert.AreEqual(test_Address, address_1.Address)
			Assert.AreEqual(test_Display, address_1.DisplayName)

		End Sub

		<Test(Description:="Shared Email Address Creation Method Testing")> _
		Public Sub Address_Creation_Test_5()

			Dim test_Address As String = "me@example.co.uk"
			Dim test_Display As String = "Test Display"

			Dim address_1 As System.Net.Mail.MailAddress = Manager.CreateAddress(test_Address, test_Display)

			Assert.IsNotNull(address_1)

			Assert.AreEqual(test_Address, address_1.Address)
			Assert.AreEqual(test_Display, address_1.DisplayName)

		End Sub

		<Test(Description:="Shared Server Creation Method Testing"), _
			ExpectedException(GetType(System.ArgumentException), _
			ExpectedMessage:="Server Name/Address must be present" & vbCrLf & "Parameter name: server")> _
		Public Sub Server_Creation_Test_1()

			Manager.CreateServer(String.Empty)

		End Sub

		<Test(Description:="Shared Server Creation Method Testing"), _
			ExpectedException(GetType(System.ArgumentException), _
			ExpectedMessage:="Server Name/Address must be valid" & vbCrLf & "Parameter name: server")> _
		Public Sub Server_Creation_Test_2()

			Manager.CreateServer(Guid.NewGuid().ToString())

		End Sub

		<Test(Description:="Shared Server Creation Method Testing"), _
			ExpectedException(GetType(System.ArgumentException), _
			ExpectedMessage:="Server Name/Address must be valid" & vbCrLf & "Parameter name: server")> _
		Public Sub Server_Creation_Test_3()

			Manager.CreateServer("1.1.1.1")

		End Sub

		<Test(Description:="Shared Server Creation Method Testing"), _
			ExpectedException(GetType(System.ArgumentException), _
			ExpectedMessage:="Server Name/Address must be valid" & vbCrLf & "Parameter name: server")> _
		Public Sub Server_Creation_Test_4()

			Manager.CreateServer("www.example.con")

		End Sub

		' <Test(Description:="Shared Server Creation Method Testing")> _
		Public Sub Server_Creation_Test_5()

			Dim dns_1 As String = "www.example.com"
			Dim ip_1 As String = "93.184.216.119"

			Dim server_1 As System.Net.IPHostEntry = Manager.CreateServer(dns_1)

			Assert.AreEqual(dns_1, server_1.HostName)

			For i As Integer = 0 To server_1.AddressList.Length - 1

				If server_1.AddressList(i).AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork Then _
					Assert.AreEqual(ip_1, server_1.AddressList(i).ToString())

			Next

			Dim server_2 As System.Net.IPHostEntry = Manager.CreateServer(ip_1)

			Assert.AreEqual("43-10.any.icann.org", server_2.HostName) ' GRRRR

		End Sub

		<Test(Description:="Shared Credential Creation Method Testing"), _
			ExpectedException(GetType(System.ArgumentException), _
			ExpectedMessage:="Username must be present" & vbCrLf & "Parameter name: username")> _
		Public Sub Credential_Creation_Test_1()

			Manager.CreateCredential(String.Empty, String.Empty, String.Empty)

		End Sub

		<Test(Description:="Shared Credential Creation Method Testing"), _
			ExpectedException(GetType(System.ArgumentException), _
			ExpectedMessage:="Domain must be present" & vbCrLf & "Parameter name: domain")> _
		Public Sub Credential_Creation_Test_2()

			Manager.CreateCredential("Test-User", String.Empty, String.Empty)

		End Sub

		<Test(Description:="Shared Credential Creation Method Testing")> _
		Public Sub Credential_Creation_Test_3()

			Dim username_1 As String = "Test-User"
			Dim domain_1 As String = "Test-Domain"
			Dim password_1 As String = "Test-Password"

			Dim credential_1 As System.Net.NetworkCredential = _
				Manager.CreateCredential(username_1, password_1, domain_1)

			Assert.IsNotNull(credential_1)
			Assert.AreEqual(username_1, credential_1.Username)
			Assert.AreEqual(domain_1, credential_1.Domain)

		End Sub

		<Test(Description:="Shared Message Creation Method Testing"), _
			ExpectedException(GetType(System.ArgumentException), _
			ExpectedMessage:="Subject must be present" & vbCrLf & "Parameter name: subject")> _
		Public Sub Message_Creation_Test_1()

			Manager.CreateMessage(String.Empty)

		End Sub

		<Test(Description:="Shared Message Creation Method Testing")> _
		Public Sub Message_Creation_Test_2()

			Dim subject_1 As String = "Message Subject"

			Dim message_1 As Message = _
				Manager.CreateMessage(subject_1)

			Assert.IsNotNull(message_1)
			Assert.AreEqual(subject_1, message_1.Subject)

		End Sub
	End Class

End Namespace

#End If