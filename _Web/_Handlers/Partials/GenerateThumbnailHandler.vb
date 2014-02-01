Imports Microsoft.VisualBasic
Imports System.IO

Namespace Web.Handlers

	Public Class GenerateThumbnailHandler
		Implements System.Web.IHttpHandler

		#Region " Public Shared Constants "

			Public Const QUERY_IMAGE As String = "image"

			Public Const QUERY_CONSTRAINT As String = "constraint"

			Public Const QUERY_VALUE As String = "value"

			Public Const QUERY_MAXIMUM_OTHER As String = "maximum-other"

		#End Region

		#Region " Private Methods "

			Private Function GetThumbnailImageAbort() As Boolean
				Return False
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

					Dim image As String = context.Request.QueryString(QUERY_IMAGE)
					Dim constraint As String = context.Request.QueryString(QUERY_CONSTRAINT)
					Dim constraintValue As String = context.Request.QueryString(QUERY_VALUE)
					Dim maximumOtherValue As String = context.Request.QueryString(QUERY_MAXIMUM_OTHER)

					context.Response.Clear()
					context.Response.Cache.SetExpires(DateTime.Now.Add(New TimeSpan(0, 1, 0)))
					context.Response.Cache.SetCacheability(System.Web.HttpCacheability.Public)
					context.Response.Cache.SetValidUntilExpires(False)

					Dim output_Format As System.Drawing.Imaging.ImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg
					If image.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) Then
						context.Response.ContentType = "image/jpg"
						output_Format = System.Drawing.Imaging.ImageFormat.Jpeg
					ElseIf image.EndsWith(".png", StringComparison.OrdinalIgnoreCase) Then
						context.Response.ContentType = "image/png"
						output_Format = System.Drawing.Imaging.ImageFormat.Png
					End If

					If Not String.IsNullOrEmpty(image) Then

						image = context.Request.MapPath(image)

						If File.Exists(image) Then

							If Not String.IsNullOrEmpty(Constraint) Then

								Dim value As Integer
								Dim other_Value As Integer = -1

								If Not String.IsNullOrEmpty(constraintValue) AndAlso Integer.TryParse(constraintValue, value) Then

									If Not String.IsNullOrEmpty(maximumOtherValue) AndAlso Not Integer.TryParse(maximumOtherValue, other_Value) Then _
										other_Value = -1

									Dim fullImage As New System.Drawing.Bitmap(image)

									Dim source_X As Integer = 0
									Dim source_Y As Integer = 0
									Dim source_Width As Integer = fullImage.Width
									Dim source_Height As Integer = fullImage.Height

									Dim destination_Width As Integer
									Dim destination_Height As Integer

									If Constraint.ToLower = "height" Then

										destination_Width = CInt((value / source_Height) * source_Width)
										destination_Height = value

										If other_Value > 0 AndAlso destination_Width > other_Value Then

											Dim scaling_Ratio As Single = CSng(other_Value / destination_Width)
											source_Width = CInt(source_Width * scaling_Ratio)
											destination_Width = other_Value

										End If

									ElseIf Constraint.ToLower = "width" Then

										destination_Height = CInt((value / source_Width) * source_Height)
										destination_Width = value

										If other_Value > 0 AndAlso destination_Height > other_Value Then

											Dim scaling_Ratio As Single = CSng(other_Value / destination_Height)
											source_Height = CInt(source_Height * scaling_Ratio)
											source_Y = fullImage.Height - source_Height
											destination_Height = other_Value

										End If

									End If

									Dim img_Output As New System.Drawing.Bitmap(destination_Width, destination_Height)
									Dim gfx_Output As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(img_Output)

									gfx_Output.DrawImage(fullImage, _
										New System.Drawing.Rectangle(0, 0, destination_Width, destination_Height), _
										New System.Drawing.Rectangle(source_X, source_Y, source_Width, source_Height), _
										System.Drawing.GraphicsUnit.Pixel)

									gfx_Output.Dispose()
									gfx_Output = Nothing

									img_Output.Save(context.Response.OutputStream, output_Format)

									img_Output = Nothing
									fullImage = Nothing

								End If

							End If

						End If

					End If

					context.Response.Flush()

				Catch ex As Exception
				End Try

			End Sub

		#End Region

	End Class

End Namespace