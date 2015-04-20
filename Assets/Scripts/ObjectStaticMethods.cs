using UnityEngine;

public class ObjectStaticMethods : MonoBehaviour {

    public void Instantiate(GameObject prefab) {
        Object.Instantiate(prefab);
    }

    public void Destroy(GameObject obj) {
        Object.Destroy(obj);
    }

    public void DestroyImmediate(GameObject obj) {
        Object.DestroyImmediate(obj);
    }
}
