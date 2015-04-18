using UnityEngine;

public class Visitor : MonoBehaviour {

    public NavMeshAgent agent;
    public Transform leftHand, rightHand;
    
    private Transform target;

    public Weapon SeekWeapon() {
        var weapons = FindObjectsOfType<Weapon>();
        var closest = null as Weapon;
        var minDistSq = float.MaxValue;
        var pos = transform.position;

        foreach (var weapon in weapons) {
            if (weapon.handle.parent != null) continue;

            var distSq = (pos - weapon.handle.position).sqrMagnitude;
            if (minDistSq > distSq) {
                minDistSq = distSq;
                closest = weapon;
            }
        }

        return closest;
    }

    private void Update() {
        if (target != null) return;

        var wpn = SeekWeapon();
        if (wpn == null) return;

        target = wpn.handle;
        agent.destination = target.position;
    }
}
