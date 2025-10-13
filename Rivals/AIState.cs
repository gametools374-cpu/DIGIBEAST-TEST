using System;

[System.Serializable]
public class AIState {
    public string name;
    public Func<object, bool> condition;
    public Func<object, MoveAction> action;
}
