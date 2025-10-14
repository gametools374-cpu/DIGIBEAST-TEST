using System;
using System.Data.SQLite;
using UnityEngine;

public class SQLiteDB : MonoBehaviour
{
    public static SQLiteDB Instance { get; private set; }
    private string dbPath;
    private SQLiteConnection connection;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }

        dbPath = Application.persistentDataPath + "/digibeast_empire.db";
        connection = new SQLiteConnection($"URI=file:{dbPath}");
        connection.Open();
        CreateTables();
    }

    private void CreateTables()
    {
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Players (
                    id INTEGER PRIMARY KEY,
                    progress_json TEXT
                );
                CREATE TABLE IF NOT EXISTS Digibeasts (
                    id INTEGER PRIMARY KEY,
                    player_id INTEGER,
                    line TEXT,
                    form INTEGER,
                    level INTEGER,
                    ivs_json TEXT,
                    evs_json TEXT,
                    nature_json TEXT,
                    stats_json TEXT,
                    pp_json TEXT
                );
                CREATE TABLE IF NOT EXISTS Inventory (
                    id INTEGER PRIMARY KEY,
                    player_id INTEGER,
                    items_json TEXT
                );
                CREATE TABLE IF NOT EXISTS Relics (
                    id INTEGER PRIMARY KEY,
                    player_id INTEGER,
                    relics_json TEXT
                );";
            cmd.ExecuteNonQuery();
        }
    }

    public void SavePlayerProgress(int playerId, string progressJson)
    {
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "INSERT OR REPLACE INTO Players (id, progress_json) VALUES (@id, @json)";
            cmd.Parameters.AddWithValue("@id", playerId);
            cmd.Parameters.AddWithValue("@json", progressJson);
            cmd.ExecuteNonQuery();
        }
    }

    public string LoadPlayerProgress(int playerId)
    {
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "SELECT progress_json FROM Players WHERE id = @id";
            cmd.Parameters.AddWithValue("@id", playerId);
            using (var reader = cmd.ExecuteReader())
            {
                return reader.Read() ? reader.GetString(0) : null;
            }
        }
    }

    // Similar methods for Digibeasts, Inventory, Relics (use JSON serialization)
    // e.g., SaveDigibeast: INSERT OR REPLACE INTO Digibeasts (id, player_id, line, ...) VALUES (...)

    void OnApplicationQuit()
    {
        connection.Close();
    }
}





using UnityEngine;
using System.Threading.Tasks;
using Newtonsoft.Json; // Install via NuGet for better JSON handling (dictionaries, etc.)
using System.Collections.Generic;
using System; // For Guid

public class DataSyncManager : MonoBehaviour
{
    public static DataSyncManager Instance { get; private set; }

    [SerializeField] private bool autoSyncOnStart = true;
    [SerializeField] private float syncInterval = 60f; // Sync every 60s if online

    private bool isOnline = false; // Stub: Check internet (use Unity's Application.internetReachability)
    private float lastSyncTime;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }

        // Initialize DBs if not already (assume SQLiteDB and SupabaseDB are singletons)
        _ = SQLiteDB.Instance; // Force init
        _ = SupabaseDB.Instance; // Force init

        if (autoSyncOnStart) SyncData();
    }

    void Update()
    {
        // Periodic sync if online
        if (Time.time - lastSyncTime > syncInterval && isOnline)
        {
            SyncData();
        }

        // Stub: Detect online status
        isOnline = Application.internetReachability != NetworkReachability.NotReachable;
    }

    public async void SyncData()
    {
        if (!isOnline)
        {
            Debug.Log("Offline: Using local SQLite only.");
            LoadFromLocal();
            return;
        }

        Debug.Log("Syncing data...");
        lastSyncTime = Time.time;

        try
        {
            // Step 1: Get local and cloud versions (use timestamps or hashes for diffs)
            string localProgress = SQLiteDB.Instance.LoadPlayerProgress(GetPlayerId());
            string cloudProgress = await SupabaseDB.Instance.LoadPlayerProgress(GetPlayerId());

            // Step 2: Resolve conflicts (simple: cloud wins if newer; add timestamps later)
            if (string.IsNullOrEmpty(cloudProgress) || IsLocalNewer(localProgress, cloudProgress))
            {
                // Push local to cloud
                await SupabaseDB.Instance.SavePlayerProgress(GetPlayerId(), localProgress);
                await SyncDigibeastsToCloud();
                await SyncInventoryToCloud();
                await SyncRelicsToCloud();
            }
            else
            {
                // Pull cloud to local
                SQLiteDB.Instance.SavePlayerProgress(GetPlayerId(), cloudProgress);
                await SyncDigibeastsFromCloud();
                await SyncInventoryFromCloud();
                await SyncRelicsFromCloud();
            }

            // Step 3: Load synced data into game state
            LoadFromLocal(); // Or directly into GameState

            Debug.Log("Sync complete.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Sync failed: {e.Message}. Falling back to local.");
            LoadFromLocal();
        }
    }

    private bool IsLocalNewer(string localJson, string cloudJson)
    {
        // Stub: Add 'last_updated' timestamp to JSON and compare
        // For now, assume local is newer if cloud empty
        return string.IsNullOrEmpty(cloudJson);
    }

    private async Task SyncDigibeastsToCloud()
    {
        // Get all local Digibeasts
        // Stub: Assume SQLiteDB has GetAllDigibeasts(int playerId) -> List<DigibeastInstance>
        List<DigibeastInstance> localDigibeasts = SQLiteDB.Instance.GetAllDigibeasts(GetPlayerId());
        foreach (var digibeast in localDigibeasts)
        {
            string json = JsonConvert.SerializeObject(digibeast);
            // Save to cloud: await SupabaseDB.Instance.SaveDigibeast(GetPlayerId(), digibeast.id, json);
        }
    }

    private async Task SyncDigibeastsFromCloud()
    {
        // Get all cloud Digibeasts
        // Stub: List<Dictionary<string, object>> cloudData = await SupabaseDB.Instance.GetAllDigibeasts(GetPlayerId());
        // foreach: DigibeastInstance digibeast = JsonConvert.DeserializeObject<DigibeastInstance>(cloudData["json"]);
        // SQLiteDB.Instance.SaveDigibeast(GetPlayerId(), digibeast);
    }

    // Similar async methods for Inventory and Relics
    // e.g., SyncInventoryToCloud: Serialize List<Item>, push to cloud

    private void LoadFromLocal()
    {
        // Load all from SQLite into GameState/DigibeastManager/Inventory
        string progressJson = SQLiteDB.Instance.LoadPlayerProgress(GetPlayerId());
        if (!string.IsNullOrEmpty(progressJson))
        {
            // JsonUtility.FromJsonOverwrite(progressJson, GameState.Instance);
            Debug.Log("Loaded from local.");
        }
        // Load Digibeasts, Inventory, Relics similarly
    }

    private string GetPlayerId()
    {
        // Stub: Use Steam ID or Guid
        return PlayerPrefs.GetString("PlayerId", Guid.NewGuid().ToString());
    }
}