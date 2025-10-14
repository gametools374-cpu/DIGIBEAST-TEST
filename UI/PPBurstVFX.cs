using UnityEngine;

public class PPBurstVFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private float duration = 0.6f;

    private System.Action onComplete;

    public void Play(string ppType, Vector3 position, System.Action onComplete)
    {
        transform.position = position;
        var main = particleSystem.main;
        main.startColor = GetPPColor(ppType);
        particleSystem.Play();
        AudioManager.Instance?.PlayUI($"PP_{ppType}_Burst");
        this.onComplete = onComplete;
        CancelInvoke(nameof(Complete));
        Invoke(nameof(Complete), duration);
    }

    private void Complete()
    {
        particleSystem.Stop();
        onComplete?.Invoke();
        onComplete = null;
    }

    private Color GetPPColor(string type) => type switch
    {
        "DPS" => new Color(1f, 0.3f, 0.3f),
        "Tank" => new Color(0.3f, 0.6f, 1f),
        "Echo" => new Color(0.7f, 0.2f, 0.9f),
        _ => Color.white
    };
}
