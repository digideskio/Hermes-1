Namespace Files

	Public Partial Class Document

		Public ReadOnly Property Path As String
			Get
				Dim ret_Val As String = Nothing
				Dim l_parent As Container = Parent

				While Not l_parent Is Nothing
					If Not String.IsNullOrEmpty(ret_Val) Then ret_Val = "|" + ret_Val
					ret_Val = l_parent.Name + ret_Val
					l_parent = l_parent.Parent
				End While
				
				Return ret_Val
			End Get
		End Property

	End Class
	
End Namespace