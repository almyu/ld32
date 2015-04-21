using UnityEngine;

public class BeerDrinker : MonoBehaviour {

    public float firstOrderTime = 3f;
    public float maxInterval = 15f;
    public float minInterval = 3f;
    public float intervalDelta = -3f;

    private float nextInterval;

    public void RequestBeer() {
        BeerUI.instance.SpawnRequest(transform.position + Vector3.up * 2f);

        nextInterval = Mathf.Max(minInterval, nextInterval + intervalDelta);
        Invoke("RequestBeer", nextInterval);
    }

    private void Start() {
        nextInterval = maxInterval;
        Invoke("RequestBeer", firstOrderTime);
    }
}
