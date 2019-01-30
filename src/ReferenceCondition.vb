'*********************************************************************************************
' Ecological Departure: A SyncroSim Package for doing ecological departure analysis.
'
' Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Class ReferenceCondition

    Private m_RelativeAmount As Integer
    Private m_Undesirability As Nullable(Of Double)
    Private m_Threshold As Nullable(Of Double)

    Public Sub New(
        ByVal relativeAmount As Integer,
        ByVal undesirability As Nullable(Of Double),
        ByVal threshold As Nullable(Of Double))

        Me.m_RelativeAmount = relativeAmount
        Me.m_Undesirability = undesirability
        Me.m_Threshold = threshold

    End Sub

    Public ReadOnly Property RelativeAmount As Integer
        Get
            Return Me.m_RelativeAmount
        End Get
    End Property

    Public ReadOnly Property Undesirability As Nullable(Of Double)
        Get
            Return Me.m_Undesirability
        End Get
    End Property

    Public ReadOnly Property Threshold As Nullable(Of Double)
        Get
            Return Me.m_Threshold
        End Get
    End Property

End Class
