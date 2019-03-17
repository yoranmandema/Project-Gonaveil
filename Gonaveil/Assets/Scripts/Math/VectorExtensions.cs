using UnityEngine;

public static class VectorExtensions {
    public static Vector3 SetX(this Vector3 vector, float v) {
        return new Vector3(v, vector.y, vector.z);
    }

    public static Vector3 SetY(this Vector3 vector, float v) {
        return new Vector3(vector.x, v, vector.z);
    }

    public static Vector3 SetZ(this Vector3 vector, float v) {
        return new Vector3(vector.x, vector.y, v);
    }
}
