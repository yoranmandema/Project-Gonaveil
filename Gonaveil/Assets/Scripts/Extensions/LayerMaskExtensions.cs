using UnityEngine;

public static class LayerMaskExtensions {
    public static LayerMask GetLayerMaskFromLayer(this LayerMask layerMask, LayerMask fromMask) {
        int mask = 0;

        for (int i = 0; i < 32; i++) {
            if (!Physics.GetIgnoreLayerCollision(fromMask, i)) {
                mask = mask | 1 << i;
            }
        }

        return mask;
    }
}
