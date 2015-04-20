using UnityEngine;

public class Entrance : MonoBehaviour {

    public GameObject[] visitorPrefabs;
    public Vector2 interval = new Vector2(3f, 5f);
    public float rate = 1f;
    public int limit = 15;

    private float nextVisitTime;

    private void Update() {
        if (nextVisitTime > Time.timeSinceLevelLoad) return;
        nextVisitTime = Time.timeSinceLevelLoad + Random.Range(interval[0], interval[1]) / rate;

        if (limit <= 0) return;
        --limit;

        var prefab = visitorPrefabs[Random.Range(0, visitorPrefabs.Length)];

        var obj = (GameObject)Instantiate(prefab, transform.position, transform.rotation);
        obj.name += "" + Random.Range(0, 30);
    }
}
