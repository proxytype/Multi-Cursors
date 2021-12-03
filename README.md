# Multi-Cursors
Quick and dirty example how to draw another cursor using C# and transparent windows form.

![alt text](https://raw.githubusercontent.com/proxytype/Multi-Cursors/main/multicursor.gif)

Create transparent window form and listen to mouse coordinates, draw new mouse cursor by coordinates.

WIN32 Functions:
- mouse_event (user32.dll)
- GetCursorPos (user32.dll)
- GetAsyncKeyState (user32.dll)
