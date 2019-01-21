'************************************************************************************
' Ecological Departure: A SyncroSim Package for doing ecological departure analysis.
'
' Copyright © 2009-2015 ApexRMS.
'
'************************************************************************************

Imports SyncroSim.Core

Class TransformerBase
    Inherits Transformer

    Protected Function EcologicalDepartureEnabled() As Boolean

        Dim dr As DataRow = Me.ResultScenario.GetDataSheet(DATASHEET_OPTIONS_NAME).GetDataRow()
        Return (dr IsNot Nothing)

    End Function

End Class
