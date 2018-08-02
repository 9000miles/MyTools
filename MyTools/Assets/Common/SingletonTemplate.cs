using System.Reflection;

public class SingletonTemplate<T> where T : class, new()
{
    protected static T singleton = null;

    public static T Singleton
    {
        get
        {
            if (singleton == null)
            {
                singleton = new T();
            }
            return singleton;
        }
    }

    public void OnDestroy()
    {
        // Clear();
        singleton = null;
    }

    public virtual void Clear()
    {
        singleton = null;
    }
}