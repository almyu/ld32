using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class Tension : MonoSingleton<Tension> {

    public float current = 0f;
    public float perMisservedBeer = 0.1f;

    public Slider funOMeter;
    public UnityEvent onOverflow;

    public void AddSome() {
        current += perMisservedBeer;

        if (funOMeter) funOMeter.value = current;

        if (current >= 1f) {
            onOverflow.Invoke();
            Overflow();
        }
    }

    public void Underflow() {
        foreach (var club in FindObjectsOfType<GenericClub>()) {
            club.GetComponentInChildren<Collider>().enabled = false;
            club.GetComponentInChildren<Rigidbody>().isKinematic = true;
        }
    }

    public void Overflow() {
        StartCoroutine(DoEnrageFolks());
    }

    private IEnumerator DoEnrageFolks() {
        foreach (var club in FindObjectsOfType<GenericClub>()) {
            club.GetComponentInChildren<Collider>().enabled = true;
            club.GetComponentInChildren<Rigidbody>().isKinematic = false;
        }

        foreach (var drinker in FindObjectsOfType<BeerDrinker>())
            drinker.enabled = false;

        foreach (var agent in FindObjectsOfType<NavMeshAgent>())
            agent.enabled = true;

        foreach (var visitor in FindObjectsOfType<Visitor>()) {
            visitor.Enrage();
            yield return new WaitForSeconds(Random.Range(0f, 0.15f));
        }
    }
}
