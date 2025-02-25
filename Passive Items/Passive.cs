using UnityEngine;

/// <summary>
/// 用來處理獲得道具後提升角色能力的class
/// </summary>

public class Passive : Item
{
    public PassiveData data;
    [SerializeField] CharacterData.Stats currentBoots;
    [System.Serializable]

    public struct Modifier
    {
        public string name, description;
        public CharacterData.Stats boosts;
    }

    //初始化道具生成
    public virtual void Initialise(PassiveData data)
    {
        base.Initialise(data);
        this.data = data;
        currentBoots = data.baseStats.boosts;
    }

    public virtual CharacterData.Stats GetBoosts()
    {
        return currentBoots;
    }

    //升級武器1等並計算相應能力值
    public override bool DoLevelUp()
    {
        base.DoLevelUp();

        //防止在滿等時升級
        if(!CanLevelUp())
        {
            Debug.LogWarning(string.Format("這玩意兒{0}已經{1}等，而滿等是{2}等", name, currentLevel, maxLevel));
            return false;
        }

        //否則為下一等級的武器增加數值
        currentBoots += data.GetLevelData(++currentLevel).boosts;
        return true;
    }
}
