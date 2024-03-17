﻿'*************************************************************************************************************************************************
' stsimecodep: SyncroSim Package for calculating ecological departure in ST-Sim using the LANDFIRE Fire Regime Condition Class.
'
' Copyright © 2007-2024 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*************************************************************************************************************************************************

Module DataTableUtilities

    Public Function GetNullableDatabaseValue(value As Nullable(Of Double)) As Object

        If value.HasValue Then
            Return value.Value
        Else
            Return DBNull.Value
        End If

    End Function

End Module
