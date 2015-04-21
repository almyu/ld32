using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Ikea))]
class IkeaEditor : Editor {

    public override void OnInspectorGUI() {
        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        var dirty = EditorGUI.EndChangeCheck();

        if (GUILayout.Button("Reset Preview") || dirty)
            (target as Ikea).SetupPreview();
    }
}
#endif

public class Ikea : MonoSingleton<Ikea> {

    public const int
        cellOccupied = 1 << 0,
        cellNextToTable = 1 << 1,
        cellTable = 1 << 2;

    public Grid grid;
    public GameObject prefab;
    public int cost;
    public bool flip;
    public bool isTable, isSeat;
    public Shader previewShader;
    public Color previewGoodTint, previewBadTint;
    public Material cellGoodMaterial, cellBadMaterial;

    private GameObject preview;
    private Material[] previewMaterials = new Material[0];
    private int gridWidth, gridDepth;

    public void CleanupPreview() {
        if (preview) DestroyImmediate(preview);
        preview = null;
    }

    public void SetupPreview() {
        CleanupPreview();

        if (!prefab) return;

        isTable = prefab.CompareTag("Table");
        isSeat = prefab.CompareTag("Seat");

        var xf = transform;
        var mtls = new List<Material>();

        preview = (GameObject) Instantiate(prefab, xf.position, xf.rotation);
        preview.transform.SetParent(xf);

        foreach (var ren in preview.GetComponentsInChildren<Renderer>())
            mtls.AddRange(ren.materials);

        foreach (var mtl in mtls) {
            mtl.shader = previewShader;
            mtl.renderQueue = 3100;
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

        preview.transform.localPosition = new Vector3((gridWidth - 1) % 2, 0, (gridDepth - 1) % 2) * 0.5f;

        if (flip) Flip();
    }

    private void Flip() {
        if (!preview) return;

        var tmp = gridWidth;
        gridWidth = gridDepth;
        gridDepth = tmp;

        var xf = preview.transform;
        var pos = xf.localPosition;
        var euler = xf.localEulerAngles;

        xf.localPosition = pos.WithXZ(pos.z, pos.x);
        xf.localEulerAngles = euler.WithY(90f - euler.y);
    }

    public bool BuyHere() {
        var inst = Cash.instance;
        if (!inst) return true;

        inst.transactionPosition = transform.position;
        if (!inst.Spend(cost)) return false;

        BuildHere();
        return true;
    }

    public void BuildHere() {
        var xf = preview.transform;
        Instantiate(prefab, xf.position, xf.rotation);
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
        if (!preview) return;

        grid.ResetMaterials();

        int x, z;
        if (!grid.GetHoveredCell(out x, out z)) {
            if (preview.activeSelf) preview.SetActive(false);

            if (Input.GetMouseButtonDown(0)) {
                CleanupPreview();
                prefab = null;
            }
            return;
        }

        if (!preview.activeSelf) preview.SetActive(true);

        transform.position = grid.CellToWorldPoint(x, z);

        var empty = grid.ContainsArea(x, z, gridWidth, gridDepth);

        Apply(x, z, (x_, z_, state, ren) => {
            var emptyCell = (state & cellOccupied) == 0 && (!isSeat || (state & cellNextToTable) != 0);
            empty = empty && emptyCell;

            ren.sharedMaterial = emptyCell ? cellGoodMaterial : cellBadMaterial;
        });

        TintPreview(empty);

        if (isSeat) {
            var turn = !flip
                ? grid.GetStateBit(x, z - 1, cellTable)
                : grid.GetStateBit(x - 1, z, cellTable);

            var angle = (turn ? 180f : 0f) + (flip ? 90f : 0f);
            preview.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.up);
        }

        if (empty && Input.GetMouseButtonUp(0)) {
            if (BuyHere()) {
                Apply(x, z, (x_, z_, state, cellRen) => grid.SetStateBit(x_, z_, cellOccupied, true));

                if (isTable) {
                    ApplyNextToTable(x, z, (x_, z_, state, r_) => grid.SetStateBit(x_, z_, cellNextToTable, true));
                    Apply(x, z, (x_, z_, state, cellRen) => grid.SetStateBit(x_, z_, cellTable, true));
                }
            }
        }

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space)) {
            flip = !flip;
            Flip();
        }
    }

    private void Apply(int x, int z, Grid.CellPred pred) {
        grid.Apply(x, z, gridWidth, gridDepth, pred);
    }

    private void ApplyNextToTable(int x, int z, Grid.CellPred pred) {
        grid.Apply(x - 1, z, gridWidth + 2, gridDepth, pred);
        grid.Apply(x, z - 1, gridWidth, gridDepth + 2, pred);
    }

    private void TintPreview(bool good) {
        var clr = good ? previewGoodTint : previewBadTint;
        var id = Shader.PropertyToID("_TintColor");

        foreach (var mtl in previewMaterials)
            mtl.SetColor(id, clr);
    }

    private void OnDrawGizmos() {
        if (!preview) return;

        var bounds = GatherVisualBounds(preview);
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
