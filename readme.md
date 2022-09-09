# Description

Basic notes application, you can add multiple notes and minimize the application to the system tray.

-   Clicking on the window's 'X' button hides the window (instead of closing it).
-   Right clicking on the main menu, brings a menu with some options.
-   Double-click on the system tray icon to show/hide the application.
-   To close the application, you need to right click on the system tray icon, and click on the 'close' button.
-   To have the application start on windows startup, just add a shortcut to the 'Startup' folder (start -> all programs -> find 'Startup' folder).
-   The application data will be stored in the default application data folder of the OS (for example, `C:/Users/username/AppData/Local/notes_app/`).

![example](assets/example.png)

# Keyboard Shortcuts

-   `ctrl + n` : New note.
-   `ctrl + r` : Remove the current note (asks for confirmation).
-   `ctrl + q` : Show the previous note.
-   `ctrl + w` : Show the next note.
-   `ctrl + o` : Open the options window.
-   `ctrl + [1-9]` : Show the note in that position (for example, `ctrl + 1` shows the first note, `ctrl + 2` shows the second, etc).
-   `Escape` : Hide/close the current active window (the notes window is hidden only if the `minimize on close` option is set).

# Development

## Code format

-   `dotnet tool run dotnet-csharpier . --check`

## Build

After building the application, copy the following files to a folder (from `/bin/Release/`).

-   assets/\*
-   \*.dll
-   notes_app.exe
-   notes_app.exe.config
