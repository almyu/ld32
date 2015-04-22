using UnityEngine;

public class Reloader : MonoBehaviour {

    public void Reload() {
        Application.LoadLevel(Application.loadedLevel);
    }
}
