using UnityEngine;
using System.Collections.Generic;

public class Ikea : MonoBehaviour {

    public Grid grid;
    public GameObject prefab;
    public Shader previewShader;
    public Color previewGoodTint, previewBadTint;
    public Material cellGoodMaterial, cellBadMaterial;

    private GameObject preview;
    private Material[] previewMaterials = new Material[0];
    private int gridWidth, gridDepth;
    public Vector3 offset;

    public void CleanupPreview() {
        if (preview) DestroyImmediate(preview);
        preview = null;
    }

    public void SetupPreview() {
        CleanupPreview();

        if (!prefab) return;

        var xf = transform;
        var mtls = new List<Material>();

        preview = (GameObject) Instantiate(prefab, xf.position, xf.rotation);
        preview.transform.SetParent(xf);

        foreach (var ren in preview.GetComponentsInChildren<Renderer>()) {
            var mtl = ren.material;
            mtl.shader = previewShader;
            mtl.renderQueue = 3100;

            mtls.Add(mtl);
        }

        previewMaterials = mtls.ToArray();

        foreach (var cmp in preview.GetComponentsInChildren<Component>()) {
            if (cmp is Transform) continue;
            if (cmp is MeshFilter) continue;
            if (cmp is MeshRenderer) continue;

            DestroyImmediate(cmp);
        }

        var bounds = GatherVisualBounds(preview);
        var size = bounds.size;

        gridWidth = Mathf.Max(1, Mathf.RoundToInt(size.x));
        gridDepth = Mathf.Max(1, Mathf.RoundToInt(size.z));

        offset = new Vector3((gridWidth - 1) % 2, 0, (gridDepth - 1) % 2) * 0.5f;

        preview.transform.localPosition = offset;
    }

    public void BuildHere() {
        Instantiate(prefab, transform.position + offset, transform.rotation);
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

        var good = true;

        grid.ResetMaterials();
        grid.Apply(x, z, gridWidth, gridDepth, (x_, z_, state, cellRen) => {
            var goodCell = (state & 1) == 0;
            good = good && goodCell;
            cellRen.sharedMaterial = goodCell ? cellGoodMaterial : cellBadMaterial;
        });

        foreach (var mtl in previewMaterials)
            mtl.SetColor("_TintColor", good ? previewGoodTint : previewBadTint);

        if (good && Input.GetMouseButtonUp(0)) {
            BuildHere();
            grid.Apply(x, z, gridWidth, gridDepth, (x_, z_, state, cellRen) => grid.SetStateBit(x_, z_, 1, true));
        }
    }

    private void OnDrawGizmos() {
        if (!preview) return;

        var bounds = GatherVisualBounds(preview);
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
