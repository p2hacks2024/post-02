using UdonSharp;
using UnityEngine;

public class ButtonTrigger : UdonSharpBehaviour
{
    public WotageiScoring wotageiManager; // `WotageiScoring`スクリプトの参照

    public override void Interact()
    {
        // ボタンを押した際の処理
        if (wotageiManager != null)
        {
            wotageiManager.StartGame(); // `StartGame`メソッドを呼び出す
        }
    }
}
