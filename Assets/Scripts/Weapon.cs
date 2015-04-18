using UnityEngine;
using System.Collections.Generic;

public class Weapon : MonoBehaviour {

    public static List<Weapon> availableWeapons = new List<Weapon>(100);

    public Transform handle;


    public static Weapon FindClosest(Vector3 pos) {
        var closest = null as Weapon;
        var minDistSq = float.MaxValue;

        foreach (var weapon in availableWeapons) {
            var distSq = (pos - weapon.transform.position).sqrMagnitude;
            if (minDistSq > distSq) {
                minDistSq = distSq;
                closest = weapon;
            }
        }
        return closest;
    }

    private void Awake() {
        if (handle == null) handle = transform;
    }

    private void OnEnable() {
        availableWeapons.Add(this);
    }

    private void OnDisable() {
        availableWeapons.Remove(this);
    }
}
