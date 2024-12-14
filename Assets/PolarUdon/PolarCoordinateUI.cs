using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using TMPro;

public class PolarCoordinateUI : UdonSharpBehaviour
{
    public PolarCoordinateConverter polarConverter; // 極座標変換スクリプト
    public TextMeshProUGUI leftControllerText; // 左コントローラーのUI
    public TextMeshProUGUI rightControllerText; // 右コントローラーのUI

    private VRCPlayerApi localPlayer; // ローカルプレイヤー
    private Vector3 polarOrigin; // 極座標の原点

    void Start()
    {
        localPlayer = Networking.LocalPlayer;

        if (localPlayer == null)
        {
            Debug.LogError("Local player not found!");
            return;
        }

        // プレイヤーの身長を取得して原点を設定
        float playerHeight = GetPlayerHeight();
        polarOrigin = new Vector3(0, 0, 0);
        polarConverter.SetPlayerHeight(playerHeight);
    }

    private float GetPlayerHeight()
    {
        Vector3 headPosition = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
        Vector3 footPosition = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Origin).position;
        return Vector3.Distance(headPosition, footPosition);
    }

    void Update()
    {
        if (localPlayer == null || polarConverter == null) return;

        // 左コントローラーの座標取得
        Vector3 leftPosition = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position - polarOrigin;
        float leftR, leftTheta, leftPhi;
        polarConverter.ConvertToPolar(leftPosition, out leftR, out leftTheta, out leftPhi);

        // 右コントローラーの座標取得
        Vector3 rightPosition = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position - polarOrigin;
        float rightR, rightTheta, rightPhi;
        polarConverter.ConvertToPolar(rightPosition, out rightR, out rightTheta, out rightPhi);

        // UIに表示
        leftControllerText.text = string.Format(
            "Left Controller:\n" +
            "Cartesian: X: {0:F2}, Y: {1:F2}, Z: {2:F2}\n" +
            "Polar: R: {3:F2}, θ: {4:F2}, φ: {5:F2}",
            leftPosition.x, leftPosition.y, leftPosition.z, leftR, leftTheta, leftPhi
        );
        rightControllerText.text = string.Format(
            "Right Controller:\n" +
            "Cartesian: X: {0:F2}, Y: {1:F2}, Z: {2:F2}\n" +
            "Polar: R: {3:F2}, θ: {4:F2}, φ: {5:F2}",
            rightPosition.x, rightPosition.y, rightPosition.z, rightR, rightTheta, rightPhi
        );
    }
}
