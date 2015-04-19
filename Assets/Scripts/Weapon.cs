using UnityEngine;
using System.Collections.Generic;

public class Weapon : MonoBehaviour {

    public static List<Weapon> availableWeapons = new List<Weapon>(100);

    [System.Serializable]
    public struct Handle {
        public Vector3 offset;
    }

    public Handle[] handles;


    public static Weapon FindClosest(Vector3 pos) {
        var closest = null as Weapon;
        var minDistSq = float.MaxValue;

        foreach (var weapon in availableWeapons) {
            var distSq = (pos - weapon.transform.position).sqrMagnitude;
            if (distSq > minDistSq) continue;

            minDistSq = distSq;
            closest = weapon;
        }
        return closest;
    }


    public struct FindHandleResult {
        public int index;
        public Vector3 position, approachDirection;
    }

    public FindHandleResult FindClosestHandle(Vector3 pos) {
        var xf = transform;
        var closest = 0;
        var minDistSq = float.MaxValue;

        for (int i = 0; i < handles.Length; ++i) {
            var distSq = (pos - xf.TransformPoint(handles[i].offset)).sqrMagnitude;
            if (distSq > minDistSq) continue;

            minDistSq = distSq;
            closest = i;
        }

        return new FindHandleResult {
            index = closest,
            position = xf.TransformPoint(handles[closest].offset),
            approachDirection = xf.TransformDirection(handles[closest].offset).normalized
        };
    }

    private void OnEnable() {
        availableWeapons.Add(this);
    }

    private void OnDisable() {
        availableWeapons.Remove(this);
    }

    private void OnDrawGizmos() {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.green.WithA(0.5f);

        foreach (var handle in handles) {
            Gizmos.DrawWireCube(handle.offset, Vector3.one * 0.2f);
        }
    }
}
