// EvolutionManager.cs
// Listens for level-ups and triggers evolutions.
using UnityEngine;

public class EvolutionManager : ManagerBase<EvolutionManager>
{
    void OnEnable()
    {
        DigibeastCore.OnLevelUp += CheckEvolution;
    }

    void OnDisable()
    {
        DigibeastCore.OnLevelUp -= CheckEvolution;
    }

    void CheckEvolution(DigibeastInstance instance)
    {
        var line = DigibeastManager.Instance.GetLine(instance.line);
        if (line == null) return;

        for (int i = 0; i < line.forms.Length; i++)
        {
            var form = line.forms[i];
            if (instance.currentForm == i && instance.level >= form.levelReq)
            {
                if (!string.IsNullOrEmpty(form.itemReq) && !Inventory.Instance.Has(form.itemReq)) continue;

                instance.currentForm = i + 1;
                DigibeastManager.Instance.CalculateStats(instance);
                AudioManager.Instance.PlayEvo(line.name);
                Debug.Log($"{instance.line} evolved to {form.name}");
                break;
            }
        }
    }
}