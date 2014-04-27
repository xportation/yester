InkscapeBatch - Inkscape SVG batch file converter - by Tokaware
=============================================================== 


Table of contents
=================

1. Overview
2. License
3. Path and Files
4. Documentation
5. Getting started
6. Howto read command syntax
7. Command list


1. Overview
===========

InkscapeBatch gives you a fast, easy and flexible way to convert 
Inkscape SVG files into other file formats such as PNG or EPS. 
You can define batch projects with steps such as group Inkscape 
objects, loops, arrays and much, much more...

Start InkscapeBatch with parameter "-t" to create a test/example 
project.


2. License
==========

Copyright (C) 2009-2012 Tokaware.org
http://www.tokaware.org

InkscapeBatch is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 2 of the License, or
(at your option) any later version.
 
InkscapeBatch is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.
 
You should have received a copy of the GNU General Public License
along with MediaPortal. If not, see <http://www.gnu.org/licenses/>.


3. Path and Files
=================

Default instaltion directory
----------------------------
C:\Program Files\Tokaware\InkscapeBatch
Only the license text and the binary is stored in the path.

Default project directory
-------------------------
Environment.SpecialFolder.Personal\InkscapeBatch
e.g. C:\Users\<YourUser>\Documents\InkscapeBatch
Dynamic data such as projetcs (e.g. tests) are stored in this path.


4. Documentation
================

- s. http://sourceforge.net/apps/mediawiki/inkscapebatch/index.php?title=Main_Page


5. Getting started
==================

- Download and install the binary version of InkscapeBatch.
- Start "Run Test" from Windows start menu (see InkscapeBatch entry).
- Look at C:\Users\<YourUser>\Documents\InkscapeBatch for results.


6. Howto read command syntax
============================

literals
--------
Words that contains only "normal" literals must be written as 
shown in the syntax description.
e.g. "ExportBackground" means "ExportBackground"
e.g. "canvas" means "canvas"
e.g. "only" means "only"

Quotes
------
Quote names (e.g. filename, path, variable, objects) that 
conatins blanks with " or '
e.g. FileSvg "my example 1.svg"

"|" symbol
----------
This symbol means "or". Select one of the options separated 
by "|". 
e.g. "FileFormat png|eps|ps|pdf|svg" means "FileFormat pdf"  

":" symbol
----------
This symbol separate different values used by one command.
e.g. "ExportArea [<x0:y0:x1:y1>|canvas|drawing]" means 
"ExportArea 10:10:500:640"

<something>
-----------
variable text that you have to write.
e.g. "FileName <name>" means "FileName temp\Image1.svg"

[something]
-----------
Text that you should write if you want or need it (optional).
e.g. "ExportDpi [<dpi>]" means "ExportDpi" or "ExportDpi 90"


7. Command list
===============

File and path commands:
-----------------------
FileFormat png|eps|ps|pdf|svg 
FileIbp <filename> [async]
FileName <name>|ExportId 
FilePrefix [<prefix>] 
FileSvg [<filename>]
PathClear true|false
PathExport <path>

Export comannds:
----------------
ExportArea [<x0:y0:x1:y1>|canvas|drawing] [snap]
ExportBackground [<color>]
ExportBackgroundOpacity [<number>]
ExportDpi [<dpi>]
ExportId [<id>|<varName>] [only]
ExportHeight [<px>]
ExportWidth [<px>]

Marko commands:
---------------
Array <name> <element1:element2:...>
Convert
Group <name> <object1:object2:objectN>
Loop <cnt>|<ArrayName> <numRows>
Replace <search> <replace>
Select [<objectId>]
Verb [<verbId>]

Loop index:
-----------
Index

Var access:
-----------
<name>

Array access:
-------------
<name>:<element>|<name>:Index