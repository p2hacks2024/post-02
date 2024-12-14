using UdonSharp;
using UnityEngine;
using TMPro;

public class WotageiScoreDisplay : UdonSharpBehaviour
{
    public TextMeshProUGUI techniqueNameText; // 技名表示用テキスト
    public TextMeshProUGUI scoreTableText; // スコア表表示用テキスト
    public GameObject displayPanel; // スコア表示全体を制御するパネル

    private string techniqueName = "Amateras"; // 技名
    private string playerName = "masaBa"; // プレイヤー名

    private string[] quarterScores = new string[4]; // 各クォーターのスコアランク
    private string totalScore; // トータルスコアのランク

    private void Start()
    {
        // ゲーム開始時にはスコア表示を非表示
        if (displayPanel != null)
        {
            displayPanel.SetActive(false);
        }
    }

    /// <summary>
    /// スコアを表示する
    /// </summary>
    /// <param name="quarterAverages">各クォーターの平均スコア</param>
    public void DisplayScores(float[] quarterAverages)
    {
        if (quarterAverages == null || quarterAverages.Length != 4)
        {
            Debug.LogError("quarterAverages must contain exactly 4 elements.");
            return;
        }

        // 各クォーターのスコアランクを計算
        for (int i = 0; i < 4; i++)
        {
            quarterScores[i] = GetGrade(quarterAverages[i]);
        }

        // トータルスコアの計算
        float totalAverage = 0f;
        for (int i = 0; i < 4; i++)
        {
            totalAverage += quarterAverages[i];
        }
        totalAverage /= 4f;
        totalScore = GetGrade(totalAverage);

        // スコアUIを設定
        if (techniqueNameText != null)
        {
            techniqueNameText.text = $"{techniqueName}";
        }

        if (scoreTableText != null)
        {
            scoreTableText.text = GenerateScoreTable(quarterAverages);
        }

        // スコア表示を有効化
        if (displayPanel != null)
        {
            displayPanel.SetActive(true);
        }
    }

    /// <summary>
    /// スコア表を生成する
    /// </summary>
    /// <param name="quarterAverages">各クォーターのスコア</param>
    /// <returns>フォーマットされたスコア表</returns>
    private string GenerateScoreTable(float[] quarterAverages)
    {
        return string.Format(
            "------------------------------------------------\n" +
            "Player     1/4    2/4    3/4    4/4    Total\n" +
            "{0}     {1:F1} ({2})  {3:F1} ({4})  {5:F1} ({6})  {7:F1} ({8})  <size=+2>{9}</size>",
            playerName,
            quarterAverages[0], quarterScores[0],
            quarterAverages[1], quarterScores[1],
            quarterAverages[2], quarterScores[2],
            quarterAverages[3], quarterScores[3],
            totalScore
        );
    }

    /// <summary>
    /// スコアからランクを取得する
    /// </summary>
    /// <param name="score">スコア</param>
    /// <returns>ランク文字列 (S, A, B, C, D)</returns>
    private string GetGrade(float score)
    {
        if (score >= 90) return "S";
        if (score >= 75) return "A";
        if (score >= 50) return "B";
        if (score >= 25) return "C";
        return "D";
    }
}
