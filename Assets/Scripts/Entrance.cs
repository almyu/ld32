using UnityEngine;

public class Entrance : MonoBehaviour {

    public GameObject visitorPrefab;
    public Vector2 interval = new Vector2(3f, 5f);
    public float rate = 1f;

    private float nextVisitTime;

    private void Update() {
        if (nextVisitTime > Time.timeSinceLevelLoad) return;

        nextVisitTime = Time.timeSinceLevelLoad + Random.Range(interval[0], interval[1]) / rate;

        Instantiate(visitorPrefab, transform.position, transform.rotation);
    }
}
