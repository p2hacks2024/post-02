using UdonSharp;
using UnityEngine;

public class PolarCoordinateConverter : UdonSharpBehaviour
{
    private float playerHeight; // プレイヤーの身長
    public float heightMultiplier = 1.2f;

    public void SetPlayerHeight(float height)
    {
        playerHeight = height;
    }

    public void ConvertToPolar(Vector3 cartesian, out float r, out float theta, out float phi)
    {
        // 半径R
        r = Mathf.Sqrt(cartesian.x * cartesian.x + cartesian.y * cartesian.y + cartesian.z * cartesian.z) * playerHeight * heightMultiplier;

        // θ（水平角）
        theta = Mathf.Atan2(cartesian.z, cartesian.x) * Mathf.Rad2Deg;

        // φ（垂直角）
        phi = Mathf.Atan2(Mathf.Sqrt(cartesian.x * cartesian.x + cartesian.z * cartesian.z), cartesian.y) * Mathf.Rad2Deg;
    }
}
