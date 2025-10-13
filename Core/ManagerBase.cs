using UnityEngine;

public static class ManagerBase {
    public static T FindManager<T>() where T : MonoBehaviour => Object.FindObjectOfType<T>();
}
