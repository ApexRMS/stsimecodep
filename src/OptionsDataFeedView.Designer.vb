<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class OptionsDataFeedView
    Inherits SyncroSim.Core.Forms.DataFeedView

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.GroupBoxOptions = New System.Windows.Forms.GroupBox()
        Me.LabelReportEvery = New System.Windows.Forms.Label()
        Me.ComboBoxTransitionAttribute = New System.Windows.Forms.ComboBox()
        Me.TextBoxTimesteps = New System.Windows.Forms.TextBox()
        Me.CheckBoxTransitionAttribute = New System.Windows.Forms.CheckBox()
        Me.LabelTimesteps = New System.Windows.Forms.Label()
        Me.LabelRefPercent = New System.Windows.Forms.Label()
        Me.PanelReferencePercent = New System.Windows.Forms.Panel()
        Me.GroupBoxOptions.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBoxOptions
        '
        Me.GroupBoxOptions.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBoxOptions.Controls.Add(Me.LabelReportEvery)
        Me.GroupBoxOptions.Controls.Add(Me.ComboBoxTransitionAttribute)
        Me.GroupBoxOptions.Controls.Add(Me.TextBoxTimesteps)
        Me.GroupBoxOptions.Controls.Add(Me.CheckBoxTransitionAttribute)
        Me.GroupBoxOptions.Controls.Add(Me.LabelTimesteps)
        Me.GroupBoxOptions.Location = New System.Drawing.Point(3, 3)
        Me.GroupBoxOptions.Name = "GroupBoxOptions"
        Me.GroupBoxOptions.Size = New System.Drawing.Size(593, 85)
        Me.GroupBoxOptions.TabIndex = 0
        Me.GroupBoxOptions.TabStop = False
        Me.GroupBoxOptions.Text = "Options"
        '
        'LabelReportEvery
        '
        Me.LabelReportEvery.AutoSize = True
        Me.LabelReportEvery.Location = New System.Drawing.Point(17, 26)
        Me.LabelReportEvery.Name = "LabelReportEvery"
        Me.LabelReportEvery.Size = New System.Drawing.Size(167, 13)
        Me.LabelReportEvery.TabIndex = 0
        Me.LabelReportEvery.Text = "Report ecological departure every"
        '
        'ComboBoxTransitionAttribute
        '
        Me.ComboBoxTransitionAttribute.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxTransitionAttribute.FormattingEnabled = True
        Me.ComboBoxTransitionAttribute.Location = New System.Drawing.Point(192, 52)
        Me.ComboBoxTransitionAttribute.Name = "ComboBoxTransitionAttribute"
        Me.ComboBoxTransitionAttribute.Size = New System.Drawing.Size(201, 21)
        Me.ComboBoxTransitionAttribute.TabIndex = 4
        '
        'TextBoxTimesteps
        '
        Me.TextBoxTimesteps.Location = New System.Drawing.Point(193, 24)
        Me.TextBoxTimesteps.Name = "TextBoxTimesteps"
        Me.TextBoxTimesteps.Size = New System.Drawing.Size(60, 20)
        Me.TextBoxTimesteps.TabIndex = 1
        '
        'CheckBoxTransitionAttribute
        '
        Me.CheckBoxTransitionAttribute.AutoSize = True
        Me.CheckBoxTransitionAttribute.Location = New System.Drawing.Point(20, 52)
        Me.CheckBoxTransitionAttribute.Name = "CheckBoxTransitionAttribute"
        Me.CheckBoxTransitionAttribute.Size = New System.Drawing.Size(113, 17)
        Me.CheckBoxTransitionAttribute.TabIndex = 3
        Me.CheckBoxTransitionAttribute.Text = "Transition attribute"
        Me.CheckBoxTransitionAttribute.UseVisualStyleBackColor = True
        '
        'LabelTimesteps
        '
        Me.LabelTimesteps.AutoSize = True
        Me.LabelTimesteps.Location = New System.Drawing.Point(258, 26)
        Me.LabelTimesteps.Name = "LabelTimesteps"
        Me.LabelTimesteps.Size = New System.Drawing.Size(51, 13)
        Me.LabelTimesteps.TabIndex = 2
        Me.LabelTimesteps.Text = "timesteps"
        '
        'LabelRefPercent
        '
        Me.LabelRefPercent.AutoSize = True
        Me.LabelRefPercent.Location = New System.Drawing.Point(2, 101)
        Me.LabelRefPercent.Name = "LabelRefPercent"
        Me.LabelRefPercent.Size = New System.Drawing.Size(111, 13)
        Me.LabelRefPercent.TabIndex = 1
        Me.LabelRefPercent.Text = "Reference conditions:"
        '
        'PanelReferencePercent
        '
        Me.PanelReferencePercent.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelReferencePercent.Location = New System.Drawing.Point(4, 119)
        Me.PanelReferencePercent.Name = "PanelReferencePercent"
        Me.PanelReferencePercent.Size = New System.Drawing.Size(592, 270)
        Me.PanelReferencePercent.TabIndex = 2
        '
        'OptionsDataFeedView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.GroupBoxOptions)
        Me.Controls.Add(Me.LabelRefPercent)
        Me.Controls.Add(Me.PanelReferencePercent)
        Me.Name = "OptionsDataFeedView"
        Me.Size = New System.Drawing.Size(599, 392)
        Me.GroupBoxOptions.ResumeLayout(False)
        Me.GroupBoxOptions.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents GroupBoxOptions As System.Windows.Forms.GroupBox
    Friend WithEvents LabelReportEvery As System.Windows.Forms.Label
    Friend WithEvents ComboBoxTransitionAttribute As System.Windows.Forms.ComboBox
    Friend WithEvents TextBoxTimesteps As System.Windows.Forms.TextBox
    Friend WithEvents CheckBoxTransitionAttribute As System.Windows.Forms.CheckBox
    Friend WithEvents LabelTimesteps As System.Windows.Forms.Label
    Friend WithEvents LabelRefPercent As System.Windows.Forms.Label
    Friend WithEvents PanelReferencePercent As System.Windows.Forms.Panel

End Class
