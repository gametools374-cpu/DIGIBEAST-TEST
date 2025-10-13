using UnityEngine;
using AK.Wwise;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance { get; private set; }
    // ...event and RTPC fields...
    void Awake() {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }
    // ...event methods...
}
