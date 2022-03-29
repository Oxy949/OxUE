using UnityEngine;
using UnityEngine.Serialization;

namespace OxUE
{
    /// <summary>
    /// Singleton class. Inherit by passing the inherited type (e.g. class GameManager : Singleton&lt;GameManager&gt;)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        [SerializeField] private bool dontDestroyOnLoad = true;

        #region Fields

        /// <summary>
        /// The instance.
        /// </summary>
        private static T _instance;

        private static bool _applicationIsQuitting = false;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null && !_applicationIsQuitting)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        _instance = obj.AddComponent<T>();
                        Debug.LogWarning("[Singleton <" + typeof(T) + ">] Singleton of type doesn't exists in scene! Created " + obj.name);
                    }
                }

                return _instance;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Use this for initialization. Don't override it completely!
        /// </summary>
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                if (dontDestroyOnLoad)
                    DontDestroyOnLoad(gameObject);
            }
            else
            {
                Debug.LogWarning("[Singleton <" + typeof(T) + ">] Singleton already exists in scene as " + _instance.name + "! Destroying " + gameObject.name);
                Destroy(gameObject);
            }
        }

        public virtual void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
        }

        #endregion
    }
}