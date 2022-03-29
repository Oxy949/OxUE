# ResourcesManager
A tool to load/unload images (png, jpg), audio (ogg, wav) and video (mp4) from any folder.

## How to use:
## Texture2D
1. Load:


    ResourcesManager.Instance.LoadImage(resID, fullFilePath, true);

2. Use in game:


    var texture = ResourcesManager.GetLoadedResource<Texture2D>(resID);

## AudioClip
1. Load (in Coroutine):


    yield return StartCoroutine(ResourcesManager.Instance.LoadAudio(resID, fullFilePath, true));

2. Use in game:


    var audioClip = ResourcesManager.GetLoadedResource<AudioClip>(resID);

## Video
1. Load (in Coroutine):


    yield return StartCoroutine(ResourcesManager.Instance.LoadVideo(resID, fullFilePath, true));

2. Use in game (in Coroutine):


    var videoPlayer = ResourcesManager.GetLoadedResource<VideoPlayer>(resID);

    videoPlayer.time = 0;

    videoPlayer.Prepare();
    yield return new WaitUntil(() => videoPlayer.isPrepared);

    videoPlayer.Play();