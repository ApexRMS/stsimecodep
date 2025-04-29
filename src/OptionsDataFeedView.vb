﻿'*************************************************************************************************************************************************
' stsimecodep: SyncroSim Package for calculating ecological departure in ST-Sim using the LANDFIRE Fire Regime Condition Class.
'
' Copyright © 2007-2024 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*************************************************************************************************************************************************

Imports SyncroSim.Core
Imports SyncroSim.Core.Forms
Imports System.Reflection
Imports System.Globalization

<ObfuscationAttribute(Exclude:=True, ApplyToMembers:=False)>
Class OptionsDataFeedView

    Private m_IsLoading As Boolean

    Public Overrides Sub LoadDataFeed(ByVal dataFeed As DataFeed)

        MyBase.LoadDataFeed(dataFeed)

        Me.SetTextBoxBinding(Me.TextBoxTimesteps, TIMESTEPS_COLUMN_NAME)

        Dim RefPercentView As DataFeedView = Me.Session.CreateMultiRowDataFeedView(Me.Scenario, Me.ControllingScenario)

        RefPercentView.LoadDataFeed(dataFeed, DATASHEET_REFERENCE_CONDITION_NAME)
        Me.PanelReferencePercent.Controls.Add(RefPercentView)

        Dim dr As DataRow = Me.DataFeed.GetDataSheet(DATASHEET_OPTIONS_NAME).GetDataRow

        If (dr IsNot Nothing AndAlso dr(TRANSITION_ATTRIBUTE_TYPE_ID_COLUMN_NAME) IsNot DBNull.Value) Then
            Me.AssignComboValidationTable()
        End If

        Me.SetCheckboxState()
        Me.MonitorDataSheet(Constants.DATASHEET_STSIM_TERMINOLOGY, AddressOf Me.OnTerminologyChanged, True)

    End Sub

    Private Sub OnTerminologyChanged(ByVal e As DataSheetMonitorEventArgs)

        Dim t As String = CStr(e.GetValue("TimestepUnits", "Timestep")).ToLower(CultureInfo.InvariantCulture)
        Me.LabelTimesteps.Text = t.ToLower(CultureInfo.InvariantCulture)

    End Sub

    Public Overrides Sub EnableView(enable As Boolean)

        If (Me.PanelReferencePercent.Controls.Count > 0) Then

            Dim v As DataFeedView = CType(Me.PanelReferencePercent.Controls(0), DataFeedView)
            v.EnableView(enable)

        End If

        Me.GroupBoxOptions.Enabled = enable
        Me.LabelReportEvery.Enabled = enable
        Me.LabelTimesteps.Enabled = enable
        Me.LabelRefPercent.Enabled = enable
        Me.CheckBoxTransitionAttribute.Enabled = enable
        Me.ComboBoxTransitionAttribute.Enabled = enable

        Me.SetCheckboxState()

        If (enable) Then
            Me.ComboBoxTransitionAttribute.Enabled = Me.CheckBoxTransitionAttribute.Checked
        End If

    End Sub

    Private Sub SetCheckboxState()

        Me.m_IsLoading = True

        Dim dr As DataRow = Me.Scenario.GetDataSheet(DATASHEET_OPTIONS_NAME).GetDataRow

        If (dr IsNot Nothing AndAlso dr(TRANSITION_ATTRIBUTE_TYPE_ID_COLUMN_NAME) IsNot DBNull.Value) Then
            Me.CheckBoxTransitionAttribute.Checked = True
        Else
            Me.CheckBoxTransitionAttribute.Checked = False
        End If

        Me.m_IsLoading = False

    End Sub

    Private Sub AssignComboValidationTable()

        Dim ds As DataSheet = Me.Project.GetDataSheet(Constants.DATASHEET_STSIM_TRANSITION_ATTRIBUTE_TYPE)
        Dim dv As New DataView(ds.GetData(), Nothing, ds.ValidationTable.DisplayMember, DataViewRowState.CurrentRows)

        If (dv.Count > 0) Then

            Me.ComboBoxTransitionAttribute.DataSource = dv
            Me.ComboBoxTransitionAttribute.DisplayMember = ds.ValidationTable.DisplayMember
            Me.ComboBoxTransitionAttribute.ValueMember = ds.ValidationTable.ValueMember

        End If

    End Sub

    Private Sub SetComboRowValue()

        If (Me.ComboBoxTransitionAttribute.Items.Count > 0) Then

            Dim val As Integer = CInt(Me.ComboBoxTransitionAttribute.SelectedValue)
            Dim ds As DataSheet = Me.DataFeed.GetDataSheet(DATASHEET_OPTIONS_NAME)

            ds.SetSingleRowData(TRANSITION_ATTRIBUTE_TYPE_ID_COLUMN_NAME, val)

        End If

    End Sub

    Private Sub ClearComboRowValue()

        Dim ds As DataSheet = Me.DataFeed.GetDataSheet(DATASHEET_OPTIONS_NAME)
        ds.SetSingleRowData(TRANSITION_ATTRIBUTE_TYPE_ID_COLUMN_NAME, Nothing)

    End Sub

    Private Sub CheckBoxTransitionAttribute_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBoxTransitionAttribute.CheckedChanged

        If (Me.m_IsLoading) Then
            Return
        End If

        Me.ComboBoxTransitionAttribute.Enabled = Me.CheckBoxTransitionAttribute.Checked

        If (Me.ComboBoxTransitionAttribute.Enabled) Then

            Me.AssignComboValidationTable()
            Me.SetComboRowValue()

        Else

            Me.ComboBoxTransitionAttribute.DataSource = Nothing
            Me.ClearComboRowValue()

        End If

    End Sub

End Class
