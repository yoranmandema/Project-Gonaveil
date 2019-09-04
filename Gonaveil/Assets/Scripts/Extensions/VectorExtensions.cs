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

    public static Vector3 WithX(this Vector3 vec, float value) {
        return new Vector3(value, vec.y, vec.z);
    }

    public static Vector3 WithY(this Vector3 vec, float value) {
        return new Vector3(vec.x, value, vec.z);
    }

    public static Vector3 WithZ(this Vector3 vec, float value) {
        return new Vector3(vec.x, vec.y, value);
    }
}
