using UnityEngine;

/// <summary>
/// 用來取代PassiveItemScriptableObject class
/// 用來將道具的所有數值存在同一個物件中，這樣比較方便整潔
/// 如果繼續用用來取代PassiveItemScriptableObject那你要做到死
/// </summary>
[CreateAssetMenu(fileName = "Passive Data", menuName = "2D Top-down Rogue-like/Passive Data")]
public class PassiveData : ItemData
{

    public Passive.Modifier baseStats;
    public Passive.Modifier[] growth;

    public Passive.Modifier GetLevelData(int level)
    {
        //獲取下一等級數值
        if (level -2 < growth.Length)
            return growth[level -2];

        //返回空值並彈出警告
        Debug.LogWarning(string.Format("這道具沒有{0}的升級配置(你沒設定啊大哥)", level));
        return new Passive.Modifier();
    }
    
}
