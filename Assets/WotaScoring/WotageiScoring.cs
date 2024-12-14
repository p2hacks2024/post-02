using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using TMPro;

public class WotageiScoring : UdonSharpBehaviour
{
    public float bpm = 120f; // 初期BPM
    private float beatInterval;

    public AudioSource metronome; // メトロノーム音
    public WotageiScoreDisplay scoreDisplay; // スコア表示用スクリプト
    public TextMeshProUGUI leftControllerText; // 左コントローラー情報表示用テキスト
    public TextMeshProUGUI rightControllerText; // 右コントローラー情報表示用テキスト
    public TextMeshProUGUI countdownText; // カウントダウン表示用テキスト
    public TextMeshProUGUI debugText; // デバッグ用テキスト
    public TextMeshProUGUI heightText; // 身長表示用テキスト

    private Vector3[] playerDataLeft = new Vector3[32];
    private Vector3[] playerDataRight = new Vector3[32];

    private bool isPlaying = false; // ゲーム中フラグ
    private int currentBeat = 0; // 現在の拍
    private float nextBeatTime; // 次の拍の時間
    private bool isCountingDown = false; // カウントダウン中フラグ
    private float countdownTimer = -1f; // カウントダウン中のタイマー
    private int countdownValue = 3; // カウントダウンの初期値
    private VRCPlayerApi localPlayer; // ローカルプレイヤー
    private float cachedPlayerHeight; // プレイヤーの身長をキャッシュ
    private void InitializeTeacherData()
    {
        // 身長補正係数を計算
        float heightDifference = cachedPlayerHeight - 1.7f; // 基準身長1.7mとの差分
        float heightAdjustment = heightDifference * 0.0325f / 0.1f; // 10cmごとに0.0325調整

        // 教師データをプレイヤーの身長に基づいて補正
        for (int i = 0; i < teacherDataLeft.Length; i++)
        {
            Vector3 originalLeft = teacherDataLeft[i];
            Vector3 originalRight = teacherDataRight[i];

            // r値に補正を加える
            teacherDataLeft[i] = new Vector3(
                originalLeft.x + heightAdjustment, 
                originalLeft.y, 
                originalLeft.z
            );

            teacherDataRight[i] = new Vector3(
                originalRight.x + heightAdjustment, 
                originalRight.y, 
                originalRight.z
            );
        }

        Debug.Log($"[DEBUG] Teacher data adjusted for height difference: {heightAdjustment:F4}");
    }
    private Vector3[] teacherDataLeft = new Vector3[]
    {
        new Vector3(0.43f, -168.76f, 146.42f),
        new Vector3(0.7f, -157f, 141.1f),
        new Vector3(0.4f, -170.6f, 95.7f),
        new Vector3(0.4f, -170.6f, 95.7f),
        new Vector3(0.44f, 2.15f, 137.98f),
        new Vector3(0.7f, -157f, 141.1f),
        new Vector3(0.16f, -33.44f, 167.5f),
        new Vector3(0.45f, 165.6f, 87.97f),
        new Vector3(0.64f, -171.6f, 135.8f),
        new Vector3(0.64f, -174.6f, 142f),
        new Vector3(0.46f, 16f, 156f),
        new Vector3(0.64f, -169f, 146f),
        new Vector3(0.24f, 55f, 156f),
        new Vector3(0.37f, 108f, 166f),
        new Vector3(0.46f, 176f, 111f),
        new Vector3(0.21f, 60f, 134f),
        new Vector3(0.51f, -150f, 155f),
        new Vector3(0.61f, 30f, 152f),
        new Vector3(0.38f, -173f, 155f),
        new Vector3(0.47f, 23.84f, 125f),
        new Vector3(0.35f, 155f, 161f),
        new Vector3(0.21f, 102f, 35f),
        new Vector3(0.19f, -9f, 113f),
        new Vector3(0.56f, -126f, 156f),
        new Vector3(0.49f, -164.6f, 159f),
        new Vector3(0.41f, 152f, 86f),
        new Vector3(0.34f, 0f, 156f),
        new Vector3(0.40f, 159.6f, 81f),
        new Vector3(0.41f, 163f, 89f),
        new Vector3(0.4f, 163.1f, 89f),
        new Vector3(0.4f, 161f, 89f),
        new Vector3(0.4f, 160f, 89f)
    };

    private Vector3[] teacherDataRight = new Vector3[]
    {
        new Vector3(0.24f, 6.1f, 105.4f),
        new Vector3(0.24f, 6.1f, 105.4f),
        new Vector3(0.23f, -178.73f, 115f),
        new Vector3(0.34f, -29f, 162.6f),
        new Vector3(0.64f, -45.25f, 148.31f),
        new Vector3(0.46f, -174.5f, 131.8f),
        new Vector3(0.18f, -166.5f, 144.9f),
        new Vector3(0.4f, 4.18f, 87.87f),
        new Vector3(0.5f, 152.74f, 140.88f),
        new Vector3(0.44f, 159f, 143f),
        new Vector3(0.61f, -16f, 147f),
        new Vector3(0.51f, 161f, 144f),
        new Vector3(0.2f, 142f, 152f),
        new Vector3(0.24f, 57.2f, 37f),
        new Vector3(0.48f, 0.7f, 118f),
        new Vector3(0.18f, 92f, 122f),
        new Vector3(0.66f, 160f, 151f),
        new Vector3(0.43f, -15f, 160f),
        new Vector3(0.49f, 153f, 127f),
        new Vector3(0.44f, 3.54f, 163f),
        new Vector3(0.32f, 103f, 57f),
        new Vector3(0.24f, 95f, 51f),
        new Vector3(0.29f, -26f, 112f),
        new Vector3(0.43f, -165f, 157f),
        new Vector3(0.37f, 26.2f, 80.26f),
        new Vector3(0.52f, -8.54f, 140f),
        new Vector3(0.31f, 2.71f, 165f),
        new Vector3(0.3f, 34.5f, 164f),
        new Vector3(0.24f, 47f, 166f),
        new Vector3(0.24f, 44f, 166f),
        new Vector3(0.25f, 43f, 166f),
        new Vector3(0.25f, 45f, 166f)
    };

    private void Start()
    {
        localPlayer = Networking.LocalPlayer;

        if (localPlayer != null)
        {
            cachedPlayerHeight = localPlayer.GetBonePosition(HumanBodyBones.Head).y;
        }

        isPlaying = false;
        isCountingDown = false;

        if (bpm < 10) bpm = 10;

        if (scoreDisplay != null)
        {
            scoreDisplay.gameObject.SetActive(false);
        }

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        InitializeTeacherData();
        UpdateBeatInterval();
    }


    public void SetBPM(float newBPM)
    {
        bpm = Mathf.Max(10, newBPM); // 最低10 BPMとする
        UpdateBeatInterval();
        Debug.Log($"BPM updated to: {bpm}");
    }

    private void UpdateBeatInterval()
    {
        beatInterval = 60f / bpm;
        Debug.Log($"Beat interval updated to: {beatInterval} seconds per beat");
    }

    public void StartGame()
    {
        if (isCountingDown || isPlaying) return;

        isCountingDown = true;
        countdownValue = 3;
        countdownTimer = 1f;

        if (localPlayer != null)
        {
            localPlayer.TeleportTo(Vector3.zero, Quaternion.identity);
        }

        if (heightText != null)
        {
            heightText.text = $"Height: {cachedPlayerHeight:F2}m";
        }

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
            countdownText.text = countdownValue.ToString();
        }
    }

    private Vector3 ConvertToPolar(Vector3 cartesian)
    {
        float r = cartesian.magnitude; // 半径（距離）

        if (r <= 1e-6)
        {
            return new Vector3(0, 0, 0); // r=0の場合、角度も0に設定
        }

        float theta = Mathf.Atan2(cartesian.z, cartesian.x); // 水平方向の角度
        float phi = Mathf.Atan2(cartesian.y, Mathf.Sqrt(cartesian.x * cartesian.x + cartesian.z * cartesian.z)); // 垂直方向の角度

        return new Vector3(r, theta, phi); // θ, φはラジアン単位
    }

    private void RecordPlayerData(int beatIndex, Vector3 leftHandPolar, Vector3 rightHandPolar)
    {
        if (beatIndex < 0 || beatIndex >= 32) return;

        // 極座標データをそのまま保存
        playerDataLeft[beatIndex] = leftHandPolar;
        playerDataRight[beatIndex] = rightHandPolar;

        // デバッグログ：極座標データを出力
        Debug.Log($"[DEBUG] Left Hand Polar: r={leftHandPolar.x:F2}, θ={Mathf.Rad2Deg * leftHandPolar.y:F2}°, φ={Mathf.Rad2Deg * leftHandPolar.z:F2}°");
        Debug.Log($"[DEBUG] Right Hand Polar: r={rightHandPolar.x:F2}, θ={Mathf.Rad2Deg * rightHandPolar.y:F2}°, φ={Mathf.Rad2Deg * rightHandPolar.z:F2}°");
    }

    private float CalculateScore(Vector3 playerPolar, Vector3 teacherPolar)
    {
        float adjustedR = teacherPolar.x * (cachedPlayerHeight / 1.7f);
        float distanceR = Mathf.Abs(playerPolar.x - adjustedR);

        float distanceTheta = CalculateAngleDifference(playerPolar.y, teacherPolar.y) / 180f;
        float distancePhi = CalculateAngleDifference(playerPolar.z, teacherPolar.z) / 180f;

        float weightedError = (distanceR * 3f) + (distanceTheta * 1.5f) + (distancePhi * 1.5f);
        float finalScore = Mathf.Clamp(100 - weightedError * 50, 0, 100);

        // デバッグログ
        Debug.Log($"[DEBUG] Player Polar: R={playerPolar.x:F2}, θ={playerPolar.y:F2}, φ={playerPolar.z:F2}");

        return finalScore;
    }

    private float CalculateAngleDifference(float angle1, float angle2)
    {
        float diff = Mathf.Abs(angle1 - angle2);
        return Mathf.Min(diff, 360f - diff);
    }

    private void EndGame()
    {
        isPlaying = false;

        float[] quarterAverages = new float[4];
        for (int i = 0; i < 4; i++)
        {
            float totalScore = 0;
            for (int j = 0; j < 8; j++)
            {
                int index = (i * 8) + j;

                if (index < teacherDataLeft.Length && index < teacherDataRight.Length)
                {
                    // 左右の手の極座標データを直接利用
                    Vector3 currentLeftHandPolar = playerDataLeft[index];
                    Vector3 currentRightHandPolar = playerDataRight[index];

                    // スコア計算
                    float leftScore = CalculateScore(currentLeftHandPolar, teacherDataLeft[index]);
                    float rightScore = CalculateScore(currentRightHandPolar, teacherDataRight[index]);

                    totalScore += (leftScore + rightScore) / 2f;
                }
                else
                {
                    Debug.LogWarning($"Index out of range: {index}");
                }
            }

            // 小節ごとの平均スコア
            quarterAverages[i] = totalScore / 8f;
        }

        // スコア表示に渡す
        if (scoreDisplay != null)
        {
            scoreDisplay.DisplayScores(quarterAverages);
        }
        else
        {
            Debug.LogError("ScoreDisplay is not assigned or null.");
        }
}

    void Update()
    {
        if (localPlayer == null) return;

        Vector3 origin = new Vector3(0, cachedPlayerHeight * 0.8f, 0);

        Vector3 leftHandPosition = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position;
        Vector3 rightHandPosition = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position;

        Vector3 leftHandPolar = ConvertToPolar(leftHandPosition - origin);
        Vector3 rightHandPolar = ConvertToPolar(rightHandPosition - origin);

        if (leftControllerText != null)
        {
            leftControllerText.text = $"Left Hand: r={leftHandPolar.x:F2}, θ={Mathf.Rad2Deg * leftHandPolar.y:F2}°, φ={Mathf.Rad2Deg * leftHandPolar.z:F2}°";
        }

        if (rightControllerText != null)
        {
            rightControllerText.text = $"Right Hand: r={rightHandPolar.x:F2}, θ={Mathf.Rad2Deg * rightHandPolar.y:F2}°, φ={Mathf.Rad2Deg * rightHandPolar.z:F2}°";
        }

        if (isCountingDown)
        {
            countdownTimer -= Time.deltaTime;
            if (countdownTimer <= 0)
            {
                countdownValue--;
                if (countdownValue > 0)
                {
                    countdownText.text = countdownValue.ToString();
                    countdownTimer = 1f;
                }
                else
                {
                    isCountingDown = false;
                    isPlaying = true;
                    currentBeat = 0;
                    nextBeatTime = Time.time + beatInterval;
                    if (countdownText != null)
                    {
                        countdownText.gameObject.SetActive(false);
                    }
                }
            }
        }

        if (!isPlaying || currentBeat >= 32) return;

        if (Time.time >= nextBeatTime)
        {
            metronome.Play();

            // 極座標データを記録
            RecordPlayerData(currentBeat, leftHandPolar, rightHandPolar);

            nextBeatTime += beatInterval;
            currentBeat++;

            if (currentBeat >= 32)
            {
                EndGame();
            }
        }
    }
}
