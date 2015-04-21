using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    public Vector3 compactModeOffset;
    public float compactModeFov = 60f;
    public float transitionTime = 0.5f;
    public AnimationCurve transitionEasing = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public bool autoCompactWhenEnabled = true;

    private Vector3 initialPosition;
    private float initialFov;

    private bool appQuitting;

    private void OnApplicationQuit() {
        appQuitting = true;
    }

    private void Awake() {
        var cam = Camera.main;
        initialPosition = cam.transform.position;
        initialFov = cam.fieldOfView;
    }

    private void OnEnable() {
        if (!autoCompactWhenEnabled) return;
        if (initialFov == 0f) Awake();

        SetCompactMode(true);
    }

    private void OnDisable() {
        if (autoCompactWhenEnabled) SetCompactMode(false);
    }

    public void SetCompactMode(bool compact) {
        if (appQuitting) return;

        var compactPosition = initialPosition + compactModeOffset;
        var proc = CameraControlRunner.instance;

        proc.StopAllCoroutines();
        proc.StartCoroutine(DoAnimate(
            compact ? initialPosition : compactPosition,
            compact ? compactPosition : initialPosition,
            compact ? initialFov : compactModeFov,
            compact ? compactModeFov : initialFov));
    }

    private IEnumerator DoAnimate(Vector3 fromPos, Vector3 toPos, float fromFov, float toFov) {
        var cam = Camera.main;
        var xf = cam.transform;

        for (var t = 0f; t <= transitionTime; t += Mathf.Min(0.01f, Time.deltaTime)) {
            var progress = transitionEasing.Evaluate(t / transitionTime);
            xf.position = Vector3.Lerp(fromPos, toPos, progress);
            cam.fieldOfView = Mathf.Lerp(fromFov, toFov, progress);
            yield return null;
        }

        xf.position = toPos;
        cam.fieldOfView = toFov;
    }
}
