
Imports System.Diagnostics
Imports System.IO
Imports System.Text
Imports System.Windows.Forms

Public Class Form1

    Public Sub New()
        ' This is required by the Windows Form Designer.
        InitializeComponent()

        ' (Optional) Any startup defaults:
        chkOverwrite.Checked = True
    End Sub


    Private Sub btnLoadLayers_Click(sender As Object, e As EventArgs) Handles btnLoadLayers.Click
        Try
            ' Clear previous layers and reset progress
            lstLayers.Items.Clear()
            ResetProgress()
            If pb IsNot Nothing Then pb.Value = 0
            If lblProgress IsNot Nothing Then lblProgress.Text = "0%"

            ' Validate MDB path
            Dim mdb = txtMDB.Text.Trim()
            If String.IsNullOrWhiteSpace(mdb) OrElse Not IO.File.Exists(mdb) Then
                Throw New IO.FileNotFoundException("Select a valid Personal Geodatabase (.mdb).", mdb)
            End If

            ' Show GDAL diagnostics
            GdalRunner.DumpGdalDiag(AddressOf Log)

            ' Check PGeo driver availability
            If Not GdalRunner.HasPGeoDriver(AddressOf Log) Then
                Throw New Exception("GDAL runtime found, but PGeo driver is not visible to the app. " &
                                "Please ensure your gdal\bin payload includes the PGeo-enabled build " &
                                "(e.g., from OSGeo4W) and that gdal\share\{gdal,proj} exist next to the EXE.")
            End If

            ' Try opening the MDB
            If Not GdalRunner.TryOpenMdb(mdb, AddressOf Log) Then
                Throw New Exception("Unable to open MDB via PGeo/ODBC. Check the log above for the exact GDAL/ODBC message.")
            End If

            ' Get layers with geometry info
            Dim layers = GdalRunner.ListLayers(mdb) ' Updated to exclude INFO/Open of and geometry None

            If layers Is Nothing OrElse layers.Count = 0 Then
                Log("No vector layers found.")
                Return
            End If

            ' Populate CheckedListBox with filters
            For Each l In layers
                ' Apply checkbox filter for T_1_* layers
                If chTab.Checked AndAlso l.StartsWith("T_1_", StringComparison.OrdinalIgnoreCase) Then
                    Continue For
                End If

                ' Add layer to UI
                lstLayers.Items.Add(l, True) ' Default: checked
            Next

            Log($"Layers loaded: {lstLayers.Items.Count}")
        Catch ex As Exception
            Log("ERROR: " & ex.Message)
            MessageBox.Show(Me, ex.Message, "Load Layers", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub chTab_CheckedChanged(sender As Object, e As EventArgs) Handles chTab.CheckedChanged
        ' Reload layers when filter changes
        btnLoadLayers_Click(Nothing, Nothing)
    End Sub

    ' Reset progress UI to 0%
    Private Sub ResetProgress()
        If pb IsNot Nothing Then pb.Value = 0
        If lblProgress IsNot Nothing Then lblProgress.Text = "0%"
    End Sub

    ' Update ProgressBar + percent label (thread-safe)
    Private Sub ReportProgress(pct As Integer)
        If Me.InvokeRequired Then
            Me.BeginInvoke(Sub() ReportProgress(pct))
            Return
        End If
        If pb IsNot Nothing Then
            pb.Value = Math.Max(pb.Minimum, Math.Min(pb.Maximum, pct))
        End If
        If lblProgress IsNot Nothing Then
            lblProgress.Text = pct.ToString() & "%"
        End If
    End Sub

    ' Append to log (thread-safe)
    Private Sub Log(msg As String)
        If Me.InvokeRequired Then
            Me.BeginInvoke(Sub() Log(msg))
            Return
        End If
        If txtLog IsNot Nothing Then
            txtLog.AppendText(msg & Environment.NewLine)
        End If
    End Sub


    Private Async Sub btnConvert_Click(sender As Object, e As EventArgs) Handles btnConvert.Click
        Try
            btnConvert.Enabled = False
            Cursor = Cursors.WaitCursor
            ResetProgress()
            If pb IsNot Nothing Then pb.Value = 0
            If lblProgress IsNot Nothing Then lblProgress.Text = "0%"

            Dim mdb As String = txtMDB.Text.Trim()
            Dim gpkg As String = txtGpkg.Text.Trim()
            If String.IsNullOrWhiteSpace(mdb) OrElse Not IO.File.Exists(mdb) Then
                Throw New IO.FileNotFoundException("The selected MDB file was not found.", mdb)
            End If
            If String.IsNullOrWhiteSpace(gpkg) Then
                Throw New Exception("Please choose an output GeoPackage (.gpkg) path.")
            End If

            Dim outDir = IO.Path.GetDirectoryName(gpkg)
            If String.IsNullOrWhiteSpace(outDir) Then Throw New Exception("Invalid output folder.")
            If Not IO.Directory.Exists(outDir) Then IO.Directory.CreateDirectory(outDir)

            Dim selected As New List(Of String)
            For Each item In lstLayers.CheckedItems
                selected.Add(item.ToString())
            Next
            If selected.Count = 0 Then Throw New Exception("Select (check) at least one layer.")

            Dim epsg As String = txtEpsg.Text.Trim()
            Dim whereClause As String = If(String.IsNullOrWhiteSpace(txtWhere.Text), Nothing, txtWhere.Text)

            Log($"Starting export of {selected.Count} layer(s) to: {gpkg}")

            Await Task.Run(Sub()
                               GdalRunner.ConvertMdbToGpkg(
                               mdbPath:=mdb,
                               gpkgPath:=gpkg,
                               layerNames:=selected,
                               dstEpsg:=If(String.IsNullOrWhiteSpace(epsg), Nothing, epsg),
                               whereClause:=whereClause,
                               overwrite:=chkOverwrite.Checked,
                               log:=AddressOf Log,
                               reportProgress:=AddressOf ReportProgress
                           )
                           End Sub)

            ReportProgress(100)
            Log("Done.")

            MessageBox.Show(Me, $"Exported {selected.Count} layer(s) to:{Environment.NewLine}{gpkg}",
                        "MDB → GPKG", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            Log("ERROR: " & ex.Message)
            MessageBox.Show(Me, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.[Error])
        Finally
            Cursor = Cursors.Default
            btnConvert.Enabled = True
        End Try
    End Sub


    Private Sub txtMdb_TextChanged(sender As Object, e As EventArgs) Handles txtMDB.TextChanged
        Try
            If Not String.IsNullOrWhiteSpace(txtMDB.Text) AndAlso String.IsNullOrWhiteSpace(txtGpkg.Text) Then
                Dim dir = Path.GetDirectoryName(txtMDB.Text)
                Dim name = Path.GetFileNameWithoutExtension(txtMDB.Text)
                txtGpkg.Text = Path.Combine(dir, name & ".gpkg")
            End If
        Catch

        End Try
    End Sub

    Private Sub btnBrowseMdb_Click(sender As Object, e As EventArgs) Handles btnBrowseMdb.Click
        Using dlg As New OpenFileDialog()
            dlg.Filter = "Personal Geodatabase (*.mdb)|*.mdb|All files (*.*)|*.*"
            dlg.CheckFileExists = True
            dlg.Title = "Select Personal Geodatabase (.mdb)"
            If Directory.Exists(Path.GetDirectoryName(txtMDB.Text)) Then
                dlg.InitialDirectory = Path.GetDirectoryName(txtMDB.Text)
            End If
            If dlg.ShowDialog(Me) = DialogResult.OK Then
                txtMDB.Text = dlg.FileName
            End If
        End Using
    End Sub

    Private Sub btnBrowseGpkg_Click(sender As Object, e As EventArgs) Handles btnBrowseGpkg.Click
        Using dlg As New SaveFileDialog()
            dlg.Filter = "GeoPackage (*.gpkg)|*.gpkg|All files (*.*)|*.*"
            dlg.AddExtension = True
            dlg.DefaultExt = "gpkg"
            dlg.OverwritePrompt = True
            dlg.Title = "Choose output GeoPackage (.gpkg)"
            If Not String.IsNullOrWhiteSpace(txtMDB.Text) Then
                Dim dir = Path.GetDirectoryName(txtMDB.Text)
                Dim name = Path.GetFileNameWithoutExtension(txtMDB.Text)
                dlg.InitialDirectory = dir
                dlg.FileName = name & ".gpkg"
            End If
            If dlg.ShowDialog(Me) = DialogResult.OK Then
                txtGpkg.Text = dlg.FileName
            End If
        End Using
    End Sub

    ' Get checked layer names from the CheckedListBox
    Private Function GetCheckedLayerNames() As List(Of String)
        Dim names As New List(Of String)
        For Each item In lstLayers.CheckedItems
            names.Add(item.ToString())
        Next
        Return names
    End Function

    Private Sub email_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles email.LinkClicked
        Try
            Process.Start("mailto:electroschite@gmail.com")
        Catch ex As Exception
            ' If opening the email client fails, copy the email to clipboard and show a message
            Clipboard.SetText("electroschite@gmail.com")
            MessageBox.Show("The email address has been copied to your clipboard.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Try

    End Sub

    Private Sub btnLic_Click(sender As Object, e As EventArgs) Handles btnLic.Click
        Dim licenseWindow As New LicenseForm()
        licenseWindow.ShowDialog()

    End Sub

    Private Sub btnHelp_Click(sender As Object, e As EventArgs) Handles btnHelp.Click
        Dim helpWindow As New Help()
        helpWindow.ShowDialog()
    End Sub
End Class
