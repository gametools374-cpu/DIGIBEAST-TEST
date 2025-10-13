using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TimeFractureZone : MonoBehaviour {
    [SerializeField] int rewindSeconds = 5;
    Queue<Vector3> positionHistory = new();
    bool isRewinding = false;

    void Update() {
        if (!isRewinding)
            positionHistory.Enqueue(transform.position);
        if (positionHistory.Count > 60 * rewindSeconds)
            positionHistory.Dequeue();
    }

    public void FractureBurst() {
        if (!isRewinding)
            StartCoroutine(RewindRoutine());
    }

    IEnumerator RewindRoutine() {
        isRewinding = true;
        var snapshot = new List<Vector3>(positionHistory);
        foreach (var pos in snapshot.AsEnumerable().Reverse()) {
            transform.position = pos;
            yield return new WaitForFixedUpdate();
        }
        isRewinding = false;
    }
}
