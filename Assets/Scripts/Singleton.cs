using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton script that can be applied to any other Monobehaviour class.
/// </summary>
/// <typeparam name="T"></typeparam>

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public bool isPersistent = true;
    private static T instance;
    public static T Instance
    {
        get { return instance; }
    }

    public static bool IsInitialized
    {
        get { return instance != null; }
    }

    protected virtual void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("[Singleton] Trying to instantiate a second instace of a singleton class " + instance.name);
            Destroy(this.gameObject);
        }
        else
        {
            instance = (T)this;
            if (isPersistent)
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
