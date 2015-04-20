using UnityEngine;

public class IkeaControl : MonoBehaviour {

    public void SetFurniturePrefab(GameObject prefab) {
        var inst = Ikea.instance;
        if (!inst) return;

        inst.prefab = prefab;
        inst.SetupPreview();
    }

    public void SetFurnitureCost(int cost) {
        var inst = Ikea.instance;
        if (inst) inst.cost = cost;
    }
}
