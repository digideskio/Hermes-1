#If TESTING Then

Imports Hermes.Email
Imports NUnit.Framework
Imports System.Net

Namespace Testing.Email
	<TestFixture(Description:="Tests Email Recipients Functionality")> _
	Public Class RecipientTests

		#Region " Private Test Setups "

			Private Function Generate_Test_List( _
				ByVal length As System.Int32, _
				Optional ByVal start_Number As System.Int32 = 1 _
			) As Distribution

				Dim return_Value As New Distribution()

				For i As Integer = start_Number To length

					return_Value.Add( _
						String.Format("me_{0}@example.com", i), _
						String.Format("Test Display {0}", i) _
					)

				Next

				Return return_Value

			End Function

			Private Sub Verify_Chunked_List( _
				ByVal value As Mail.MailAddress(), _
				ByVal start_Number As System.Int32 _
			)

				Dim verification_List As Distribution = Generate_Test_List(value.Length, start_Number + 1)

				For i As Integer = 0 To verification_List.Addresses.Count - 1

					Assert.AreEqual(verification_List.Addresses(i).DisplayName, value(i).DisplayName)
					Assert.AreEqual(verification_List.Addresses(i).Address, value(i).Address)

				Next

			End Sub

			Private Sub Torture_Chunked_List( _
				ByVal start_Number As System.Int32, _
				ByVal end_Number As System.Int32, _
				ByVal step_Number As System.Int32 _
			)

				For i As Integer = start_Number To end_Number

					Dim recipient As Distribution = Generate_Test_List(i)

					' Check Every Address was Added
					Assert.AreEqual(i, recipient.Addresses.Count)

					' Check the Non-Chunked Case
					Assert.AreEqual(i, recipient.Split(0 - i)(0).Length)

					For j As Integer = start_Number To end_Number Step step_Number

						Dim chunked_Recipients As Mail.MailAddress()() = recipient.Split(j)

						Dim total_Addresses As System.Int32 = i
						Dim current_Addresses As System.Int32 = 0
						Dim total_Chunks As System.Int32 = CInt(Math.Ceiling(i / j))

						Assert.AreEqual(total_Chunks, chunked_Recipients.Length)

						For k As Integer = 0 To total_Chunks - 1

							Dim expected_Size As System.Int32 = Math.Min(total_Addresses, j)

							Assert.AreEqual(expected_Size, chunked_Recipients(k).Length)
							Verify_Chunked_List(chunked_Recipients(k), current_Addresses)

							current_Addresses += expected_Size
							total_Addresses -= expected_Size

						Next

					Next

				Next

			End Sub

		#End Region

		<Test(Description:="Check Addition Method will not accept Empty Email Addresses"), _
			ExpectedException(GetType(System.ArgumentException), _
			ExpectedMessage:="Email Address must be present and Valid" & vbCrLf & "Parameter name: address")> _
		Public Sub Address_Addition_Test_1()

			Dim recipient As New Distribution
			recipient.Add(String.Empty, String.Empty)

		End Sub

		<Test(Description:="Check Addition Method will add a single email address")> _
		Public Sub Address_Addition_Test_2()

			Dim test_Address As String = "me@example.com"
			Dim test_Display As String = "Test Display"

			Dim recipient As New Distribution()
			recipient.Add(test_Address, test_Display)

			Assert.AreEqual(1, recipient.Addresses.Count)
			Assert.AreEqual(test_Display, recipient.Addresses(0).DisplayName)
			Assert.AreEqual(test_Address, recipient.Addresses(0).Address)

		End Sub

		<Test(Description:="Check Addition Method will not add duplicate Email Addresses")> _
		Public Sub Address_Addition_Test_3()

			Dim test_Address As String = "me@example.com"
			Dim test_Display As String = "Test Display"

			Dim recipient As New Distribution()
			recipient.Add(test_Address, test_Display)
			recipient.Add(test_Address, test_Display)

			Assert.AreEqual(1, recipient.Addresses.Count)
			Assert.AreEqual(test_Display, recipient.Addresses(0).DisplayName)
			Assert.AreEqual(test_Address, recipient.Addresses(0).Address)

		End Sub

		<Test(Description:="Check a 20 Email Split with a Max Chunk of 10")> _
		Public Sub Address_Spliting_Test_1()

			Dim recipient As Distribution = Generate_Test_List(20)

			Assert.AreEqual(20, recipient.Addresses.Count)

			Dim chunked_Recipients As Mail.MailAddress()() = recipient.Split(10)

			Assert.AreEqual(2, chunked_Recipients.Length)

			Assert.AreEqual(10, chunked_Recipients(0).Length)
			Assert.AreEqual(10, chunked_Recipients(1).Length)

			Verify_Chunked_List(chunked_Recipients(0), 0)
			Verify_Chunked_List(chunked_Recipients(1), 10)

		End Sub

		<Test(Description:="Check a 24 Email Split with a Max Chunk of 10")> _
		Public Sub Address_Spliting_Test_2()

			Dim recipient As Distribution = Generate_Test_List(24)

			Assert.AreEqual(24, recipient.Addresses.Count)

			Dim chunked_Recipients As Mail.MailAddress()() = recipient.Split(10)

			Assert.AreEqual(3, chunked_Recipients.Length)

			Assert.AreEqual(10, chunked_Recipients(0).Length)
			Assert.AreEqual(10, chunked_Recipients(1).Length)
			Assert.AreEqual(4, chunked_Recipients(2).Length)

			Verify_Chunked_List(chunked_Recipients(0), 0)
			Verify_Chunked_List(chunked_Recipients(1), 10)
			Verify_Chunked_List(chunked_Recipients(2), 20)

		End Sub

		<Test(Description:="Check a 17 Email Split with a Max Chunk of 6")> _
		Public Sub Address_Spliting_Test_3()

			Dim recipient As Distribution = Generate_Test_List(17)

			Assert.AreEqual(17, recipient.Addresses.Count)

			Dim chunked_Recipients As Mail.MailAddress()() = recipient.Split(6)

			Assert.AreEqual(3, chunked_Recipients.Length)

			Assert.AreEqual(6, chunked_Recipients(0).Length)
			Assert.AreEqual(6, chunked_Recipients(1).Length)
			Assert.AreEqual(5, chunked_Recipients(2).Length)

			Verify_Chunked_List(chunked_Recipients(0), 0)
			Verify_Chunked_List(chunked_Recipients(1), 6)
			Verify_Chunked_List(chunked_Recipients(2), 12)

		End Sub

		<Test(Description:="Check Splitting of a Zero Length Email Address List")> _
		Public Sub Address_Spliting_Test_4()

			Dim recipient As Distribution = Generate_Test_List(0)

			Assert.AreEqual(0, recipient.Addresses.Count)

			Dim chunked_Recipients As Mail.MailAddress()() = recipient.Split(0)

			Assert.AreEqual(1, chunked_Recipients.Length)
			Assert.AreEqual(0, chunked_Recipients(0).Length)

			chunked_Recipients = recipient.Split(10)

			Assert.AreEqual(1, chunked_Recipients.Length)
			Assert.AreEqual(0, chunked_Recipients(0).Length)

		End Sub

		<Test(Description:="Email Address Spliting Method Testing")> _
		Public Sub Address_Spliting_Test_5()

			Dim start_Numbers As System.Int32() = New System.Int32() {1, 213, 319}

			Dim size_Numbers As System.Int32() = New System.Int32() {10, 23, 39}

			Dim step_Numbers As System.Int32() = New System.Int32() {1, 3, 7}

			For i As Integer = 0 To start_Numbers.Length - 1

				Torture_Chunked_List(start_Numbers(i), _
					start_Numbers(i) + size_Numbers(i), step_Numbers(i))

			Next

		End Sub

		<Test(Description:="Email Address Spliting Method Testing")> _
		Public Sub Address_Spliting_Test_6()

			Dim recipient As Distribution = Generate_Test_List(101)
			recipient.Sending_Format = SendingType.Single_Recipient

			For i As Integer = 0 To 100 Step 4

				Dim chunked_Recipients As Mail.MailAddress()() = recipient.Split(i)

				Assert.AreEqual(101, chunked_Recipients.Length)

				For j As Integer = 0 To 101 - 1

					Assert.AreEqual(1, chunked_Recipients(j).Length)

				Next

			Next

		End Sub

	End Class

End Namespace

#End If