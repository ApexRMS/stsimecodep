'*********************************************************************************************
' Ecological Departure: A SyncroSim Package for doing ecological departure analysis.
'
' Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports System.Drawing
Imports Microsoft.Office.Interop.Excel
Imports System.Runtime.InteropServices
Imports SyncroSim.Common.Forms

Partial Class ReportTransformer

    ''' <summary>
    ''' Exports the data to Excel
    ''' </summary>
    ''' <param name="dt">The table of data</param>
    ''' <param name="attributeSpecified">Whether or not an attribute was specified for at least one scenario</param>
    ''' <param name="maxIteration">The maximum iteration</param>
    ''' <param name="fileName">The file name for the report</param>
    ''' <remarks></remarks>
    Private Shared Sub ExportToExcel(
        ByVal dt As System.Data.DataTable,
        ByVal attributeSpecified As Boolean,
        ByVal maxIteration As Integer,
        ByVal fileName As String)

        Dim app As New Application()
        Dim wb As Workbook = CreateWorkbook(app)
        Dim wks As Worksheet = CreateWorksheet(wb)

        Try

            Using h As New HourGlass

                ExportData(dt, wks)
                CreateHeaderRow(dt, wks, attributeSpecified)
                ApplyCaptionFormatting(dt, wks)
                ApplyNumberFormatting(dt, wks)
                ApplyConditionalFormatting(dt, wks)
                ApplyBorderFormatting(dt, wks)
                AutoFilterWorksheet(dt, wks)
                AutoFitWorksheet(dt, wks)
                MergeAndCenterHeaderRow(wks, attributeSpecified, maxIteration)
                ShowAndSave(app, wb, fileName)

            End Using

        Catch ex As COMException

            MsgBox("An error occurred while exporting to Excel.  More information:" & vbCrLf & vbCrLf & ex.Message,
                   MsgBoxStyle.OkOnly,
                   "Ecological Departure")

        Finally

            Marshal.ReleaseComObject(wks)
            Marshal.ReleaseComObject(wb)
            Marshal.ReleaseComObject(app)

        End Try

        GC.Collect()

    End Sub

    ''' <summary>
    ''' Release control of Excel to the user
    ''' </summary>
    ''' <param name="app"></param>
    ''' <param name="wb"></param>
    ''' <param name="fileName"></param>
    ''' <remarks></remarks>
    Private Shared Sub ShowAndSave(ByVal app As Application, ByVal wb As Workbook, ByVal fileName As String)

        app.Visible = True
        app.UserControl = True
        app.WindowState = XlWindowState.xlMaximized

        wb.SaveAs(fileName, FileFormat:=51)

    End Sub

    ''' <summary>
    ''' Creates a new workbook in the specified app
    ''' </summary>
    ''' <param name="app"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function CreateWorkbook(ByVal app As Application) As Workbook

        app.Workbooks.Add()
        Return app.ActiveWorkbook

    End Function

    ''' <summary>
    ''' Creates a new worksheet in the specified workbook
    ''' </summary>
    ''' <param name="wb"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function CreateWorksheet(ByVal wb As Workbook) As Worksheet

        Dim wks As Worksheet = CType(wb.Worksheets.Add(), Worksheet)
        wks.Name = "Ecological Departure"

        DeleteWorksheet("Sheet1", wb)
        DeleteWorksheet("Sheet2", wb)
        DeleteWorksheet("Sheet3", wb)

        Return wks

    End Function

    ''' <summary>
    ''' Exports the data to the specified worksheet
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <param name="wks"></param>
    ''' <remarks></remarks>
    Private Shared Sub ExportData(ByVal dt As System.Data.DataTable, ByVal wks As Worksheet)

        Dim rg As Range = GetRange(wks, 2, 1, dt.Rows.Count + 2, dt.Columns.Count)
        Dim arr(dt.Rows.Count + 2, dt.Columns.Count) As Object
        Dim dv As New DataView(dt, Nothing, "Scenario,Timestep,Stratum", DataViewRowState.CurrentRows)
        Dim RowIndex As Integer = 1

        'Header row
        For ColumnIndex As Integer = 0 To dt.Columns.Count - 1
            arr(0, ColumnIndex) = dt.Columns(ColumnIndex).Caption
        Next

        'Data
        For Each drv As DataRowView In dv

            Dim dr As DataRow = drv.Row

            For ColumnIndex As Integer = 0 To dt.Columns.Count - 1
                arr(RowIndex, ColumnIndex) = dr(ColumnIndex)
            Next

            RowIndex += 1

        Next

        rg.Value = arr
        Marshal.ReleaseComObject(rg)

    End Sub

    ''' <summary>
    ''' Creates the primary header row
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <param name="wks"></param>
    ''' <param name="attributeSpecified"></param>
    ''' <remarks></remarks>
    Private Shared Sub CreateHeaderRow(ByVal dt As System.Data.DataTable, ByVal wks As Worksheet, ByVal attributeSpecified As Boolean)

        Dim ColsPerBlock As Integer = (dt.Columns.Count - 3) \ 2
        Dim Index1 As Integer = 4
        Dim Index2 As Integer = 3 + ColsPerBlock + 1

        Dim rg1 As Range = GetRange(wks, 1, Index1, 1, Index1)
        rg1.Value = "Departure"

        Marshal.ReleaseComObject(rg1)

        If (attributeSpecified) Then

            Dim rg2 As Range = GetRange(wks, 1, Index2, 1, Index2)
            rg2.Value = "Cumulative Attribute"

            Marshal.ReleaseComObject(rg2)

        End If

    End Sub

    ''' <summary>
    ''' Applies formatting to the column captions
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <param name="wks"></param>
    ''' <remarks></remarks>
    Private Shared Sub ApplyCaptionFormatting(ByVal dt As System.Data.DataTable, ByVal wks As Worksheet)

        Dim rg As Range = GetRange(wks, 2, 1, 2, dt.Columns.Count)

        rg.Cells.Interior.Color = RGBFromColor(CAPTION_INTERIOR_COLOR)
        rg.Cells.Borders.Color = RGBFromColor(CAPTION_BORDER_COLOR)
        rg.Cells.HorizontalAlignment = XlHAlign.xlHAlignCenter

        Marshal.ReleaseComObject(rg)

    End Sub

    ''' <summary>
    ''' Applies number formatting to the value cells
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <param name="wks"></param>
    ''' <remarks></remarks>
    Private Shared Sub ApplyNumberFormatting(ByVal dt As System.Data.DataTable, ByVal wks As Worksheet)

        Dim rg As Range = GetRange(wks, 3, 3, 3 + dt.Rows.Count, 3 + dt.Columns.Count)
        rg.NumberFormat = "0"

        Marshal.ReleaseComObject(rg)

    End Sub

    ''' <summary>
    ''' Applies conditional color formatting to the value cells
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <param name="wks"></param>
    ''' <remarks></remarks>
    Private Shared Sub ApplyConditionalFormatting(ByVal dt As System.Data.DataTable, ByVal wks As Worksheet)

        Dim rg As Range = GetRange(wks, 3, 3, 3 + dt.Rows.Count, 3 + dt.Columns.Count)

        'Green
        Dim fc1 As FormatCondition = CType(rg.FormatConditions.Add(XlFormatConditionType.xlCellValue, XlFormatConditionOperator.xlBetween, ".00001", "33.33333"), FormatCondition)
        fc1.Interior.Color = RGBFromColor(Color.LimeGreen)
        Marshal.ReleaseComObject(fc1)

        'Yellow
        Dim fc2 As FormatCondition = CType(rg.FormatConditions.Add(XlFormatConditionType.xlCellValue, XlFormatConditionOperator.xlBetween, "33.33333", "66.66666"), FormatCondition)
        fc2.Interior.Color = RGBFromColor(Color.Yellow)
        Marshal.ReleaseComObject(fc2)

        'Red
        Dim fc3 As FormatCondition = CType(rg.FormatConditions.Add(XlFormatConditionType.xlCellValue, XlFormatConditionOperator.xlBetween, "66.66666", "100"), FormatCondition)
        fc3.Interior.Color = RGBFromColor(Color.Red)
        Marshal.ReleaseComObject(fc3)

        Marshal.ReleaseComObject(rg)

    End Sub

    ''' <summary>
    ''' Formats the borders of each non caption or header row cell
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <param name="wks"></param>
    ''' <remarks></remarks>
    Private Shared Sub ApplyBorderFormatting(ByVal dt As System.Data.DataTable, ByVal wks As Worksheet)

        Dim rg As Range = GetRange(wks, 3, 1, 3 + dt.Rows.Count, dt.Columns.Count)
        rg.Borders.Color = RGBFromColor(VALUE_BORDER_COLOR)

        Marshal.ReleaseComObject(rg)

    End Sub

    ''' <summary>
    ''' Applies merge and center formatting to the primary header row
    ''' </summary>
    ''' <param name="wks"></param>
    ''' <param name="attributeSpecified"></param>
    ''' <param name="maxIteration"></param>
    ''' <remarks>
    ''' Strangely, this must be called after the worksheet has had auto-fitting applied or the columns
    ''' do not get sized correctly.  We also apply coloring for the header row cells in this function.
    ''' </remarks>
    Private Shared Sub MergeAndCenterHeaderRow(
        ByVal wks As Worksheet,
        ByVal attributeSpecified As Boolean,
        ByVal maxIteration As Integer)

        Dim Index1 As Integer = 4
        Dim rg1 As Range = GetRange(wks, 1, Index1, 1, Index1 + maxIteration)

        rg1.HorizontalAlignment = XlHAlign.xlHAlignCenter
        rg1.Merge()

        rg1.Cells.Interior.Color = RGBFromColor(CAPTION_INTERIOR_COLOR)
        rg1.Cells.Borders.Color = RGBFromColor(CAPTION_BORDER_COLOR)

        Marshal.ReleaseComObject(rg1)

        If (attributeSpecified) Then

            Dim Index2 As Integer = Index1 + maxIteration + 1
            Dim rg2 As Range = GetRange(wks, 1, Index2, 1, Index2 + maxIteration)
            rg2.HorizontalAlignment = XlHAlign.xlHAlignCenter
            rg2.Merge()

            rg2.Cells.Interior.Color = RGBFromColor(CAPTION_INTERIOR_COLOR)
            rg2.Cells.Borders.Color = RGBFromColor(CAPTION_BORDER_COLOR)

            Marshal.ReleaseComObject(rg2)

        End If

    End Sub

    ''' <summary>
    ''' Auto-fits the worksheet
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <param name="wks"></param>
    ''' <remarks></remarks>
    Private Shared Sub AutoFitWorksheet(ByVal dt As System.Data.DataTable, ByVal wks As Worksheet)

        Dim rg As Range = GetRange(wks, 1, 1, 1, dt.Columns.Count)
        Dim rg2 As Range = rg.EntireColumn

        rg2.AutoFit()

        Marshal.ReleaseComObject(rg)
        Marshal.ReleaseComObject(rg2)

    End Sub

    ''' <summary>
    ''' Auto-filters the worksheet
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <param name="wks"></param>
    ''' <remarks></remarks>
    Private Shared Sub AutoFilterWorksheet(ByVal dt As System.Data.DataTable, ByVal wks As Worksheet)

        Dim rg As Range = GetRange(wks, 2, 1, 2, dt.Columns.Count)
        rg.AutoFilter(1, Type.Missing, XlAutoFilterOperator.xlAnd, Type.Missing, True)
        Marshal.ReleaseComObject(rg)

    End Sub

    ''' <summary>
    ''' Determines if a worksheet with the specified name exists
    ''' </summary>
    ''' <param name="worksheetName"></param>
    ''' <param name="workbook"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Shared Function WorksheetExists(ByVal worksheetName As String, ByVal workbook As Workbook) As Boolean

        For Each Worksheet As Worksheet In workbook.Worksheets

            Dim n As String = Worksheet.Name
            Marshal.ReleaseComObject(Worksheet)

            If (n = worksheetName) Then
                Return True
            End If

        Next

        Return False

    End Function

    ''' <summary>
    ''' Gets the worksheet with the specified name
    ''' </summary>
    ''' <param name="worksheetName"></param>
    ''' <param name="workbook"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Shared Function GetWorksheet(ByVal worksheetName As String, ByVal workbook As Workbook) As Worksheet

        If (WorksheetExists(worksheetName, workbook)) Then
            Return CType(workbook.Worksheets(worksheetName), Worksheet)
        Else
            Return Nothing
        End If

    End Function

    ''' <summary>
    ''' Deletes the worksheet with the specified name
    ''' </summary>
    ''' <param name="worksheetName"></param>
    ''' <param name="workbook"></param>
    ''' <remarks></remarks>
    Friend Shared Sub DeleteWorksheet(ByVal worksheetName As String, ByVal workbook As Workbook)

        Dim Worksheet As Worksheet = GetWorksheet(worksheetName, workbook)

        If (Not Worksheet Is Nothing) Then

            Worksheet.Delete()
            Marshal.ReleaseComObject(Worksheet)

        End If

    End Sub

    ''' <summary>
    ''' Gets an Excel range for the specified worksheet
    ''' </summary>
    ''' <param name="worksheet"></param>
    ''' <param name="startingRow"></param>
    ''' <param name="startingCol"></param>
    ''' <param name="endingRow"></param>
    ''' <param name="endingCol"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Shared Function GetRange(
        ByVal worksheet As Worksheet,
        ByVal startingRow As Integer,
        ByVal startingCol As Integer,
        ByVal endingRow As Integer,
        ByVal endingCol As Integer) As Range

        Return worksheet.Range(
            worksheet.Cells(startingRow, startingCol),
            worksheet.Cells(endingRow, endingCol))

    End Function

    ''' <summary>
    ''' Converts a .Net color to an RGB value
    ''' </summary>
    ''' <param name="clr"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function RGBFromColor(ByVal clr As Color) As Integer
        Return RGB(clr.R, clr.G, clr.B)
    End Function

End Class
