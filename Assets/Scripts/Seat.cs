using UnityEngine;
using System.Collections.Generic;

public class Seat : MonoBehaviour {

    [System.Serializable]
    public struct Preferences {

        public static readonly Preferences none = new Preferences();
    }

    public static List<Seat> availableSeats = new List<Seat>(100);

    public static Seat Pick(Preferences prefs) {
        return availableSeats[Random.Range(0, availableSeats.Count)];
    }

    private void OnEnable() {
        availableSeats.Add(this);
    }

    private void OnDisable() {
        availableSeats.Remove(this);
    }
}
