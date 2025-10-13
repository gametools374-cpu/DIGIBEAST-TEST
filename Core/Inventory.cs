using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {
    public static Inventory Instance { get; private set; }
    private List<string> items = new();

    void Awake() {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    public bool HasItem(string itemName) => items.Contains(itemName);
    public void RemoveItem(string itemName) => items.Remove(itemName);
    public void AddItem(string itemName) => items.Add(itemName);
}
