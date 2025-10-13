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
        Inventory.Instance.AddItem("EtherShard");
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

// Small CLI runner for GameCI / Unity Test Runner
public static class DigibeeTestCLI
{
    // Called by Unity Test Runner via -executeMethod
    public static void RunAllTestsFromCLI()
    {
        Debug.Log("DigibeeTestCLI: Running editor smoke tests...");
        var go = new GameObject("__DigibeeTestRunner__");
        var t = go.AddComponent<DigibeeTests>();
        t.TestEvo();
        t.TestStats();
        Debug.Log("DigibeeTestCLI: Completed.");
        Object.DestroyImmediate(go);
    }
}
#endif