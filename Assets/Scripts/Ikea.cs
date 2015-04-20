using UnityEngine;

public class Ikea : MonoBehaviour {

    public Grid grid;
    public GameObject prefab;
    public Shader previewShader;
    public Color previewTint;

    private GameObject preview;

    public void CleanupPreview() {
        if (preview) DestroyImmediate(preview);
        preview = null;
    }

    public void SetupPreview() {
        CleanupPreview();

        if (!prefab) return;

        var xf = transform;

        preview = (GameObject) Instantiate(prefab, xf.position, xf.rotation);
        preview.transform.SetParent(xf);

        foreach (var ren in preview.GetComponentsInChildren<Renderer>()) {
            var mtl = ren.material;
            mtl.shader = previewShader;
            mtl.renderQueue = 3100;
            mtl.SetColor("_TintColor", previewTint);
        }

        foreach (var cmp in preview.GetComponentsInChildren<Component>()) {
            if (cmp is Transform) continue;
            if (cmp is MeshFilter) continue;
            if (cmp is MeshRenderer) continue;

            DestroyImmediate(cmp);
        }
    }

    public void BuildHere() {
        Instantiate(prefab, transform.position, transform.rotation);
    }

    private static Bounds GatherVisualBounds(GameObject obj) {
        var bounds = new Bounds();
        var first = true;

        foreach (var ren in obj.GetComponentsInChildren<Renderer>()) {
            if (first) {
                bounds = ren.bounds;
                first = false;
                continue;
            }
            bounds.Encapsulate(ren.bounds);
        }
        return bounds;
    }

    private void OnEnable() {
        SetupPreview();
    }

    private void Update() {
        int x, z;
        grid.GetHoveredCell(out x, out z);
        transform.position = grid.CellToWorldPoint(x, z);

        if (Input.GetMouseButtonUp(0)) {
            BuildHere();
        }
    }

    private void OnDrawGizmos() {
        if (!preview) return;

        var bounds = GatherVisualBounds(preview);
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
