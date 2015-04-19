using UnityEngine;
using System.Collections.Generic;

public class Seat : MonoBehaviour {

    [System.Serializable]
    public struct Preferences {

        public static readonly Preferences none = new Preferences();
    }

    public static List<Seat> availableSeats = new List<Seat>(100);

    public Vector3 mountPoint = Vector3.up, mountDirection = Vector3.forward;
    public Vector2 mountAngleRange = new Vector2(-90f, 90f);

    public float approachHalfAngleCos {
        get { return Mathf.Cos((mountAngleRange[1] - mountAngleRange[0]) * 0.5f * Mathf.Deg2Rad); }
    }
    public Vector3 approachDirection {
        get { return Quaternion.AngleAxis((mountAngleRange[0] + mountAngleRange[1]) * 0.5f, Vector3.forward) * mountDirection; }
    }

    public Vector3 GetWaypoint(Vector3 pos) {
        var worldMountPoint = transform.TransformPoint(mountPoint);
        var approachDir = transform.TransformDirection(approachDirection);
        var toPos = (pos - worldMountPoint).normalized;

        if (Vector3.Dot(toPos, approachDir) >= approachHalfAngleCos)
            return worldMountPoint + toPos;

        return worldMountPoint + approachDir;
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

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.white.WithA(0.3f);

        UnityEditor.Handles.matrix = transform.localToWorldMatrix;
        UnityEditor.Handles.color = Color.white.WithA(0.1f);

        UnityEditor.Handles.DrawSolidArc(mountPoint, Vector3.forward,
            Quaternion.AngleAxis(mountAngleRange[0], Vector3.forward) * mountDirection,
            mountAngleRange[1] - mountAngleRange[0], mountDirection.magnitude);

        Gizmos.DrawRay(mountPoint, mountDirection);
        Gizmos.DrawSphere(mountPoint, 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(mountPoint, approachDirection);

        Gizmos.matrix = Matrix4x4.identity;

        for (int i = 0; i < 36; ++i) {
            var dir = Quaternion.AngleAxis(i * 10f, Vector3.up) * Vector3.right;
            var point = transform.position + dir;
            Gizmos.DrawLine(transform.TransformPoint(mountPoint), GetWaypoint(point));
        }
    }
#endif
}
