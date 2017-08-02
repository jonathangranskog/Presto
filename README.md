# Presto - A VR Application to Practice Presentations

Presto is a virtual reality application for SteamVR that lets you practice presentations in virtual environments. Its goal is to let you use tools that help you improve your presentations and prepare for the real deal. Presto reads normal PDF files so no need to do anything extra.

## How to install

A binary file is provided, but you can also choose to build the Unity scene yourself. In order for Presto to open PDF files, you need to install GhostScript: https://www.ghostscript.com/download/

There are some additional DLLs that are used by Presto, such as System.Drawing and Magick.Net. I have no idea if the supplied System.Drawing will work with other computers so you might have to replace it with your own. It can be found here: C:\Windows\Microsoft.NET\Framework\v2.0.50727\

With these set up, Presto should build just fine in Unity 5.5.3f1, but please let me know if you encounter any issues with anything. 

## How to use

Presto will start with a main menu scene where you can select a scene to load up. It will open up a scene if you point and press the trigger. It will load up the selected scene and you will be placed in the environment. Each scene has a certain amount of screens in the scene which the PDF slides will be displayed on. 

A file can be selected by opening up the file browser window with the menu button. The menu also contains a few buttons such as The home button that takes you back to the main menu and a settings button that lets you, for example, show a timer and reverse its counting direction. A timer is started by pointing and pressing the trigger. Time can be added or removed from the timer by pointing at it and pressing the left and right sides of the touch pad. These same controls are used to change slides of PDFs. Finally, by pointing at a screen and holding a trigger down a laser pointer will be visible. 

## How to add your own scenes

A new scene is created by adding in all the necessary prefabs, scripts and connections to a new unity scene. The steps that need to be taken are:

* Bring in the geometry that will be used for the scene
* Add a SteamVR Camera from the SteamVR Plugin and add "SteamVR Controller Actions", "SteamVR_TrackedController" and "SceneActionManager" scripts to each controller (Controller(left) and Controller(right))
* Place some amount of PageScreen prefabs in the environment. These will be used to place slides onto. Remember to rotate them such that the Z-axis points away from the Camera. 
* Add a PageManager and a PDFConverter to the scene. The PageManager needs a reference to the PDFConverter object and the PDFConverter needs a reference to the PageManager object. In addition, increase the size of the Screens parameter on the PageManager script and reference your PageScreen scripts
* To get the UI working, Menu and Timer prefabs need to be brought in. The Menu also needs a reference to the PageManager
* The Menu also needs a connection to the Timer object's functions on the toggles inside the SettingsMenu. Open the SettingsMenu object and click on the TimerToggle and move to the OnValueChanged field where you need to add TimerCounter.Show as a dynamic bool (top of the drop down menu). Then do the same for ReverseTimerToggle but with the function called SetReversed. Both functions are accessible from the Timer Counter script on the timer.
* Afterwards, move your Timer to where you want it. Also, make sure that its oriented the right way. (Z-axis away from Camera)
* Finally, go back to your Controller objects and connect everything to the SceneActionManager. The CursorMaterial is found within the Materials folder.
* In order to see your scene in the mainmenu scene, you need to open up the mainmenu and add the scene to the MainMenuManager script on the SceneMenu object. A thumbnail can be added by importing an image as a 2D Sprite and adding it to the Sprite field. The Scene Index value can be checked from the Build window. Opening scenes within the Unity Editor will not work, so build everything to test if it works. 
* Hopefully, everything works as expected. 

## Ideas for Improvement / Features to Implement

* More scenes (Meeting Room and Stage)
* Ability to enable 'Annoying Audience' from SettingsMenu, which plays random audio clips such as coughing, phone ringing in various places in the environment
* Multi-thread PDF conversion (PDFConverter)
* Upgrade to 2017.1
* Make a normal Presto setup in an empty scene or into a single prefab that can be imported into a new scene instantly