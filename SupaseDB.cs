using Supabase;
using Supabase.Gotrue;
using UnityEngine;
using System.Threading.Tasks;

public class SupabaseDB : MonoBehaviour
{
    public static SupabaseDB Instance { get; private set; }
    private Client client;
    private string supabaseUrl = "https://your-project.supabase.co"; // Replace with your URL
    private string supabaseKey = "sb_publishable_lJlwRHD6YKfOYR4zYV5PnA_6LNByem0"; // Your publishable key

    async void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }

        var options = new ClientOptions { AutoConnectRealtime = true };
        client = new Client(supabaseUrl, supabaseKey, options);
        await client.InitializeAsync();
        await Authenticate(); // Stub: Use email/anon auth
    }

    private async Task Authenticate()
    {
        // Anon auth for now; add email/password later
        var session = await client.Auth.SignInAnonymously();
        if (session == null) Debug.LogError("Supabase auth failed");
    }

    public async Task SavePlayerProgress(string playerId, string progressJson)
    {
        var data = new { id = playerId, progress_json = progressJson };
        await client.From("players").Upsert(data);
    }

    public async Task<string> LoadPlayerProgress(string playerId)
    {
        var response = await client.From("players").Where("id", playerId).Single();
        return response?["progress_json"].ToString();
    }

    // Similar async methods for Digibees, Inventory, Relics (use JSONB fields)
    // e.g., SaveDigibee: client.From("digibees").Upsert({ id, player_id, line, ... })

    // For Trades: Realtime subscribe for updates
    public void SubscribeToTrades(System.Action<TradeUpdate> onUpdate)
    {
        client.Realtime.Channel("trades").OnInsert(msg => onUpdate(msg.Payload)).Subscribe();
    }
}