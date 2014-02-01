#If TESTING Then

Imports Hermes.Web
Imports NUnit.Framework

Namespace Testing.Web

	<TestFixture(Description:="Tests Web Url Functionality")> _
	Public Class UrlTests

		<Test(Description:="URL String Parsing Test 1"), _
			ExpectedException(GetType(System.ArgumentException), _
			ExpectedMessage:="Value must be present" & vbCrLf & "Parameter name: value")> _
		Public Sub Url_Parsing_Test_1()

			Dim _url As Url = Url.Parse(String.Empty)

		End Sub

		<Test(Description:="URL String Parsing Test 2")> _
		Public Sub Url_Parsing_Test_2()

			Dim _url As Url = Url.Parse("http://localhost/default.aspx?test=true")

			Assert.AreEqual("http://localhost/default.aspx", _url.Base)
			Assert.AreEqual("default.aspx", _url.Relative)

			Assert.AreEqual(_url.Queries.Count, 1)
			For Each single_Key As String in _url.Queries.Keys
				Assert.AreEqual(single_Key, "test")
				Assert.AreEqual(_url.Queries(single_Key), "true")
			Next

		End Sub

		<Test(Description:="URL String Parsing Test 3")> _
		Public Sub Url_Parsing_Test_3()

			Dim _url As Url = Url.Parse("http://localhost:32768/default.aspx?test=true")

			Assert.AreEqual("http://localhost:32768/default.aspx", _url.Base)
			Assert.AreEqual("default.aspx", _url.Relative)

			Assert.AreEqual(_url.Queries.Count, 1)
			For Each single_Key As String in _url.Queries.Keys
				Assert.AreEqual(single_Key, "test")
				Assert.AreEqual(_url.Queries(single_Key), "true")
			Next

		End Sub

		<Test(Description:="URL String Parsing Test 4")> _
		Public Sub Url_Parsing_Test_4()

			Dim _url As Url = Url.Parse("http://localhost:32768/default.aspx?test=true")

			Assert.AreEqual("http://localhost:32768/default.aspx?test=true", _url.Build_Full)
			Assert.AreEqual("default.aspx?test=true", _url.Build_Relative)

		End Sub

		<Test(Description:="URL String Parsing Test 5")> _
		Public Sub Url_Parsing_Test_5()

			Dim _url As Url = Url.Parse("http://localhost:32768/default.aspx?test=true")

			_url.Update_Param("test", "false")
			Assert.AreEqual("http://localhost:32768/default.aspx?test=false", _url.Build_Full)
			Assert.AreEqual("default.aspx?test=false", _url.Build_Relative)

		End Sub

		<Test(Description:="URL String Parsing Test 6")> _
		Public Sub Url_Parsing_Test_6()

			Dim _url As Url = Url.Parse("http://localhost:32768/default.aspx?test=true")

			_url.Clear_Params()
			Assert.AreEqual("http://localhost:32768/default.aspx", _url.Build_Full)
			Assert.AreEqual("default.aspx", _url.Build_Relative)

		End Sub

		<Test(Description:="URL String Parsing Test 7")> _
		Public Sub Url_Parsing_Test_7()

			Dim _url As Url = Url.Parse("http://localhost:32768/default.aspx?test=true")

			_url.Remove_Param("test")
			Assert.AreEqual("http://localhost:32768/default.aspx", _url.Build_Full)
			Assert.AreEqual("default.aspx", _url.Build_Relative)

		End Sub

	End Class

End Namespace

#End If