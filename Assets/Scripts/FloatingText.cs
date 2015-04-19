using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(FloatingText))]
public class FloatingTextEditor : Editor {

    private static bool spawning = false;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (Application.isPlaying) {
            spawning = GUILayout.Toggle(spawning, "Spawn Continuously");

            if (spawning || GUILayout.Button("Spawn")) {
                var pos = Camera.main.transform.position + Camera.main.transform.forward * 3f;
                (target as FloatingText).Spawn(pos, Random.Range(0, 20000));
            }
        }
    }
}
#endif

[RequireComponent(typeof(RectTransform))]
public class FloatingText : MonoSingleton<FloatingText> {

    private static Gradient MakeFlatGradient(Color color) {
        return new Gradient {
            colorKeys = new[] { new GradientColorKey(color, 0f) },
            alphaKeys = new[] { new GradientAlphaKey(color.a, 0f) }
        };
    }

    private static Gradient MakeFadingGradient() {
        return new Gradient {
            colorKeys = new[] {
                new GradientColorKey(Color.white, 0f)
            },
            alphaKeys = new[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 0.8f),
                new GradientAlphaKey(0f, 1f)
            }
        };
    }

    public RectTransform template;
    public Vector2 minMaxValue = new Vector2(0, 1000);
    public Vector2 minMaxValueFont = new Vector2(24, 32);
    public Gradient colorOverValue = MakeFlatGradient(Color.white);

    public Vector2 spread = new Vector2(15, 10);

    public float distance = 100f;
    public AnimationCurve distanceOverLifetime = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public Vector2 minMaxAngle = new Vector2(-10f, 10f);
    public Gradient tintMul = MakeFadingGradient();
    public Gradient tintAdd = MakeFlatGradient(Color.clear);
    public float lifetime = 1f;

    public float extraScale = 0f;
    public AnimationCurve extraScaleOverLifetime = new AnimationCurve {
        keys = new[] { new Keyframe(0f, 1f, 0f, -2f), new Keyframe(1f, 0f, 0f, 0f) }
    };

    public Vector2 shake = Vector2.one * 2;
    public AnimationCurve shakeOverLifetime = new AnimationCurve {
        keys = new[] { new Keyframe(0f, 1f, 0f, -3.14f), new Keyframe(1f, 0f, 0f, 0f) }
    };


    private static Font defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");


    public void Spawn(Vector3 worldPos, int value) {
        var obj = Instantiate();
        obj.SetActive(true);

        var xf = obj.GetComponent<RectTransform>();
        var text = obj.GetComponentInChildren<Text>();

        TuneTransform(xf, WorldToScreenPoint(worldPos));
        TuneText(text, value);
        StartCoroutine(Animate(worldPos, xf, text));
    }


    private GameObject Instantiate() {
        if (template)
            return (GameObject) Instantiate(template.gameObject);

        var obj = new GameObject("Text");
        obj.AddComponent<RectTransform>();

        var text = obj.AddComponent<Text>();
        text.supportRichText = false;
        text.font = defaultFont;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;

        obj.AddComponent<Shadow>();

        return obj;
    }

    private Vector2 WorldToScreenPoint(Vector3 worldPos) {
        return Camera.main.WorldToScreenPoint(worldPos);
    }

    private void TuneTransform(RectTransform xf, Vector2 screenPos) {
        xf.SetParent(transform, false);
        xf.anchorMin = xf.anchorMax = Vector2.zero;
        xf.anchoredPosition = screenPos;
    }

    private void TuneText(Text text, int value) {
        var t = Mathf.Clamp01((value - minMaxValue[0]) / (minMaxValue[1] - minMaxValue[0]));
        text.fontSize = (int) Mathf.Lerp(minMaxValueFont[0], minMaxValueFont[1], t);
        text.color = colorOverValue.Evaluate(t);
        text.text = value.ToString();
    }

    private IEnumerator Animate(Vector3 worldPos, RectTransform xf, Text text) {
        var angle = Random.Range(minMaxAngle[0], minMaxAngle[1]);
        var offset = (Vector2)(Quaternion.AngleAxis(angle, Vector3.forward) * Vector2.up * distance);

        var initial = Vector2.Scale(spread, new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)));

        var color = text.color;

        for (var time = 0f; time <= lifetime; time += Mathf.Max(Time.deltaTime, 0.01f)) {
            var t = time / lifetime;

            var dist = distanceOverLifetime.Evaluate(t);
            xf.anchoredPosition = WorldToScreenPoint(worldPos) + initial + offset * dist;

            if (extraScale != 0f)
                xf.localScale = Vector3.one * (1f + extraScale * extraScaleOverLifetime.Evaluate(t));

            if (shake != Vector2.zero) {
                var sample = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                var ease = shakeOverLifetime.Evaluate(t);

                xf.anchoredPosition += Vector2.Scale(shake, sample) * ease;
            }

            var mul = tintMul.Evaluate(t);
            var add = tintAdd.Evaluate(t);
            text.color = color * mul + add * add.a;

            yield return null;
        }

        Destroy(xf.gameObject);
    }
}
