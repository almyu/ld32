using UnityEngine;

public class Ikea : MonoBehaviour {

    public Grid grid;
    public GameObject prefab;
    public Shader previewShader;
    public Color previewTint;

    public void CleanupPreview() {
        var xf = transform;

        for (int i = xf.childCount; i-- != 0; )
            DestroyImmediate(xf.GetChild(i).gameObject);
    }

    public void SetupPreview() {
        CleanupPreview();

        if (!prefab) return;

        var xf = transform;

        var obj = (GameObject) Instantiate(prefab, xf.position, xf.rotation);
        obj.transform.SetParent(xf);

        foreach (var ren in obj.GetComponentsInChildren<Renderer>()) {
            var mtl = ren.material;
            mtl.shader = previewShader;
            mtl.renderQueue = 3100;
            mtl.SetColor("_TintColor", previewTint);
        }
    }

    private void OnEnable() {
        SetupPreview();
    }

    private void Update() {
        int x, z;
        grid.GetHoveredCell(out x, out z);
        transform.position = grid.CellToWorldPoint(x, z);
    }
}
