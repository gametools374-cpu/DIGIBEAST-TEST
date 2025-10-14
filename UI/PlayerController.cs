using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    public static event System.Action<float, int, bool> OnPPChanged;
    public static event System.Action<DigibeastInstance> OnDigibeastChanged;

    public DigibeastInstance starterDigibeast;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Broadcast initial digibeast if present
        if (starterDigibeast != null) OnDigibeastChanged?.Invoke(starterDigibeast);
    }

    void Update()
    {
        UpdatePP();
        if (Input.GetKeyDown(KeyCode.Q))
            TimeFractureManager.Instance?.OnTimeFractureInput();
    }

    public void SetStarterDigibeast(DigibeastInstance newBee)
    {
        starterDigibeast = newBee;
        OnDigibeastChanged?.Invoke(newBee);
    }

    public void UpdatePP()
    {
        if (starterDigibeast == null) return;
        float totalPP = starterDigibeast.pp.Values.Sum();
        int charges = Mathf.FloorToInt(totalPP / 20f);
        bool hasEcho = starterDigibeast.pp.ContainsKey("Echo");
        OnPPChanged?.Invoke(totalPP, charges, hasEcho);
    }
}
