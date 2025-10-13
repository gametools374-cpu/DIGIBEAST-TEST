// DigibeeInstance.cs
// Runtime copy of a Digibee.
using System;
using UnityEngine;

[Serializable]
public class DigibeeInstance
{
    public string line;
    public int currentForm = 0;
    public int level = 5;
    public DigibeeStats ivs = new DigibeeStats();
    public DigibeeStats evs = new DigibeeStats();
    public Nature nature = new Nature();
    public DigibeeStats stats = new DigibeeStats();
    public System.Collections.Generic.Dictionary<string, int> pp = new System.Collections.Generic.Dictionary<string, int>();
}

[Serializable]
public class DigibeeStats
{
    public int hp, atk, def, spAtk, spDef, speed;
}

[Serializable]
public class Nature
{
    public float atk = 1f, def = 1f, spAtk = 1f, spDef = 1f, speed = 1f, hp = 1f;
}