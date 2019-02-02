'*************************************************************************************************************************************************
' stsim-ecodep: SyncroSim Add-On Package (to stsim) for calculating ecological departure in ST-Sim using the LANDFIRE Fire Regime Condition Class.
'
' Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*************************************************************************************************************************************************

Imports SyncroSim.Core

Class TransformerBase
    Inherits Transformer

    Protected Function EcologicalDepartureEnabled() As Boolean

        Dim dr As DataRow = Me.ResultScenario.GetDataSheet(DATASHEET_OPTIONS_NAME).GetDataRow()
        Return (dr IsNot Nothing)

    End Function

End Class
