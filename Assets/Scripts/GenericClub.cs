using UnityEngine;
using UnityEngine.Events;

public class GenericClub : MonoBehaviour {

    public float durability = 1f;
    public float damage = 1f;

    public UnityEvent onHit, onDestroy;

    /*private float lastCollisionTime;

    private void OnCollisionEnter(Collision collision) {
        if (collision.contacts.Length == 0) return;
        //if (Time.timeSinceLevelLoad - lastCollisionTime < 0.1f) return;

        //lastCollisionTime = Time.timeSinceLevelLoad;

        var vel = collision.relativeVelocity.magnitude;

        durability -= vel * 0.01f;

        //Debug.Log("Hit: " + this + " -> " + string.Join(", ", System.Array.ConvertAll(collision.contacts, p => p.otherCollider + "|" + p.thisCollider)));

        foreach (var contact in collision.contacts) {
            var victim = contact.otherCollider.GetComponent<Target>();
            if (!victim) continue;

            var dmg = vel * damage;

            victim.Hit(dmg);
            onHit.Invoke();

            var visualDmg = Mathf.RoundToInt(dmg);
            if (visualDmg > 0) FloatingText.instance.Spawn(contact.point, visualDmg);
        }

        if (durability <= 0f)
            onDestroy.Invoke();
    }*/

    private void OnTriggerEnter(Collider other) {
        var victim = other.GetComponent<Target>();
        if (!victim) {
            var body = other.attachedRigidbody;
            if (body) {
                var prop = body.GetComponent<Destructible>();
                if (prop) prop.Damage(damage);
            }
            return;
        }

        var dmg = damage * Random.Range(0.8f, 1.2f);

        victim.Hit(dmg);
        onHit.Invoke();

        Damage.instance.Hit(other.transform.position + Vector3.up * 2f, dmg);

        durability -= 1f;

        if (durability <= 0f)
            onDestroy.Invoke();
    }
}
