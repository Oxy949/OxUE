# CursorHandler
An advanced cursor behavior using state precedence.

## How to use:
1. Place prefab Prefabs/CursorHandler.prefab to your in your scene
2. (optional) Change some prefab options using Inspector window
3. Use `CursorHandler.Instance.SetCursorPriority(<CursorMode>, <priority>);` to set the priority of the `<CursorMode>` and `CursorHandler.Instance.RemoveCursorPriority(<priority>);` to delete the setted priority.
	All `<CursorMode>`: HiddenLocked (cursor locked and centered at game window), Normal (free and visible cursor), HiddenFree (cursor is hidden but not locked)