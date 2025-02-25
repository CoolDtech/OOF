using UnityEngine;

/// <summary>
/// 附加在所有武器的Prefab上
/// 武器的Prefab跟WeaponDataScriptableObject共同運作，用來管理所有武器的行為
/// </summary>

public abstract class Weapon : Item
{
    [System.Serializable]
    public struct Stats
    {
        public string name, description;

        [Header("Visuals")]
        public Projectile projectilePrefab;
        public Aura auraPrefab;
        public ParticleSystem hitEffect;
        public Rect spawnVariance;

        [Header("Values")]
        public float lifespan;
        public float damage, damageVariance, area, speed, cooldown, projectileInterval, Knockback;
        public int number, piercing, maxInstances;

        //使用operator把兩個等級的能力相加

        public static Stats operator +(Stats s1,Stats s2)
        {
            Stats result = new Stats();
            result.name = s2.name ?? s1.name;
            result.description = s2.description ?? s1.description;
            result.projectilePrefab = s2.projectilePrefab ?? s1.projectilePrefab;
            result.auraPrefab = s2.auraPrefab ?? s1.auraPrefab;
            result.hitEffect = s2.hitEffect == null ? s1.hitEffect : s2.hitEffect;
            result.spawnVariance = s2.spawnVariance;
            result.lifespan = s1.lifespan + s2.lifespan;
            result.damage = s1.damage + s2.damage;
            result.damageVariance = s1.damageVariance + s2.damageVariance;
            result.area = s1.area + s2.area;
            result.speed = s1.speed + s2.speed;
            result.cooldown = s1.cooldown + s2.cooldown;
            result.number = s1.number + s2.number;
            result.piercing = s1.piercing + s2.piercing;
            result.projectileInterval = s1.projectileInterval + s2.projectileInterval;
            result.Knockback = s1.Knockback + s2.Knockback;
            return result;
        }

        public float GetDamage()
        {
            return damage + Random.Range(0, damageVariance);
        }

    }


    protected Stats currentStats;
    public WeaponData data;
    protected float currentCooldown;
    protected PlayerMovement movement; //反應玩家行動
    //調用initialise來進行態創建的武器的設置。
    public virtual void Initialise(WeaponData data)
    {

        base.Initialise(data);
        this.data = data;
        currentStats = data.baseStats;
        movement = GetComponentInParent<PlayerMovement>();
        currentCooldown = currentStats.cooldown;
    }

    protected virtual void Awake()
    {
        if(data) currentStats = data.baseStats;
    }

    protected virtual void Start()
    {
        if(data)
        {
            Initialise(data);
        }
    }

    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if(currentCooldown <= 0f)
        {
            Attack(currentStats.number);
        }
    }


    public override bool DoLevelUp()
    {
        base.DoLevelUp();
        if(!CanLevelUp())
        {
            Debug.LogWarning(string.Format("沒辦法將{0}升到{1}級，已達等級上限{2}", name, currentLevel, data.maxLevel));
            return false;
        }

        currentStats += data.GetLevelData(++currentLevel);
        return true;
    }
    //檢查武器是否能攻擊
    public virtual bool CanAttack()
    {
        return currentCooldown <= 0;
    }
    //就這樣
    protected virtual bool Attack(int attackCount = 1)
    {
        if(CanAttack())
        {
            currentCooldown += currentStats.cooldown;
            return true;
        }
        return false;
    }
    public virtual float GetDamage()
    {
        return currentStats.GetDamage() * owner.CurrentMight;
    }

    public virtual Stats GetStats() { return currentStats; }
}
