# MDB2GPKG
This utility allows you to convert ESRI Personal Geodatabase files (*.mdb) into GeoPackage (*.gpkg) format, compatible with ArcGIS Pro.
Step-by-Step Guide
------------------

1. Copy the archive locally to your laptop.
2. Extract the contents into a dedicated folder.
3. Run the executable file (no installation or admin rights required).

Select the MDB File:
- Click "Browse MDB" and choose your .mdb file.

Choose the Output GPKG File:
- Click "Browse GPKG" and specify the output location and filename.

Load and Select Layers:
- Click "Load Layers" to scan the MDB.
- Check the layers you want to convert.
- Optional: enable the filter to exclude layers starting with "T_1_".

Optional Settings:
- EPSG Code: enter a code to reproject the data.
- Where Clause: filter records using SQL-like syntax.
- Overwrite: check to replace an existing GPKG file.

Start the Conversion:
- Click "Convert".
- Monitor progress and logs.
- A confirmation message will appear when done.

System Requirements
-------------------
- OS: Windows 10 or later (64-bit)
- .NET Framework: 4.7.2 or higher
- RAM: Minimum 4 GB (8 GB recommended)
- Disk Space: 500 MB free
- GDAL Runtime: Included with app (must support PGeo driver)
- Microsoft Access ODBC Driver: Required for MDB support

FAQ
---
Q1: Do I need admin rights?
A: No, the app runs without installation.

Q2: MDB file doesn’t load?
A: Ensure the file exists and GDAL supports PGeo.

Q3: Can I convert multiple layers?
A: Yes, select multiple layers before converting.

Q4: What if I skip EPSG code?
A: Data will use its original coordinate system.

Q5: Why are descriptions missing?
A: GeoPackage does not support coded value domains.

Q6: Can I use GPKG in ArcGIS Pro?
A: Yes, it is natively supported.

Q7: Can I convert GPKG back to GDB?
A: A Python utility is planned for this.

Q8: Who do I contact for help?
A: Email electroschite@gmail.com (copied to clipboard if email client fails).
