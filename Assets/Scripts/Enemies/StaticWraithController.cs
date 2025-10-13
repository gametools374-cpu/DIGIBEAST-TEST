using UnityEngine;

public class StaticWraithController : MonoBehaviour
{
    public enum Phase { One, Two, Three }
    public Phase currentPhase = Phase.One;
    public float hp = 100f;

    void Update()
    {
        if (hp <= 60f && currentPhase == Phase.One) PhaseTransition(Phase.Two);
        if (hp <= 30f && currentPhase == Phase.Two) PhaseTransition(Phase.Three);
    }

    void PhaseTransition(Phase newPhase)
    {
        currentPhase = newPhase;
        AudioManager.Instance.PlaySignature($"Wraith_{newPhase}");
        // TODO: Trigger phase-specific logic (hex-grid, EMP, slow-mo)
    }

    public void TakeDamage(float dmg)
    {
        hp -= dmg;
        if (hp <= 0) OnDefeat();
    }

    void OnDefeat()
    {
        Inventory.Instance.AddItem("Time-Shard Relic");
        GameState.Instance.currentFloor = 2;
        GameState.Instance.Save();
        AudioManager.Instance.PlaySignature("Wraith_Death");
    }
}
