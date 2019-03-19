using UnityEngine;

public static class LayerMaskExtensions {
    public static LayerMask GetReverseLayerMask(this LayerMask layerMask) {
        int mask = 0;

        for (int i = 0; i < 32; i++) {
            if (!Physics.GetIgnoreLayerCollision(layerMask, i)) {
                mask = mask | 1 << i;
            }
        }

        return mask;
    }
}
