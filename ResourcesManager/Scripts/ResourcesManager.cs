using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.Video;

namespace OxUE
{
    /// <summary>
    /// A tool to load/unload images (png, jpg), audio (ogg, wav) and video (mp4) from any folder.
    /// </summary>
    public class ResourcesManager : Singleton<ResourcesManager>
    {
        private Dictionary<string, System.Object> _loadedResources = new Dictionary<string, System.Object>() { };

        public static T GetLoadedResource<T>(string key)
        {
            if (Instance._loadedResources.ContainsKey(key))
                return (T)Instance._loadedResources[key];

            return default;
        }

        public static void UnloadResource(string key)
        {
            if (Instance._loadedResources.ContainsKey(key))
            {
                Instance._loadedResources.Remove(key);
            }
        }

        public static void UnloadAllResources()
        {
            Debug.Log("[ResourcesManager] Unloading data resources");
            //MapsSystem.Instance.UnloadResources();
            Instance._loadedResources.Clear();
            FreeMemory();
        }

        private static void FreeMemory()
        {
            GC.Collect();
            Resources.UnloadUnusedAssets();
        }

        public void LoadImage(string key, string path, bool isAbsPath = false, bool force = false)
        {
            if (!_loadedResources.ContainsKey(key) || force)
            {
                if (!isAbsPath)
                    path = Helpers.GetProjectDirectory(path);

                //Debug.Log("[ResourcesManager] Loading image from " + path + " as " + key);

                Texture2D tex = null;
                //Sprite image = null;
                byte[] fileData;

                if (File.Exists(path))
                {
                    fileData = File.ReadAllBytes(path);
                    tex = new Texture2D(2, 2);
                    tex.filterMode = FilterMode.Trilinear;
                    tex.wrapMode = TextureWrapMode.Clamp;
                    tex.anisoLevel = 1;
                    tex.LoadImage(fileData);
                    tex.Apply(true);
                }
                else
                {
                    Debug.LogWarning("[ResourcesManager] Load image error: " + path + " not found");
                }

                if (_loadedResources.ContainsKey(key))
                {
                    if (force)
                    {
                        _loadedResources[key] = tex;
                    }
                }
                else
                {
                    _loadedResources.Add(key, tex);
                }
            }
        }

        public IEnumerator LoadImageFromURL(string key, string url, bool force = false)
        {
            if (!_loadedResources.ContainsKey(key) || (_loadedResources.ContainsKey(key) && _loadedResources[key] == null) || force)
            {
                //Debug.Log("[ResourcesManager] Loading image from " + url + " as " + key);

                Texture2D image = null;

                using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
                {
                    // Request and wait for the desired page.
                    yield return webRequest.SendWebRequest();
                    if (webRequest.result == UnityWebRequest.Result.Success)
                    {
                        var textureSource = DownloadHandlerTexture.GetContent(webRequest);
                        image = textureSource;
                        image.filterMode = FilterMode.Trilinear;
                        image.wrapMode = TextureWrapMode.Clamp;
                        image.anisoLevel = 1;
                        image.Apply(true);
                    }
                    else
                        Debug.LogWarning("[ResourcesManager] Load image error: " + webRequest.error);
                }

                if (_loadedResources.ContainsKey(key))
                {
                    if (force)
                    {
                        _loadedResources[key] = image;
                    }
                }
                else
                {
                    _loadedResources.Add(key, image);
                }
            }
        }

        public IEnumerator LoadAudio(string key, string path, bool isAbsPath = false, bool force = false)
        {
            if (!_loadedResources.ContainsKey(key) || force)
            {
                if (!isAbsPath)
                    path = Helpers.GetProjectDirectory(path);

                //Debug.Log("[ResourcesManager] Loading audio from " + path + " as " + key);
                AudioClip resultAudio = null;

                using (UnityWebRequest webRequest = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.UNKNOWN))
                {
                    // Request and wait for the desired page.
                    yield return webRequest.SendWebRequest();
                    if (webRequest.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogWarning("[ResourcesManager] Load audio error: " + webRequest.error);
                    }
                    else
                    {
                        resultAudio = DownloadHandlerAudioClip.GetContent(webRequest);
                    }
                }

                if (_loadedResources.ContainsKey(key))
                {
                    if (force)
                    {
                        _loadedResources[key] = resultAudio;
                    }
                }
                else
                {
                    _loadedResources.Add(key, resultAudio);
                }
            }
        }

        public IEnumerator LoadVideo(string key, string path, bool isAbsPath = false, bool force = false)
        {
            if (!_loadedResources.ContainsKey(key) || force)
            {
                if (!isAbsPath)
                    path = Helpers.GetProjectDirectory(path);

                Debug.Log("[ResourcesManager] Loading video from " + path + " as " + key);
                string resultVideoPath = "file://" + path;


                // Will attach a VideoPlayer to the main camera.
                Transform res = Instance.transform;

                // VideoPlayer automatically targets the camera backplane when it is added
                // to a camera object, no need to change videoPlayer.targetCamera.
                var obj = new GameObject(key);
                obj.transform.SetParent(res);
                var videoPlayer = obj.AddComponent<VideoPlayer>();

                // Play on awake defaults to true. Set it to false to avoid the url set
                // below to auto-start playback since we're in Start().
                videoPlayer.playOnAwake = false;

                // By default, VideoPlayers added to a camera will use the far plane.
                // Let's target the near plane instead.
                videoPlayer.renderMode = VideoRenderMode.APIOnly;

                videoPlayer.audioOutputMode = VideoAudioOutputMode.None;

                // This will cause our Scene to be visible through the video being played.
                videoPlayer.targetCameraAlpha = 0.5F;

                // Set the video to play. URL supports local absolute or relative paths.
                // Here, using absolute.
                videoPlayer.url = resultVideoPath;

                // Skip the first 100 frames.
                videoPlayer.frame = 0;

                // Restart from beginning when done.
                videoPlayer.isLooping = false;

                // Start playback. This means the VideoPlayer may have to prepare (reserve
                // resources, pre-load a few frames, etc.). To better control the delays
                // associated with this preparation one can use videoPlayer.Prepare() along with
                // its prepareCompleted event.
                videoPlayer.Prepare();

                yield return new WaitUntil(() => videoPlayer.isPrepared);

                if (_loadedResources.ContainsKey(key))
                {
                    if (force)
                    {
                        _loadedResources[key] = videoPlayer;
                    }
                }
                else
                {
                    _loadedResources.Add(key, videoPlayer);
                }
            }
        }

        public void UnloadVideo(string resourceID)
        {
            if (_loadedResources.ContainsKey(resourceID))
            {
                if (GetLoadedResource<VideoPlayer>(resourceID))
                    Destroy(GetLoadedResource<VideoPlayer>(resourceID).gameObject);
            }
        }

        public void LoadText(string key, string path, bool isAbsPath = false, bool force = false)
        {
            if (!_loadedResources.ContainsKey(key) || force)
            {
                if (!isAbsPath)
                    path = Helpers.GetProjectDirectory(path);

                //Debug.Log("[ResourcesManager] Loading text from " + path);
                string normalPath = new FileInfo(path).FullName;
                string result = "";
                if (File.Exists(normalPath))
                    result = File.ReadAllText(normalPath);
                else
                    Debug.LogWarning("[ResourcesManager] Load text error: not found");


                if (_loadedResources.ContainsKey(key))
                {
                    if (force)
                    {
                        _loadedResources[key] = result;
                    }
                }
                else
                {
                    _loadedResources.Add(key, result);
                }
            }
        }

        private void OnDestroy()
        {
            UnloadAllResources();
        }
    }
}