using UnityEngine;
using UnityEngine.UI;
using JamSuite.Events;

public class Cash : MonoSingleton<Cash> {

    public int amount = 100;
    public StringEvent onChangeText;

    public FloatingText notifier;
    public string noCashText = "Too expensive";
    public Vector3 transactionPosition;

    public bool Spend(int some) {
        if (amount < some) {
            if (notifier) {
                var obj = notifier.Spawn(transactionPosition, 0);
                obj.GetComponentInChildren<Text>().text = noCashText;
            }
            return false;
        }

        amount -= some;
        Notify(-some);
        onChangeText.Invoke(amount + "");
        return true;
    }

    public void Earn(int some) {
        amount += some;
        Notify(some);
        onChangeText.Invoke(amount + "");
    }

    public void Notify(int value) {
        if (notifier) notifier.Spawn(transactionPosition, value);
    }

    private void OnEnable() {
        onChangeText.Invoke(amount + "");
    }
}
