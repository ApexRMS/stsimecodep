'*************************************************************************************************************************************************
' stsim-ecodep: SyncroSim Add-On Package (to stsim) for calculating ecological departure in ST-Sim using the LANDFIRE Fire Regime Condition Class.
'
' Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*************************************************************************************************************************************************

Class DepartureData

    Public m_Departure As Double
    Public m_Cumulative As Nullable(Of Double)

    Public Sub New(ByVal departure As Double, ByVal cumulative As Nullable(Of Double))

        Me.m_Departure = departure
        Me.m_Cumulative = cumulative

    End Sub

End Class
