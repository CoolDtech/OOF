using UnityEngine;


///<summary>
///根據武器種類產生的效果物件 例如：投射物、靈氣、脈衝等等
///<summary>


public abstract class WeaponEffect : MonoBehaviour
{
    [HideInInspector] public PlayerStats owner;
    [HideInInspector] public Weapon weapon;

    public float GetDamage()
    {
        return weapon.GetDamage();
    }

}
