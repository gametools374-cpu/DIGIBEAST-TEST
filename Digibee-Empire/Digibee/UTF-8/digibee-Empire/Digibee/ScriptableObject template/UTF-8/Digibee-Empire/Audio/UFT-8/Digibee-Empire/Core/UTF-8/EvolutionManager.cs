// EvolutionManager.cs
// Listens for level-ups and triggers evolutions.
using UnityEngine;

public class EvolutionManager : ManagerBase<EvolutionManager>
{
    void OnEnable()
    {
        DigibeeCore.OnLevelUp += CheckEvolution;
    }

    void OnDisable()
    {
        DigibeeCore.OnLevelUp -= CheckEvolution;
    }

    void CheckEvolution(DigibeeInstance instance)
    {
        var line = DigibeeManager.Instance.GetLine(instance.line);
        if (line == null) return;

        for (int i = 0; i < line.forms.Length; i++)
        {
            var form = line.forms[i];
            if (instance.currentForm == i && instance.level >= form.levelReq)
            {
                if (!string.IsNullOrEmpty(form.itemReq) && !Inventory.Instance.Has(form.itemReq)) continue;

                instance.currentForm = i + 1;
                DigibeeManager.Instance.CalculateStats(instance);
                AudioManager.Instance.PlayEvo(line.name);
                Debug.Log($"{instance.line} evolved to {form.name}");
                break;
            }
        }
    }
}