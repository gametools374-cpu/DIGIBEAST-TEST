// AudioManager.cs
// Wwise-only audio dispatcher.
using UnityEngine;

public class AudioManager : ManagerBase<AudioManager>
{
    [Header("Banks")]
    public string[] banksToLoad = { "Cries", "Signatures", "Ambients", "Taunts", "UI", "Evo" };

    void Start()
    {
        foreach (var b in banksToLoad) AkSoundEngine.LoadBank(b, out _);
    }

    public void LoadBank(string bankName) => AkSoundEngine.LoadBank(bankName, out _);

    public void PlayCry(string line, int form) =>
        AkSoundEngine.PostEvent($"Cry_{line}_{(form == 2 ? "Mega" : form == 1 ? "Champion" : "Rookie")}", gameObject);

    public void PlaySignature(string move) =>
        AkSoundEngine.PostEvent($"Sig_{move}", gameObject);

    public void PlayTaunt(string rival, string key) =>
        AkSoundEngine.PostEvent($"Taunt_{rival}_{key}", gameObject);

    public void PlayAmbient(int floor)
    {
        if (floor <= 25) AkSoundEngine.PostEvent("Ambient_NullSpire_Low", gameObject);
        else if (floor <= 50) AkSoundEngine.PostEvent("Ambient_NullSpire_Mid", gameObject);
        else if (floor <= 75) AkSoundEngine.PostEvent("Ambient_NullSpire_Deep", gameObject);
        else AkSoundEngine.PostEvent("Ambient_NullSpire_Void", gameObject);
    }

    public void PlayUI(string action) => AkSoundEngine.PostEvent($"UI_{action}", gameObject);
    public void PlayEvo(string line) => AkSoundEngine.PostEvent($"Evo_{line}", gameObject);
}