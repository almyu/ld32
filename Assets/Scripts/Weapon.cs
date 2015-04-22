using UnityEngine;
using System.Collections.Generic;

public class Weapon : MonoBehaviour {

    public static List<Weapon> availableWeapons = new List<Weapon>(100);
    public static int existingWeaponCount;

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

    public Vector3 handleRotation;
    public Vector3 handleOffset;

    private Rigidbody cachedBody;
    private Collider cachedCollider;
    private NavMeshObstacle cachedObstacle;

    private void Awake() {
        cachedBody = GetComponent<Rigidbody>();
        cachedCollider = GetComponentInChildren<Collider>();
        cachedObstacle = GetComponent<NavMeshObstacle>();

        ++existingWeaponCount;
    }

    private void OnDestroy() {
        --existingWeaponCount;
    }

    public void SetHeld(bool held) {
        //cachedBody.isKinematic = !held;
        cachedCollider.isTrigger = held;
        cachedObstacle.enabled = !held;
        enabled = !held;
    }

    public void SetBeingAnimated(bool animated) {
        cachedBody.isKinematic = animated;
        cachedBody.useGravity = !animated;
        cachedCollider.enabled = !animated;
    }

    private void OnEnable() {
        availableWeapons.Add(this);
    }

    private void OnDisable() {
        availableWeapons.Remove(this);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.green;

        Gizmos.DrawCube(handleOffset, Vector3.one * 0.2f);
    }
}
