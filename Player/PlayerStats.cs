using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerStats : MonoBehaviour
{
    CharacterData characterData;
    public CharacterData.Stats baseStats;
    [SerializeField] CharacterData.Stats actualStats;



    float health;

    #region Current Stats Properties
    
    public float CurrentHealth
    {
        //取得當前生命值
        get { return health; }
        //設定currentHealth時UI介面同樣會更新
        set
        {

            if(health != value)
            {
                health = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentHealthDisplay.text = string.Format(
                    "生命：{0} / {1}",
                    health, actualStats.maxHealth
                    );
                }
                //更新即時生命值數值
                //可以在這裡加入額外算式來處理數值變化
            }
        }
    }

    public float MaxHealth
    {
        //取得當前生命值
        get { return actualStats.maxHealth; }
        
        //設定maxHealth時UI介面同樣會更新
        set
        {

            if(actualStats.maxHealth != value)
            {
                health = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentHealthDisplay.text = string.Format(
                    "生命：{0} / {1}",
                    health, actualStats.maxHealth
                    );
                }
                //更新即時生命值數值
                //可以在這裡加入額外算式來處理數值變化
            }
        }
    }
    public float CurrentRecovery
    {
        get {return Recovery; }
        set {Recovery = value; }
    }

    public float Recovery
    {
        //取得當前恢復力
        get { return actualStats.recovery; }
        set
        {
            if(actualStats.recovery != value)
            {
                actualStats.recovery = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentRecoveryDisplay.text = "恢復力：" + actualStats.recovery;
                }
                //更新即時恢復力數值
                //可以在這裡加入額外算式來處理數值變化
            }
        }
    }

    public float CurrentMoveSpeed
    {
        get { return MoveSpeed; }
        set { MoveSpeed = value; }
    }
    public float MoveSpeed
    {
        //取得當前移動速度
        get { return actualStats.moveSpeed; }
        set
        {
            if(actualStats.moveSpeed != value)
            {
                actualStats.moveSpeed = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentMoveSpeedDisplay.text = "移動速度：" + actualStats.moveSpeed;
                }
                //更新即時移動速度數值
                //可以在這裡加入額外算式來處理數值變化
            }
        }
    }

    public float CurrentMight
    {
        get { return Might;}
        set { Might = value;}
    }
    public float Might
    {
        //取得當前力量
        get { return actualStats.might; }
        set
        {
            if(actualStats.might != value)
            {
                actualStats.might = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentMightDisplay.text = "力量：" + actualStats.might;
                }
                //更新即時力量數值
                //可以在這裡加入額外算式來處理數值變化
            }
        }
    }

    public float CurrentProjectileSpeed
    {
        get { return Speed; }
        set { Speed = value; }
    }

    public float Speed
    {
        //取得當前投射物速度
        get { return actualStats.speed; }
        set
        {
            if(actualStats.speed != value)
            {
                actualStats.speed = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentProjectileSpeedDisplay.text = "投射物速度：：" + actualStats.speed;
                }
                //更新即時投射物速度數值
                //可以在這裡加入額外算式來處理數值變化
            }
        }
    }

    public float CurrentMagnet
    {
        get { return Magnet; }
        set {Magnet = value; }
    }
    public float Magnet
    {
        //取得當前磁力
        get { return actualStats.magnet; }
        set
        {
            if(actualStats.magnet != value)
            {
                actualStats.magnet = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentMagnetDisplay.text = "磁力：" + actualStats.magnet;
                }
                //更新即時磁力數值
                //可以在這裡加入額外算式來處理數值變化
            }
        }
    }
    
    
    
    #endregion


    public ParticleSystem damageEffect;

    

    //玩家的等級跟經驗    
    [Header("Experience/Level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;

    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    }

    //無敵幀
    [Header("I-Frames")]
    public float invincibilityDuration;
    float invincibilityTimer;
    bool isInvincible;

    public List<LevelRange> levelRanges;

    PlayerInventory inventory;
    public int weaponIndex;
    public int passiveItemIndex;

    [Header("UI")]
    public Image healthBar;
    public Image expBar;
    public TMP_Text levelText;

    PlayerAnimation playerAnimator;

    //public GameObject secondWeaponTest;
    //public GameObject firstPassiveItemTest, SeconePassiveItemTest;


    void Awake()
    {
        characterData = CharacterSelector.GetData();
        if(CharacterSelector.instance)
        CharacterSelector.instance.DestorySingleton();


        inventory = GetComponent<PlayerInventory>();

        //加入變數
        baseStats = actualStats = characterData.stats;
        health = actualStats.maxHealth;

        playerAnimator = GetComponent<PlayerAnimation>();
        if(characterData.controller)
            playerAnimator.SetAnimatorController(characterData.controller);
    }

    void Start()
    {
        //生成起始武器
        inventory.Add(characterData.StartingWeapon);
        //生成經驗需求
        experienceCap = levelRanges[0].experienceCapIncrease;

        //顯示當前能力值設定
        GameManager.instance.currentHealthDisplay.text = "生命：" + CurrentHealth;
        GameManager.instance.currentRecoveryDisplay.text = "恢復力：" + CurrentRecovery;
        GameManager.instance.currentMoveSpeedDisplay.text = "移動速度：" + CurrentMoveSpeed;
        GameManager.instance.currentMightDisplay.text = "力量：" + CurrentMight + "x";
        GameManager.instance.currentProjectileSpeedDisplay.text = "投射物速度：" + CurrentProjectileSpeed;
        GameManager.instance.currentMagnetDisplay.text = "磁力：" + CurrentMagnet;

        GameManager.instance.AssignChosenCharacterUI(characterData);

        UpdateHealthBar();
        UpdateExpBar();
        UpdateLevelText();
    }

    void Update()
    {
        if(invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
        else if(isInvincible)
        {
            isInvincible = false;
        }

        Recover();
    }

    public void RecalculateStats()
    {
        actualStats = baseStats;
        foreach (PlayerInventory.Slot s in inventory.passiveSlots)
        {
            Passive p = s.item as Passive;
            if(p)
            {
                actualStats += p.GetBoosts();
            }
        }
    }

    public void IncreaseExperience(int amount)
    {
        experience += amount;

        LevelUpChecker();

        UpdateExpBar();
        
    }

    void LevelUpChecker()
    {
        if(experience >= experienceCap)
        {
            level++;
            experience -= experienceCap;

            int experienceCapIncrease = 0;
            foreach (LevelRange range in levelRanges)
            {
                if(level >= range.startLevel && level <= range.endLevel)
                {
                    experienceCapIncrease = range.experienceCapIncrease;
                    break;
                }
            }
            experienceCap += experienceCapIncrease;

            UpdateLevelText();
            
            GameManager.instance.StartLevelUp();           

        }
    }  

    void UpdateExpBar()
    {
        // 更新經驗值條
        expBar.fillAmount = (float)experience / experienceCap;
    }

    void UpdateLevelText()
    {
        levelText.text = "等級" + level.ToString();
    }

    public void TakeDamage(float dmg)
    {   
        if(!isInvincible)
        {
            
            CurrentHealth -= dmg;


            //如果受到傷害就執行
            if(damageEffect) Destroy(Instantiate(damageEffect, transform.position, Quaternion.identity), 5f);

            invincibilityTimer = invincibilityDuration;
            isInvincible = true;
            
            if(CurrentHealth <= 0)
            {
                Kill();
            }

            UpdateHealthBar();
        }
    }



    void UpdateHealthBar()
    {
        //更新生命條
        healthBar.fillAmount = CurrentHealth / actualStats.maxHealth;
    }

    public void Kill()
    {
        if(!GameManager.instance.isGameOver)
        {
            GameManager.instance.AssignLevelReachedUI(level);
            GameManager.instance.AssignChosenWeaponsAndPassiveItemsUI(inventory.weaponSlots, inventory.passiveSlots);
            GameManager.instance.GameOver();
        }
    }

    public void RestoreHealth(float amount)
    {
        if(CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += amount;
            if(CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }

        }
    }

    void Recover()
    {
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += CurrentRecovery * Time.deltaTime;
            if(CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }
        }
        UpdateHealthBar();
    }


    [System.Obsolete("老方法，應該晚點會刪掉")]
    public void SpwanWeapon(GameObject weapon)
    {
        if(weaponIndex >= inventory.weaponSlots.Count -1)
        {
            Debug.LogError("你包滿了");
            return;
        }

        GameObject spwanedWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        spwanedWeapon.transform.SetParent(transform); //把武器設定為玩家的子集
        //inventory.AddWeapon(weaponIndex, spwanedWeapon.GetComponent<WeaponController>()); //將武器加入武器格

        weaponIndex++;
    }

    [System.Obsolete("現在不用直接生成道具了")]
    public void SpwanPassiveItem(GameObject passiveItem)
    {
        if(passiveItemIndex >= inventory.passiveSlots.Count -1)
        {
            Debug.LogError("你包滿了");
            return;
        }

        GameObject spawnedPassiveItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
        spawnedPassiveItem.transform.SetParent(transform); //把道具設定為玩家的子集
        //inventory.AddPassiveItem(passiveItemIndex, spawnedPassiveItem.GetComponent<PassiveItem>()); //將道具加入道具格

        passiveItemIndex++;
    }


}
