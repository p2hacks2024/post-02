using UdonSharp;
using UnityEngine;
using TMPro;

public class BPMController : UdonSharpBehaviour
{
    public WotageiScoring wotageiScoring; // BPMを設定する対象のスクリプト
    public TMP_InputField bpmInputField; // BPMの値を表示・編集するUI

    private float bpm = 120f;

    void Start()
    {
        if (bpmInputField != null)
        {
            bpmInputField.text = bpm.ToString(); // 初期値を表示
        }
    }

    /// <summary>
    /// 入力欄の値が変更されたときに呼び出される
    /// </summary>
    public void GetInputName()
    {
        if (bpmInputField != null)
        {
            if (float.TryParse(bpmInputField.text, out float newBPM))
            {
                bpm = Mathf.Max(10, newBPM); // BPMは最低10以上に制限
                ApplyBPM();
            }
            else
            {
                Debug.LogWarning("[BPMController] Invalid BPM input.");
                bpmInputField.text = bpm.ToString(); // 無効な値の場合、元の値を復元
            }
        }
    }

    private void ApplyBPM()
    {
        if (wotageiScoring != null)
        {
            wotageiScoring.SetBPM(bpm);
            Debug.Log($"[BPMController] BPM applied: {bpm}");
        }
    }
}
