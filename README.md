# Presto - A VR Application to Practice Presentations

Presto is a virtual reality application for SteamVR that lets you practice presentations in virtual environments. Its goal is to let you use tools that help you improve your presentations and prepare for the real deal. Presto reads normal PDF files so need to do anything extra.

## How to install

A binary file is provided, but you can also choose to build the Unity scene yourself. In order for Presto to open PDF files, you need to install GhostScript: https://www.ghostscript.com/download/

There are some additional DLLs that are used by Presto, such as System.Drawing and Magick.Net. I have no idea if the supplied System.Drawing will work with other computers so you might have to replace it with your own. It can be found here: C:\Windows\Microsoft.NET\Framework\v2.0.50727\

With these set up, Presto should build just fine. Let me know if you encounter any issues. 

## How to use

Presto will start with a main menu scene where you can select a scene to load up. It will open up a scene if you point and press the trigger. It will load up the selected scene and you will be placed in the environment. Each scene has a certain amount of screens in the scene which the PDF slides will be displayed on. 

A file can be selected by opening up the file browser window with the menu button. The menu also contains a few buttons such as The home button that takes you back to the main menu and a settings button that lets you, for example, show a timer and reverse its counting direction. A timer is started by pointing and pressing the trigger. Time can be added or removed from the timer by pointing at it and pressing the left and right sides of the touch pad. These same controls are used to change slides of PDFs. Finally, by pointing at a screen and holding a trigger down a laser pointer will be visible. 