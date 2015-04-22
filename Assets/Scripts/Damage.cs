using UnityEngine;

public class Damage : MonoSingleton<Damage> {

    public FloatingText notifier;
    public AudioSource sfxPlayer;
    public AudioClip hitWood, hitMetal;
    public float expectedMaxDamage;

    public void Hit(Vector3 position, float damage) {
        var visualDmg = Mathf.RoundToInt(damage);
        if (visualDmg > 0) notifier.Spawn(position, visualDmg);

        sfxPlayer.PlayOneShot(hitWood, Mathf.Clamp01(damage / expectedMaxDamage));
    }
}
