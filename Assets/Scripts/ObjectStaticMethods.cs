using UnityEngine;

public class ObjectStaticMethods : MonoBehaviour {

    public void Instantiate(GameObject prefab) {
        Object.Instantiate(prefab);
    }

    public void InstantiateHere(GameObject prefab) {
        var xf = transform;
        Object.Instantiate(prefab, xf.position, xf.rotation);
    }

    public void Destroy(GameObject obj) {
        Object.Destroy(obj);
    }

    public void DestroyImmediate(GameObject obj) {
        Object.DestroyImmediate(obj);
    }

    public void DestroyThis() {
        Object.Destroy(gameObject);
    }
}
