// ManagerBase.cs
// Singleton base for every manager in the game.
using UnityEngine;

public abstract class ManagerBase<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<T>();
            return _instance;
        }
    }
    protected virtual void Awake()
    {
        if (_instance == null) { _instance = this as T; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }
}