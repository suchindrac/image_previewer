# Image Previewer for Inverted desktops

## Summary:

This is a small image previewer which designed to be helpful to view images in 
 inverted desktops. Following options are available in the app as of now: 

* Resize images
* Invert images
* Make images transparent

## Compilation:

Please execute the following commands:

```
C:\ImagePreviewer> compile.bat
```

## Execution:

The executable named imagePreviewer.exe, and a copy of the executable named 
 ipv.exe will be generated in the same folder.Please execute it as follows:

```
C:\ImagePreviewer> ipv.exe
Invalid inputs supplied
Syntax: ipv.exe [image path] [Size in WxH Format | Full] [[Invert | NoInvert]] 
 [[ Trans [Opacity] ]]
Press a key to continue...
C:\ImagePreviewer>
```

An example of using this tool for creating inverted previews of images 
 (suitable to view in inverted desktop), is below:

```
C:\ImagePreviewer> ipv.exe test.jpg 200x200 Invert Trans 0.5
```							

The above command can also be put in a batch script in **C:\Windows\System32**
 folder, say ipv.bat. User can then configure ipv.bat as the default image 
 viewer. This will ensure that the image is displayed as 200x200 thumbnail 
 with 50% transparency, and as inverted, upon double clicking on it. 

Once the image is shown, following are the keys which are linked to the image:

* i : Invert image
* + : Make image more transparent
* - : Make image less transparent
* Esc : Close the image
* s : Save the full sized image
