'*************************************************************************************************************************************************
' stsim-ecodep: SyncroSim Add-On Package (to stsim) for calculating ecological departure in ST-Sim using the LANDFIRE Fire Regime Condition Class.
'
' Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*************************************************************************************************************************************************

Imports SyncroSim.Core
Imports System.Globalization

Module Utilities

    ''' <summary>
    ''' Creates a string key from two integers
    ''' </summary>
    ''' <param name="n1"></param>
    ''' <param name="n2"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateKey(ByVal n1 As Integer, ByVal n2 As Integer) As String
        Return String.Format(CultureInfo.InvariantCulture, "{0}-{1}", n1, n2)
    End Function

    ''' <summary>
    ''' Creates a string key from three integers
    ''' </summary>
    ''' <param name="n1"></param>
    ''' <param name="n2"></param>
    ''' <param name="n3"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateKey(ByVal n1 As Integer, ByVal n2 As Integer, ByVal n3 As Integer) As String
        Return String.Format(CultureInfo.InvariantCulture, "{0}-{1}-{2}", n1, n2, n3)
    End Function

    Public Function CreateKey(ByVal n1 As Integer, ByVal n2 As Integer, ByVal n3 As Integer, ByVal n4 As Integer) As String
        Return String.Format(CultureInfo.InvariantCulture, "{0}-{1}-{2}-{3}", n1, n2, n3, n4)
    End Function

    ''' <summary>
    ''' Gets the selected attribute Id
    ''' </summary>
    ''' <returns>The Id if specified or Nothing if not.</returns>
    ''' <remarks></remarks>
    Public Function GetSelectedAttributeId(ByVal scenario As Scenario) As Nullable(Of Integer)

        Dim ds As DataSheet = scenario.GetDataSheet(DATASHEET_OPTIONS_NAME)
        Dim dr As DataRow = ds.GetDataRow

        If (dr Is Nothing) Then
            Return Nothing
        End If

        If (dr(TRANSITION_ATTRIBUTE_TYPE_ID_COLUMN_NAME) Is DBNull.Value) Then
            Return Nothing
        End If

        Return CInt(dr(TRANSITION_ATTRIBUTE_TYPE_ID_COLUMN_NAME))

    End Function

End Module
