Imports System.Convert
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.Encoding

Namespace Cryptography

	Partial Public Class Cipher

		#Region " Public Constants "

			Public Const SALT_SMALL_MIN_SIZE As Integer = 4 ' Default Small Salt Min Size (if required)

			Public Const SALT_SMALL_MAX_SIZE As Integer = 8 ' Default Small Salt Max Size (if required)

			Public Const SALT_LARGE_MIN_SIZE As Integer = 8 ' Default Small Salt Min Size (if required)

			Public Const SALT_LARGE_MAX_SIZE As Integer = 16 ' Default Small Salt Max Size (if required)

		#End Region

		#Region " Private Shared Generation Methods "

			''' <summary>Generates a hash for the given bytes.</summary>
			''' <param name="bytesToHash">Byte Array to Hash</param>
			''' <param name="hash_Algorithm">Hash Algorithm to use.</returns>
			''' <returns>Array of Hash Bytes</returns>
			Private Shared Function Generate_Hash_FromBytes( _
				ByVal bytesToHash As Byte(), _
				ByVal hash_Algorithm As HashType _
			) As Byte()

				Select Case hash_Algorithm

					Case HashType.SHA1

						Return New SHA1Managed().ComputeHash(bytesToHash)

					Case HashType.SHA256

						Return New SHA256Managed().ComputeHash(bytesToHash)

					Case HashType.SHA384

						Return New SHA384Managed().ComputeHash(bytesToHash)

					Case HashType.SHA512

						Return New SHA512Managed().ComputeHash(bytesToHash)

					Case HashType.MD5

						Return New MD5CryptoServiceProvider().ComputeHash(bytesToHash)

					Case Else

						Return New MD5CryptoServiceProvider().ComputeHash(bytesToHash)

				End Select

			End Function

		#End Region

		#Region " Public Shared Generation Methods "

			''' <summary>Generates salt bytes for Crytography</summary>
			''' <param name="min_Size">The (inclusive) minimum size for the salt array.</param>
			''' <param name="max_Size">The (exclusive) maximum size for the saly array.</param>
			''' <returns>An Array of Table Salt</returns>
			Public Shared Function Generate_Salt( _
				ByVal min_Size As Integer, _
				ByVal max_Size As Integer _
			) As Byte()

				Dim salted_Size As Integer = New Random().Next(min_Size, max_Size)
				Dim saltBytes As Byte() = New Byte(salted_Size - 1) {}

				' Make a 'cryptographically strong' byte array
				Dim rnd_Gen As New RNGCryptoServiceProvider()
				rnd_Gen.GetNonZeroBytes(saltBytes)

				Return saltBytes

			End Function

			''' <summary>
			''' Generates a hash for the given plain text value and returns a base64-encoded result. Before the hash is computed, a random salt
			''' is generated and appended to the plain text. This salt is stored at the end of the hash value, so it can be used later for hash
			''' verification.
			''' </summary>
			''' <param name="plain_Text">Plaintext value to be hashed.</param>
			''' <param name="salt_Bytes">Salt bytes. This parameter can be null, in which case a random salt value will be generated.</param>
			''' <param name="hash_Algorithm">Hash Algorithm to use (Enum, defaults to MD5 [0]).</param>
			''' <returns>Hash value formatted as a base64-encoded string.</returns>
			''' <remarks></remarks>
			Public Shared Function Generate_Salted_Hash( _
				ByVal plain_Text As String, _
				ByVal salt_Bytes() As Byte, _
				Optional ByVal hash_Algorithm As HashType = HashType.MD5 _
			) As String

				If String.IsNullOrEmpty(plain_Text) Then Return String.Empty

				If salt_Bytes Is Nothing Then salt_Bytes = Generate_Salt(SALT_SMALL_MIN_SIZE, SALT_SMALL_MAX_SIZE) ' Create Salt if required

				Dim plainText_Bytes As Byte() = System.Text.Encoding.UTF8.GetBytes(plain_Text) ' Plain Text As Byte Array
				Dim saltedPlainText_Bytes() As Byte = New Byte(plainText_Bytes.Length + salt_Bytes.Length - 1) {} ' Array for both Plain Text & Salt
				plainText_Bytes.CopyTo(saltedPlainText_Bytes, 0) ' Copy in Plain Text Bytes
				salt_Bytes.CopyTo(saltedPlainText_Bytes, plainText_Bytes.Length) ' Then Add in a touch of Salt

				Dim hash_Bytes As Byte() = Generate_Hash_FromBytes(saltedPlainText_Bytes, hash_Algorithm) ' Do the Hashing

				Dim hashWithSalt_Bytes(hash_Bytes.Length + salt_Bytes.Length - 1) As Byte ' Array for both Hash Bytes & Salt

				hash_Bytes.CopyTo(hashWithSalt_Bytes, 0) ' Copy in Hash Bytes
				salt_Bytes.CopyTo(hashWithSalt_Bytes, hash_Bytes.Length) ' Then Add in a touch of Salt

				Return ToBase64String(hashWithSalt_Bytes)

			End Function

			''' <summary>Generates an (unsalted) hash for the given string</summary>
			''' <param name="stringToHash">String Value to generate hash for</param>
			''' <param name="hash_Algorithm">Hash Algorithm to use (Enum, defaults to MD5 [0])</param>
			''' <returns>Hash value as an Array of Bytes</returns>
			Public Shared Function Generate_Hash( _
				ByVal stringToHash As String, _
				Optional ByVal hash_Algorithm As HashType = HashType.MD5 _
			) As String

				Return ToBase64String(Generate_Hash_FromBytes(New UnicodeEncoding().GetBytes(stringToHash), hash_Algorithm))

			End Function

			''' <summary>Generates a hash for the given file</summary>
			''' <param name="file">Path and Name of the File to be Hashed</param>
			''' <param name="hashFirstNBytes">Hash first 'N' number of Bytes in the file stream</param>
			''' <returns>Hash value formatted as a base64-encoded string</returns>
			Public Shared Function Generate_Hash( _
				ByVal file As IO.FileInfo, _
				Optional ByVal hashFirstNBytes As Long = 0 _
			) As String

				Dim returnHash As String = Nothing
				Dim fileIO As FileStream = Nothing

				Try

					fileIO = file.Open(FileMode.Open, FileAccess.Read)

					Dim fileLength As Long

					If hashFirstNBytes = 0 OrElse hashFirstNBytes >= fileIO.Length Then

						fileLength = fileIO.Length - 1

					Else

						fileLength = hashFirstNBytes

					End If

					Dim fileBytes(CInt(fileLength)) As Byte

					fileIO.Read(fileBytes, 0, fileBytes.Length)

					Dim hashBytes As Byte() = Generate_Hash_FromBytes(fileBytes, HashType.MD5)

					returnHash = ToBase64String(hashBytes)

				Catch
				Finally

					If Not fileIO Is Nothing Then

						fileIO.Close()
						fileIO = Nothing

					End If

				End Try

				Return returnHash

			End Function

		#End Region

		#Region " Public Shared Verfication Methods "

			''' <summary>
			''' Compares a hash of the specified plain text value to a given hash value.
			''' Plain text is hashed with the same salt value as the original hash.
			''' </summary>
			''' <param name="plainText">Plain text to be verified against the specified hash. The function
			''' does not check whether this parameter is null.</param>
			''' <param name="hashValue">Base64-encoded hash value produced by ComputeHash function.
			''' This value includes the original salt appended to it.</param>
			''' <param name="hash_Algorithm">Hash Algorithm to use (Enum, defaults to MD5 [0]).</returns>
			''' <remarks></remarks>
			Public Shared Function Verify_Hash( _
				ByVal plain_Text As String, _
				ByVal hash_Value As String, _
				Optional ByVal hash_Algorithm As HashType = HashType.MD5 _
			) As Boolean

				Return String.Compare(hash_Value, Generate_Hash(plain_Text, hash_Algorithm)) = 0

			End Function

			''' <summary>
			''' Compares a hash of the specified plain text value to a given hash value.
			''' Plain text is hashed with the same salt value as the original hash.
			''' </summary>
			''' <param name="plainText">Plain text to be verified against the specified hash. The function
			''' does not check whether this parameter is null.</param>
			''' <param name="hashValue">Base64-encoded hash value produced by ComputeHash function.
			''' This value includes the original salt appended to it.</param>
			''' <param name="hash_Algorithm">Hash Algorithm to use (Enum, defaults to MD5 [0]).</returns>
			''' <remarks></remarks>
			Public Shared Function Verify_Salted_Hash( _
				ByVal plain_Text As String, _
				ByVal hash_Value As String, _
				Optional ByVal hash_Algorithm As HashType = HashType.MD5 _
			) As Boolean

				' Convert base64-encoded hash value into a byte array.
				Dim hashWithSaltBytes As Byte() = FromBase64String(hash_Value)

				' We must know size of hash (without salt).
				Dim hashSizeInBits As Integer
				Dim hashSizeInBytes As Integer

				' Size of hash is based on the specified algorithm.
				Select Case hash_Algorithm

					Case HashType.SHA1

						hashSizeInBits = New SHA1Managed().HashSize

					Case HashType.SHA256

						hashSizeInBits = New SHA256Managed().HashSize

					Case HashType.SHA384

						hashSizeInBits = New SHA384Managed().HashSize

					Case HashType.SHA512

						hashSizeInBits = New SHA512Managed().HashSize

					Case HashType.MD5

						hashSizeInBits = New MD5CryptoServiceProvider().HashSize

					Case Else ' Must be MD5

						hashSizeInBits = New MD5CryptoServiceProvider().HashSize

				End Select

				' Convert size of hash from bits to bytes.
				hashSizeInBytes = CInt(hashSizeInBits / 8)

				' Make sure that the specified hash value is long enough.
				If (hashWithSaltBytes.Length < hashSizeInBytes) Then Return False

				' Allocate array to hold original salt bytes retrieved from hash.
				Dim saltBytes(hashWithSaltBytes.Length - hashSizeInBytes - 1) As Byte

				' Copy salt from the end of the hash to the new array.
				Array.Copy(hashWithSaltBytes, hashSizeInBytes, saltBytes, 0, saltBytes.Length)

				' Compute a new hash string.
				Dim expectedHashString As String = Generate_Salted_Hash(plain_Text, saltBytes, hash_Algorithm)

				' If the computed hash matches the specified hash,
				' the plain text value must be correct.
				Return String.Compare(hash_Value, expectedHashString) = 0

			End Function

		#End Region

		#Region " Public Shared Encryption/Decryption Methods "

			''' <summary>Encrypts a Plain Text Value</summary>
			''' <param name="plain_Text">The Text to Encrypt.</param>
			''' <param name="encryption_Key">The Key to Use.</param>
			''' <returns>The Cipher Text</returns>
			''' <remarks>The First Byte is salt length + final part of cipher text is salt</remarks>
			Public Shared Function Encrypt( _
				ByVal plain_Text As String, _
				ByVal encryption_Key As String _
			) As String

				Dim plainText_Bytes As Byte() = Encoding.Unicode.GetBytes(plain_Text)
				Dim salt_Bytes As Byte() = Generate_Salt(SALT_LARGE_MIN_SIZE, SALT_LARGE_MAX_SIZE)

				Dim secret_Key As New Rfc2898DeriveBytes(encryption_Key, salt_Bytes)

				Dim memory_Stream As New MemoryStream()
				Dim cryptoStream As New CryptoStream(memory_Stream, _
					New RijndaelManaged().CreateEncryptor(secret_Key.GetBytes(32), secret_Key.GetBytes(16)), _
					CryptoStreamMode.Write)
				cryptoStream.Write(plainText_Bytes, 0, plainText_Bytes.Length)
				cryptoStream.FlushFinalBlock()

				Dim cipherText_Bytes As Byte() = memory_Stream.ToArray()

				memory_Stream.Close()
				cryptoStream.Close()

				Dim cipherText_Array(cipherText_Bytes.Length + salt_Bytes.Length) As Byte
				Array.Copy(cipherText_Bytes, 0, cipherText_Array, 1, cipherText_Bytes.Length)
				cipherText_Array(0) = CByte(salt_Bytes.Length)
				Array.Copy(salt_Bytes, 0, cipherText_Array, cipherText_Bytes.Length + 1, salt_Bytes.Length)

				Return ToBase64String(cipherText_Array)

			End Function

			''' <summary>Decrypts an Encrypted Value back to Plain Text</summary>
			''' <param name="cipher_Text">The Cipher Text</param>
			''' <param name="encryption_Key">The Key to Use</param>
			''' <returns>The Plain Text</returns>
			''' <remarks></remarks>
			Public Shared Function Decrypt( _
				ByVal cipher_Text As String, _
				ByVal encryption_Key As String _
			) As String

				Dim cipherText_Array As Byte() = FromBase64String(cipher_Text)

				Dim salt_Bytes(cipherText_Array(0) - 1) As Byte
				Array.Copy(cipherText_Array, cipherText_Array.Length - salt_Bytes.Length, salt_Bytes, 0, salt_Bytes.Length)

				Dim cipherTextBytes_Array(cipherText_Array.Length - salt_Bytes.Length - 2) As Byte
				Array.Copy(cipherText_Array, 1, cipherTextBytes_Array, 0, cipherTextBytes_Array.Length)

				Dim secret_Key As New Rfc2898DeriveBytes(encryption_Key, salt_Bytes)

				Dim memory_Stream As New MemoryStream(cipherTextBytes_Array)
				Dim cryptoStream As New CryptoStream(memory_Stream, _
					New RijndaelManaged().CreateDecryptor(secret_Key.GetBytes(32), secret_Key.GetBytes(16)), _
					CryptoStreamMode.Read)
				Dim plainTextBytes(cipherTextBytes_Array.Length - 1) As Byte

				Dim decryptedCount As Integer = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length)
				cryptoStream.Read(cipherTextBytes_Array, 0, cipherTextBytes_Array.Length)
				memory_Stream.Close()
				cryptoStream.Close()

				Return Encoding.Unicode.GetString(plainTextBytes, 0, decryptedCount)

			End Function

		#End Region

		#Region " Public Shared Password Methods "

			''' <summary>
			''' Method to Generate a Password of a given Length and with a given number of Special/
			''' Non-Alpha Numeric Characters.
			''' </summary>
			''' <param name="length">The Password Length Required.</param>
			''' <param name="numberOfNonAlphanumericCharacters">The Number of Characters that cannot
			''' be Alpha-Numeric.</param>
			''' <returns>The new Password.</returns>
			''' <remarks>This generation method is only as secure as the RNGCryptoServiceProvider Random
			''' Number generator.</remarks>
			Public Shared Function Create_Password( _
				ByVal length As Integer, _
				ByVal numberOfNonAlphanumericCharacters As Integer _
			) As String

				' -- Check Input Parameters --
				If (length < 1 OrElse length > 128) Then Throw New ArgumentException("Password Length Not Valid")

				If (numberOfNonAlphanumericCharacters > length OrElse numberOfNonAlphanumericCharacters < 0) Then _
					Throw New ArgumentException("Non-AlphaNumeric Characters Length Not Valid")
				' ----------------------------

				Do While True

					Dim nonAlphanumericCount As Integer = 0
					Dim buffer As Byte() = New Byte(length - 1) {}
					Dim ary_Password As Char() = New Char(length - 1) {}

					Dim ary_AlphaNumeric As Char() = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray()
					Dim ary_NonAlphaNumeric As Char() = "!@$%^^*()_-+=[{]};:>|./?~".ToCharArray()

					' Make a 'cryptographically strong' byte array
					Dim rdn_Gen As New RNGCryptoServiceProvider()
					rdn_Gen.GetBytes(buffer)

					For i As Integer = 0 To length - 1

						' Randon Byte --> Char Conversion
						Dim rnd_Char As Integer = (buffer(i) Mod 87)
						If rnd_Char < 10 Then
							ary_Password(i) = System.Convert.ToChar(System.Convert.ToUInt16(48 + rnd_Char))
						ElseIf rnd_Char < 36 Then
							ary_Password(i) = System.Convert.ToChar(System.Convert.ToUInt16((65 + rnd_Char) - 10))
						ElseIf rnd_Char < 62 Then
							ary_Password(i) = System.Convert.ToChar(System.Convert.ToUInt16((97 + rnd_Char) - 36))
						Else
							ary_Password(i) = ary_NonAlphaNumeric(rnd_Char - 62)
							nonAlphanumericCount += 1
						End If

					Next

					If nonAlphanumericCount < numberOfNonAlphanumericCharacters Then

						Dim rnd_Number As New Random()

						For i As Integer = 0 To (numberOfNonAlphanumericCharacters - nonAlphanumericCount) - 1

							Dim passwordPos As Integer

							Do
								passwordPos = rnd_Number.Next(0, length)
							Loop While Not Char.IsLetterOrDigit(ary_Password(passwordPos))

							ary_Password(passwordPos) = ary_NonAlphaNumeric(rnd_Number.Next(0, ary_NonAlphaNumeric.Length))

						Next

					ElseIf nonAlphanumericCount > numberOfNonAlphanumericCharacters Then

						Dim rnd_Number As New Random()

						For i = 0 To (nonAlphanumericCount - numberOfNonAlphanumericCharacters) - 1

							Dim passwordPos As Integer

							Do
								passwordPos = rnd_Number.Next(0, length)
							Loop While Char.IsLetterOrDigit(ary_Password(passwordPos))

							ary_Password(passwordPos) = ary_AlphaNumeric(rnd_Number.Next(0, ary_AlphaNumeric.Length))

						Next

					End If

					Return New String(ary_Password)
				Loop

				Return Nothing

			End Function

		#End Region

	End Class

End Namespace