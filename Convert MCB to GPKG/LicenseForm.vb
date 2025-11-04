Imports System.IO

Public Class LicenseForm
    Private Sub LicenseForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.ActiveControl = Nothing
        'Try
        '    Dim licensePath As String = Path.Combine(Application.StartupPath, "LICENSE.md") ' or "LICENSE.md"
        '    If File.Exists(licensePath) Then
        '        txtLicense.Text = File.ReadAllText(licensePath)
        '    Else
        '        txtLicense.Text = "License file not found."
        '    End If
        'Catch ex As Exception
        '    txtLicense.Text = "Error loading license: " & ex.Message
        'End Try

    End Sub
End Class