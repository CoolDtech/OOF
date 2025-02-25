using UnityEngine;

public class ProjectileWeapon : Weapon
{
    protected float currentAttackInterval; //攻擊間隔
    protected int currentAttackCount; // 攻擊次數

    protected override void Update()
    {
        base.Update(); 

        //攻擊間隔大於或小於0都攻擊
        if(currentAttackInterval > 0)
        {
            currentAttackInterval -= Time.deltaTime;
            if(currentAttackInterval <= 0) Attack(currentAttackCount);
        }
    }

    public override bool CanAttack()
    {
        if(currentAttackCount > 0) return true;//如果還有攻擊次數就回傳true
        return base.CanAttack();
    }

    protected override bool Attack(int attackCount = 1)
    {   
        //如果投射物的prefab沒有設定就產生警告
        if(!currentStats.projectilePrefab)
        {
            Debug.LogWarning(string.Format("{0}投射物沒有被設定", name));
            currentCooldown = data.baseStats.cooldown;
            return false;
        }
        
        //是否能攻擊
        if(!CanAttack()) return false;
        //檢測偏角
        float spawnAngle = GetSpawnAngle();

        //生成投射物複製品
        Projectile prefab = Instantiate(
            currentStats.projectilePrefab,
            owner.transform.position + (Vector3)GetSpawnOffset(spawnAngle),
            Quaternion.Euler(0, 0, spawnAngle)
        );

        prefab.weapon = this;
        prefab.owner = owner;

        //當武器發動後重設冷卻時間
        if(currentCooldown <= 0)
            currentCooldown += currentStats.cooldown;

        attackCount--;
        //有沒有發動其他攻擊
        if(attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = data.baseStats.projectileInterval;
        }
        return true;
    }

    //取得武器朝向

    protected virtual float GetSpawnAngle()
    {
        return Mathf.Atan2(movement.lastMovedVector.y, movement.lastMovedVector.x) * Mathf.Rad2Deg;
    }

    //產生投射物隨機升成典
    //將投射物轉向spwanAngle
    protected virtual Vector2 GetSpawnOffset(float spawnAngle = 0)
    {
        return Quaternion.Euler(0, 0, spawnAngle)* new Vector2(
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax)
        );
    }

    

}
