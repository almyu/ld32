using UnityEngine;
using JamSuite.Events;

public class Cash : MonoSingleton<Cash> {

    public int amount = 100;
    public IntEvent onChange;

    public FloatingText notifier;
    public Vector3 transactionPosition;

    public bool Spend(int some) {
        if (amount < some) return false;

        amount -= some;
        onChange.Invoke(amount);
        return true;
    }

    public void Earn(int some) {
        amount += some;
        onChange.Invoke(amount);
    }

    public void Notify(int value) {
        if (notifier) notifier.Spawn(transactionPosition, value);
    }
}
