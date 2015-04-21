using UnityEngine;

public class Pause : MonoBehaviour {

    private float lastTimeScale;

    private void OnEnable() {
        lastTimeScale = Time.timeScale;
        Time.timeScale = 0f;
    }

    private void OnDisable() {
        Time.timeScale = lastTimeScale;
    }
}
