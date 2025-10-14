// DigibeastInstance.cs
// Runtime copy of a Digibeast.
using System;
using UnityEngine;

[Serializable]
public class DigibeastInstance
{
    public string line;
    public int currentForm = 0;
    public int level = 5;
    public DigibeastStats ivs = new DigibeastStats();
    public DigibeastStats evs = new DigibeastStats();
    public Nature nature = new Nature();
    public DigibeastStats stats = new DigibeastStats();
    public System.Collections.Generic.Dictionary<string, int> pp = new System.Collections.Generic.Dictionary<string, int>();
}

[Serializable]
public class DigibeastStats
{
    public int hp, atk, def, spAtk, spDef, speed;
}

[Serializable]
public class Nature
{
    public float atk = 1f, def = 1f, spAtk = 1f, spDef = 1f, speed = 1f, hp = 1f;
}