'*************************************************************************************************************************************************
' stsim-ecodep: SyncroSim Add-On Package (to stsim) for calculating ecological departure in ST-Sim using the LANDFIRE Fire Regime Condition Class.
'
' Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*************************************************************************************************************************************************

Imports SyncroSim.Core
Imports System.Globalization

Partial Class ReportTransformer

    ''' <summary>
    ''' Adds the departure value columns
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <param name="maxIteration"></param>
    ''' <remarks></remarks>
    Private Shared Sub AddDepartureColumns(ByVal dt As DataTable, ByVal maxIteration As Integer)

        For i As Integer = 1 To maxIteration

            Dim s As String = i.ToString(CultureInfo.InvariantCulture)
            AddValueColumn(dt, "DIteration" & s, "Iteration " & s)

        Next

        AddValueColumn(dt, "DMean", "Mean")

    End Sub

    ''' <summary>
    ''' Adds the attribute value columns
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <param name="maxIteration"></param>
    ''' <remarks></remarks>
    Private Shared Sub AddAttributeColumns(ByVal dt As DataTable, ByVal maxIteration As Integer)

        For i As Integer = 1 To maxIteration

            Dim s As String = i.ToString(CultureInfo.InvariantCulture)
            AddValueColumn(dt, "CIteration" & s, "Iteration " & s)

        Next

        AddValueColumn(dt, "CMean", "Mean")

    End Sub

    ''' <summary>
    ''' Adds the primary columns
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <remarks></remarks>
    Private Sub AddPrimaryColumns(ByVal dt As DataTable)

        dt.Columns.Add(New DataColumn("Scenario", GetType(Object)))
        dt.Columns.Add(New DataColumn("Timestep", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Stratum", GetType(Object)))

        dt.Columns("Scenario").Caption = "Scenario"
        dt.Columns("Timestep").Caption = Me.GetTimestepUnits()
        dt.Columns("Stratum").Caption = "Stratum"

    End Sub

    ''' <summary>
    ''' Gets the timestep units from ST_Sim's Terminology Data Sheet
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetTimestepUnits() As String

        Dim dr As DataRow = Me.Project.GetDataSheet("STSim_Terminology").GetDataRow()

        If (dr Is Nothing OrElse dr("TimestepUnits") Is DBNull.Value) Then
            Return "Timestep"
        Else
            Return CStr(dr("TimestepUnits"))
        End If

    End Function

    ''' <summary>
    ''' Determines if any of the result scenarios have an attribute specified
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function AnyAttributeSpecified() As Boolean

        For Each s As Scenario In Me.Project.Results

            If (GetSelectedAttributeId(s).HasValue) Then
                Return True
            End If

        Next

        Return False

    End Function

    ''' <summary>
    ''' Adds a value column to the data table
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <param name="columnName"></param>
    ''' <param name="caption"></param>
    ''' <remarks></remarks>
    Private Shared Sub AddValueColumn(ByVal dt As DataTable, ByVal columnName As String, ByVal caption As String)

        Dim c As New DataColumn(columnName, GetType(Object))

        c.Caption = caption
        c.DefaultValue = Nothing

        dt.Columns.Add(c)

    End Sub

    ''' <summary>
    ''' Creates a dictionary for mapping Ids to names for the specified definition
    ''' </summary>
    ''' <param name="definitionName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreateDefinitionDictionary(ByVal definitionName As String) As Dictionary(Of Integer, String)

        Dim dict As New Dictionary(Of Integer, String)
        Dim ds As DataSheet = Me.Project.GetDataSheet(definitionName)
        Dim dt As DataTable = ds.GetData()

        For Each dr As DataRow In dt.Rows

            If (dr.RowState <> DataRowState.Deleted) Then
                dict.Add(CInt(dr(ds.ValueMember)), CStr(dr(ds.DisplayMember)))
            End If

        Next

        Return dict

    End Function

    ''' <summary>
    ''' Creates a dictionary for mapping scenario Ids to names
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreateScenarioDictionary() As Dictionary(Of Integer, String)

        Dim dict As New Dictionary(Of Integer, String)

        For Each s As Scenario In Me.Project.Results
            dict.Add(s.Id, s.DisplayName)
        Next

        Return dict

    End Function

    ''' <summary>
    ''' Gets the output data for the current result scenarios
    ''' </summary>
    ''' <param name="store"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetOutputDataTable(ByVal store As DataStore) As DataTable

        Dim q As String = String.Format(CultureInfo.InvariantCulture,
            "SELECT * FROM ED_Output WHERE ScenarioID IN({0})",
            Me.CreateActiveResultScenarioFilter())

        Return store.CreateDataTableFromQuery(q, "ED_Output")

    End Function

    ''' <summary>
    ''' Validates that the specified data table can be loaded into excel
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ValidateForExcel(ByVal dt As DataTable) As Boolean

        If (dt.Rows.Count > EXCEL_MAX_ROWS) Then

            MsgBox("There are too many rows to load into Excel.  Please reduce the timestep frequency or the number of result scenarios.",
                   MsgBoxStyle.OkOnly,
                   "Ecological Departure")

            Return False

        Else
            Return True
        End If

    End Function

End Class
