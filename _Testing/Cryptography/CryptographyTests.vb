#If TESTING Then

Imports Hermes.Cryptography
Imports Hermes.Cryptography.Cipher
Imports System.Configuration
Imports NUnit.Framework

Namespace Testing

	<TestFixture(Description:="Tests the Cryptographic Functions.")> _
	Public Class CryptographyTests

		<Test(Description:="Cryptography Encryption / Decryption Testing")> _
		Public Sub EncryptionTest_1()

			Dim encryption_Key As String = Create_Password(30,5)
			Dim plain_Text As String = Create_Password(100,10)
			Dim cipher_Text As String = Encrypt(plain_Text, encryption_Key)

			Assert.AreEqual(plain_Text, Decrypt(cipher_Text, encryption_Key))

		End Sub

		<Test(Description:="Cryptography Hash Testing (With Salt)")> _
		Public Sub HashTest_1()

			Dim plain_Text As String = Create_Password(100,10)

			Dim hash_Text As String = Generate_Salted_Hash(plain_Text, Nothing, HashType.MD5)
			Assert.IsTrue(Verify_Salted_Hash(plain_Text, hash_Text, HashType.MD5))

			hash_Text = Generate_Salted_Hash(plain_Text, Nothing, HashType.SHA1)
			Assert.IsTrue(Verify_Salted_Hash(plain_Text, hash_Text, HashType.SHA1))

			hash_Text = Generate_Salted_Hash(plain_Text, Nothing, HashType.SHA256)
			Assert.IsTrue(Verify_Salted_Hash(plain_Text, hash_Text, HashType.SHA256))

			hash_Text = Generate_Salted_Hash(plain_Text, Nothing, HashType.SHA384)
			Assert.IsTrue(Verify_Salted_Hash(plain_Text, hash_Text, HashType.SHA384))

			hash_Text = Generate_Salted_Hash(plain_Text, Nothing, HashType.SHA512)
			Assert.IsTrue(Verify_Salted_Hash(plain_Text, hash_Text, HashType.SHA512))

		End Sub

		<Test(Description:="Cryptography Hash Testing (Without Salt)")> _
		Public Sub HashTest_2()

			Dim plain_Text As String = Create_Password(100,10)

			Dim hash_Text As String = Generate_Hash(plain_Text, HashType.MD5)
			Assert.IsTrue(Verify_Hash(plain_Text, hash_Text, HashType.MD5))

			hash_Text = Generate_Hash(plain_Text, HashType.SHA1)
			Assert.IsTrue(Verify_Hash(plain_Text, hash_Text, HashType.SHA1))

			hash_Text = Generate_Hash(plain_Text, HashType.SHA256)
			Assert.IsTrue(Verify_Hash(plain_Text, hash_Text, HashType.SHA256))

			hash_Text = Generate_Hash(plain_Text, HashType.SHA384)
			Assert.IsTrue(Verify_Hash(plain_Text, hash_Text, HashType.SHA384))

			hash_Text = Generate_Hash(plain_Text, HashType.SHA512)
			Assert.IsTrue(Verify_Hash(plain_Text, hash_Text, HashType.SHA512))

		End Sub

	End Class

End Namespace

#End If