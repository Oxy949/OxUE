using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OxUE
{
    public class ResourcesManager : Singleton<ResourcesManager>
    {
        private Dictionary<string, System.Object> _loadedResources = new Dictionary<string, System.Object>() { };

        public static System.Object GetLoadedResource(string key)
        {
            if (Instance._loadedResources.ContainsKey(key))
                return Instance._loadedResources[key]; 
            
            return null;
        }

        void OnDestroy()
        {
            UnloadResources();
        }

        public static void UnloadResources()
        {
            Debug.Log("[ResourcesManager] Unloading data resources");
            //MapsSystem.Instance.UnloadResources();
            Instance._loadedResources.Clear();
            GC.Collect();
            Resources.UnloadUnusedAssets();
        }

        public static void UnloadResource(string key)
        {
            if (Instance._loadedResources.ContainsKey(key))
            {
                Instance._loadedResources.Remove(key);
            }
        }

        public static void FreeMemory()
        {
            GC.Collect();
            Resources.UnloadUnusedAssets();
        }
        
        
        public void LoadSprite(string key, string path, bool isAbsPath = false, bool force = false)
        {
            if (!_loadedResources.ContainsKey(key) || force)
            {
                if (!isAbsPath)
                    path = Helpers.GetProjectDirectory(path);

                //Debug.Log("[ResourcesManager] Loading image from " + path + " as " + key);

                Texture2D tex = null;
                Sprite image = null;
                byte[] fileData;

                if (File.Exists(path))
                {
                    fileData = File.ReadAllBytes(path);
                    tex = new Texture2D(2, 2);
                    tex.LoadImage(fileData);
                    tex.wrapMode = TextureWrapMode.Clamp;
                    tex.filterMode = FilterMode.Bilinear;
                    //image = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                    image = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100F, 0, SpriteMeshType.FullRect);
                }
                else
                {
                    Debug.LogWarning("[ResourcesManager] Load image error: " + path + " not found");
                }

                if (_loadedResources.ContainsKey(key))
                {
                    if (force)
                        _loadedResources[key] = image;
                }
                else
                    _loadedResources.Add(key, image);
            }
        }

        public IEnumerator LoadSpriteFromURL(string key, string url, bool force = false)
        {
            if (!_loadedResources.ContainsKey(key) || force)
            {
                //Debug.Log("[ResourcesManager] Loading image from " + url + " as " + key);

                Sprite image = null;
                Texture2D texture;

                WWW www = new WWW(url);
                yield return www;
                if (string.IsNullOrEmpty(www.error))
                {
                    texture = new Texture2D(2, 2);
                    www.LoadImageIntoTexture(texture);
                    texture.wrapMode = TextureWrapMode.Clamp;
                    texture.filterMode = FilterMode.Bilinear;
                    //image = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
                    image = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100F, 0, SpriteMeshType.FullRect);

                    yield return image;
                }
                else
                    Debug.LogWarning("[ResourcesManager] Load image error: " + www.error);

                if (_loadedResources.ContainsKey(key))
                {
                    if (force)
                        _loadedResources[key] = image;
                }
                else
                    _loadedResources.Add(key, image);
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

                WWW mGET = new WWW("file://" + path);
                yield return mGET;
                if (!string.IsNullOrEmpty(mGET.error))
                    Debug.LogWarning("[ResourcesManager] Load audio error: " + mGET.error);
                else
                {
                    resultAudio = mGET.GetAudioClip(false);
                    yield return resultAudio;
                }

                if (_loadedResources.ContainsKey(key))
                {
                    if (force)
                        _loadedResources[key] = resultAudio;
                }
                else
                    _loadedResources.Add(key, resultAudio);
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
                        _loadedResources[key] = result;
                }
                else
                    _loadedResources.Add(key, result);
            }
        }

        public void LoadTextWithLocalizationFix(string key, string path, bool isAbsPath = false, bool force = false)
        {
            if (!_loadedResources.ContainsKey(key) || force)
            {
                if (!isAbsPath)
                    path = Helpers.GetProjectDirectory(path);

                //Debug.Log("[ResourcesManager] Loading text from " + path);
                string normalPath = new FileInfo(path).FullName;
                string result = "";
                if (File.Exists(normalPath))
                {
                    bool first = true;
                    try
                    {
                        foreach (var line in File.ReadAllLines(normalPath))
                        {
                            if (first)
                            {
                                result += line + ",viewkeys" + System.Environment.NewLine;
                                first = false;
                            }
                            else
                                result += line + "," + System.Environment.NewLine;
                        }
                        File.ReadAllText(normalPath);
                    }catch(Exception e)
                    {
                        Debug.LogError("[ResourcesManager] Load localization file error: " + e.ToString());
                    }
                }
                else
                    Debug.LogWarning("[ResourcesManager] Load text error: not found");


                if (_loadedResources.ContainsKey(key))
                {
                    if (force)
                        _loadedResources[key] = result;
                }
                else
                    _loadedResources.Add(key, result);
            }
        }
    }
}