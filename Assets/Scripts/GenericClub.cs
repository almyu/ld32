using UnityEngine;
using UnityEngine.Events;

public class GenericClub : MonoBehaviour {

    public float durability = 1f;
    public float damage = 1f;

    public UnityEvent onHit, onDestroy;

    private void OnCollisionEnter(Collision collision) {
        if (collision.contacts.Length == 0) return;

        var vel = collision.relativeVelocity.magnitude;

        durability -= vel;

        var other = collision.contacts[0].otherCollider;
        var victim = other.GetComponent<Target>();

        if (victim) {
            var dmg = vel * damage;

            victim.Hit(dmg);
            onHit.Invoke();

            var visualDmg = Mathf.RoundToInt(dmg);
            if (visualDmg > 0) FloatingText.instance.Spawn(collision.contacts[0].point, visualDmg);
        }

        if (durability <= 0f)
            onDestroy.Invoke();
    }
}
