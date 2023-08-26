using UnityEngine;


public static class MonoSingletonProperty<T> where T : MonoBehaviour, ISingleton
{
    private static T mInstance;

    public static T Instance
    {
        get
        {
            if (null == mInstance)
            {
                mInstance = SingletonCreator.CreateMonoSingleton<T>();
            }

            return mInstance;
        }
    }

    public static void Dispose()
    {
        if (SingletonCreator.IsUnitTestMode)
        {
            UnityEngine.Object.DestroyImmediate(mInstance.gameObject);
        }
        else
        {
            UnityEngine.Object.Destroy(mInstance.gameObject);
        }

        mInstance = null;
    }
}