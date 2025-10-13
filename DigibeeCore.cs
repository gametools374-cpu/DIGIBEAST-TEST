// RivalAI.cs
// State-machine brain.
using System;
using System.Collections.Generic;
using UnityEngine;

public class RivalAI : MonoBehaviour
{
    public string rivalName;
    public DigibeeInstance digibee;
    public List<AIState> states = new List<AIState>();
    public string currentState = "Idle";

    public void DecideAction(BattleState state)
    {
        foreach (var s in states)
        {
            if (s.condition(state))
            {
                var action = s.action(state);
                if (!string.IsNullOrEmpty(action.taunt))
                    AudioManager.Instance.PlayTaunt(rivalName, action.taunt);
                if (!string.IsNullOrEmpty(action.move))
                    digibee.GetComponent<DigibeeCore>().UseMove(action.move);
                currentState = s.name;
                break;
            }
        }
    }
}

[Serializable]
public class AIState
{
    public string name;
    public Func<BattleState, bool> condition;
    public Func<BattleState, MoveAction> action;
}

[Serializable]
public struct MoveAction
{
    public string move;
    public string taunt;
}