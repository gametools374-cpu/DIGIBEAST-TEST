using System.Collections.Generic;

[System.Serializable]
public class DigibeastInstance {
    public string line;
    public int currentForm;
    public int level;
    public DigibeastStats ivs;
    public DigibeastStats evs;
    public DigibeastNature nature;
    public DigibeastStats stats;
    public Dictionary<string, int> pp;
}
