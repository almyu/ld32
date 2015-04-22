using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Entrance))]
public class EntranceEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if (GUILayout.Button("Spawn extra"))
            (target as Entrance).Spawn();
    }
}
#endif

public class Entrance : MonoSingleton<Entrance> {

    public GameObject[] visitorPrefabs;
    public Vector2 interval = new Vector2(3f, 5f);
    public float rate = 1f;
    public int limit = 15;
    public int standerLimit = 3;

    private float nextVisitTime;

    private void OnEnable() {
        limit = Seat.availableSeats.Count + standerLimit;
    }

    private void Update() {
        if (nextVisitTime > Time.timeSinceLevelLoad) return;
        nextVisitTime = Time.timeSinceLevelLoad + Random.Range(interval[0], interval[1]) / rate;

        if (limit <= 0) return;
        --limit;

        Spawn();
    }

    public void Spawn() {
        var prefab = visitorPrefabs[Random.Range(0, visitorPrefabs.Length)];

        var obj = (GameObject)Instantiate(prefab, transform.position, transform.rotation);
        obj.name += "" + Random.Range(0, 30);
    }
}
