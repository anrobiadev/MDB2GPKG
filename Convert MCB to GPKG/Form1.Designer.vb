<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        btnLoadLayers = New Button()
        btnConvert = New Button()
        txtMDB = New TextBox()
        btnBrowseMdb = New Button()
        txtGpkg = New TextBox()
        btnBrowseGpkg = New Button()
        lstLayers = New CheckedListBox()
        txtLog = New TextBox()
        txtEpsg = New TextBox()
        txtWhere = New TextBox()
        chkOverwrite = New CheckBox()
        pb = New ProgressBar()
        lblProgress = New Label()
        chTab = New CheckBox()
        Label1 = New Label()
        email = New LinkLabel()
        Label2 = New Label()
        Label4 = New Label()
        btnHelp = New Button()
        btnLic = New Button()
        SuspendLayout()
        ' 
        ' btnLoadLayers
        ' 
        btnLoadLayers.Location = New Point(12, 127)
        btnLoadLayers.Name = "btnLoadLayers"
        btnLoadLayers.Size = New Size(95, 29)
        btnLoadLayers.TabIndex = 0
        btnLoadLayers.Text = "Load Layers"
        btnLoadLayers.UseVisualStyleBackColor = True
        ' 
        ' btnConvert
        ' 
        btnConvert.Location = New Point(345, 127)
        btnConvert.Name = "btnConvert"
        btnConvert.Size = New Size(94, 29)
        btnConvert.TabIndex = 1
        btnConvert.Text = "Convert file"
        btnConvert.UseVisualStyleBackColor = True
        ' 
        ' txtMDB
        ' 
        txtMDB.Location = New Point(12, 21)
        txtMDB.Name = "txtMDB"
        txtMDB.PlaceholderText = "MDB path"
        txtMDB.Size = New Size(532, 27)
        txtMDB.TabIndex = 2
        ' 
        ' btnBrowseMdb
        ' 
        btnBrowseMdb.Font = New Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        btnBrowseMdb.Location = New Point(565, 21)
        btnBrowseMdb.Name = "btnBrowseMdb"
        btnBrowseMdb.Size = New Size(94, 29)
        btnBrowseMdb.TabIndex = 3
        btnBrowseMdb.Text = "Browse MDB"
        btnBrowseMdb.UseVisualStyleBackColor = True
        ' 
        ' txtGpkg
        ' 
        txtGpkg.Location = New Point(12, 71)
        txtGpkg.Name = "txtGpkg"
        txtGpkg.PlaceholderText = "GPKG path"
        txtGpkg.Size = New Size(532, 27)
        txtGpkg.TabIndex = 4
        ' 
        ' btnBrowseGpkg
        ' 
        btnBrowseGpkg.Font = New Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        btnBrowseGpkg.Location = New Point(565, 71)
        btnBrowseGpkg.Name = "btnBrowseGpkg"
        btnBrowseGpkg.Size = New Size(94, 29)
        btnBrowseGpkg.TabIndex = 5
        btnBrowseGpkg.Text = "Browse GPKG"
        btnBrowseGpkg.UseVisualStyleBackColor = True
        ' 
        ' lstLayers
        ' 
        lstLayers.CheckOnClick = True
        lstLayers.FormattingEnabled = True
        lstLayers.Location = New Point(12, 173)
        lstLayers.Name = "lstLayers"
        lstLayers.Size = New Size(467, 136)
        lstLayers.TabIndex = 6
        ' 
        ' txtLog
        ' 
        txtLog.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        txtLog.Location = New Point(12, 357)
        txtLog.Multiline = True
        txtLog.Name = "txtLog"
        txtLog.PlaceholderText = "Log..."
        txtLog.ScrollBars = ScrollBars.Both
        txtLog.Size = New Size(776, 296)
        txtLog.TabIndex = 7
        ' 
        ' txtEpsg
        ' 
        txtEpsg.Location = New Point(671, 127)
        txtEpsg.Name = "txtEpsg"
        txtEpsg.PlaceholderText = "EPSG code (opt.)"
        txtEpsg.Size = New Size(117, 27)
        txtEpsg.TabIndex = 8
        txtEpsg.Text = "3844"
        ' 
        ' txtWhere
        ' 
        txtWhere.Location = New Point(488, 214)
        txtWhere.Multiline = True
        txtWhere.Name = "txtWhere"
        txtWhere.PlaceholderText = "Ex: FLUID_TYPE = 44"
        txtWhere.Size = New Size(297, 95)
        txtWhere.TabIndex = 9
        ' 
        ' chkOverwrite
        ' 
        chkOverwrite.AutoSize = True
        chkOverwrite.ForeColor = Color.ForestGreen
        chkOverwrite.Location = New Point(454, 130)
        chkOverwrite.Name = "chkOverwrite"
        chkOverwrite.Size = New Size(143, 24)
        chkOverwrite.TabIndex = 10
        chkOverwrite.Text = "Overwrite output"
        chkOverwrite.UseVisualStyleBackColor = True
        ' 
        ' pb
        ' 
        pb.Location = New Point(12, 318)
        pb.Name = "pb"
        pb.Size = New Size(717, 29)
        pb.TabIndex = 11
        ' 
        ' lblProgress
        ' 
        lblProgress.AutoSize = True
        lblProgress.Location = New Point(748, 318)
        lblProgress.Name = "lblProgress"
        lblProgress.Size = New Size(29, 20)
        lblProgress.TabIndex = 12
        lblProgress.Text = "0%"
        ' 
        ' chTab
        ' 
        chTab.AutoSize = True
        chTab.ForeColor = Color.Red
        chTab.Location = New Point(123, 132)
        chTab.Name = "chTab"
        chTab.Size = New Size(189, 24)
        chTab.TabIndex = 13
        chTab.Text = "Exclude technical tables"
        chTab.UseVisualStyleBackColor = True
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(19, 656)
        Label1.Name = "Label1"
        Label1.Size = New Size(281, 20)
        Label1.TabIndex = 15
        Label1.Text = "© AnRoVia Dev., 2025. All rights reserved"
        ' 
        ' email
        ' 
        email.AutoSize = True
        email.Location = New Point(678, 656)
        email.Name = "email"
        email.Size = New Size(107, 20)
        email.TabIndex = 16
        email.TabStop = True
        email.Text = "Contact e-mail"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Font = New Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label2.ForeColor = Color.Maroon
        Label2.Location = New Point(671, 107)
        Label2.Name = "Label2"
        Label2.Size = New Size(106, 17)
        Label2.TabIndex = 17
        Label2.Text = "EPSG code (opt.)"
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Font = New Font("Segoe UI", 8F)
        Label4.ForeColor = Color.Brown
        Label4.Location = New Point(488, 173)
        Label4.Name = "Label4"
        Label4.Size = New Size(285, 38)
        Label4.TabIndex = 19
        Label4.Text = "SQL WHERE (optional and only for advanced " & vbCrLf & "users, not for the Firecracker team)"
        ' 
        ' btnHelp
        ' 
        btnHelp.Font = New Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        btnHelp.Location = New Point(691, 71)
        btnHelp.Name = "btnHelp"
        btnHelp.Size = New Size(94, 29)
        btnHelp.TabIndex = 20
        btnHelp.Text = "Help"
        btnHelp.UseVisualStyleBackColor = True
        ' 
        ' btnLic
        ' 
        btnLic.Font = New Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        btnLic.Location = New Point(691, 21)
        btnLic.Name = "btnLic"
        btnLic.Size = New Size(94, 29)
        btnLic.TabIndex = 21
        btnLic.Text = "License"
        btnLic.UseVisualStyleBackColor = True
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 686)
        Controls.Add(btnLic)
        Controls.Add(btnHelp)
        Controls.Add(Label4)
        Controls.Add(Label2)
        Controls.Add(email)
        Controls.Add(Label1)
        Controls.Add(chTab)
        Controls.Add(lblProgress)
        Controls.Add(pb)
        Controls.Add(chkOverwrite)
        Controls.Add(txtWhere)
        Controls.Add(txtEpsg)
        Controls.Add(txtLog)
        Controls.Add(lstLayers)
        Controls.Add(btnBrowseGpkg)
        Controls.Add(txtGpkg)
        Controls.Add(btnBrowseMdb)
        Controls.Add(txtMDB)
        Controls.Add(btnConvert)
        Controls.Add(btnLoadLayers)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MaximumSize = New Size(818, 753)
        Name = "Form1"
        Text = "Green Wolf Converter 4 ArcGIS PROst - Mdb2Gpkg "
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents btnLoadLayers As Button
    Friend WithEvents btnConvert As Button
    Friend WithEvents txtMDB As TextBox
    Friend WithEvents btnBrowseMdb As Button
    Friend WithEvents txtGpkg As TextBox
    Friend WithEvents btnBrowseGpkg As Button
    Friend WithEvents lstLayers As CheckedListBox
    Friend WithEvents txtLog As TextBox
    Friend WithEvents txtEpsg As TextBox
    Friend WithEvents txtWhere As TextBox
    Friend WithEvents chkOverwrite As CheckBox
    Friend WithEvents pb As ProgressBar
    Friend WithEvents lblProgress As Label
    Friend WithEvents chTab As CheckBox
    Friend WithEvents Label1 As Label
    Friend WithEvents email As LinkLabel
    Friend WithEvents Label2 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents btnHelp As Button
    Friend WithEvents btnLic As Button

End Class
