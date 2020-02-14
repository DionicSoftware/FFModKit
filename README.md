![Modding](https://forum.foundersfortune.com/uploads/default/original/1X/e34496d98c2801f79a8f93f21eaaaf11c7a3cc82.jpeg)

# FFModKit
This Mod Kit is a collection of tools to make modding Founders' Fortune easier.

Below you can find instruction how to install various parts of the ModKit. Please refer to the [tutorials on the forum](https://forum.foundersfortune.com/t/introduction-to-modding/928) to learn how to actually use these tools.

Modding can require some time to get used to all the tools. If you feel like some tutorials are inadequate, feel free to ask questions or suggest improvements on the Forum or on the Discord Server!

## How to set up FFModKitUnity

FFModKitUnity is necessary to make 3D assets for the game and includes various tools for you to make them.

1. Download the ModKit
2. Download and install [Unity version 2019.3.0f6](https://unity3d.com/get-unity/download/archive)
3. Launch Unity and open the FFModKitUnity folder to launch the project
4. You should be ready to use the tools described in the forum tutorial!

## How to set up automatic icon processing

Founders' Fortune icons for furniture and equipment exist in 512x512 and 128x128 sizes with a white drop shadow.

Before you go trough the effort of setting up automatic icon progressing, think about if you actually need it. Often times, it's easier to simply apply a white drop shadow in gimp yourself. Automatic icon processing is useful if you're adding a lot of items or have to update them often.
If you do want to set up automatic icon processing follow these steps:

1. Download and install the pyhton 3 programming language
2. Download and install [Image Magick](https://imagemagick.org/script/download.php)
3. Open Icons/applyWhiteShadow.py in the ModKit and set `magickInstall` to the location of your image magick install location
4. Double clicking on applyWhiteShadow should now start a script that uses Image Magick to apply white drop shadows to all images in PhotoStudioIconsMedium and PhotoStudioIconsBig

## How to compile ExampleScriptMod

ExampleScriptMod is a basic mod that uses C# to change the game's behaviour.

1. Download and install Visual Studio Community 2019
2. Open the project by double clicking on `ExampleScriptMod.sln` in the ExampleScriptMod folder
3. Next, we should set up library references. I included some .dll files, but they might be out of date when you download the FFModKit. You can get the latest dll files directly from the game:
   - Go to your game install directory. Usually it's somewhere under `C:\Program Files (x86)\Steam\steamapps\common\Founders' Fortune`
   -  Go to `Founders Fortune_Data\Managed`. These are all the dlls you might need.
   - Move the dll files you need over to the lib folder in `ExampleScriptMod`. For the example mod it's enough to just replace the ones that are already in lib.
   - In visual studio, make sure all references are pointing to the correct dlls. You can manage them in the solution explorer under `ExampleScriptMod -> Dependencies -> Assemblies`

4. Have a look at the code and try to understand what it's doing. Also check the script mod tutorial.
5. In the menu, click on `Build -> Build Solution`
6. A new dll file has been compiled and moved into `ExampleScriptMod\ExampleScript\bin\Debug\netstandard2.0\ExampleScriptMod.dll`.
7. Copy only `ExampleScriptMod.dll` into your mod directory. Founders' Fortune will now load the dll, look for any classes that inherit from `Mod` and start them with the game.
8. Start Founders' Fortune and see if the mod displays messages each time you click on something.
