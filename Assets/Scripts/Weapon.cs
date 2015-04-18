using UnityEngine;

public class Weapon : MonoBehaviour {

    public Transform handle;

    private void Awake() {
        if (handle == null) handle = transform;
    }
}
