using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneBehaviour : ProjectileWeaponBehaviour
{
    public int currentBounces; // 用來追蹤當前剩餘反彈次數
    bool hitBorder;
    protected override void Start()
    {
        base.Start();
        currentBounces = weaponData.Pierce; // 使用WeaponScriptableObject的Pierce值作為反彈次數
        hitBorder = false;
    }

    void Update()
    {
        
        transform.position += direction.normalized * currentSpeed * Time.deltaTime;    //Set the movement of the knife

    }

    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;

    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        // 檢查是否碰到敵人
        if(col.CompareTag("Enemy"))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                enemy.TakeDamage(GetCurrentDamage(), transform.position); // 對敵人造成傷害
                BounceOrDestroy(col.transform); // 反彈或銷毀
            }
        }
        // 檢查是否碰到可破壞物品
        else if(col.CompareTag("Props"))
        {
            if(col.gameObject.TryGetComponent(out BreakableProps breakable))
            {
                breakable.TakeDamage(GetCurrentDamage());
                BounceOrDestroy(col.transform);
            }
        }
        else if(col.CompareTag("Border"))
        {          
            hitBorder = true;     
            BounceOrDestroy(col.transform);
        }
    }
    // 根據Pierce反彈次數來進行處理
    private void BounceOrDestroy(Transform hitTransform)
    {
        if(currentBounces > 0)
        {
            currentBounces--; // 減少反彈次數

            //轉向
            float randomAngle = Random.Range(1f, -1f);
            if(!hitBorder)
            {if(randomAngle > 0){randomAngle = 90f;}else {randomAngle = -90f;}}
            else
            {
                randomAngle = 180f;
            } 

            direction = Quaternion.Euler(0, 0, randomAngle) * direction;
            hitBorder = false;

            // 如果反彈次數耗盡，銷毀飛刀
            if (currentBounces <= 0)
            {
                Destroy(gameObject);
            }

        }
        else
        {
            // 如果沒有剩餘反彈次數，銷毀骨頭
            Destroy(gameObject);
        }
    }    
}