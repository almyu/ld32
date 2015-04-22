using UnityEngine;

public class Destructible : MonoBehaviour {

    public float durability = 10f;
    public GameObject effect;

    public void Damage(float amount) {
        durability -= amount;
        if (durability <= 0f) Destruct();
    }

    public void Destruct() {
        if (effect) Instantiate(effect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
