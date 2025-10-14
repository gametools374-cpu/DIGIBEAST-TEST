using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    public static event System.Action<float, int, bool> OnPPChanged;
    public static event System.Action<DigibeeInstance> OnDigibeeChanged;

    public DigibeeInstance starterDigibee;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Broadcast initial digibee if present
        if (starterDigibee != null) OnDigibeeChanged?.Invoke(starterDigibee);
    }

    void Update()
    {
        UpdatePP();
        if (Input.GetKeyDown(KeyCode.Q))
            TimeFractureManager.Instance?.OnTimeFractureInput();
    }

    public void SetStarterDigibee(DigibeeInstance newBee)
    {
        starterDigibee = newBee;
        OnDigibeeChanged?.Invoke(newBee);
    }

    public void UpdatePP()
    {
        if (starterDigibee == null) return;
        float totalPP = starterDigibee.pp.Values.Sum();
        int charges = Mathf.FloorToInt(totalPP / 20f);
        bool hasEcho = starterDigibee.pp.ContainsKey("Echo");
        OnPPChanged?.Invoke(totalPP, charges, hasEcho);
    }
}
