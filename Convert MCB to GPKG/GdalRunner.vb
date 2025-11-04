Imports System.Diagnostics
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions

Public Module GdalRunner

    ' Adjust if you place GDAL elsewhere under your app
    Private ReadOnly BaseDir As String = AppContext.BaseDirectory
    Private ReadOnly BinDir As String = Path.Combine(BaseDir, "gdal\bin")
    Private ReadOnly ShareDir As String = Path.Combine(BaseDir, "gdal\share")
    Private ReadOnly GdalDataDir As String = Path.Combine(ShareDir, "gdal")
    Private ReadOnly ProjDir As String = Path.Combine(ShareDir, "proj")

    Private Function StartProcess(
    exePath As String, args As String,
    Optional captureOutput As Boolean = True,
    Optional onOutput As Action(Of String) = Nothing,
    Optional onProgress As Action(Of Integer) = Nothing
) As (exitCode As Integer, stdOut As String, stdErr As String)

        If Not IO.File.Exists(exePath) Then
            Throw New IO.FileNotFoundException("Executable not found at: " & exePath, exePath)
        End If

        Dim psi As New ProcessStartInfo(exePath, args) With {
        .UseShellExecute = False,
        .RedirectStandardOutput = captureOutput,
        .RedirectStandardError = captureOutput,
        .CreateNoWindow = True
    }

        ' Point to bundled GDAL/PROJ (same as before)
        psi.EnvironmentVariables("PATH") = BinDir & ";" & psi.EnvironmentVariables("PATH")
        psi.EnvironmentVariables("GDAL_DATA") = GdalDataDir
        psi.EnvironmentVariables("PROJ_LIB") = ProjDir

        ' Hint ACE ODBC driver for PGeo (mirrors your working shell test)
        psi.EnvironmentVariables("PGEO_DRIVER_TEMPLATE") = "DRIVER=Microsoft Access Driver (*.mdb, *.accdb);DBQ=%s"
        ' Optional: turn on GDAL debug messages while developing
        ' psi.EnvironmentVariables("CPL_DEBUG") = "ON"

        psi.WorkingDirectory = BaseDir

        Dim p As New Process() With {.StartInfo = psi}
        Dim sbOut As New StringBuilder()
        Dim sbErr As New StringBuilder()

        ' Regex to grab a leading integer (0..100) anywhere in a progress line like "0...10...30..."
        Dim rePct As New Regex("(?<!\d)(\d{1,3})(?!\d)")

        If captureOutput Then
            AddHandler p.OutputDataReceived, Sub(sender, e)
                                                 If e.Data Is Nothing Then Return
                                                 Dim line = e.Data
                                                 sbOut.AppendLine(line)
                                                 onOutput?.Invoke(line)

                                                 ' Try parse a percentage and report it
                                                 Dim m = rePct.Match(line)
                                                 If m.Success Then
                                                     Dim n As Integer
                                                     If Integer.TryParse(m.Value, n) AndAlso n >= 0 AndAlso n <= 100 Then
                                                         onProgress?.Invoke(n)
                                                     End If
                                                 End If
                                             End Sub

            AddHandler p.ErrorDataReceived, Sub(sender, e)
                                                If e.Data Is Nothing Then Return
                                                Dim line = e.Data
                                                sbErr.AppendLine(line)
                                                onOutput?.Invoke(line) ' GDAL sometimes writes progress/info to stderr too
                                            End Sub
        End If

        ' Use ACE driver in DSN-less connection strings for both PGeo and MDB paths
        psi.EnvironmentVariables("PGEO_DRIVER_TEMPLATE") = "DRIVER=Microsoft Access Driver (*.mdb, *.accdb);DBQ=%s"
        psi.EnvironmentVariables("MDB_DRIVER_TEMPLATE") = "DRIVER=Microsoft Access Driver (*.mdb, *.accdb);DBQ=%s"

        ' (Optional) verbose GDAL logs while we test
        ' psi.EnvironmentVariables("CPL_DEBUG") = "ON"

        p.Start()
        If captureOutput Then
            p.BeginOutputReadLine()
            p.BeginErrorReadLine()
        End If
        p.WaitForExit()

        Return (p.ExitCode, sbOut.ToString(), sbErr.ToString())
    End Function

    Public Function GetLayerGeometryType(mdbPath As String, layerName As String) As String
        Dim ogrinfo As String = IO.Path.Combine(BinDir, "ogrinfo.exe")

        Dim args As String =
        "--config PGEO_DRIVER_TEMPLATE ""DRIVER=Microsoft Access Driver (*.mdb, *.accdb);DBQ=%s"" " &
        "--config MDB_DRIVER_TEMPLATE ""DRIVER=Microsoft Access Driver (*.mdb, *.accdb);DBQ=%s"" " &
        $"""{mdbPath}"" ""{layerName}"""

        Dim res = StartProcess(ogrinfo, args)
        If res.exitCode <> 0 Then
            Throw New Exception("ogrinfo failed: " & res.stdErr)
        End If

        For Each line In res.stdOut.Split({Environment.NewLine}, StringSplitOptions.None)
            If line.Contains("Geometry:") Then
                Return line.Split(":"c)(1).Trim()
            End If
        Next

        Return Nothing
    End Function

    Public Function CheckPGeoSupport(mdbPath As String) As Boolean
        Dim ogrinfo As String = Path.Combine(BinDir, "ogrinfo.exe")
        Dim args As String = $"""{ "PGeo:" & mdbPath }"" -so"
        Dim res = StartProcess(ogrinfo, args)
        Return res.exitCode = 0 AndAlso res.stdOut.IndexOf("using driver `PGeo'", StringComparison.OrdinalIgnoreCase) >= 0
    End Function

    Public Function ListLayers(mdbPath As String) As List(Of String)
        Dim ogrinfo As String = IO.Path.Combine(BinDir, "ogrinfo.exe")
        Dim args As String = $"""{mdbPath}"""
        Dim res = StartProcess(ogrinfo, args)
        If res.exitCode <> 0 Then Throw New Exception("ogrinfo failed: " & res.stdErr)

        Dim layers As New List(Of String)
        Using sr As New StringReader(res.stdOut)
            While True
                Dim line = sr.ReadLine()
                If line Is Nothing Then Exit While

                Dim trimmed = line.Trim()

                ' Skip non-layer lines
                If trimmed.StartsWith("INFO:", StringComparison.OrdinalIgnoreCase) Then Continue While
                If trimmed.StartsWith("Open of", StringComparison.OrdinalIgnoreCase) Then Continue While
                If Not Regex.IsMatch(trimmed, "^\d+:") Then Continue While

                ' Extract layer name and geometry
                Dim colon = trimmed.IndexOf(":"c)
                Dim name = trimmed.Substring(colon + 1).Trim()
                Dim paren = name.IndexOf(" (")
                Dim geomType As String = Nothing
                If paren > 0 Then
                    geomType = name.Substring(paren + 2).Replace(")", "").Trim()
                    name = name.Substring(0, paren)
                End If

                ' Exclude layers with geometry type None
                If Not String.IsNullOrWhiteSpace(geomType) AndAlso geomType.Equals("None", StringComparison.OrdinalIgnoreCase) Then
                    Continue While
                End If

                ' Add valid layer
                If name.Length > 0 Then layers.Add(name)
            End While
        End Using
        Return layers
    End Function

    ' Z-safe, ArcGIS-friendly MDB → GPKG exporter
    ' - Preserves Z; drop M by default (set dropM:=False to keep M)
    ' - Adds -xyRes <val>[ m|mm|deg ] for XY coordinate precision
    ' - No layer renaming: output layer names == input MDB layer names

    Public Sub ConvertMdbToGpkg(
    mdbPath As String,
    gpkgPath As String,
    Optional layerNames As IEnumerable(Of String) = Nothing,
    Optional dstEpsg As String = Nothing,
    Optional whereClause As String = Nothing,
    Optional overwrite As Boolean = True,
    Optional log As Action(Of String) = Nothing,
    Optional reportProgress As Action(Of Integer) = Nothing,
    Optional dropM As Boolean = True, _                 ' True = keep Z, drop M (ArcGIS-friendly)
    Optional xyRes As Double? = Nothing, _              ' e.g., 0.001
    Optional xyResUnit As String = Nothing, _           ' "m" | "mm" | "deg"   (optional)
    Optional gpkgLayerCreationOptions As IEnumerable(Of String) = Nothing,
    Optional gpkgDatasetCreationOptions As IEnumerable(Of String) = Nothing
)

        Dim ogr2ogr As String = IO.Path.Combine(BinDir, "ogr2ogr.exe")

        ' Overwrite target file if requested
        If overwrite AndAlso IO.File.Exists(gpkgPath) Then
            Try
                IO.File.Delete(gpkgPath)
            Catch ex As Exception
                Throw New IOException("Failed to delete existing GeoPackage: " & gpkgPath, ex)
            End Try
        End If

        ' Default LCOs if none provided
        If gpkgLayerCreationOptions Is Nothing Then
            gpkgLayerCreationOptions = New String() {"GEOMETRY_NAME=geom", "SPATIAL_INDEX=YES"}
        End If

        Dim layers As IEnumerable(Of String) = If(layerNames, ListLayers(mdbPath))
        Dim isFirst As Boolean = True

        For Each lyr In layers
            log?.Invoke($"Exporting layer: {lyr}")
            reportProgress?.Invoke(0)

            Dim args As New List(Of String)

            ' ---- Driver & creation options ----
            args.Add("-f ""GPKG""")

            If isFirst AndAlso gpkgDatasetCreationOptions IsNot Nothing Then
                For Each dsco In gpkgDatasetCreationOptions
                    If Not String.IsNullOrWhiteSpace(dsco) Then args.Add($"-dsco {QuoteIfNeeded(dsco)}")
                Next
            End If

            If gpkgLayerCreationOptions IsNot Nothing Then
                For Each lco In gpkgLayerCreationOptions
                    If Not String.IsNullOrWhiteSpace(lco) Then args.Add($"-lco {QuoteIfNeeded(lco)}")
                Next
            End If

            If Not isFirst Then args.Add("-update")
            If Not String.IsNullOrWhiteSpace(dstEpsg) Then args.Add($"-t_srs ""EPSG:{dstEpsg.Trim()}""")
            If Not String.IsNullOrWhiteSpace(whereClause) Then args.Add($"-where ""{whereClause.Replace("""", """""")}""")
            args.Add("-progress")

            ' ---- XY precision (ogr2ogr: -xyRes <val>[ m|mm|deg ]) ----
            If xyRes.HasValue Then
                Dim s = xyRes.Value.ToString(Globalization.CultureInfo.InvariantCulture)
                If Not String.IsNullOrWhiteSpace(xyResUnit) Then
                    Dim u = xyResUnit.Trim().ToLowerInvariant()
                    If u = "m" OrElse u = "mm" OrElse u = "deg" Then
                        s &= " " & u
                    Else
                        log?.Invoke($"[warn] xyResUnit '{xyResUnit}' is not one of m|mm|deg. Emitting -xyRes without unit.")
                    End If
                End If
                args.Add($"-xyRes {s}") ' Quantize XY coords to this resolution
            End If

            ' ---- Output & input datasets ----
            args.Add($"""{gpkgPath}""")
            args.Add($"""{mdbPath}""")

            ' ---- Geometry dimensionality (keep Z; optionally drop M) ----
            Dim geomType As String = Nothing
            Try
                geomType = GetLayerGeometryType(mdbPath, lyr) ' uses ogrinfo
            Catch ex As Exception
                log?.Invoke($"Warning: GetLayerGeometryType failed for '{lyr}': {ex.Message}")
            End Try

            If Not String.IsNullOrWhiteSpace(geomType) Then
                Dim nlt As String = NormalizeGeometryType(geomType) ' e.g., LINESTRINGZ, LINESTRINGZM, MULTILINESTRINGZ...
                If Not String.IsNullOrWhiteSpace(nlt) Then
                    If dropM Then nlt = StripMFromNormalizedType(nlt) ' ZM→Z; M→base
                    args.Add($"-nlt {nlt}")
                End If
            End If

            If dropM Then
                args.Add("-dim XYZ")       ' keep Z, remove M  (per ogr2ogr docs)
            Else
                args.Add("-dim layer_dim") ' preserve source XY/XYZ/XYM/XYZM
            End If

            ' ---- Source layer name last (no renaming) ----
            args.Add($"""{lyr}""")

            ' ---- Execute ogr2ogr ----
            Dim argStr = String.Join(" ", args)
            Dim result = StartProcess(ogr2ogr, argStr, True, onOutput:=log, onProgress:=reportProgress)
            If result.exitCode <> 0 Then
                Throw New Exception($"ogr2ogr failed for layer '{lyr}': {result.stdErr}{Environment.NewLine}{result.stdOut}")
            End If

            isFirst = False
            reportProgress?.Invoke(100)
        Next
    End Sub

    ' ---------- small helpers ----------
    Private Function QuoteIfNeeded(value As String) As String
        If String.IsNullOrWhiteSpace(value) Then Return """"""
        If value.StartsWith("""") AndAlso value.EndsWith("""") Then Return value
        Return $"""{value}"""
    End Function

    ' Convert "...ZM" → "...Z", "...M" → base type (no suffix)
    Private Function StripMFromNormalizedType(nlt As String) As String
        If String.IsNullOrWhiteSpace(nlt) Then Return nlt
        Dim u = nlt.ToUpperInvariant()
        If u.EndsWith("ZM") Then Return nlt.Substring(0, nlt.Length - 2) & "Z"
        If u.EndsWith("M") Then Return nlt.Substring(0, nlt.Length - 1)
        Return nlt
    End Function

    Private Function SourceHasZ(geomType As String) As Boolean
        If String.IsNullOrWhiteSpace(geomType) Then Return False
        Dim t = geomType.ToUpperInvariant()
        ' ogrinfo often prints "3D" or appends Z; also handle "... ZM"
        Return t.Contains("3D") OrElse t.Contains(" Z") OrElse t.EndsWith("Z") OrElse t.Contains(" ZM")
    End Function

    Private Function GetBaseGeometryType(rawType As String) As String
        Dim t = rawType.ToUpper()
        If t.Contains("POINT") Then Return If(t.Contains("MULTI"), "MULTIPOINT", "POINT")
        If t.Contains("LINE") Then Return If(t.Contains("MULTI"), "MULTILINESTRING", "LINESTRING")
        If t.Contains("POLYGON") Then Return If(t.Contains("MULTI"), "MULTIPOLYGON", "POLYGON")
        Return Nothing
    End Function

    Private Function NormalizeGeometryType(rawType As String) As String
        Dim t = rawType.ToUpper().Trim()

        Dim hasZ = t.Contains("3D")
        Dim hasM = t.Contains("MEASURED")

        Dim baseType As String
        If t.Contains("POINT") Then
            baseType = If(t.Contains("MULTI"), "MULTIPOINT", "POINT")
        ElseIf t.Contains("LINE") Then
            baseType = If(t.Contains("MULTI"), "MULTILINESTRING", "LINESTRING")
        ElseIf t.Contains("POLYGON") Then
            baseType = If(t.Contains("MULTI"), "MULTIPOLYGON", "POLYGON")
        Else
            Return Nothing
        End If

        ' Append Z/M suffix
        If hasZ AndAlso hasM Then
            Return baseType & "ZM"
        ElseIf hasZ Then
            Return baseType & "Z"
        ElseIf hasM Then
            Return baseType & "M"
        End If

        Return baseType
    End Function

    ' Lists formats and returns True if PGeo is present.
    Public Function HasPGeoDriver(log As Action(Of String)) As Boolean
        Dim ogrinfo = IO.Path.Combine(BinDir, "ogrinfo.exe")

        ' Show version (useful in the log)
        Dim v = StartProcess(ogrinfo, "--version")
        log?.Invoke("[ogrinfo] " & v.stdOut.Trim())

        ' List formats in the same child process env
        Dim fmts = StartProcess(ogrinfo, "--formats")
        If fmts.exitCode <> 0 Then
            log?.Invoke("[formats-err] " & fmts.stdErr)
            Return False
        End If

        Dim hasPGeo =
            fmts.stdOut.IndexOf(" PGeo ", StringComparison.OrdinalIgnoreCase) >= 0 OrElse
            fmts.stdOut.IndexOf("PGeo -vector-", StringComparison.OrdinalIgnoreCase) >= 0 OrElse
            fmts.stdOut.IndexOf("ESRI Personal GeoDatabase", StringComparison.OrdinalIgnoreCase) >= 0

        log?.Invoke(If(hasPGeo, "PGeo driver: PRESENT", "PGeo driver: MISSING"))
        Return hasPGeo
    End Function

    ' Tries to open the MDB with the same driver template we used in CLI. Returns True on success.
    Public Function TryOpenMdb(mdbPath As String, log As Action(Of String)) As Boolean
        Dim ogrinfo = IO.Path.Combine(BinDir, "ogrinfo.exe")

        Dim args As String =
    "--config PGEO_DRIVER_TEMPLATE ""DRIVER=Microsoft Access Driver (*.mdb, *.accdb);DBQ=%s"" " &
    "--config MDB_DRIVER_TEMPLATE  ""DRIVER=Microsoft Access Driver (*.mdb, *.accdb);DBQ=%s"" " &
    "--debug on " &
    $"""{mdbPath}"" -so"

        Dim res = StartProcess(ogrinfo, args, True, onOutput:=log)
        If res.exitCode = 0 AndAlso res.stdOut.IndexOf("FAILURE", StringComparison.OrdinalIgnoreCase) < 0 Then
            Return True
        End If

        ' Surface the exact ODBC/GDAL message in the log
        If Not String.IsNullOrWhiteSpace(res.stdErr) Then log?.Invoke("[ogrinfo-err] " & res.stdErr)
        Return False
    End Function

    ' Simple diag of where we expect binaries/data (helps spot wrong output folder)
    Public Sub DumpGdalDiag(log As Action(Of String))
        log?.Invoke("BaseDir = " & BaseDir)
        log?.Invoke("BinDir  = " & BinDir)
        log?.Invoke("GDAL_DATA = " & GdalDataDir)
        log?.Invoke("PROJ_LIB  = " & ProjDir)
        If IO.Directory.Exists(BinDir) Then
            For Each f In IO.Directory.GetFiles(BinDir)
                log?.Invoke("bin> " & IO.Path.GetFileName(f))
            Next
        Else
            log?.Invoke("bin> (missing)")
        End If
    End Sub

    Public Function ListLayersWithGeometry(mdbPath As String) As Dictionary(Of String, String)
        Dim ogrinfo As String = IO.Path.Combine(BinDir, "ogrinfo.exe")
        Dim args As String = $"""{mdbPath}"""
        Dim res = StartProcess(ogrinfo, args)
        If res.exitCode <> 0 Then Throw New Exception("ogrinfo failed: " & res.stdErr)

        Dim layers As New Dictionary(Of String, String)
        Using sr As New StringReader(res.stdOut)
            While True
                Dim line = sr.ReadLine()
                If line Is Nothing Then Exit While

                Dim trimmed = line.Trim()

                ' Skip non-layer lines
                If trimmed.StartsWith("INFO:", StringComparison.OrdinalIgnoreCase) Then Continue While
                If trimmed.StartsWith("Open of", StringComparison.OrdinalIgnoreCase) Then Continue While
                If Not Regex.IsMatch(trimmed, "^\d+:") Then Continue While

                ' Extract layer name and geometry type
                Dim colon = trimmed.IndexOf(":"c)
                Dim name = trimmed.Substring(colon + 1).Trim()
                Dim paren = name.IndexOf(" (")
                Dim geomType As String = "Unknown"
                If paren > 0 Then
                    geomType = name.Substring(paren + 2).Replace(")", "").Trim()
                    name = name.Substring(0, paren)
                End If

                ' Add to dictionary if valid
                If name.Length > 0 Then layers(name) = geomType
            End While
        End Using
        Return layers
    End Function


End Module