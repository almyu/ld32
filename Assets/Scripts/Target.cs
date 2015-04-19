using UnityEngine;
using System.Collections.Generic;

public class Target : MonoBehaviour {

    public const int numFactions = 3;

    public static List<Target>[] targets = new[] { new List<Target>(100), new List<Target>(100), new List<Target>(100) };

    public static Target FindClosest(int faction, Vector3 pos) {
        var closest = null as Target;
        var minDistSq = float.MaxValue;

        foreach (var target in targets[faction]) {
            var distSq = (pos - target.transform.position).sqrMagnitude;
            if (minDistSq > distSq) {
                minDistSq = distSq;
                closest = target;
            }
        }
        return closest;
    }

    public int faction;
    public float hp = 100f;

    public Target FindClosestEnemy() {
        var pos = transform.position;

        var enemy0 = FindClosest((faction + 1) % numFactions, pos);
        var enemy1 = FindClosest((faction + 2) % numFactions, pos);

        if (!enemy0) return enemy1;
        if (!enemy1) return enemy0;

        var dist0 = (enemy0.transform.position - pos).sqrMagnitude;
        var dist1 = (enemy1.transform.position - pos).sqrMagnitude;

        return dist0 < dist1 ? enemy0 : enemy1;
    }

    public void Hit(float damage) {
        hp -= damage;
    }

    private void OnEnable() {
        targets[faction].Add(this);
    }

    private void OnDisable() {
        targets[faction].Remove(this);
    }
}
