using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class BattleUI : MonoBehaviour
{
    [Header("Player Health")]
    [SerializeField] private Image healthFill;
    [SerializeField] private Gradient healthGradient;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private float healthSmoothTime = 0.12f;
    [SerializeField] private Color criticalFlashColor = Color.red;
    [SerializeField] private float criticalFlashDuration = 0.8f;

    [Header("PP Display")]
    [SerializeField] private Transform ppContainer;
    [SerializeField] private PPBar ppBarPrefab;
    [SerializeField] private PPBurstVFX ppBurstVFXPrefab;
    private Dictionary<string, PPBar> ppBars = new Dictionary<string, PPBar>();
    private Queue<PPBurstVFX> ppBurstPool = new Queue<PPBurstVFX>();
    private const int MAX_BURST_VFX = 6;

    [Header("Time-Fracture Ring")]
    [SerializeField] private TimeFractureCooldownRing timeFractureRing;
    [SerializeField] private Color fractureReadyColor = new Color(0f, 0.83f, 1f);
    [SerializeField] private Color fractureChargingColor = new Color(0.3f, 0.3f, 0.3f);

    [Header("Rival Taunt")]
    [SerializeField] private CanvasGroup tauntOverlay;
    [SerializeField] private ParticleSystem featherVFX;
    [SerializeField] private float tauntFadeDuration = 0.6f;

    // Cached refs
    private PlayerController player => PlayerController.Instance;
    private GameState gameState => GameState.Instance;
    private Coroutine flashRoutine;
    private float healthVel;

    void Awake()
    {
        InitializePopupsIfNeeded();
        InitializePPBurstVFX();
        SubscribeToEvents();
    }

    void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    void Update()
    {
        if (gameState == null || player == null) return;

        float currentHP = gameState.player.stats.hp;
        float maxHP = gameState.player.stats.maxHp;
        float target = maxHP > 0 ? currentHP / maxHP : 0f;
        float smoothed = Mathf.SmoothDamp(healthFill.fillAmount, target, ref healthVel, healthSmoothTime);
        if (healthFill) healthFill.fillAmount = smoothed;
        if (healthFill && healthGradient != null) healthFill.color = healthGradient.Evaluate(smoothed);
        if (healthText) healthText.text = $"{Mathf.RoundToInt(smoothed * 100)}%";

        bool isCritical = currentHP <= maxHP * 0.25f;
        if (isCritical && flashRoutine == null)
        {
            flashRoutine = StartCoroutine(CriticalFlashRoutine());
        }

        UpdateTimeFractureRing();
    }

    void SubscribeToEvents()
    {
        if (GameState.IsInstantiated) GameState.OnPlayerHealthChanged += OnHealthChanged;
        PlayerController.OnDigibeastChanged += OnDigibeastChanged;
        PlayerController.OnPPChanged += OnPPChanged;
        StaticWraithController.OnPhaseChanged += (phase) => ShowBossPhase("WRAITH", phase, Color.red);
        ChronoGuardController.OnPhaseChanged += (phase) => ShowBossPhase("CHRONO", phase, new Color(0.6f, 0f, 0.8f));
        TimeFractureManager.OnFractureActivated += OnFractureActivated;
        HoloDisplay.OnRivalTaunt += OnRivalTaunt;
    }

    void UnsubscribeFromEvents()
    {
        if (GameState.IsInstantiated) GameState.OnPlayerHealthChanged -= OnHealthChanged;
        PlayerController.OnDigibeastChanged -= OnDigibeastChanged;
        PlayerController.OnPPChanged -= OnPPChanged;
        StaticWraithController.OnPhaseChanged -= (phase) => { };
        ChronoGuardController.OnPhaseChanged -= (phase) => { };
        TimeFractureManager.OnFractureActivated -= OnFractureActivated;
        HoloDisplay.OnRivalTaunt -= OnRivalTaunt;
    }

    void OnHealthChanged(float newHpPct, float delta)
    {
        // handled in Update for smoothness
    }

    void OnDigibeastChanged(DigibeastInstance newDigibeast)
    {
        RebuildPPBars(newDigibeast);
    }

    void OnPPChanged(float totalPP, int charges, bool hasEcho)
    {
        if (player == null || player.starterDigibeast == null) return;
        foreach (var kvp in player.starterDigibeast.pp)
        {
            string type = kvp.Key;
            float ppValue = kvp.Value;
            if (ppBars.ContainsKey(type))
            {
                ppBars[type].SetValue(ppValue / 100f);
                if (Mathf.Approximately(ppValue, 100f))
                    TriggerPPBurst(type, ppBars[type].transform.position);
            }
        }
    }

    IEnumerator CriticalFlashRoutine()
    {
        AudioManager.Instance?.PlayUI("HealthCritical");
        float elapsed = 0f;
        while (elapsed < criticalFlashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / criticalFlashDuration;
            if (healthFill) healthFill.color = Color.Lerp(criticalFlashColor, healthGradient.Evaluate(healthFill.fillAmount), t);
            yield return null;
        }
        flashRoutine = null;
    }

    void RebuildPPBars(DigibeastInstance digibeast)
    {
        foreach (var bar in ppBars.Values) if (bar != null) Destroy(bar.gameObject);
        ppBars.Clear();
        if (digibeast == null) return;
        foreach (var kvp in digibeast.pp)
        {
            var bar = Instantiate(ppBarPrefab, ppContainer);
            bar.Initialize(kvp.Key, GetPPColor(kvp.Key));
            ppBars[kvp.Key] = bar;
        }
    }

    Color GetPPColor(string type) => type switch
    {
        "DPS" => new Color(1f, 0.3f, 0.3f),
        "Tank" => new Color(0.3f, 0.6f, 1f),
        "Echo" => new Color(0.7f, 0.2f, 0.9f),
        _ => Color.white
    };

    void ShowBossPhase(string name, int phase, Color color)
    {
        // Simplified: could show banner etc. Kept minimal to avoid missing refs
        Debug.Log($"ShowBossPhase {name} phase {phase}");
    }

    void InitializePopupsIfNeeded()
    {
        // Placeholder: popups are handled elsewhere in the project. Keep this method to avoid null refs.
    }

    void InitializePPBurstVFX()
    {
        if (ppBurstVFXPrefab == null || ppContainer == null) return;
        for (int i = 0; i < MAX_BURST_VFX; i++)
        {
            var vfx = Instantiate(ppBurstVFXPrefab, ppContainer);
            vfx.gameObject.SetActive(false);
            ppBurstPool.Enqueue(vfx);
        }
    }

    void TriggerPPBurst(string ppType, Vector3 position)
    {
        if (ppBurstPool.Count == 0) return;
        var vfx = ppBurstPool.Dequeue();
        vfx.gameObject.SetActive(true);
        vfx.Play(ppType, position, () =>
        {
            vfx.gameObject.SetActive(false);
            ppBurstPool.Enqueue(vfx);
        });
        AudioManager.Instance?.PlayUI("PP_ChargeComplete");
        if (Camera.main) Camera.main.DOShakePosition(0.3f, 0.15f);
    }

    void OnFractureActivated() { if (timeFractureRing != null) timeFractureRing.Activate(); }
    void OnRivalTaunt(string rival, string context)
    {
        AudioManager.Instance?.PlayVoice(context);
        if (tauntOverlay != null) tauntOverlay.DOFade(0.3f, tauntFadeDuration).SetLoops(2, LoopType.Yoyo);
        featherVFX?.Play();
    }

    void UpdateTimeFractureRing()
    {
        if (timeFractureRing == null) return;
        float percent = TimeFractureManager.Instance != null ? TimeFractureManager.Instance.GetCooldownPercentage() : 0f;
        timeFractureRing.SetProgress(percent, percent >= 1f ? fractureReadyColor : fractureChargingColor);
    }
}
