Imports System.Collections.Specialized
Imports System.Drawing
Imports System.Drawing.ColorTranslator
Imports System.IO.File
Imports System.Net
Imports System.Net.Sockets
Imports System.Text.RegularExpressions
Imports System.Web
Imports System.Web.Caching
Imports System.Xml

Namespace Web.Handlers

	Public Class InterpretedStyleSheetHandler
		Implements System.Web.IHttpHandler

		#Region " Private Shared Constants "

			Private Const BROWSER_IE = "ie"

			Private Const BROWSER_OPERA = "opera"

			Private Const BROWSER_CHROME = "chrome"

			Private Const BROWSER_SAFARI = "safari"

			Private Const BROWSER_FIREFOX = "firefox"

			Private Const BROWSER_WEBKIT = "webkit"

			Private Const BROWSER_MOZILLA = "mozilla"

			Private Const BROWSER_OTHER = "other"

		#End Region

		#Region " Public Shared Constants "

			Public Const QUERY_SOURCE As String = "source"

			Public Const PARAMETER_MARKER As String = "@"

			Public Const PARAMETER_DELINEATOR As String = ","

			Public Const XML_MARKER As String = "$"

			Public Const XML_FUNCTION_SELECT As String = "select"

			Public Const XML_FUNCTION_RANDOM As String = "random"

			Public Const XML_ROOT_SELECTOR As String = "//"

			Public Const XML_DOCUMENT_SUFFIX As String = ".XML"

			Public Const XML_PRELOAD_DELINEATOR As String = "|"

			Public Const FUNCTION_MARKER As String = "!"

			Public Const FUNCTION_IF As String = "if"

			Public Const FUNCTION_DELINEATOR As String = "|"

			Public Const FUNCTION_AND As String = "&&"

			Public Const FUNCTION_LOOP As String = "loop"

			Public Const LOOP_RANDOM As String = "random"

			Public Const LOOP_ITERATOR As String = "iterator"

			Public Const LOOP_RANGE As String = "-"

			Public Const LOOP_LIST As String = ";"

			Public Const OPEN_BRACKET As String = "("

			Public Const CLOSE_BRACKET As String = ")"

		#End Region

		#Region " Private Functions "

			''' <summary>Changes a Colour</summary>
			''' <param name="value">The Colour to Change</param>
			''' <param name="delta">Negative delta darkens, Positive delta lightens</param>
			Private Function Change_Colour( _
				ByVal value As Color, _
				ByVal delta As System.Int32 _
			) As Color

				If delta < 0 Then

					Return Color.FromArgb( _
						Math.Max(value.R + delta, 0), _
						Math.Max(value.G + delta, 0), _
						Math.Max(value.B + delta, 0))

				ElseIf delta > 0 Then

					Return Color.FromArgb( _
						Math.Min(value.R + delta, 255), _
						Math.Min(value.G + delta, 255), _
						Math.Min(value.B + delta, 255))

				Else

					Return value

				End If

			End Function

			''' <summary>Loads XML Document from the XML Source Line.</summary>
			''' <param name="xml_SourceLine">The CSS Class Line which contains document name/path (e.g. document.xml//DocumentNode/ChildNode)</param>
			''' <param name="server">Current HTTP Request Server Utility (for path mapping)</param>
			''' <remarks></remarks>
			Private Function Load_XML( _
				ByRef xml_SourceLine As String, _
				ByVal server As HttpServerUtility, _
				ByVal loaded_Nodes As Dictionary(Of String, XmlNode) _
			) As XmlNode

				Dim xml_Name As String = xml_SourceLine.Substring(0, xml_SourceLine.IndexOf(XML_ROOT_SELECTOR))
				xml_SourceLine = xml_SourceLine.Substring(xml_SourceLine.IndexOf(XML_ROOT_SELECTOR))

				If xml_Name.ToUpper().EndsWith(XML_DOCUMENT_SUFFIX) Then

					Dim xml_Path As String = server.MapPath(xml_Name)

					Dim return_Doc As New XmlDocument

					If Exists(xml_Path) Then return_Doc.Load(xml_Path)

					Return return_Doc

				Else

					If loaded_Nodes.ContainsKey(xml_Name) Then
						xml_SourceLine = xml_SourceLine.Substring(XML_ROOT_SELECTOR.Length)
						Return loaded_Nodes(xml_Name)
					Else
						Return New XmlDocument()
					End If

				End If

			End Function

			''' <summary>Replaces xml tags in the CSS with variables from the Query String.</summary>
			''' <param name="variables">The Query String Parameters passed in the Request Context</param>
			''' <remarks>X-PATH format: FUNCTION$document.xml//Document/Node[@attribute=value]/Child$</remarks>
			Private Function Enumerate_XML( _
				ByVal css_Content As String, _
				ByVal server As HttpServerUtility, _
				ByVal loaded_Nodes As Dictionary(Of String, XmlNode) _
			) As String

				' -- Do the RANDOM Functions --
				Dim found_Random As Integer = css_Content.IndexOf(XML_FUNCTION_RANDOM.ToUpper() & XML_MARKER, 0)

				Do Until found_Random < 0

					Dim start_Position As Integer = found_Random + XML_MARKER.Length + XML_FUNCTION_RANDOM.Length
					Dim end_Position As Integer = css_Content.IndexOf(XML_MARKER, start_Position)

					Dim xml_SourceLine As String = css_Content.Substring(start_Position, end_Position - start_Position)

					Dim target_Name As String = Nothing
					If xml_SourceLine.IndexOf(XML_PRELOAD_DELINEATOR) > 0 Then
						target_Name = xml_SourceLine.Split(XML_PRELOAD_DELINEATOR(0))(1)
            			xml_SourceLine = xml_SourceLine.Split(XML_PRELOAD_DELINEATOR(0))(0)
					End If
					Dim target_Doc As XmlNode = Load_XML(xml_SourceLine, server, loaded_Nodes)

					Dim random_Parameters As String() = xml_SourceLine.Split(PARAMETER_DELINEATOR(0))

					If random_Parameters.Length = 3 Then

						Dim target_Nodeset As XmlNodeList = target_Doc.SelectNodes(random_Parameters(0))

						Dim total_Weighting As Double = 0
						Dim weighted_List As New SortedDictionary(Of System.Double, XmlNode)

						For Each target_Node As XmlNode In target_Nodeset

							Dim single_Weighting As Double = Double.Parse(target_Node.SelectSingleNode(random_Parameters(1)).Value)

							If single_Weighting > 0 Then
								total_Weighting += single_Weighting
								weighted_List.Add(total_Weighting, target_Node)
							End If

						Next

						Dim rnd As Double = New Random().NextDouble() * total_Weighting

						For Each target_Weighting As Double In weighted_List.Keys

							If rnd <= target_Weighting Then

								css_Content = css_Content.Remove(found_Random, (end_Position + XML_MARKER.Length) - found_Random)

								If String.IsNullOrEmpty(target_Name) Then
									css_Content = css_Content.Insert(found_Random, weighted_List(target_Weighting).SelectSingleNode(random_Parameters(2)).InnerText)
								Else
									Loaded_Nodes.Add(target_Name, weighted_List(target_Weighting).SelectSingleNode(random_Parameters(2)))
								End If

								end_Position = found_Random
								Exit For

							End If

						Next

					End If

					found_Random = css_Content.IndexOf(XML_FUNCTION_RANDOM.ToUpper() & XML_MARKER, end_Position)

				Loop

				' -- Do the SELECT Functions --
				Dim found_Select As Integer = css_Content.IndexOf(XML_FUNCTION_SELECT.ToUpper() & XML_MARKER, 0)

				Do Until found_Select < 0

					Dim start_Position As Integer = found_Select + XML_FUNCTION_SELECT.Length + XML_MARKER.Length
					Dim end_Position As Integer = css_Content.IndexOf(XML_MARKER, start_Position)

					Dim xml_SourceLine As String = css_Content.Substring(start_Position, end_Position - start_Position)
					Dim target_Name As String = Nothing
					If xml_SourceLine.IndexOf(XML_PRELOAD_DELINEATOR) > 0 Then
						target_Name = xml_SourceLine.Split(XML_PRELOAD_DELINEATOR(0))(1)
            			xml_SourceLine = xml_SourceLine.Split(XML_PRELOAD_DELINEATOR(0))(0)
					End If

					Dim target_Doc As XmlNode = Load_XML(xml_SourceLine, server, loaded_Nodes)

					Dim target_Node As XmlNode = target_Doc.SelectSingleNode(xml_SourceLine)

					If Not target_Node Is Nothing Then

						css_Content = css_Content.Remove(found_Select, (end_Position + XML_MARKER.Length) - found_Select)

						If String.IsNullOrEmpty(target_Name) Then
							css_Content = css_Content.Insert(found_Select, target_Node.InnerText)
						Else
							Loaded_Nodes.Add(target_Name, target_Node)
						End If
						end_Position = found_Select

					End If

					found_Select = css_Content.IndexOf(XML_FUNCTION_SELECT.ToUpper() & XML_MARKER, end_Position)

				Loop

				Return css_Content

			End Function

			''' <summary>Evaluates Simple Conditional</summary>
			Private Function Enumerate_Conditional( _
				ByVal evaluation_Statement As String _
			) As Boolean

				Dim evaluates As Boolean = False

				Dim evaluation_left As String
				Dim evaluation_right As String()
				Dim operator_Position As Integer = -1
				Dim operator_Length As Integer = 1
				Dim operator_Type As ComparativeOperator = ComparativeOperator.None

				If evaluation_Statement.IndexOf(">=") > 0 Then

					operator_Position = evaluation_Statement.IndexOf(">=")
					operator_Length = 2
					operator_Type = ComparativeOperator.Greater_Than_Or_Equals

				ElseIf evaluation_Statement.IndexOf("<=") > 0 Then

					operator_Position = evaluation_Statement.IndexOf("<=")
					operator_Length = 2
					operator_Type = ComparativeOperator.Less_Than_Or_Equals

				ElseIf evaluation_Statement.IndexOf("=") > 0 Then

					operator_Position = evaluation_Statement.IndexOf("=")
					operator_Type = ComparativeOperator.Equals

				ElseIf evaluation_Statement.IndexOf("<") > 0 Then

					operator_Position = evaluation_Statement.IndexOf("<")
					operator_Type = ComparativeOperator.Less_Than

				ElseIf evaluation_Statement.IndexOf(">") > 0 Then

					operator_Position = evaluation_Statement.IndexOf(">")
					operator_Type = ComparativeOperator.Greater_Than

				End If

				If operator_Type <> ComparativeOperator.None Then

					evaluation_left = evaluation_Statement.SubString(0, operator_Position)
					evaluation_right = evaluation_Statement.SubString(operator_Position + operator_Length).Split(PARAMETER_DELINEATOR(0))

					For i As Integer = 0 To evaluation_right.Length - 1

						If operator_Type = ComparativeOperator.Equals Then

							If String.Compare(evaluation_left, evaluation_right(i), True) = 0 Then
								evaluates = True
								Exit For
							End If

						Else

							Dim numerical_Left As Double = Double.Parse(evaluation_left)
							Dim numerical_Right As Double = Double.Parse(evaluation_right(i))

							If operator_Type = ComparativeOperator.Greater_Than_Or_Equals Then
									
								If numerical_Left >= numerical_Right Then
									evaluates = True
									Exit For
								End If

							ElseIf operator_Type = ComparativeOperator.Less_Than_Or_Equals Then
									
								If numerical_Left <= numerical_Right Then
									evaluates = True
									Exit For
								End If

							ElseIf operator_Type = ComparativeOperator.Greater_Than Then
									
								If numerical_Left > numerical_Right Then
									evaluates = True
									Exit For
								End If

							ElseIf operator_Type = ComparativeOperator.Less_Than Then

								If numerical_Left < numerical_Right Then
									evaluates = True
									Exit For
								End If

							End If

						End If

					Next

				End If

				Return evaluates

			End Function

			''' <summary>Evaluates Simple Conditionals</summary>
			''' <remarks>IF!@type@=staff|background: rgba(0, 0, 0, 0.2);!</remarks>
			Private Function Enumerate_Conditionals( _
				ByVal css_Content As String _
			) As String

				Dim found_If As Integer = css_Content.IndexOf(FUNCTION_IF.ToUpper() & FUNCTION_MARKER, 0)

				Do Until found_If < 0

					Dim start_Position As Integer = found_If + FUNCTION_MARKER.Length + FUNCTION_IF.Length
					Dim end_Position As Integer = css_Content.IndexOf(FUNCTION_MARKER, start_Position)

					Dim evaluates As Boolean = False
					Dim conditional_SourceLine As String = css_Content.Substring(start_Position, end_Position - start_Position)
					Dim evaluation_Statement As String = conditional_SourceLine.Split(FUNCTION_DELINEATOR(0))(0)
					Dim conditional_Output As String = conditional_SourceLine.Split(FUNCTION_DELINEATOR(0))(1)

					If evaluation_Statement.IndexOf(FUNCTION_AND) > 0 Then ' AND
						Dim evaluation_Statements As New List(Of String)(evaluation_Statement.Split( _
							New String() {FUNCTION_AND}, StringSplitOptions.RemoveEmptyEntries))
						evaluates = True
						For i As Integer = 0 To evaluation_Statements.Count - 1
							If Not enumerate_Conditional(evaluation_Statements(i)) Then
								evaluates = False
								Exit For
							End If
						Next
					Else
						evaluates = Enumerate_Conditional(evaluation_Statement)
					End If

					css_Content = css_Content.Remove(found_If, (end_Position + FUNCTION_MARKER.Length) - found_If)

					If evaluates Then css_Content = css_Content.Insert(found_If, conditional_Output)

					found_If = css_Content.IndexOf(FUNCTION_IF.ToUpper() & FUNCTION_MARKER, found_If)

				Loop

				Return css_Content

			End Function

			''' <summary>Replaces parameter tags in the CSS with variables from the Query String.</summary>
			''' <param name="variables">The Query String Parameters passed in the Request Context</param>
			''' <remarks>Variable format: @HEADER@ or @BACKGROUND-50@</remarks>
			Private Function Enumerate_Parameters( _
				ByVal css_Content As String, _
				ByVal variables As NameValueCollection, _
				ByVal param_Marker As String _
			) As String

				For Each param As String In variables.AllKeys

					If Not param = QUERY_SOURCE Then

						Dim found_Position As Integer = css_Content.IndexOf(param_Marker & param.ToUpper(), 0)

						Do Until found_Position < 0

							Dim start_Position As Integer = found_Position + param_Marker.Length + param.Length
							Dim end_Position As Integer = css_Content.IndexOf(param_Marker, start_Position)

							Dim value As String = Nothing

							If end_Position = start_Position Then

								value = variables(param)

							ElseIf end_Position > start_Position

								' Colour Variation
								Dim colour_Variation As Integer
								If Integer.TryParse(css_Content.Substring(start_Position, end_Position - start_Position), colour_Variation) Then _
									value = ToHtml(Change_Colour(FromHtml(variables(param)), colour_Variation))

							End If

							If Not String.IsNullOrEmpty(value) Then

								css_Content = css_Content.Remove(found_Position, (end_Position + param_Marker.Length) - found_Position) _
									.Insert(found_Position, value)
								found_Position = css_Content.IndexOf(param_Marker & param.ToUpper(), value.Length)

							Else

								found_Position = css_Content.IndexOf(param_Marker & param.ToUpper(), found_Position + 1)

							End If

						Loop

					End If

				Next

				Return css_Content

			End Function

			''' <summary>Executes Simple Loops</summary>
			''' <remarks>LOOP!1-10|background: rgba(RANDOM(0-255), RANDOM(0;10;20;30), @ITERATOR@, 0.2);!</remarks>
			Private Function Execute_Loops( _
				ByVal css_Content As String _
			) As String

				Dim found_Loop As Integer = css_Content.IndexOf(FUNCTION_LOOP.ToUpper() & FUNCTION_MARKER, 0)
				Dim r As New Random()

				Do Until found_Loop < 0

					Dim start_Position As Integer = found_Loop + FUNCTION_MARKER.Length + FUNCTION_LOOP.Length
					Dim end_Position As Integer = css_Content.IndexOf(FUNCTION_MARKER, start_Position)

					Dim loop_SourceLine As String = css_Content.Substring(start_Position, end_Position - start_Position)
					Dim loop_Statement As String = loop_SourceLine.Split(FUNCTION_DELINEATOR(0))(0)
					Dim loop_Output As String = loop_SourceLine.Split(FUNCTION_DELINEATOR(0))(1)

					Dim loop_From As Integer = Integer.Parse(loop_Statement.Split(LOOP_RANGE(0))(0))
					Dim loop_To As Integer = Integer.Parse(loop_Statement.Split(LOOP_RANGE(0))(1))
					
					Dim loop_Lines As String = Nothing

					For i As Integer = loop_From To loop_To

						Dim loop_Line As String = loop_Output.Replace(PARAMETER_MARKER & LOOP_ITERATOR.ToUpper() & PARAMETER_MARKER, i.ToString())

						Dim found_Random As Integer = loop_Line.IndexOf(LOOP_RANDOM.ToUpper() & OPEN_BRACKET, 0)
						
						Do Until found_Random < 0

							Dim start_Random As Integer = found_Random + OPEN_BRACKET.Length + LOOP_RANDOM.Length
							Dim end_Random As Integer = loop_Line.IndexOf(CLOSE_BRACKET, start_Random)
							Dim random_Details As String = loop_Line.Substring(start_Random, end_Random - start_Random)

							Dim insertion As String = Nothing

							If random_Details.IndexOf(LOOP_LIST) > 0 Then

								Dim random_Options As String() = random_Details.Split(LOOP_LIST(0))
								insertion = random_Options(r.Next(0, random_Options.Length))

							ElseIf random_Details.IndexOf(LOOP_RANGE) > 0 Then

								Dim random_From As Integer = Integer.Parse(random_Details.Split(LOOP_RANGE(0))(0))
								Dim random_To As Integer = Integer.Parse(random_Details.Split(LOOP_RANGE(0))(1))
								insertion = r.Next(random_From, random_To + 1).ToString()

							End If

							loop_Line = loop_Line.Remove(found_Random, (end_Random + CLOSE_BRACKET.Length) - found_Random)
							loop_Line = loop_Line.Insert(found_Random, insertion)

							found_Random = loop_Line.IndexOf(LOOP_RANDOM.ToUpper() & OPEN_BRACKET, found_Random)

						Loop

						loop_Lines = loop_Lines & loop_Line

					Next

					css_Content = css_Content.Remove(found_Loop, (end_Position + FUNCTION_MARKER.Length) - found_Loop)

					css_Content = css_Content.Insert(found_Loop, loop_Lines)

					found_Loop = css_Content.IndexOf(FUNCTION_LOOP.ToUpper() & FUNCTION_MARKER, found_Loop)

				Loop

				Return css_Content

			End Function

			''' <summary>Gets the Browser Code</summary>
			Private Function Get_Browser( _
				ByVal browser As HttpBrowserCapabilities, _
				ByVal userAgent As String _
			) As String

				If browser.Browser.ToUpper().Contains("MSIE") OrElse browser.Browser.ToUpper().Contains("IE") Then

					Return BROWSER_IE

				Else If browser.Browser.ToUpper().Contains("OPERA") Then

					Return BROWSER_OPERA

				Else If browser.Browser.ToUpper().Contains("FIREFOX") Then

					Return BROWSER_FIREFOX

				Else If browser.Browser.ToUpper().Contains("CHROME") Then

					return BROWSER_CHROME

				Else If browser.Browser.ToUpper().Contains("APPLEWEBKIT") Then

					return BROWSER_SAFARI

				Else If browser.Browser.ToUpper().Contains("WEBKIT")

					return BROWSER_WEBKIT

				Else If browser.Browser.ToUpper().Contains("MOZILLA") Then

					return BROWSER_MOZILLA

				Else

					return BROWSER_OTHER

				End If

			End Function

			''' <summary>Gets the Browser Version</summary>
			Private Function Get_Browser_Version( _
				ByVal browser As HttpBrowserCapabilities, _
				ByVal userAgent As String _
			) As Double

				Dim version As String = "Version/"
				If userAgent.IndexOf(version) > 0 Then
					Dim version_String As String = userAgent.SubString(userAgent.IndexOf(version) + version.Length)
					If version_String.IndexOf(" ") > 0 Then
						version_String = version_String.SubString(0, version_String.IndexOf(" "))
					End If
					Dim return_Val As Double
					If Double.TryParse(version_String, return_Val) Then return return_Val
				End If

				Return CDbl(browser.Version)

			End Function

			''' <summary>Loads and Compresses Stylesheet, replacing parameter tags in the CSS with variables from the Query String.</summary>
			''' <param name="variables">The Query String Parameters passed in the Request Context</param>
			''' <remarks>Variable format: @HEADER@ or @BACKGROUND-50@</remarks>
			Private Function Format_Stylesheet( _
				ByVal css_Content As String, _
				ByVal variables As NameValueCollection, _
				ByVal server As HttpServerUtility, _
				ByVal browser As HttpBrowserCapabilities, _
				ByVal userAgent As String _
			) As String

				' -- Get Browser Code as Variable --
				variables = New NameValueCollection(variables)
				variables.Add("BROWSER", Get_Browser(browser, userAgent))
				variables.Add("BROWSER_VERSION", Get_Browser_Version(browser, userAgent).ToString())

				' -- Execute Loops --
				css_Content = Execute_Loops(css_Content)

				' -- Replace Placeholder Parameters (Pre-XML) --
				css_Content = Enumerate_Parameters(css_Content, variables, PARAMETER_MARKER)

				' -- Evaluate Conditionals (Pre-XML) --
				css_Content = Enumerate_Conditionals(css_Content)

				' -- Replace XML Parameters --
				css_Content = Enumerate_XML(css_Content, server, New Dictionary(Of String, XmlNode))

				' -- Remove Whitespace --
				css_Content = css_Content _
					.Replace("  ", " ") _
					.Replace(Environment.NewLine, String.Empty) _
					.Replace("\\t", String.Empty) _
					.Replace(vbTab, String.Empty) _
					.Replace(" {", "{") _
					.Replace(" :", ":") _
					.Replace(": ", ":") _
					.Replace(", ", ",") _
					.Replace("; ", ";") _
					.Replace(";}", "}") _
					.Replace("?", String.Empty)

				' -- Remove Comments etc --
				css_Content = Regex.Replace(css_Content, "(?<=[>])\s{2,}(?=[<])|(?<=[>])\s{2,}(?=&nbsp;)|(?<=&ndsp;)\s{2,}(?=[<])", String.Empty)
				css_Content = Regex.Replace(css_Content, "/\*[\d\D]*?\*/", String.Empty)

				Return css_Content

			End Function

		#End Region

		#Region " IHttpHandler Implementation "

			Public ReadOnly Property IsReusable() As Boolean _
			Implements System.Web.IHttpHandler.IsReusable
				Get
					Return True
				End Get
			End Property

			Public Sub ProcessRequest( _
				ByVal context As System.Web.HttpContext _
			) Implements System.Web.IHttpHandler.ProcessRequest

				Try

					context.Response.Clear()

					Dim css_Path As String = context.Server.MapPath(context.Request.QueryString(QUERY_SOURCE))

					If Exists(css_Path) Then

						Dim return_Css As String = Format_Stylesheet(ReadAllText(css_Path), context.Request.QueryString, context.Server, context.Request.Browser, context.Request.UserAgent)

						context.Response.ContentType = "text/css"
						context.Response.Cache.VaryByHeaders("Accept-Encoding") = True

						Dim caching_Tag As String = return_Css.GetHashCode().ToString()
						Dim request_Tag As String = context.Request.Headers("If-None-Match")

						context.Response.Cache.SetExpires(DateTime.Now.ToUniversalTime().AddHours(1))
						context.Response.Cache.SetMaxAge(New TimeSpan(1, 0, 0))
						context.Response.Cache.SetETag(caching_Tag)
						context.Response.Cache.SetCacheability(System.Web.HttpCacheability.Public)
						context.Response.Cache.SetValidUntilExpires(True)
						context.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches)

						If String.Compare(request_Tag, caching_Tag) = 0 Then

							context.Response.StatusCode = System.Net.HttpStatusCode.NotModified
							context.Response.SuppressContent = True

						End If

						context.Response.Write(return_Css)

					Else

						context.Response.StatusCode = System.Net.HttpStatusCode.NotFound

					End If

					context.Response.Flush()

				Catch ex As Exception

					context.Response.StatusCode = System.Net.HttpStatusCode.InternalServerError
					context.Response.Write(ex.ToString)

				End Try

			End Sub

		#End Region

	End Class

End Namespace
