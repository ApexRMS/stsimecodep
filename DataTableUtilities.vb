'*********************************************************************************************
' Ecological Departure: A SyncroSim module for doing ecological departure analysis.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Module DataTableUtilities

    Public Function GetNullableDatabaseValue(value As Nullable(Of Double)) As Object

        If value.HasValue Then
            Return value.Value
        Else
            Return DBNull.Value
        End If

    End Function

End Module
