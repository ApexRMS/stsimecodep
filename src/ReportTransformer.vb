'*************************************************************************************************************************************************
' stsim-ecodep: SyncroSim Add-On Package (to stsim) for calculating ecological departure in ST-Sim using the LANDFIRE Fire Regime Condition Class.
'
' Copyright © 2007-2021 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*************************************************************************************************************************************************

Imports System.IO
Imports SyncroSim.Core
Imports SyncroSim.Core.Forms
Imports System.Reflection
Imports System.Globalization

<ObfuscationAttribute(Exclude:=True, ApplyToMembers:=False)>
Class ReportTransformer
    Inherits ExportTransformer

    ''' <summary>
    ''' Overrides Export
    ''' </summary>
    ''' <param name="location"></param>
    ''' <param name="exportType"></param>
    ''' <remarks></remarks>
    Protected Overrides Sub Export(location As String, exportType As ExportType)

        Debug.Assert(exportType = Core.Forms.ExportType.ExcelFile)

        Dim RawData As DataTable = Me.GetRawDataTable()

        If (RawData Is Nothing) Then
            Return
        End If

        If (RawData.Rows.Count = 0) Then
            MsgBox("There is no data for the specified scenarios.", MsgBoxStyle.OkOnly, "Ecological Departure")
            Return
        End If

        Dim AttributeSpecified As Boolean = Me.AnyAttributeSpecified()
        Dim MaxIteration As Integer = CInt(RawData.Compute("MAX(Iteration)", Nothing))
        Dim FinalData As DataTable = Me.CreateDepartureReportTable(RawData, AttributeSpecified, MaxIteration)

        If (Not ValidateForExcel(FinalData)) Then
            Return
        End If

        If (File.Exists(location)) Then
            File.Delete(location)
        End If

        Me.PostProcessFinalTable(FinalData)
        ExportToExcel(FinalData, AttributeSpecified, MaxIteration, location)

    End Sub

    ''' <summary>
    ''' Gets the raw departure data
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetRawDataTable() As DataTable

        Using store As DataStore = Me.Project.Library.CreateDataStore()
            Return Me.GetOutputDataTable(store)
        End Using

    End Function

    ''' <summary>
    ''' Creates the departure report table
    ''' </summary>
    ''' <param name="dtRawData">The raw data from the database</param>
    ''' <param name="attributeSpecified">Whether or not any of the scenarios have an attribute specified</param>
    ''' <param name="maxIteration">The maximum iteration</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreateDepartureReportTable(
        ByVal dtRawData As DataTable,
        ByVal attributeSpecified As Boolean,
        ByVal maxIteration As Integer) As DataTable

        Dim dt As New DataTable("EDData")
        dt.Locale = CultureInfo.InvariantCulture

        Me.AddPrimaryColumns(dt)
        AddDepartureColumns(dt, maxIteration)

        If (attributeSpecified) Then
            AddAttributeColumns(dt, maxIteration)
        End If

        Dim dict As Dictionary(Of String, Dictionary(Of Integer, DepartureData)) =
            CreateSTSDictionary(dtRawData)

#If DEBUG Then
        ValidateSTSDictionary(dict, Me.Project)
#End If

        For Each k As String In dict.Keys

            Dim r As DataRow = dt.NewRow
            Dim s() As String = k.Split(CChar("-"))

            r("Scenario") = s(0)
            r("Timestep") = s(1)
            r("Stratum") = s(2)

#If DEBUG Then
            ValidateScenario(CInt(s(0)), Me.Project)
            ValidateStratum(CInt(s(2)), Me.Project)
#End If

            Dim d As Dictionary(Of Integer, DepartureData) = dict(k)

            FillDepartureRowValues(r, d, maxIteration)

            If (attributeSpecified) Then
                FillAttributeRowValues(r, d, maxIteration)
            End If

            dt.Rows.Add(r)

        Next

        Return dt

    End Function

    ''' <summary>
    ''' Fills the departure values for the specified row
    ''' </summary>
    ''' <param name="r"></param>
    ''' <param name="d"></param>
    ''' <param name="maxIter"></param>
    ''' <remarks></remarks>
    Private Shared Sub FillDepartureRowValues(
        ByVal r As DataRow,
        ByVal d As Dictionary(Of Integer, DepartureData),
        ByVal maxIter As Integer)

        Dim Total As Double = 0.0
        Dim NumValues As Integer = 0

        For i As Integer = 1 To maxIter

            If (d.ContainsKey(i)) Then

                Dim data As DepartureData = d(i)

                r("DIteration" & i.ToString(CultureInfo.InvariantCulture)) = data.m_Departure

                Total += data.m_Departure
                NumValues += 1

            Else
                r("DIteration" & i.ToString(CultureInfo.InvariantCulture)) = Nothing
            End If

        Next

        If (NumValues > 0) Then
            r("DMean") = Total / NumValues
        End If

    End Sub

    ''' <summary>
    ''' Fills the cumulative attribute values for the specified row
    ''' </summary>
    ''' <param name="r"></param>
    ''' <param name="d"></param>
    ''' <param name="maxIter"></param>
    ''' <remarks></remarks>
    Private Shared Sub FillAttributeRowValues(
        ByVal r As DataRow,
        ByVal d As Dictionary(Of Integer, DepartureData),
        ByVal maxIter As Integer)

        Dim Total As Double = 0.0
        Dim NumValues As Integer = 0

        For i As Integer = 1 To maxIter

            If (d.ContainsKey(i)) Then

                Dim data As DepartureData = d(i)

                If (data.m_Cumulative.HasValue) Then

                    r("CIteration" & i.ToString(CultureInfo.InvariantCulture)) = data.m_Cumulative

                    Total += data.m_Cumulative.Value
                    NumValues += 1

                End If

            Else
                r("CIteration" & i.ToString(CultureInfo.InvariantCulture)) = Nothing
            End If

        Next

        If (NumValues > 0) Then
            r("CMean") = Total / NumValues
        End If

    End Sub

    ''' <summary>
    ''' Creates a scenario-timestep-stratum dictionary
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function CreateSTSDictionary(ByVal dt As DataTable) As Dictionary(Of String, Dictionary(Of Integer, DepartureData))

        Dim dict As New Dictionary(Of String, Dictionary(Of Integer, DepartureData))

        For Each dr As DataRow In dt.Rows

            Dim sn As Integer = CInt(dr("ScenarioID"))
            Dim ts As Integer = CInt(dr("Timestep"))
            Dim st As Integer = CInt(dr("StratumID"))
            Dim it As Integer = CInt(dr("Iteration"))
            Dim dep As Double = CDbl(dr("Departure"))
            Dim ca As Nullable(Of Double) = Nothing

            If (dr("CumulativeAttribute") IsNot DBNull.Value) Then
                ca = CDbl(dr("CumulativeAttribute"))
            End If

            Dim k As String = CreateKey(sn, ts, st)
            Dim d As Dictionary(Of Integer, DepartureData)

            If (dict.ContainsKey(k)) Then
                d = dict(k)
            Else
                d = New Dictionary(Of Integer, DepartureData)
                dict.Add(k, d)
            End If

            d.Add(it, New DepartureData(dep, ca))

        Next

        Return dict

    End Function

    ''' <summary>
    ''' Converts scenario and stratum Ids to strings
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <remarks></remarks>
    Private Sub PostProcessFinalTable(ByVal dt As DataTable)

        Dim d1 As Dictionary(Of Integer, String) = Me.CreateScenarioDictionary()
        Dim d2 As Dictionary(Of Integer, String) = Me.CreateDefinitionDictionary(Constants.DATASHEET_STSIM_STRATUM)

        For Each dr As DataRow In dt.Rows

            dr("Scenario") = d1(CInt(dr("Scenario")))
            dr("Stratum") = d2(CInt(dr("Stratum")))

        Next

    End Sub

End Class