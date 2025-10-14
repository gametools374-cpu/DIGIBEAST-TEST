// BattleManager.cs
// Turn queue + move resolution.
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : ManagerBase<BattleManager>
{
    public delegate void TurnAction(BattleState state);
    private Queue<TurnAction> turnQueue = new Queue<TurnAction>();

    public void EnqueueAction(TurnAction action) => turnQueue.Enqueue(action);

    public void ProcessTurn(BattleState state)
    {
        while (turnQueue.Count > 0) turnQueue.Dequeue().Invoke(state);
    }
}

[System.Serializable]
public class BattleState
{
    public DigibeastInstance player;
    public DigibeastInstance rival;
    public bool playerWon;
}