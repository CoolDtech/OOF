using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public int currentLevel = 1, maxLevel = 1;
    protected ItemData.Evolution[] evolutionData;
    protected PlayerInventory inventory;
    protected PlayerStats owner;

    public virtual void Initialise(ItemData data)
    {
        maxLevel = data.maxLevel;

        //追蹤所有配方都在被包裏才能存取進化data
        evolutionData = data.evolutionData;

        //未來有更有效率的方法吧這邊
        inventory = FindObjectOfType<PlayerInventory>();
        owner = FindObjectOfType<PlayerStats>();
    }

    //這個方法能呼叫所有現在能夠進化的武器
    public virtual ItemData.Evolution[] CanEvolve()
    {
        List<ItemData.Evolution> possibleEvolutions = new List<ItemData.Evolution>();

        //依次檢查列表並確定那些在背包裡
        foreach (ItemData.Evolution e in evolutionData)
        {
            if (CanEvolve(e)) possibleEvolutions.Add(e);
        }

        return possibleEvolutions.ToArray();
    }

    //檢查道具是否達到進化條件
    public virtual bool CanEvolve(ItemData.Evolution evolution, int levelUpAmount =1)
    {
        //如果道具還沒到等級就不能進化
        if(evolution.evolutionLevel > currentLevel + levelUpAmount)
        {
            Debug.LogWarning(string.Format("進化失敗，當前等級{0}，進化需求等級{1}", currentLevel, evolution.evolutionLevel));
            return false;
        }

        //檢查是否所有素材都在背包內
        foreach (ItemData.Evolution.Config c in evolution.catalysts)
        {
            Item item = inventory.Get(c.itemType);
            if(!item || item.currentLevel < c.level)
            {
                Debug.LogWarning(string.Format("進化失敗，缺少{0}", c.itemType.name));
                return false;
            }
        }
        return true;

    }

    
    //AttemptEvolution方法會生成新的武器，並且消耗相應的素材武器&道具
    public virtual bool AttemptEvolution(ItemData.Evolution evolutionData, int levelUpAmount = 1)
    {
        if(!CanEvolve(evolutionData, levelUpAmount))
        return false;

        //是否該吸收武器/道具?
        bool consumePassives = (evolutionData.consumes & ItemData.Evolution.Consumption.passive) > 0;
        bool consumeWeapons = (evolutionData.consumes & ItemData.Evolution.Consumption.weapons) > 0;

        //重複檢測所有配方並檢查是否應該消耗
        foreach (ItemData.Evolution.Config c in evolutionData.catalysts)
        {
            if(c.itemType is PassiveData && consumePassives) inventory.Remove(c.itemType, true);
            if(c.itemType is WeaponData && consumeWeapons) inventory.Remove(c.itemType, true);
        }

        //是否該消耗自己
        if(this is Passive && consumePassives) inventory.Remove((this as Passive).data, true);
        else if(this is Weapon && consumeWeapons) inventory.Remove((this as Weapon).data, true);

        //加入新武器到背包裏
        inventory.Add(evolutionData.outcome.itemType);

        return true;
    }

    public virtual bool CanLevelUp()
    {
        return currentLevel <= maxLevel;
    }
    
    //每當道具提升等級，嘗試讓其進化
    public virtual bool DoLevelUp()
    {
        if(evolutionData == null) return true;

        //當武器達到進化條件時嘗試進化列表上的武器
        foreach (ItemData.Evolution e in evolutionData)
        {
            if (e.condition == ItemData.Evolution.Condition.auto)
            AttemptEvolution(e);
        }

        return true;
    }

    //裝備時的效果
    public virtual void OnEquip(){}
    //移除裝備時的效果
    public virtual void OnUnequip(){}

}
