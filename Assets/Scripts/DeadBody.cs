using UnityEngine;
using UnityEngine.Events;

public class DeadBody : MonoBehaviour {

    public int minCash = 30, maxCash = 70;
    public float autoLootAfter = -1f;

    public UnityEvent onLoot;

    public void Loot() {
        Cash.instance.transactionPosition = transform.position;
        Cash.instance.Earn(Random.Range(minCash, maxCash + 1));
        onLoot.Invoke();
        Destroy(gameObject);
    }

    private void Start() {
        if (autoLootAfter >= 0f)
            Invoke("Loot", autoLootAfter);
    }
}
