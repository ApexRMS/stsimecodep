'*************************************************************************************************************************************************
' stsim-ecodep: SyncroSim Add-On Package (to stsim) for calculating ecological departure in ST-Sim using the LANDFIRE Fire Regime Condition Class.
'
' Copyright © 2007-2021 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*************************************************************************************************************************************************

Imports SyncroSim.Core

Class DBUpdate
    Inherits DotNetUpdateProvider

    <Update(0.101, "This update converts the schema from v2 to v3")>
    Public Shared Sub Update_0_101(ByVal store As DataStore)

        'We have no v3 updates to do, but since we have an update provider we need to delete our
        'legacy schema table stsimecodep_Schema.

        DropTable(store, "stsimecodep_Schema")

    End Sub

End Class
