using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class BeerRequest : MonoBehaviour {

    public int cost = 5;

    public RectTransform waitingBar;
    public float waitingTime = 5f;
    public Vector3 position;

    private float timer;

    public void Serve() {
        Cash.instance.transactionPosition = position;
        Cash.instance.Earn(cost);
        Destroy(gameObject);
    }

    private void OnEnable() {
        timer = waitingTime;
    }

    private void Update() {
        waitingBar.anchorMax = waitingBar.anchorMax.WithY(timer / waitingTime);
        timer -= Time.deltaTime;
    }
}
