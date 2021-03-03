namespace OxUE
{
    public interface IInitableSystem
    {
        bool IsInited
        {
            get;
            set;
        }

        void Init();
    }
}
