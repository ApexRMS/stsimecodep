'*********************************************************************************************
' Ecological Departure: A SyncroSim Package for doing ecological departure analysis.
'
' Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports SyncroSim.Core
Imports System.Reflection
Imports System.Globalization

<ObfuscationAttribute(Exclude:=True, ApplyToMembers:=False)>
Class PostProcessTransformer
    Inherits TransformerBase

    Public Overrides Sub Transform()

        If (Me.EcologicalDepartureEnabled()) Then
            Me.CalculateEcologicalDeparture()
        End If

    End Sub

    ''' <summary>
    ''' Calculates the ecological departure and writes it to the output data feed
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CalculateEcologicalDeparture()

        Dim dt As DataTable = CreateComputationTable()

        Me.FillPrimaryStateClassData(dt)
        FillStratumAmounts(dt)
        Me.FillReferenceConditions(dt)
        FillPercentAndMinimum(dt)
        Me.WriteDepartureData(dt)

    End Sub

    ''' <summary>
    ''' Creates the computation table
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function CreateComputationTable() As DataTable

        Dim dt As New DataTable("Computation")
        dt.Locale = CultureInfo.InvariantCulture

        dt.Columns.Add(New DataColumn("Iteration", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Timestep", GetType(Integer)))
        dt.Columns.Add(New DataColumn("StratumID", GetType(Integer)))
        dt.Columns.Add(New DataColumn("StateClassID", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Amount", GetType(Double)))
        dt.Columns.Add(New DataColumn("StratumAmount", GetType(Double)))
        dt.Columns.Add(New DataColumn("Percent", GetType(Double)))
        dt.Columns.Add(New DataColumn("ReferencePercent", GetType(Double)))
        dt.Columns.Add(New DataColumn("Minimum", GetType(Double)))
        dt.Columns.Add(New DataColumn("Undesirability", GetType(Double)))
        dt.Columns.Add(New DataColumn("Threshold", GetType(Double)))

        Return dt

    End Function

    ''' <summary>
    ''' Fills the state class data computation table with the primary data from the ST-Sim output table
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <remarks></remarks>
    Private Sub FillPrimaryStateClassData(ByVal dt As DataTable)

        Dim ReportingFrequency As Integer = Me.GetReportingFrequency
        Dim STMinTimestep As Integer = Me.GetSTMinimumTimestep()
        Dim STMaxTimestep As Integer = Me.GetSTMaxTimestep
        Dim OSSData As DataTable = Me.GetOutputStratumStateDataTable()
        Dim dict As New Dictionary(Of String, PrimaryStateClassData)

        For Each dr As DataRow In OSSData.Rows

            Dim OSSTimestep As Integer = CInt(dr("Timestep"))

            If (((OSSTimestep - STMinTimestep) Mod ReportingFrequency) <> 0) Then

                If (OSSTimestep <> STMinTimestep And OSSTimestep <> STMaxTimestep) Then
                    Continue For
                End If

            End If

            Dim iteration As Integer = CInt(dr("Iteration"))
            Dim stratumId As Integer = CInt(dr("StratumID"))
            Dim stateClassId As Integer = CInt(dr("StateClassID"))
            Dim amount As Double = CDbl(dr("Amount"))

            Dim key As String = CreateKey(iteration, OSSTimestep, stratumId, stateClassId)

            If dict.ContainsKey(key) Then
                dict(key).Amount += amount
            Else

                Dim psd As New PrimaryStateClassData()

                psd.iteration = iteration
                psd.timesetep = OSSTimestep
                psd.StratumId = stratumId
                psd.StateClassId = stateClassId
                psd.Amount = amount

                dict.Add(key, psd)

            End If

        Next

        For Each psd As PrimaryStateClassData In dict.Values

            Dim NewRow As DataRow = dt.NewRow

            NewRow("Iteration") = psd.iteration
            NewRow("Timestep") = psd.timesetep
            NewRow("StratumID") = psd.StratumId
            NewRow("StateClassID") = psd.StateClassId
            NewRow("Amount") = psd.Amount

            dt.Rows.Add(NewRow)

        Next

    End Sub

    ''' <summary>
    ''' Fills the stratum amounts for each iteration, timestep, and stratum
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <remarks></remarks>
    Private Shared Sub FillStratumAmounts(ByVal dt As DataTable)

        Dim dict As New Dictionary(Of String, Double)

        For Each dr As DataRow In dt.Rows

            Dim it As Integer = CInt(dr("Iteration"))
            Dim ts As Integer = CInt(dr("Timestep"))
            Dim st As Integer = CInt(dr("StratumID"))
            Dim am As Double = CDbl(dr("Amount"))
            Dim k As String = CreateKey(it, ts, st)

            If (dict.ContainsKey(k)) Then
                dict(k) += am
            Else
                dict.Add(k, am)
            End If

        Next

        For Each dr As DataRow In dt.Rows

            Dim it As Integer = CInt(dr("Iteration"))
            Dim ts As Integer = CInt(dr("Timestep"))
            Dim st As Integer = CInt(dr("StratumID"))
            Dim k As String = CreateKey(it, ts, st)

            dr("StratumAmount") = dict(k)

        Next

    End Sub

    ''' <summary>
    ''' Fills the reference conditions for each stratum and state class
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <remarks></remarks>
    Private Sub FillReferenceConditions(ByVal dt As DataTable)

        Dim ds As DataSheet = Me.ResultScenario.GetDataSheet(DATASHEET_REFERENCE_CONDITION_NAME)
        Dim dict As New Dictionary(Of String, ReferenceCondition)
        Dim dtrp As DataTable = ds.GetData()

        For Each dr As DataRow In dtrp.Rows

            Dim st As Integer = CInt(dr(STRATUM_ID_COLUMN_NAME))
            Dim sc As Integer = CInt(dr(STATECLASS_ID_COLUMN_NAME))
            Dim amt As Integer = CInt(dr(RELATIVE_AMOUNT_COLUMN_NAME))
            Dim und As Nullable(Of Double) = Nothing
            Dim thr As Nullable(Of Double) = Nothing
            Dim k As String = CreateKey(st, sc)

            If (dr(UNDESIRABILITY_COLUMN_NAME) IsNot DBNull.Value) Then
                und = CType(dr(UNDESIRABILITY_COLUMN_NAME), Double?)
            End If

            If (dr(THRESHOLD_COLUMN_NAME) IsNot DBNull.Value) Then
                thr = CType(dr(THRESHOLD_COLUMN_NAME), Double?)
            End If

            If (Not dict.ContainsKey(k)) Then
                dict.Add(k, New ReferenceCondition(amt, und, thr))
            End If

        Next

        For Each dr As DataRow In dt.Rows

            Dim st As Integer = CInt(dr("StratumID"))
            Dim sc As Integer = CInt(dr("StateClassID"))
            Dim k As String = CreateKey(st, sc)

            If (dict.ContainsKey(k)) Then

                Dim rc As ReferenceCondition = dict(k)

                dr("ReferencePercent") = rc.RelativeAmount
                dr("Undesirability") = DataTableUtilities.GetNullableDatabaseValue(rc.Undesirability)
                dr("Threshold") = DataTableUtilities.GetNullableDatabaseValue(rc.Threshold)

            Else
                dr("ReferencePercent") = 0.0
            End If

        Next

    End Sub

    ''' <summary>
    ''' Fills the percent and minimim values in the state class computation table
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <remarks>
    ''' If the stratum amount is 0.0 we can't do the computation and so we will put
    ''' DBNull.Value in the table.  Later, when calcuating the departure, we will check
    ''' for DBNull.Value and not write a record if we find it.
    ''' </remarks>
    Private Shared Sub FillPercentAndMinimum(ByVal dt As DataTable)

        For Each dr As DataRow In dt.Rows

            Dim sa As Double = CDbl(dr("StratumAmount"))

            If (sa = 0.0) Then

                dr("Percent") = DBNull.Value
                dr("Minimum") = DBNull.Value

            Else

                Dim am As Double = CDbl(dr("Amount"))
                Dim per As Double = (am / sa) * 100.0
                Dim rp As Double = CDbl(dr("ReferencePercent"))

                dr("Percent") = per
                dr("Minimum") = Math.Min(per, rp)

            End If

        Next

    End Sub

    ''' <summary>
    ''' Writes the departure data to the output data feed
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <remarks></remarks>
    Private Sub WriteDepartureData(ByVal dt As DataTable)

        Dim dictdep As New Dictionary(Of String, UnifiedDepartureValues)
        Dim dictattr As Dictionary(Of String, Double) = Nothing
        Dim edout As DataTable = Me.ResultScenario.GetDataSheet(DATASHEET_OUTPUT_NAME).GetData()
        Dim attrid As Nullable(Of Integer) = GetSelectedAttributeId(Me.ResultScenario)

        If (attrid.HasValue) Then

            Dim dtattrout As DataTable = Me.GetAttributeDataTable(attrid.Value)
            dictattr = CreateCADictionary(dtattrout)

#If DEBUG Then
            ValidateCADictionary(dictattr, Me.Project)
#End If

        End If

        For Each dr As DataRow In dt.Rows

            If (dr("Percent") Is DBNull.Value) Then

                Debug.Assert(dr("Minimum") Is DBNull.Value)
                Continue For

            End If

            Dim it As Integer = CInt(dr("Iteration"))
            Dim ts As Integer = CInt(dr("Timestep"))
            Dim st As Integer = CInt(dr("StratumID"))
            Dim min As Double = CDbl(dr("Minimum"))
            Dim percent As Double = CDbl(dr("Percent"))
            Dim adjustment As Double = 0.0

            If dr("Undesirability") IsNot DBNull.Value Then

                Dim undesirability As Double = CDbl(dr("Undesirability"))
                Dim HRF As Double = Math.Exp(10 * (undesirability - 1)) / (1 + Math.Exp(10 * (undesirability - 1)))

                adjustment += HRF * percent

            End If

            If dr("Threshold") IsNot DBNull.Value Then

                Dim threshold As Double = CDbl(dr("Threshold"))
                adjustment -= Math.Min(percent, threshold)

            End If

            Dim k As String = CreateKey(it, ts, st)

#If DEBUG Then
            ValidateStratum(st, Me.Project)
#End If

            If (dictdep.ContainsKey(k)) Then

                dictdep(k).Min += min
                dictdep(k).Adjustment += adjustment

            Else

                Dim udv As New UnifiedDepartureValues

                udv.Adjustment = adjustment
                udv.Min = min

                dictdep.Add(k, udv)

            End If

        Next

        For Each key As String In dictdep.Keys

            Dim val As Double = dictdep(key).Min
            Dim adj As Double = dictdep(key).Adjustment
            Dim dep As Double = 100.0 - val

            dep += adj

            If dep < 0.0 Then
                dep = 0.0
            ElseIf dep > 100.0 Then
                dep = 100.0
            End If

            Dim s() As String = key.Split(CChar("-"))
            Dim it As Integer = CInt(s(0))
            Dim ts As Integer = CInt(s(1))
            Dim st As Integer = CInt(s(2))
            Dim ca As Object = DBNull.Value

#If DEBUG Then
            ValidateStratum(st, Me.Project)
#End If

            If (attrid.HasValue) Then

                Dim k As String = CreateKey(it, ts, st)

                If (dictattr.ContainsKey(k)) Then
                    ca = dictattr(k)
                End If

            End If

            Debug.Assert(Not Double.IsNaN(dep))
            edout.Rows.Add({it, ts, st, dep, ca})

        Next

    End Sub

    ''' <summary>
    ''' Creates a dictionary of cumulative attribute amounts for the specified attribute output table
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Shared Function CreateCADictionary(ByVal dt As DataTable) As Dictionary(Of String, Double)

        Dim dict As New Dictionary(Of String, Double)

        If (dt.Rows.Count = 0) Then
            Return dict
        End If

        dt.Columns.Add(New DataColumn("CumulativeAmount", GetType(Double)))
        Dim dv As New DataView(dt, Nothing, "Iteration,StratumID,Timestep", DataViewRowState.CurrentRows)

        Dim ThisIt As Integer = -1
        Dim ThisSt As Integer = -1
        Dim Total As Double = 0.0

        For Each drv As DataRowView In dv

            Dim dr As DataRow = drv.Row

            Dim it As Integer = CInt(dr("Iteration"))
            Dim st As Integer = CInt(dr("StratumID"))
            Dim am As Double = CDbl(dr("Amount"))

            If (it <> ThisIt Or st <> ThisSt) Then

                Total = am
                ThisIt = it
                ThisSt = st

            Else
                Total += am
            End If

            dr("CumulativeAmount") = Total

        Next

        For Each dr As DataRow In dt.Rows

            Dim it As Integer = CInt(dr("Iteration"))
            Dim ts As Integer = CInt(dr("Timestep"))
            Dim st As Integer = CInt(dr("StratumID"))
            Dim am As Double = CDbl(dr("CumulativeAmount"))
            Dim k As String = CreateKey(it, ts, st)

            If (Not dict.ContainsKey(k)) Then
                dict.Add(k, am)
            Else
                dict.Item(k) += am
            End If

        Next

        Return dict

    End Function

    ''' <summary>
    ''' Gets the reporting frequency from the options data feed
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetReportingFrequency() As Integer

        Dim ds As DataSheet = Me.ResultScenario.GetDataSheet(DATASHEET_OPTIONS_NAME)
        Dim dr As DataRow = ds.GetDataRow

        Return CInt(dr(TIMESTEPS_COLUMN_NAME))

    End Function

    ''' <summary>
    ''' Gets the maximum timestep value for STSim run control
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetSTMaxTimestep() As Integer

        Dim ds As DataSheet = Me.ResultScenario.GetDataSheet("STSim_RunControl")
        Dim dr As DataRow = ds.GetDataRow()

        Return CInt(dr("MaximumTimestep"))

    End Function

    ''' <summary>
    ''' Gets the minimum timestep value for STSim run control
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetSTMinimumTimestep() As Integer

        Dim ds As DataSheet = Me.ResultScenario.GetDataSheet("STSim_RunControl")
        Dim dr As DataRow = ds.GetDataRow()

        Return CInt(dr("MinimumTimestep"))

    End Function

    ''' <summary>
    ''' Gets the output stratum state data for the current scenario
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetOutputStratumStateDataTable() As DataTable

        Using store As DataStore = Me.Project.Library.CreateDataStore()

            Dim q As String = String.Format(CultureInfo.InvariantCulture,
                "SELECT * FROM STSim_OutputStratumState WHERE ScenarioID={0}", Me.ResultScenario.Id)

            Return store.CreateDataTableFromQuery(q, "OutputStratumState")

        End Using

    End Function

    ''' <summary>
    ''' Gets a data table from STSim's output filtered by the specified attribute type id
    ''' </summary>
    ''' <param name="attrId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetAttributeDataTable(ByVal attrId As Integer) As DataTable

        Using store As DataStore = Me.Project.Library.CreateDataStore()

            Dim q As String = String.Format(CultureInfo.InvariantCulture,
                "SELECT * FROM STSim_OutputTransitionAttribute WHERE ScenarioID={0} AND TransitionAttributeTypeID={1}",
                Me.ResultScenario.Id, attrId)

            Return store.CreateDataTableFromQuery(q, "CumulativeAttributeData")

        End Using

    End Function

End Class

''' <summary>
''' UnifiedDepartureValues
''' </summary>
''' <remarks></remarks>
Class UnifiedDepartureValues
    Public Min As Double
    Public Adjustment As Double
End Class

''' <summary>
''' PrimaryStateClassData
''' </summary>
''' <remarks></remarks>
Class PrimaryStateClassData

    Public iteration As Integer
    Public timesetep As Integer
    Public StratumId As Integer
    Public StateClassId As Integer
    Public Amount As Double

End Class
