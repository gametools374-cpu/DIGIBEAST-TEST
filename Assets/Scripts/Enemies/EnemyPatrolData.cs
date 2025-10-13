using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Digibee/EnemyPatrol")]
public class EnemyPatrolData : ScriptableObject
{
    public string zone;
    public List<Vector3> waypoints;
    public string enemyType; // "Rot-Drone", "Glitch-Hound"
    public int count;
}
