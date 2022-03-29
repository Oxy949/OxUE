# CursorHandler
An advanced cursor behavior using state precedence.

## How to use:
1. Place prefab Prefabs/CursorHandler.prefab to your in your scene
2. (optional) Change some prefab options using Inspector window
3. Use `CursorHandler.Instance.SetCursorPriority(CursorMode <mode>, int <priority>);` to set the priority and `CursorHandler.Instance.RemoveCursorPriority(int <priority>);` to delete a priority.
    All `CursorMode`'s: `HiddenLocked` (cursor locked and centered at game window), `Normal` (free and visible cursor), `HiddenFree` (cursor is hidden but not locked)

## Example use:
Hide cursor:

    CursorHandler.Instance.SetCursorPriority(CursorHandler.CursorIconMode.HiddenLocked, 10);
To show cursor when it's already hidden by priority 10:

    CursorHandler.Instance.SetCursorPriority(CursorHandler.CursorIconMode.Normal, 20);

Remove hiding on priority 10

    CursorHandler.Instance.RemoveCursorPriority(10);