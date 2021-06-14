'*************************************************************************************************************************************************
' stsim-ecodep: SyncroSim Add-On Package (to stsim) for calculating ecological departure in ST-Sim using the LANDFIRE Fire Regime Condition Class.
'
' Copyright © 2007-2021 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*************************************************************************************************************************************************

Imports SyncroSim.Core
Imports System.Globalization

Class DBUpdate
    Inherits UpdateProvider

    Public Overrides Sub PerformUpdate(store As DataStore, currentSchemaVersion As Integer)

        PerformUpdateInternal(store, currentSchemaVersion)

#If DEBUG Then

        'Verify that all expected indexes exist after the update because it Is easy to forget to recreate them after 
        'adding a column to an existing table (which requires the table to be recreated if you want to preserve column order.)

        ASSERT_INDEX_EXISTS(store, "stsimecodep_Output")

#End If

    End Sub

#If DEBUG Then

    Private Shared Sub ASSERT_INDEX_EXISTS(ByVal store As DataStore, ByVal tableName As String)

        If (store.TableExists(tableName)) Then

            Dim IndexName As String = tableName + "_Index"
            Dim Query As String = String.Format(CultureInfo.InvariantCulture, "SELECT COUNT(name) FROM sqlite_master WHERE type = 'index' AND name = '{0}'", IndexName)
            Debug.Assert(CInt(store.ExecuteScalar(Query)) = 1)

        End If

    End Sub

#End If

    Private Shared Sub PerformUpdateInternal(store As DataStore, currentSchemaVersion As Integer)

        'Start at 100 for 2.1.x
        If (currentSchemaVersion < 100) Then
            ECODEP_0000100(store)
        End If

    End Sub

    ''' <summary>
    ''' ECODEP_0000100
    ''' </summary>
    ''' <param name="store"></param>
    ''' <remarks>
    ''' This update renames the ecological departure tables for the new namespace rules.
    ''' </remarks>
    Private Shared Sub ECODEP_0000100(ByVal store As DataStore)

        UpdateProvider.RenameTablesWithPrefix(store, "ED_", "stsimecodep_")

        store.ExecuteNonQuery("DROP INDEX IF EXISTS ED_Output_Index")
        UpdateProvider.CreateIndex(store, "stsimecodep_Output", New String() {"ScenarioID", "Iteration", "Timestep", "StratumID"})

    End Sub

End Class
