using UnityEngine;
using UnityEngine.UI;

public class BeerUI : MonoSingleton<BeerUI> {

    public BeerRequest requestTemplate;

    public BeerRequest SpawnRequest(Vector3 worldPos) {
        var obj = Instantiate(requestTemplate.gameObject);
        obj.SetActive(true);

        var xf = obj.GetComponent<RectTransform>();
        xf.SetParent(GetComponent<RectTransform>(), false);
        xf.anchorMin = xf.anchorMax = Vector2.zero;
        xf.anchoredPosition = Camera.main.WorldToScreenPoint(worldPos);

        var req = obj.GetComponent<BeerRequest>();
        req.position = worldPos;

        return req;
    }

    public void ServeAll() {
        foreach (var req in FindObjectsOfType<BeerRequest>())
            req.Serve();
    }
}
