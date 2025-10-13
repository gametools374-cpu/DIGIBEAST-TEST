using System.Collections.Generic;

[System.Serializable]
public class DigibeeInstance {
    public string line;
    public int currentForm;
    public int level;
    public DigibeeStats ivs;
    public DigibeeStats evs;
    public DigibeeNature nature;
    public DigibeeStats stats;
    public Dictionary<string, int> pp;
}
