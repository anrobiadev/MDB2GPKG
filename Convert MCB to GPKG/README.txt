
README: MDB to GPKG Conversion Utility
======================================

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


---------------------------------------Romana---------------------------------

README_RO: Utilitar Conversie MDB în GPKG
=========================================

Acest utilitar permite conversia fișierelor ESRI Personal Geodatabase (*.mdb) în format GeoPackage (*.gpkg), compatibil cu ArcGIS Pro.

Ghid Pas cu Pas
---------------
1. Copiați arhiva local pe laptop.
2. Dezarhivați conținutul într-un folder dedicat.
3. Rulați fișierul executabil (nu necesită instalare sau drepturi de administrator).

Selectarea fișierului MDB:
- Apăsați „Browse MDB” și selectați fișierul .mdb dorit.

Alegerea fișierului GPKG de ieșire:
- Apăsați „Browse GPKG” și specificați locația și numele fișierului de ieșire.

Încărcarea și selectarea straturilor:
- Apăsați „Load Layers” pentru a scana fișierul MDB.
- Bifați straturile pe care doriți să le convertiți.
- Opțional: activați filtrul pentru a exclude straturile care încep cu „T_1_”.

Setări opționale:
- Cod EPSG: introduceți un cod pentru reproiectarea datelor.
- Where Clause: filtrați înregistrările folosind sintaxă SQL.
- Overwrite: bifați pentru a suprascrie un fișier GPKG existent.

Conversia:
- Apăsați „Convert”.
- Monitorizați progresul și jurnalul.
- La final, va apărea un mesaj de confirmare.

Cerințe de Sistem
-----------------
- Sistem de operare: Windows 10 sau mai nou (64-bit)
- .NET Framework: Versiunea 4.7.2 sau mai mare
- RAM: Minim 4 GB (recomandat 8 GB)
- Spațiu pe disc: Minim 500 MB liber
- GDAL Runtime: Inclus în aplicație (trebuie să suporte driverul PGeo)
- Driver ODBC Microsoft Access: Necesare pentru suport MDB

Întrebări Frecvente (FAQ)
--------------------------
Î: Am nevoie de drepturi de administrator?
R: Nu, aplicația rulează fără instalare.

Î: Fișierul MDB nu se încarcă?
R: Verificați dacă fișierul există și dacă GDAL include suport pentru PGeo.

Î: Pot converti mai multe straturi simultan?
R: Da, selectați mai multe straturi înainte de conversie.

Î: Ce se întâmplă dacă nu introduc un cod EPSG?
R: Datele vor fi exportate în sistemul de coordonate original.

Î: De ce lipsesc descrierile în tabelele de ieșire?
R: GeoPackage nu suportă domenii de valori codificate ca MDB.

Î: Pot folosi fișierul GPKG în ArcGIS Pro?
R: Da, este suportat nativ.

Î: Pot converti GPKG înapoi în GDB?
R: Este planificat un utilitar Python pentru conversie directă în GDB standard OMVP.

Î: Cui mă pot adresa pentru suport?
R: Trimiteți un e-mail la electroschite@gmail.com (adresa va fi copiată automat în clipboard dacă clientul de e-mail nu se deschide).