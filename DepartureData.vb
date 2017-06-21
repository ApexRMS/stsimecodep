'*********************************************************************************************
' Ecological Departure: A SyncroSim module for doing ecological departure analysis.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Class DepartureData

    Public m_Departure As Double
    Public m_Cumulative As Nullable(Of Double)

    Public Sub New(ByVal departure As Double, ByVal cumulative As Nullable(Of Double))

        Me.m_Departure = departure
        Me.m_Cumulative = cumulative

    End Sub

End Class
