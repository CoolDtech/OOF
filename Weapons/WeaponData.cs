using UnityEngine;

/// <summary>
/// 用來取代WeaponScriptableObject class，可以只將所有武器進化的資料存在一個物件裡
/// 不用每次創造一個新武器就要做一堆物件
/// </summary>
[CreateAssetMenu(fileName = "Weapon Data", menuName = "2D Top-down Rogue-like/Weapon Data")]
public class WeaponData : ItemData
{

    [HideInInspector] public string behaviour;
    public Weapon.Stats baseStats;
    public Weapon.Stats[] linearGrowth;
    public Weapon.Stats[] randomGrowth;

    //下一級的成長/敘述

    public Weapon.Stats GetLevelData(int level)
    {
    //取得下一級的能力
        if(level -2 < linearGrowth.Length)
            return linearGrowth[level - 2];

        if(randomGrowth.Length > 0)
            return randomGrowth[Random.Range(0, randomGrowth.Length)];

        Debug.LogWarning(string.Format("這個武器沒有Level{0}的配置!", level));
        return new Weapon.Stats();

    }
}
