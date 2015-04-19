using UnityEngine;
using UnityEngine.Events;

public class GenericClub : MonoBehaviour {

    public float durability = 1f;
    public float damage = 1f;

    public UnityEvent onDestroy;

    private void OnCollisionEnter(Collision collision) {
        var vel = collision.relativeVelocity.magnitude;

        durability -= vel;

        var other = collision.contacts[0].otherCollider;
        var victim = other.GetComponent<Target>();

        if (victim) {
            var dmg = vel * damage;

            victim.Hit(dmg);
            FloatingText.instance.Spawn(collision.contacts[0].point, Mathf.RoundToInt(dmg));
        }

        if (durability <= 0f)
            onDestroy.Invoke();
    }
}
