using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Tension : MonoSingleton<Tension> {

    public float current = 0f;
    public float perMisservedBeer = 0.1f;

    public Slider funOMeter;
    public UnityEvent onOverflow;

    public void AddSome() {
        current += perMisservedBeer;

        if (funOMeter) funOMeter.value = current;

        if (current >= 1f)
            onOverflow.Invoke();
    }
}
