using UnityEngine;

public class Grid : MonoSingleton<Grid> {

    public int width = 10, depth = 6;

    public MeshRenderer cellPrefab;

    private int[,] states;
    private MeshRenderer[,] renderers;
    private Material initialMaterial;

    private Plane raycastPlane;

    public Vector3 Snap(Vector3 point) {
        return transform.position + new Vector3(Mathf.Round(point.x), 0f, Mathf.Round(point.z));
    }

    public void Generate() {
        states = new int[width, depth];
        renderers = new MeshRenderer[width, depth];
        initialMaterial = cellPrefab.sharedMaterial;

        var prefab = cellPrefab.gameObject;
        var rot = prefab.transform.rotation;
        var xf = transform;

        raycastPlane = new Plane(-xf.up, xf.position);

        for (int x = 0; x < width; ++x) {
            for (int z = 0; z < depth; ++z) {
                var obj = (GameObject) Instantiate(prefab, new Vector3(x, 0f, z), rot);
                obj.transform.SetParent(xf, false);
                obj.name = x + "_" + z;

                renderers[x, z] = obj.GetComponent<MeshRenderer>();
            }
        }
    }

    public bool GetHoveredCell(out int x, out int z) {
        return ScreenPointToCell(Input.mousePosition, out x, out z);
    }

    public bool ScreenPointToCell(Vector3 point, out int x, out int z) {
        var ray = Camera.main.ScreenPointToRay(point);

        var dist = 0f;
        raycastPlane.Raycast(ray, out dist);

        var pos = Snap(ray.origin + ray.direction * dist);
        x = (int) pos.x;
        z = (int) pos.z;

        return ContainsCell(x, z);
    }

    public bool ContainsCell(int x, int z) {
        return x >= 0 && z >= 0 && x < width && z < depth;
    }

    public Material foo;
    private void Update() {
        ResetMaterials();
        int x, z;
        if (GetHoveredCell(out x, out z))
            Apply(x, z, 2, 3, (x_, z_, state, ren) => ren.sharedMaterial = foo);
    }

    public void Clamp(ref int x, ref int z) {
        if (x < 0) x = 0;
        else if (x >= width) x = width - 1;

        if (z < 0) z = 0;
        else if (z >= depth) z = depth - 1;
    }

    public void ClampSize(ref int w, ref int d) {
        if (w > width) w = width;
        if (d > depth) d = depth;
    }

    public int GetState(int x, int z) {
        Clamp(ref x, ref z);
        return states[x, z];
    }

    public bool GetStateBit(int x, int z, int bit) {
        return (GetState(x, z) & bit) != 0;
    }

    public void SetState(int x, int z, int state) {
        Clamp(ref x, ref z);
        states[x, z] = state;
    }

    public void SetStateBit(int x, int z, int bit, bool set) {
        Clamp(ref x, ref z);
        states[x, z] = set ? (states[x, z] | bit) : (states[x, z] & ~bit);
    }

    public delegate void CellPred(int x, int z, int state, MeshRenderer ren);

    public void Apply(int x, int z, int w, int d, CellPred pred) {
        int endx = x + w, endz = z + d;
        ClampSize(ref endx, ref endz);
        Clamp(ref x, ref z);

        for (int curx = x; curx < endx; ++curx)
            for (int curz = z; curz < endz; ++curz)
                pred(curx, curz, states[curx, curz], renderers[curx, curz]);
    }

    public void Apply(CellPred pred) {
        Apply(0, 0, width, depth, pred);
    }

    public void ResetMaterials() {
        Apply((x, z, state, ren) => ren.sharedMaterial = initialMaterial);
    }

    private void Awake() {
        Generate();
    }
}
