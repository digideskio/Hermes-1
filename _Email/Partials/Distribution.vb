Imports System.Net

Namespace Email

	Public Partial Class Distribution

		#Region " Public Properties "

			Public ReadOnly Property Count As Integer
				Get
					If Addresses Is Nothing Then
						Return 0
					Else
						Return Addresses.Count
					End If
				End Get
			End Property

		#End Region

		#Region " Private Methods "

			Private Function DivideWithRemainder( _
				ByVal dividend As System.Int32, _
				ByVal divisor As System.Int32, _
				ByRef remainder As System.Int32 _
			) As System.Int32

				Dim quotient As System.Int32 = CInt(Math.Floor(dividend / divisor))
				remainder = dividend - (divisor * quotient)
				Return quotient

			End Function

		#End Region

		#Region " Public Methods "

			Public Function Add( _
				ByVal addresses As String() _
			) As Distribution

				If Not addresses Is Nothing

					For i As Integer = 0 to addresses.Length - 1

						Add(addresses(i))

					Next

				End If

				Return Me

			End Function

			Public Function Add( _
				ByVal addresses As List(Of String) _
			) As Distribution

				If Not addresses Is Nothing

					For i As Integer = 0 to addresses.Count - 1

						Add(addresses(i))

					Next

				End If

				Return Me

			End Function

			Public Function Add( _
				ByVal address As String, _
				Optional ByVal display As String = Nothing _
			) As Distribution

				Return Add(Manager.CreateAddress(address, display))

			End Function

			Public Function Add( _
				ByVal address As Mail.MailAddress _
			) As Distribution

				If Not Addresses.Contains(address) Then Addresses.Add(address)

				Return Me

			End Function

			Public Function Split( _
				ByVal max_Chunk As System.Int32 _
			) As Mail.MailAddress()()

				If Sending_Format = SendingType.Single_Recipient Then

					Dim return_Array(Addresses.Count - 1)() As Mail.MailAddress

					For i As Integer = 0 To Addresses.Count - 1

						return_Array(i) = New Mail.MailAddress() {Addresses(i)}

					Next

					Return return_Array

				ElseIf max_Chunk > 0 AndAlso Addresses.Count > max_Chunk Then

					Dim final_Batch As System.Int32
					Dim total_FullBatches As System.Int32 = DivideWithRemainder(Addresses.Count, max_Chunk, final_Batch)

					Dim return_List As New List(Of Mail.MailAddress())

					For i As Integer = 0 To total_FullBatches - 1

						Dim address_Array(max_Chunk - 1) As Mail.MailAddress
						Addresses.CopyTo(i * max_Chunk, address_Array, 0, max_Chunk)
						return_List.Add(address_Array)

					Next

					If final_Batch > 0 Then

						Dim address_Array(final_Batch - 1) As Mail.MailAddress
						Addresses.CopyTo(Addresses.Count - final_Batch, address_Array, 0, final_Batch)
						return_List.Add(address_Array)

					End If

					Return return_List.ToArray()

				Else

					Return New Mail.MailAddress()() {Addresses.ToArray()}

				End If

			End Function

		#End Region

	End Class

End Namespace