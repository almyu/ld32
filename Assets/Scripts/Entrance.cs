using UnityEngine;

public class Entrance : MonoBehaviour {

    public GameObject visitorPrefab;
    public Vector2 interval = new Vector2(3f, 5f);
    public float rate = 1f;

    private float nextVisitTime;

    private void Update() {
        if (nextVisitTime > Time.timeSinceLevelLoad) return;

        nextVisitTime = Time.timeSinceLevelLoad + Random.Range(interval[0], interval[1]) / rate;

        var obj = (GameObject) Instantiate(visitorPrefab, transform.position, transform.rotation);
        obj.name += "" + Random.Range(0, 30);
        
        var target = obj.AddComponent<Target>();
        target.enabled = false;
        target.faction = Random.Range(0, Target.numFactions);
        target.enabled = true;
    }
}
