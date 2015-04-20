﻿using UnityEngine;
using System.Collections.Generic;

public class Ikea : MonoBehaviour {

    public const int
        cellOccupied    = 1 << 0,
        cellNextToTable = 1 << 1;

    public Grid grid;
    public GameObject prefab;
    public bool isTable, isSeat;
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

        isTable = prefab.CompareTag("Table");
        isSeat = prefab.CompareTag("Seat");

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
        if (!preview) return;

        grid.ResetMaterials();

        int x, z;
        if (!grid.GetHoveredCell(out x, out z)) {
            if (preview.activeSelf) preview.SetActive(false);
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

        if (empty && Input.GetMouseButtonUp(0)) {
            BuildHere();
            Apply(x, z, (x_, z_, state, cellRen) => grid.SetStateBit(x_, z_, 1, true));

            if (isTable) ApplyNextToTable(x, z, (x_, z_, state, r_) => grid.SetStateBit(x_, z_, cellNextToTable, true));
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
