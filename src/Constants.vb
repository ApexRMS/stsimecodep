'*************************************************************************************************************************************************
' stsim-ecodep: SyncroSim Add-On Package (to stsim) for calculating ecological departure in ST-Sim using the LANDFIRE Fire Regime Condition Class.
'
' Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*************************************************************************************************************************************************

Module Constants

    'Data Sheets
    Public Const DATASHEET_OPTIONS_NAME As String = "stsim_ecodep__Options"
    Public Const DATASHEET_REFERENCE_CONDITION_NAME As String = "stsim_ecodep__ReferenceCondition"
    Public Const DATASHEET_OUTPUT_NAME = "stsim_ecodep__Output"
    Public Const DATASHEET_STSIM_STRATUM = "stsim__Stratum"
    Public Const DATASHEET_STSIM_TERMINOLOGY = "stsim__Terminology"
    Public Const DATASHEET_STSIM_TRANSITION_ATTRIBUTE_TYPE = "stsim__TransitionAttributeType"
    Public Const DATASHEET_STSIM_RUN_CONTROL = "stsim__RunControl"
    Public Const DATASHEET_STSIM_OUTPUT_OPTIONS = "stsim__OutputOptions"

    'Column names
    Public Const SCENARIO_ID_COLUMN_NAME As String = "ScenarioID"
    Public Const STRATUM_ID_COLUMN_NAME As String = "StratumID"
    Public Const STATECLASS_ID_COLUMN_NAME As String = "StateClassID"
    Public Const TRANSITION_ATTRIBUTE_TYPE_ID_COLUMN_NAME As String = "TransitionAttributeTypeID"
    Public Const TIMESTEPS_COLUMN_NAME As String = "Timesteps"
    Public Const TIMESTEP_COLUMN_NAME As String = "Timestep"
    Public Const ITERATION_COLUMN_NAME As String = "Iteration"
    Public Const RELATIVE_AMOUNT_COLUMN_NAME As String = "RelativeAmount"
    Public Const UNDESIRABILITY_COLUMN_NAME As String = "Undesirability"
    Public Const THRESHOLD_COLUMN_NAME As String = "Threshold"
    Public Const DEPARTURE_COLUMN_NAME As String = "Departure"
    Public Const CUMULATIVE_ATTRIBUTE_COLUMN_NAME = "CumulativeAttribute"

    'Excel
    Public Const EXCEL_MAX_ROWS As Integer = 1048576 - 1
    Public CAPTION_INTERIOR_COLOR As System.Drawing.Color = Drawing.Color.LightGray
    Public CAPTION_BORDER_COLOR As System.Drawing.Color = Drawing.Color.Gray
    Public VALUE_BORDER_COLOR As System.Drawing.Color = Drawing.Color.LightGray

End Module
