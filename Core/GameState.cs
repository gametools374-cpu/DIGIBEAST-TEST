using UnityEngine;

public class GameState : MonoBehaviour {
    public static GameState Instance { get; private set; }
    public int currentFloor;
    public bool battleActive;
    public bool tradeMode;

    void Awake() {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }
}
