# scheduled-shutdown

This program can be run as a background process in Windows to automatically shutdown the PC at scheduled times throughout the week.

# How to use

1. Build the source in Visual Studio as a Windows Application.
2. Go to the build directory and edit the `scheduled-shutdown.exe.config` file to schedule at what times the PC should be shutdown. Multiple times can be entered for a day by seperating the times with a comma.
3. Create a shortcut to the `scheduled-shutdown.exe` executable and place the shortcut in the Windows startup folder.

Now whenever Windows is launched the `scheduled-shutdown.exe` application will run in the background.
