'*************************************************************************************************************************************************
' stsimecodep: SyncroSim Package for calculating ecological departure in ST-Sim using the LANDFIRE Fire Regime Condition Class.
'
' Copyright © 2007-2024 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*************************************************************************************************************************************************

Imports SyncroSim.Core

Module Validate

#If DEBUG Then

    Public Sub ValidateScenario(ByVal id As Integer, ByVal project As Project)

        Dim found As Boolean = False

        For Each scen As Scenario In project.Results

            If (scen.Id = id) Then
                found = True
                Exit For
            End If

        Next

        Debug.Assert(found)

    End Sub

    Public Sub ValidateStratum(ByVal id As Integer, ByVal project As Project)


        Dim ds As DataSheet = project.GetDataSheet(Constants.DATASHEET_STSIM_STRATUM)
        Dim dt As DataTable = ds.GetData()

        Dim found As Boolean = False

        For Each dr As DataRow In dt.Rows

            If (CInt(dr(ds.PrimaryKeyColumn.Name)) = id) Then
                found = True
                Exit For
            End If

        Next

        Debug.Assert(found)

    End Sub

    Public Sub ValidateSTSDictionary(
        ByVal dict As Dictionary(Of String, 
        Dictionary(Of Integer, DepartureData)),
        ByVal project As Project)

        For Each k As String In dict.Keys

            Dim s() As String = k.Split(CChar("-"))
            Dim snid As Integer = CInt(s(0))
            Dim stid As Integer = CInt(s(2))

            ValidateScenario(snid, project)
            ValidateStratum(stid, project)

        Next

    End Sub

    Public Sub ValidateCADictionary(
        ByVal dict As Dictionary(Of String, Double),
        ByVal project As Project)

        For Each k As String In dict.Keys

            Dim s() As String = k.Split(CChar("-"))
            Dim stid As Integer = CInt(s(2))

            ValidateStratum(stid, project)

        Next

    End Sub

#End If

End Module
