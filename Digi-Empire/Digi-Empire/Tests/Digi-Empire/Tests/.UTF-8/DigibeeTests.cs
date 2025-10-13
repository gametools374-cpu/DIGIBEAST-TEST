// DigibeeTests.cs
#if UNITY_EDITOR
using UnityEngine;

public class DigibeeTests : MonoBehaviour
{
    [ContextMenu("Run Evo Test")]
    void TestEvo()
    {
        var line = DigibeeManager.Instance.GetLine("Aetheriel");
        var inst = new DigibeeInstance { line = "Aetheriel", currentForm = 0, level = 28 };
        Inventory.Instance.Add("EtherShard");
        EvolutionManager.Instance.CheckEvolution(inst);
        Debug.Assert(inst.currentForm == 1, "Evo failed");
    }

    [ContextMenu("Stat Test Lv50")]
    void TestStats()
    {
        var inst = new DigibeeInstance { line = "Aetheriel", currentForm = 2, level = 50 };
        DigibeeManager.Instance.CalculateStats(inst);
        Debug.Assert(inst.stats.hp == 120, "Stat calc wrong");
    }
}
#endif