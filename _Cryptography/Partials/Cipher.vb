Imports System.Convert
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.Encoding

Namespace Cryptography

	Partial Public Class Cipher

		#Region " Phonetic Digits "

			Public Const PHONETIC_ZERO As String = "Zero"

			Public Const PHONETIC_ONE As String = "One"

			Public Const PHONETIC_TWO As String = "Two"

			Public Const PHONETIC_THREE As String = "Three"

			Public Const PHONETIC_FOUR As String = "Four"

			Public Const PHONETIC_FIVE As String = "Five"

			Public Const PHONETIC_SIX As String = "Six"

			Public Const PHONETIC_SEVEN As String = "Seven"

			Public Const PHONETIC_EIGHT As String = "Eight"

			Public Const PHONETIC_NINE As String = "Nine"

			Public Shared PHONETIC_DIGITS As String() = _
				New String() _
					{ _
						PHONETIC_ZERO, PHONETIC_ONE, PHONETIC_TWO, _
						PHONETIC_THREE, PHONETIC_FOUR, PHONETIC_FIVE, _
						PHONETIC_SIX, PHONETIC_SEVEN, PHONETIC_EIGHT, PHONETIC_NINE _
					}

		#End Region

		#Region " Phonetic Letters "

			Public Const PHONETIC_LOWERCASE_A As String = "Alpha"

			Public Const PHONETIC_LOWERCASE_B As String = "Bravo"

			Public Const PHONETIC_LOWERCASE_C As String = "Charlie"

			Public Const PHONETIC_LOWERCASE_D As String = "Delta"

			Public Const PHONETIC_LOWERCASE_E As String = "Echo"

			Public Const PHONETIC_LOWERCASE_F As String = "Foxtrot"

			Public Const PHONETIC_LOWERCASE_G As String = "Golf"

			Public Const PHONETIC_LOWERCASE_H As String = "Hotel"

			Public Const PHONETIC_LOWERCASE_I As String = "India"

			Public Const PHONETIC_LOWERCASE_J As String = "Juliet"

			Public Const PHONETIC_LOWERCASE_K As String = "Kilo"

			Public Const PHONETIC_LOWERCASE_L As String = "Lima"

			Public Const PHONETIC_LOWERCASE_M As String = "Mike"

			Public Const PHONETIC_LOWERCASE_N As String = "November"

			Public Const PHONETIC_LOWERCASE_O As String = "Oscar"

			Public Const PHONETIC_LOWERCASE_P As String = "Papa"

			Public Const PHONETIC_LOWERCASE_Q As String = "Quebec"

			Public Const PHONETIC_LOWERCASE_R As String = "Romeo"

			Public Const PHONETIC_LOWERCASE_S As String = "Sierra"

			Public Const PHONETIC_LOWERCASE_T As String = "Tango"

			Public Const PHONETIC_LOWERCASE_U As String = "Uniform"

			Public Const PHONETIC_LOWERCASE_V As String = "Victor"

			Public Const PHONETIC_LOWERCASE_W As String = "Whiskey"

			Public Const PHONETIC_LOWERCASE_X As String = "X-Ray"

			Public Const PHONETIC_LOWERCASE_Y As String = "Yankee"

			Public Const PHONETIC_LOWERCASE_Z As String = "Zulu"

			Public Shared PHONETIC_LOWERCASE_LETTERS As String() = _
				New String() _
					{ _
						PHONETIC_LOWERCASE_A, PHONETIC_LOWERCASE_B, PHONETIC_LOWERCASE_C, _
						PHONETIC_LOWERCASE_D, PHONETIC_LOWERCASE_E, PHONETIC_LOWERCASE_F, _
						PHONETIC_LOWERCASE_G, PHONETIC_LOWERCASE_H, PHONETIC_LOWERCASE_I, _
						PHONETIC_LOWERCASE_J, PHONETIC_LOWERCASE_K, PHONETIC_LOWERCASE_L, _
						PHONETIC_LOWERCASE_M, PHONETIC_LOWERCASE_N, PHONETIC_LOWERCASE_O, _
						PHONETIC_LOWERCASE_P, PHONETIC_LOWERCASE_Q, PHONETIC_LOWERCASE_R, _
						PHONETIC_LOWERCASE_S, PHONETIC_LOWERCASE_T, PHONETIC_LOWERCASE_U, _
						PHONETIC_LOWERCASE_V, PHONETIC_LOWERCASE_W, PHONETIC_LOWERCASE_X, _
						PHONETIC_LOWERCASE_Y, PHONETIC_LOWERCASE_Z _
					}

			Public Shared PHONETIC_UPPERCASE_A As String = PHONETIC_LOWERCASE_A.ToUpper

			Public Shared PHONETIC_UPPERCASE_B As String = PHONETIC_LOWERCASE_B.ToUpper

			Public Shared PHONETIC_UPPERCASE_C As String = PHONETIC_LOWERCASE_C.ToUpper

			Public Shared PHONETIC_UPPERCASE_D As String = PHONETIC_LOWERCASE_D.ToUpper

			Public Shared PHONETIC_UPPERCASE_E As String = PHONETIC_LOWERCASE_E.ToUpper

			Public Shared PHONETIC_UPPERCASE_F As String = PHONETIC_LOWERCASE_F.ToUpper

			Public Shared PHONETIC_UPPERCASE_G As String = PHONETIC_LOWERCASE_G.ToUpper

			Public Shared PHONETIC_UPPERCASE_H As String = PHONETIC_LOWERCASE_H.ToUpper

			Public Shared PHONETIC_UPPERCASE_I As String = PHONETIC_LOWERCASE_I.ToUpper

			Public Shared PHONETIC_UPPERCASE_J As String = PHONETIC_LOWERCASE_J.ToUpper

			Public Shared PHONETIC_UPPERCASE_K As String = PHONETIC_LOWERCASE_K.ToUpper

			Public Shared PHONETIC_UPPERCASE_L As String = PHONETIC_LOWERCASE_L.ToUpper

			Public Shared PHONETIC_UPPERCASE_M As String = PHONETIC_LOWERCASE_M.ToUpper

			Public Shared PHONETIC_UPPERCASE_N As String = PHONETIC_LOWERCASE_N.ToUpper

			Public Shared PHONETIC_UPPERCASE_O As String = PHONETIC_LOWERCASE_O.ToUpper

			Public Shared PHONETIC_UPPERCASE_P As String = PHONETIC_LOWERCASE_P.ToUpper

			Public Shared PHONETIC_UPPERCASE_Q As String = PHONETIC_LOWERCASE_Q.ToUpper

			Public Shared PHONETIC_UPPERCASE_R As String = PHONETIC_LOWERCASE_R.ToUpper

			Public Shared PHONETIC_UPPERCASE_S As String = PHONETIC_LOWERCASE_S.ToUpper

			Public Shared PHONETIC_UPPERCASE_T As String = PHONETIC_LOWERCASE_T.ToUpper

			Public Shared PHONETIC_UPPERCASE_U As String = PHONETIC_LOWERCASE_U.ToUpper

			Public Shared PHONETIC_UPPERCASE_V As String = PHONETIC_LOWERCASE_V.ToUpper

			Public Shared PHONETIC_UPPERCASE_W As String = PHONETIC_LOWERCASE_W.ToUpper

			Public Shared PHONETIC_UPPERCASE_X As String = PHONETIC_LOWERCASE_X.ToUpper

			Public Shared PHONETIC_UPPERCASE_Y As String = PHONETIC_LOWERCASE_Y.ToUpper

			Public Shared PHONETIC_UPPERCASE_Z As String = PHONETIC_LOWERCASE_Z.ToUpper

			Public Shared PHONETIC_UPPERCASE_LETTERS As String() = _
				New String() _
					{ _
						PHONETIC_UPPERCASE_A, PHONETIC_UPPERCASE_B, PHONETIC_UPPERCASE_C, _
						PHONETIC_UPPERCASE_D, PHONETIC_UPPERCASE_E, PHONETIC_UPPERCASE_F, _
						PHONETIC_UPPERCASE_G, PHONETIC_UPPERCASE_H, PHONETIC_UPPERCASE_I, _
						PHONETIC_UPPERCASE_J, PHONETIC_UPPERCASE_K, PHONETIC_UPPERCASE_L, _
						PHONETIC_UPPERCASE_M, PHONETIC_UPPERCASE_N, PHONETIC_UPPERCASE_O, _
						PHONETIC_UPPERCASE_P, PHONETIC_UPPERCASE_Q, PHONETIC_UPPERCASE_R, _
						PHONETIC_UPPERCASE_S, PHONETIC_UPPERCASE_T, PHONETIC_UPPERCASE_U, _
						PHONETIC_UPPERCASE_V, PHONETIC_UPPERCASE_W, PHONETIC_UPPERCASE_X, _
						PHONETIC_UPPERCASE_Y, PHONETIC_UPPERCASE_Z _
					}

		#End Region

		#Region " Digits "

			Public Const DIGIT_ZERO As Char = "0"c

			Public Const DIGIT_ONE As Char = "1"c

			Public Const DIGIT_TWO As Char = "2"c

			Public Const DIGIT_THREE As Char = "3"c

			Public Const DIGIT_FOUR As Char = "4"c

			Public Const DIGIT_FIVE As Char = "5"c

			Public Const DIGIT_SIX As Char = "6"c

			Public Const DIGIT_SEVEN As Char = "7"c

			Public Const DIGIT_EIGHT As Char = "8"c

			Public Const DIGIT_NINE As Char = "9"c

			Public Shared DIGITS As Char() = _
				New Char() _
					{ _
						DIGIT_ZERO, _
						DIGIT_ONE, _
						DIGIT_TWO, _
						DIGIT_THREE, _
						DIGIT_FOUR, _
						DIGIT_FIVE, _
						DIGIT_SIX, _
						DIGIT_SEVEN, _
						DIGIT_EIGHT, _
						DIGIT_NINE _
					}

		#End Region

		#Region " Letters "

			Public Const LETTER_A As Char = "A"c

			Public Const LETTER_B As Char = "B"c

			Public Const LETTER_C As Char = "C"c

			Public Const LETTER_D As Char = "D"c

			Public Const LETTER_E As Char = "E"c

			Public Const LETTER_F As Char = "F"c

			Public Const LETTER_G As Char = "G"c

			Public Const LETTER_H As Char = "H"c

			Public Const LETTER_I As Char = "I"c

			Public Const LETTER_J As Char = "J"c

			Public Const LETTER_K As Char = "K"c

			Public Const LETTER_L As Char = "L"c

			Public Const LETTER_M As Char = "M"c

			Public Const LETTER_N As Char = "N"c

			Public Const LETTER_O As Char = "O"c

			Public Const LETTER_P As Char = "P"c

			Public Const LETTER_Q As Char = "Q"c

			Public Const LETTER_R As Char = "R"c

			Public Const LETTER_S As Char = "S"c

			Public Const LETTER_T As Char = "T"c

			Public Const LETTER_U As Char = "U"c

			Public Const LETTER_V As Char = "V"c

			Public Const LETTER_W As Char = "W"c

			Public Const LETTER_X As Char = "X"c

			Public Const LETTER_Y As Char = "Y"c

			Public Const LETTER_Z As Char = "Z"c

			Public Shared UPPERCASE_LETTERS As Char() = _
				New Char() _
					{ _
						LETTER_A, LETTER_B, LETTER_C, LETTER_D, LETTER_E, LETTER_F, LETTER_G, _
						LETTER_H, LETTER_I, LETTER_J, LETTER_K, LETTER_L, LETTER_M, LETTER_N, _
						LETTER_O, LETTER_P, LETTER_Q, LETTER_R, LETTER_S, LETTER_T, LETTER_U, _
						LETTER_V, LETTER_W, LETTER_X, LETTER_Y, LETTER_Z _
					}

			Public Shared LOWERCASE_LETTERS As Char() = _
				New Char() _
					{ _
						Char.ToLower(LETTER_A), Char.ToLower(LETTER_B), Char.ToLower(LETTER_C), _
						Char.ToLower(LETTER_D), Char.ToLower(LETTER_E), Char.ToLower(LETTER_F), _
						Char.ToLower(LETTER_G), Char.ToLower(LETTER_H), Char.ToLower(LETTER_I), _
						Char.ToLower(LETTER_J), Char.ToLower(LETTER_K), Char.ToLower(LETTER_L), _
						Char.ToLower(LETTER_M), Char.ToLower(LETTER_N), Char.ToLower(LETTER_O), _
						Char.ToLower(LETTER_P), Char.ToLower(LETTER_Q), Char.ToLower(LETTER_R), _
						Char.ToLower(LETTER_S), Char.ToLower(LETTER_T), Char.ToLower(LETTER_U), _
						Char.ToLower(LETTER_V), Char.ToLower(LETTER_W), Char.ToLower(LETTER_X), _
						Char.ToLower(LETTER_Y), Char.ToLower(LETTER_Z) _
					}

		#End Region

		#Region " Punctuation / Symbols "

			Public Const AMPERSAND As Char = "&"c

			Public Const ANGLE_BRACKET_END As Char = ">"c

			Public Const ANGLE_BRACKET_START As Char = "<"c

			Public Const ASTERISK As Char = "*"c

			Public Const AT_SIGN As Char = "@"c

			Public Const BACK_SLASH As Char = "\"c

			Public Const BRACE_END As Char = "}"c

			Public Const BRACE_START As Char = "{"c

			Public Const BRACKET_END As Char = ")"c

			Public Const BRACKET_START As Char = "("c

			Public Const COLON As Char = ":"c

			Public Const COMMA As Char = ","c

			Public Const DOLLAR As Char = "$"c

			Public Const EXCLAMATION_MARK As Char = "!"c

			Public Const EQUALS_SIGN As Char = "="c

			Public Const FORWARD_SLASH As Char = "/"c

			Public Const FULL_STOP As Char = "."c

			Public Const HASH As Char = "#"c

			Public Const HYPHEN As Char = "-"c

			Public Const PERCENTAGE_MARK As Char = "%"c

			Public Const PIPE As Char = "|"c

			Public Const PLUS As Char = "+"c

			Public Const QUESTION_MARK As Char = "?"c

			Public Const QUOTE_SINGLE As Char = "'"c

			Public Const QUOTE_DOUBLE As Char = """"c

			Public Const SEMI_COLON As Char = ";"c

			Public Const SPACE As Char = " "c

			Public Const SQUARE_BRACKET_END As Char = "]"c

			Public Const SQUARE_BRACKET_START As Char = "["c

			Public Const TILDA As Char = "~"c

			Public Const [TAB] As Char = Microsoft.VisualBasic.ChrW(&H9)

			Public Const UNDER_SCORE As Char = "_"c

		#End Region

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


			''' <summary>
			''' Method to Create a Phonetic Representation of a Password to help ensure complex
			''' Password comprehension.
			''' </summary>
			''' <param name="password">The Password to create the Phonetic of.</param>
			''' <returns>The Phonetic Representation as a String.</returns>
			''' <remarks></remarks>
			Public Shared Function PhoneticPassword( _
				ByVal password As String _
			) As String

				Dim hashPhonetic As New Hashtable

				' Numbers
				For i As Integer = 0 To DIGITS.Length - 1

					hashPhonetic.Add(DIGITS(i), PHONETIC_DIGITS(i))

				Next

				' Upper Case Letters
				For i As Integer = 0 To UPPERCASE_LETTERS.Length - 1

					hashPhonetic.Add(UPPERCASE_LETTERS(i), PHONETIC_UPPERCASE_LETTERS(i))

				Next

				' Lower Case Letters
				For i As Integer = 0 To LOWERCASE_LETTERS.Length - 1

					hashPhonetic.Add(LOWERCASE_LETTERS(i), PHONETIC_LOWERCASE_LETTERS(i))

				Next

				Dim sbBuilder As New System.Text.StringBuilder

				sbBuilder.Append(BRACE_START)

				For i As Integer = 0 To password.Length - 1

					Dim singleCharacter As Char = password.Substring(i, 1).Chars(0)

					sbBuilder.Append(SPACE)
					If i <> 0 Then
						sbBuilder.Append(HYPHEN)
						sbBuilder.Append(SPACE)
					End If

					If hashPhonetic.Contains(singleCharacter) Then
						sbBuilder.Append(hashPhonetic(singleCharacter))
					Else
						sbBuilder.Append(singleCharacter)
					End If

				Next

				sbBuilder.Append(SPACE)
				sbBuilder.Append(BRACE_END)

				Return sbBuilder.ToString

			End Function


		#End Region

	End Class

End Namespace