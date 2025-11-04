<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LicenseForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(LicenseForm))
        txtLicense = New TextBox()
        SuspendLayout()
        ' 
        ' txtLicense
        ' 
        txtLicense.Dock = DockStyle.Fill
        txtLicense.Font = New Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        txtLicense.Location = New Point(0, 0)
        txtLicense.Multiline = True
        txtLicense.Name = "txtLicense"
        txtLicense.ReadOnly = True
        txtLicense.ScrollBars = ScrollBars.Vertical
        txtLicense.Size = New Size(627, 413)
        txtLicense.TabIndex = 0
        txtLicense.Text = resources.GetString("txtLicense.Text")
        ' 
        ' LicenseForm
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(627, 413)
        Controls.Add(txtLicense)
        Name = "LicenseForm"
        Text = "LicenseForm"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents txtLicense As TextBox
End Class
