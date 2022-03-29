# LoadingUI
An async loading screen with animations and custom text.

## How to use:
1. Place prefab Prefabs/LoadingUI.prefab to your in your scene
2. (optional) Change some prefab options using Inspector window
3. Use `LoadingUI.Instance.Show()` to show UI and `LoadingUI.Instance.Hide()` to hide it wherever you want.

Additionally you can use C# Actions to do things when UI becomes ready:

	LoadingUI.Instance.Show(() =>
	{
		Debug.Log("Ready!");
	});

Use `LoadingUI.Instance.SetLoadingText(<string>)` to update UI text while it's active.