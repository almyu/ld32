using UnityEngine;
using System.Collections.Generic;

public class Seat : MonoBehaviour {

    [System.Serializable]
    public struct Preferences {

        public static readonly Preferences none = new Preferences();
    }

    public static List<Seat> availableSeats = new List<Seat>(100);

    public Vector3 mountPoint;

    public Vector3 worldMountPoint {
        get { return transform.TransformPoint(mountPoint); }
    }

    public static Seat Pick(Preferences prefs) {
        return availableSeats[Random.Range(0, availableSeats.Count)];
    }

    private void OnEnable() {
        availableSeats.Add(this);
    }

    private void OnDisable() {
        availableSeats.Remove(this);
    }

    private void OnDrawGizmos() {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.white.WithA(0.3f);

        Gizmos.DrawRay(Vector3.zero, mountPoint);
        Gizmos.DrawSphere(mountPoint, 0.1f);
    }
}
