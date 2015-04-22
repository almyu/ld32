using UnityEngine;

public class Lifetime : MonoBehaviour {

    public float lifetime = 1f;
    public bool inferFromParticles = true;

    private void Start() {
        if (inferFromParticles) {
            foreach (var ps in GetComponentsInChildren<ParticleSystem>()) {
                var time = ps.duration + ps.startLifetime;
                if (lifetime < time) lifetime = time;
            }
        }
        Destroy(gameObject, lifetime);
    }
}
