csc /target:winexe /win32icon:imagePreviewer.ico /out:imagePreviewer.exe imagePreviewer.cs /reference:Microsoft.VisualBasic.dll
del ipv.exe
copy imagePreviewer.exe ipv.exe