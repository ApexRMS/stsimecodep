'*************************************************************************************************************************************************
' stsim-ecodep: SyncroSim Add-On Package (to stsim) for calculating ecological departure in ST-Sim using the LANDFIRE Fire Regime Condition Class.
'
' Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*************************************************************************************************************************************************

Imports SyncroSim.Core
Imports System.Reflection
Imports System.Globalization

<ObfuscationAttribute(Exclude:=True, ApplyToMembers:=False)>
Class PreProcessTransformer
    Inherits TransformerBase

    ''' <summary>
    ''' Overrides Configure
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Configure()

        MyBase.Configure()

        If (Me.EcologicalDepartureEnabled()) Then

            Me.CreateOutputOptionsRowIfNotExist()
            Me.SetTimestepsValueIfMissing()
            Me.NormalizeTransitionAttributes()
            Me.NormalizeStateClassOutput()
            Me.NormalizeRelativeAmounts()

        End If

    End Sub

    ''' <summary>
    ''' Overrides Transform
    ''' </summary>
    ''' <remarks>
    ''' Just suppress the base class code here
    ''' </remarks>
    Public Overrides Sub Transform()
        Return
    End Sub

    ''' <summary>
    ''' Creates a row for STSim's Output Options data feed if it is missing
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CreateOutputOptionsRowIfNotExist()

        Dim dsoo As DataSheet = Me.ResultScenario.GetDataSheet("STSim_OutputOptions")
        Dim droo As DataRow = dsoo.GetDataRow()

        If (droo Is Nothing) Then

            droo = dsoo.GetData().NewRow()
            dsoo.GetData().Rows.Add(droo)

        End If

    End Sub

    ''' <summary>
    ''' Sets the timesteps value to a default if it is missing
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetTimestepsValueIfMissing()

        Dim dr As DataRow = Me.ResultScenario.GetDataSheet(DATASHEET_OPTIONS_NAME).GetDataRow()

        If (dr(TIMESTEPS_COLUMN_NAME) Is DBNull.Value) Then

            dr(TIMESTEPS_COLUMN_NAME) = 1

            Me.RecordStatus(StatusType.Warning,
                "Ecological departure: No timestep frequency specified.  Using default.")

        End If

    End Sub

    ''' <summary>
    ''' Normalizes ST-Sim's transition attributes value if it is missing
    ''' </summary>
    ''' <remarks>
    ''' If an attribute is selected, ensure that ST-Sim's output options has transition
    ''' attribute output selected for every timestep.
    ''' </remarks>
    Private Sub NormalizeTransitionAttributes()

        Dim dro As DataRow = Me.ResultScenario.GetDataSheet(DATASHEET_OPTIONS_NAME).GetDataRow()

        If (dro(TRANSITION_ATTRIBUTE_TYPE_ID_COLUMN_NAME) IsNot DBNull.Value) Then

            Dim droo As DataRow = Me.ResultScenario.GetDataSheet("STSim_OutputOptions").GetDataRow()

            If (droo("SummaryOutputTA") Is DBNull.Value) Then

                droo("SummaryOutputTA") = True
                Me.RecordStatus(StatusType.Information,
                    "Ecological departure: Transition attribute summary output not selected.  Setting to true.")

            End If

            If (droo("SummaryOutputTATimesteps") Is DBNull.Value) Then

                droo("SummaryOutputTATimesteps") = 1
                Me.RecordStatus(StatusType.Information,
                    "Ecological departure: Transition attribute summary timesteps not specified.  Using default.")

            End If

            If (CInt(droo("SummaryOutputTATimesteps")) <> 1) Then

                droo("SummaryOutputTATimesteps") = 1
                Me.RecordStatus(StatusType.Information,
                    "Ecological departure: Transition attribute summary timesteps not specified for all timesteps.  Using value of 1.")

            End If

        End If

    End Sub

    ''' <summary>
    ''' Normalizes STSim's state class output for ecological departure
    ''' </summary>
    ''' <remarks>
    ''' Ensure that ST-Sim will have state class summary output at least at the frequency
    ''' specified for the ecological departure timesteps value as follows:
    ''' 
    ''' (1.) If it is not set, set it and give it the same frequency as for ecological departure
    ''' (2.) If it is set but it is too low, give it the same frequency as for ecological departure
    ''' (3.) If it is set but it is too high:
    ''' 
    '''      (3a.) If it can be divided into the ecological departure frequency that is OK
    '''      (3b.) If it cannot be divided, give it the same frequency as for ecological departure
    ''' 
    ''' </remarks>
    Private Sub NormalizeStateClassOutput()

        Dim dro As DataRow = Me.ResultScenario.GetDataSheet(DATASHEET_OPTIONS_NAME).GetDataRow()
        Dim droo As DataRow = Me.ResultScenario.GetDataSheet("STSim_OutputOptions").GetDataRow()
        Dim tso As Integer = CInt(dro(TIMESTEPS_COLUMN_NAME))

        If (droo("SummaryOutputSC") Is DBNull.Value) Then

            droo("SummaryOutputSC") = True
            Me.RecordStatus(StatusType.Information,
                "Ecological departure: Summary output not selected.  Setting to true.")

        End If

        If (droo("SummaryOutputSCTimesteps") Is DBNull.Value) Then

            droo("SummaryOutputSCTimesteps") = tso
            Me.RecordStatus(StatusType.Information,
                "Ecological departure: State class summary output timesteps not specified.  Using ecological departure timesteps value.")

        End If

        Dim tsoo As Integer = CInt(droo("SummaryOutputSCTimesteps"))

        If (tsoo > tso) Then

            droo("SummaryOutputSCTimesteps") = tso
            Me.RecordStatus(StatusType.Information,
                "Ecological departure: State class summary output timestep frequency too low.  Using ecological departure timesteps value.")

        End If

        If (tsoo < tso) Then

            If (tsoo <> 0) Then

                If ((tso Mod tsoo) <> 0) Then
                    Me.RecordStatus(StatusType.Warning,
                        "Ecological departure: State class summary timesteps do not divide evenly into ecological departure timesteps.  Ecological departure data will not be complete.")
                End If

            End If

        End If

    End Sub

    ''' <summary>
    ''' Normalize Relative Amounts as percentages
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub NormalizeRelativeAmounts()

        Dim RefConSheet As DataSheet = Me.ResultScenario.GetDataSheet(DATASHEET_REFERENCE_CONDITION_NAME)
        Dim RefConData As DataTable = RefConSheet.GetData()

        If (RefConData.Rows.Count = 0) Then
            Return
        End If

        Dim ZeroTotalMessage As Boolean = False
        Dim StratumDataSheet As DataSheet = Me.Project.GetDataSheet("STSim_Stratum")
        Dim StratumData As DataTable = StratumDataSheet.GetData()

        Debug.Assert(StratumData.Rows.Count > 0)

        For Each StratumRow As DataRow In StratumData.Rows

            Dim StratumId As Integer = CInt(StratumRow(StratumDataSheet.PrimaryKeyColumn.Name))

            Dim RefConRows() As DataRow = RefConData.Select(
                String.Format(CultureInfo.InvariantCulture, "{0}={1}", STRATUM_ID_COLUMN_NAME, StratumId))

            If (RefConRows.Length > 0) Then

                Dim Total As Integer = GetRelativeAmountTotal(RefConRows)

                If (Total > 0) Then

                    For Each RefConRow In RefConRows

                        Dim OldVal As Integer = CInt(RefConRow(RELATIVE_AMOUNT_COLUMN_NAME))
                        Dim NewVal As Integer = CInt(Math.Round(100.0 / Total * OldVal))

                        RefConRow(RELATIVE_AMOUNT_COLUMN_NAME) = NewVal

                    Next

                Else
                    ZeroTotalMessage = True
                End If

            End If

        Next

        If (ZeroTotalMessage) Then

            Me.RecordStatus(StatusType.Warning,
                "The reference relative amounts for at least one stratum totals to zero.  Ecological departure calculations for that stratum will not be valid.")

        End If

        RefConSheet.Changes.Add(New ChangeRecord(Me, "Normalized relative amounts"))

    End Sub

    ''' <summary>
    ''' Gets the total relative amount for the specified row collection
    ''' </summary>
    ''' <param name="rows"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function GetRelativeAmountTotal(ByVal rows() As DataRow) As Integer

        Dim t As Integer = 0

        For Each dr As DataRow In rows
            t += CInt(dr(RELATIVE_AMOUNT_COLUMN_NAME))
        Next

        Return t

    End Function

End Class
