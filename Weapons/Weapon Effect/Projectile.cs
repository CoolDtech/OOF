using UnityEngine;

/// <summary>
/// 所有投射物的組成物件，投射物會往朝向發出，命中敵人造成傷害
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : WeaponEffect
{
    
    public enum DamageSource { projectile, owner };
    public DamageSource damageSource = DamageSource.projectile;
    public bool hasAutoAim = false;
    public Vector3 rotationSpeed = new Vector3(0, 0, 0);

    protected Rigidbody2D rb;
    protected int piercing;

    //在第一幀之前呼叫開始

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Weapon.Stats stats = weapon.GetStats();
        if (rb.bodyType == RigidbodyType2D.Dynamic)//如果投射物是Dynamic Type(動態，不受外力影響移動)
        {
            rb.angularVelocity = rotationSpeed.z;
            rb.velocity = transform.right * stats.speed;
        }

        //根據武器數值的area來增加武器大小
        float area = stats.area == 0 ? 1 : stats.area;
    transform.localScale = new Vector3(
        area * Mathf.Sign(transform.localScale.x),
        area * Mathf.Sign(transform.localScale.y), 1
    );
        //設定穿透數值
        piercing = stats.piercing;
        //壽命到了就清除投射物
        if(stats.lifespan >0) Destroy(gameObject, stats.lifespan);
        //自動找目標的投射物方法
        if(hasAutoAim) AcquireAutoAimFacing();
    }
    
    public virtual void AcquireAutoAimFacing()
    {
        float aimAngle;//方向導航

        //搜尋所有畫面上的敵人
        EnemyStats[] targets = FindObjectsOfType<EnemyStats>();

        //如果至少有一個敵人，選擇隨機敵人
        //然後選獲得隨機角度
        if(targets.Length > 0)
        {
            EnemyStats selectedTarget = targets[Random.Range(0, targets.Length)];
            Vector2 difference = selectedTarget.transform.position - transform.position;
            aimAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        }
        else
        {
            aimAngle = Random.Range(0f, 360f);
        }

        //標記出投射物瞄準方向

        transform.rotation = Quaternion.Euler(0, 0, aimAngle);
        
    }
    //每幀更新
    
    protected virtual void FixedUpdate()
    {   //如果碰撞箱是Kinematic type(運動學，根據受力影響運動)
        if(rb.bodyType == RigidbodyType2D.Kinematic)
        {
            Weapon.Stats stats = weapon.GetStats();
            transform.position += transform.right * stats.speed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position);
            transform.Rotate(rotationSpeed * Time.fixedDeltaTime);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        EnemyStats es = other.GetComponent<EnemyStats>();
        BreakableProps p = other.GetComponent<BreakableProps>();

        //敵人跟可破壞物件的破撞箱
        if(es)
        {
            //對owner物件造成damage source
            //根據owner來計算擊退方向而不是投射物
            Vector3 source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;

            //造成傷害並清除投射物
            es.TakeDamage(GetDamage(), source);

            Weapon.Stats stats = weapon.GetStats();
            piercing--;
            if(stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }    
        else if (p)
        {
            p.TakeDamage(GetDamage());
            piercing--;
                
            Weapon.Stats stats = weapon.GetStats();
            if(stats.hitEffect)
            
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }

        }
        

        //穿透力消耗光後清除投射物
        if(piercing <=0) Destroy(gameObject);

    }
       

    

}
