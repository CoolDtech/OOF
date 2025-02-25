using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Aura就是一種範圍DOT效果
/// 反正這段就是用來產生範圍持續傷害用的
/// </summary>

public class Aura : WeaponEffect
{
    
    Dictionary<EnemyStats, float> affectedTargets = new Dictionary<EnemyStats, float>();
    List<EnemyStats> targetToUnaffect = new List<EnemyStats>();

    //每幀更新
    void Update()
    {
        Dictionary<EnemyStats, float> affectedTargsCopy = new Dictionary<EnemyStats, float>(affectedTargets);

        //在所有受Aura影響的目標中迴圈，並減少Aura的cooldown
        //如果cooldown達到0，就造成傷害
        foreach (KeyValuePair<EnemyStats, float> pair in affectedTargsCopy)
        {
            affectedTargets[pair.Key] -= Time.deltaTime;
            if (pair.Value <= 0)
            {
                if (targetToUnaffect.Contains(pair.Key))
                {
                    //如果目標失去標記，就將其從受影響目標中移除
                    affectedTargets.Remove(pair.Key);
                    targetToUnaffect.Remove(pair.Key);
                }
                else
                {
                    //重設冷卻時間並造成傷害
                    Weapon.Stats stats = weapon.GetStats();
                    affectedTargets[pair.Key] = stats.cooldown;
                    pair.Key.TakeDamage(GetDamage(), transform.position, stats.Knockback);
                    //Debug.Log("Dealing " + GetDamage() + " to " + pair.Key.gameObject.name);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent(out EnemyStats es))
        {
            //Debug.Log("No Hit");
            //如果目標尚未被該Aure影響，添加他到受影響清單內
            if(!affectedTargets.ContainsKey(es))
            {
                //Debug.Log("Hit! Number of affected targets: " + affectedTargets.Count);
                //總是從間隔為0開始計算
                //這樣目標會自下一幀開始受傷害
                affectedTargets.Add(es, 0);
            }
            else
            {
                if(targetToUnaffect.Contains(es))
                {
                    targetToUnaffect.Remove(es);
                }
            }

        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.TryGetComponent(out EnemyStats es))
        {
            //不要直接將目標從影響清單內移除
            //因為我們仍須追蹤其cooldown
            if(affectedTargets.ContainsKey(es))
            {
                targetToUnaffect.Add(es);
            }
        }
    }
}
