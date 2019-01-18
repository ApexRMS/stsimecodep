'*********************************************************************************************
' Ecological Departure: A SyncroSim Package for doing ecological departure analysis.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Module Constants

    'Data Sheets
    Public Const DATASHEET_OPTIONS_NAME As String = "ED_Options"
    Public Const DATASHEET_REFERENCE_CONDITION_NAME As String = "ED_ReferenceCondition"
    Public Const DATASHEET_OUTPUT_NAME = "ED_Output"

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
