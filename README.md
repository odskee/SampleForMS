# The Sample
This project is a sample for Microsft.

It repdocues the duplicate file name error shown by VS when an additional project is included with a MAUI project.

## How to Reproduce
1) Download both projects in this repo.
2) Open DCSMCT and run the DCSMCT.slnx file.
3) Build the solution - observe the following error

```
Severity	Code	Description	Project	File	Line	Suppression State
Error (active)		One or more duplicate file names were detected.  All image output filenames must be unique:
appicon (...\DCSMCT\Resources\AppIcon\appicon.svg)	DCSMCT.D
....nuget\packages\microsoft.maui.resizetizer\10.0.41\buildTransitive\Microsoft.Maui.Resizetizer.After.targets 633	
``` 
