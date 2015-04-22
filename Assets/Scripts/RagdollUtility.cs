using UnityEngine;

public static class RagdollUtility {

    public static void CopyPose(Transform src, Transform dst) {
        var numChildren = Mathf.Min(src.childCount, dst.childCount);

        for (int i = 0; i < numChildren; ++i) {
            var srcChild = src.GetChild(i);
            var dstChild = dst.GetChild(i);

            CopyTransform(srcChild, dstChild);
            CopyPose(srcChild, dstChild);
        }
    }

    public static void CopyTransform(Transform src, Transform dst) {
        dst.localPosition = src.localPosition;
        dst.localRotation = src.localRotation;
        dst.localScale = src.localScale;
    }
}
