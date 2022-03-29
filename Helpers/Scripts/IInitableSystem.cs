namespace OxUE
{
    /// <summary>
    /// Use it for initialization, when order do matter
    /// </summary>
    public interface IInitableSystem
    {
        /// <summary>
        /// Is system ready?
        /// </summary>
        bool IsInited
        {
            get;
            internal set;
        }

        /// <summary>
        /// System Initialization
        /// </summary>
        void Init();
    }
}
