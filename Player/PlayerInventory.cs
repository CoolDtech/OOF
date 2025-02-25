using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class Slot
    {
        public Item item;
        public Image image;

        public void Assign(Item assignedItem)
        {
            item = assignedItem;
            if (item is Weapon)
            {
                Weapon w = item as Weapon;
                image.enabled = true;
            }
            else
            {
                Passive p = item as Passive;
                image.enabled = true;
                image.sprite = p.data.icon;
            }
            Debug.Log(string.Format("添加{0}給玩家", item.name));
        }

        public void Clear()
        {
            item = null;
            image.enabled = false;
            image.sprite = null;
        }

        public bool IsEmpty(){ return item == null; }
    }
    public List<Slot> weaponSlots = new List<Slot>(6);
    public List<Slot> passiveSlots = new List<Slot>(6);

    [System.Serializable]
    public class UpgradeUI
    {
        public TMP_Text upgradeNameDisplay;
        public TMP_Text upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button upgradeButton;
    }

    [Header("UI Elements")]
    public List<WeaponData> availableWeapons = new List<WeaponData>();//武器升級選項的清單 
    public List<PassiveData> availablePassives = new List<PassiveData>();//道具升級選項的清單
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>();//畫面中升級選項的清單

    PlayerStats player;

    void Start()
    {
        player = GetComponent<PlayerStats>();
        Debug.Log("List: " + string.Join(", ", availableWeapons));
        Debug.Log("List: " + string.Join(", ", availablePassives));
        Debug.Log("List: " + string.Join(", ", upgradeUIOptions));

    }

    //檢查背包有相應的type
    public bool Has(ItemData type) {return Get(type); }

    public Item Get(ItemData type)
    {
        if (type is WeaponData) return Get(type as WeaponData);
        else if(type is PassiveData) return Get(type as PassiveData);
        return null;
    }

    //搜尋道具type
    public Passive Get(PassiveData type)
    {
        foreach (Slot s in passiveSlots)
        {
            Passive p = s.item as Passive;
            if (p.data == type)
                return p;
        }
        return null;
    }

    //搜尋武器type
    public Weapon Get(WeaponData type)
    {
        foreach (Slot s in weaponSlots)
        {
            Weapon w = s.item as Weapon;
            if(w.data == type)
                return w;
        }
        return null;
    }

    //移除<data>指定的武器type
    public bool Remove(WeaponData data, bool removeUpgradeAvailablitity = false)
    {
        //將這個武器從升級池內移除
        if(removeUpgradeAvailablitity) availableWeapons.Remove(data);

        for(int i = 0; i < weaponSlots.Count; i++)
        {
            Weapon w = weaponSlots[i].item as Weapon; 
            if(w.data == data)
            {
                weaponSlots[i].Clear();
                w.OnUnequip();
                Destroy(w.gameObject);
                return true;
            }
        }
        return false;
    }

    //移除<data>指定的道具type
    public bool Remove(PassiveData data, bool removeUpgradeAvailablitity = false)
    {
        //將這個道具從升級池中移除
        if(removeUpgradeAvailablitity) availablePassives.Remove(data);

        for(int i = 0; i < passiveSlots.Count; i++)
        {
            Passive p = passiveSlots[i].item as Passive;
            if(p.data == data)
            {
                passiveSlots[i].Clear();
                p.OnUnequip();
                Destroy(p.gameObject);
                return true;
            }
        }
        return false;

    }

    //如果傳入了 ItemData，請判斷其類型並調用相應的重載方法。
    //我們還有一個可選的布林值，用於將此項目從升級列表中移除。

    public bool Remove(ItemData data, bool removeUpgradeAvailablitity = false)
    {
        if (data is PassiveData) return Remove(data as PassiveData, removeUpgradeAvailablitity);
        else if (data is WeaponData) return Remove(data as WeaponData, removeUpgradeAvailablitity);
        return false;
    }

    //搜尋空格加入武器，回傳序號給該武器
    public int Add(WeaponData data)
    {
        int slotNum = -1;

        //嘗試搜尋空格
        for(int i = 0; i < weaponSlots.Capacity; i++)
        {            
            if(weaponSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }

        //如果沒空格就離開
        if(slotNum < 0) return slotNum;

        //創建武器
        //獲取我們要加入的武器type
        Type weaponType = Type.GetType(data.behaviour);

        if(weaponType != null)
        {
            //生成武器的gameObject
            GameObject go = new GameObject(data.baseStats.name + "Controller");
            Weapon spwanedWeapon = (Weapon)go.AddComponent(weaponType);
            spwanedWeapon.Initialise(data);
            spwanedWeapon.transform.SetParent(transform); //把武器設定至玩家的子集
            spwanedWeapon.transform.localPosition = Vector2.zero;
            spwanedWeapon.OnEquip();
            Debug.Log(spwanedWeapon);

            //將武器加入格子
            weaponSlots[slotNum].Assign(spwanedWeapon);

            //關閉升級UI(如果有)
            if(GameManager.instance != null && GameManager.instance.choosingUpgrade) 
                GameManager.instance.EndLevelUp();

            return slotNum;

        }
        else
        {
            Debug.LogWarning(string.Format(
                "指定的武器Type{0}無效",
                data.name
            ));
        }

        return -1;
    }

    //搜尋空格加入道具，回傳序號給該道具
    public int Add(PassiveData data)
    {
        int slotNum = -1;

        //嘗試搜尋空格
        for(int i = 0; i < passiveSlots.Capacity; i++)
        {
            if(passiveSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }

        //如果沒空格就離開
        if(slotNum < 0) return slotNum;

        //創進道具
        //獲取我們要加入的武器type
        GameObject go = new GameObject(data.baseStats.name + "Passive");
        Passive p = go.AddComponent<Passive>();
        p.Initialise(data);
        p.transform.SetParent(transform); //把道具設定至玩家的子集
        p.transform.localPosition = Vector2.zero;

        passiveSlots[slotNum].Assign(p);
        if(GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
        player.RecalculateStats();

        return slotNum;         
        
    }
    
    //如果不知道正在添加的是哪個物品，這個函式將負責確定。
    public int Add(ItemData data)
    {
        if (data is WeaponData) return Add(data as WeaponData);
        else if (data is PassiveData) return Add(data as PassiveData);
        return -1;
    }

    public void LevelUpWeapon(int slotIndex, int upgradeIndex)
    {
        if(weaponSlots.Count > slotIndex)
        {
            Weapon weapon = weaponSlots[slotIndex].item as Weapon;

            //如果已經滿等就不升級
            if(!weapon.DoLevelUp())
            {
                Debug.LogWarning(string.Format(
                    "{0}升級失敗",
                    weapon.name
                ));
                return;
            }
        }

        if(GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    public void LevelUpPassiveItem(int slotIndex, int upgradeIndex)
    {
        if(passiveSlots.Count > slotIndex)
        {
            Passive p = passiveSlots[slotIndex].item as Passive;
            if(!p.DoLevelUp())
            {
                Debug.LogWarning(string.Format(
                    "{0}升級失敗",
                    p.name
                ));
                return;
            }
        }

        if(GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
        player.RecalculateStats();
    }

    //決定該出現的升級選項
    void ApplyUpgradeOptions()
    {
        //製作可用武器／被動升級清單的副本，以便我們能夠在函式中對其進行迭代
        List<WeaponData> availableWeaponUpgrades = new List<WeaponData>(availableWeapons); Debug.Log("List: " + string.Join(", ", availableWeapons));
        List<PassiveData> availablePassiveUpgrades = new List<PassiveData>(availablePassives); Debug.Log("List: " + string.Join(", ", availablePassives));
        
        //在升級UI中迭代每個格子
        foreach (UpgradeUI upgradeOption in upgradeUIOptions)
        {        
            //如果沒有可用的升級，那就終止
            if(availableWeaponUpgrades.Count == 0 && availablePassiveUpgrades.Count == 0)
                return;

            //決定這是武器還是道具的升級
            int upgradeType;
            if(availableWeaponUpgrades.Count == 0)
            {
                upgradeType = 2;
            }
            else if (availablePassiveUpgrades.Count == 0)
            {
                upgradeType = 1;
            }
            else
            {
                //隨機產生1或2的數字
                upgradeType = UnityEngine.Random.Range(1, 3);
            }
            //產生武器升級
            if(upgradeType == 1)
            {
                //取得武器升級，然後刪除他避免重複取用
                WeaponData chosenWeaponUpgrade = availableWeaponUpgrades[UnityEngine.Random.Range(0, availableWeaponUpgrades.Count)];Debug.Log(availableWeaponUpgrades.Count);
                availableWeaponUpgrades.Remove(chosenWeaponUpgrade);
                
                //確定選擇有效的武器data
                if(chosenWeaponUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOption);

                    //呼叫event listener到對應的武器Button中，點選以升級該選項
                    bool isLevelUp = false;
                    for (int i = 0; 1 < weaponSlots.Count; i++)
                    {
                        Weapon w = weaponSlots[i].item as Weapon;
                        if(w != null && w.data == chosenWeaponUpgrade)
                        {

                            if(chosenWeaponUpgrade.maxLevel <= w.currentLevel)
                            {
                                DisableUpgradeUI(upgradeOption);
                                isLevelUp = true;
                                break;
                            }

                            //設定下個等級的Event Listener、道具、等級敘述
                            upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i, i));//添加按鈕功能
                            Weapon.Stats nextLevel = chosenWeaponUpgrade.GetLevelData(w.currentLevel + 1);
                            upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                            upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                            upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.icon;
                            isLevelUp = true;
                            break;
                            
                        }                        

                    }
                    //跑到這邊代表我們正在加入新武器，而不是升級現有武器
                    if(!isLevelUp)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenWeaponUpgrade));//添加按鈕功能                        
                        upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.baseStats.description;
                        upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.baseStats.name;
                        upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.icon;
                    }
                }                
            }
            else if (upgradeType == 2)
            {
                //教學筆記：這個系統要重寫，現在的版本會在我們點選滿等武器後取消該升級格子

                PassiveData chosenPassiveUpgrade = availablePassiveUpgrades[UnityEngine.Random.Range(0, availablePassiveUpgrades.Count)];Debug.Log(availablePassiveUpgrades.Count);
                availablePassiveUpgrades.Remove(chosenPassiveUpgrade);

                if (chosenPassiveUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOption);

                    bool isLevelUp = false;
                    for(int i = 0; i < passiveSlots.Count; i++)
                    {
                        Passive p = passiveSlots[i].item as Passive;
                        if(p != null && p.data == chosenPassiveUpgrade)
                        {
                            //如果道具滿等就不允許升級
                            if(chosenPassiveUpgrade.maxLevel <= p.currentLevel)
                            {
                                DisableUpgradeUI(upgradeOption);
                                isLevelUp = true;
                                break;
                            }
                            upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(i, i));//添加按鈕功能
                            Passive.Modifier nextLevel = chosenPassiveUpgrade.GetLevelData(p.currentLevel + 1);
                            upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                            upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                            upgradeOption.upgradeIcon.sprite = chosenPassiveUpgrade.icon;
                            isLevelUp = true;
                            break;
                        }
                    }

                    if(!isLevelUp)//新的道具
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenPassiveUpgrade));
                        Passive.Modifier nextLevel = chosenPassiveUpgrade.baseStats;
                        upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                        upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                        upgradeOption.upgradeIcon.sprite = chosenPassiveUpgrade.icon;
                    }
                }
            }
        }
    }

    void RemoveUpgradeOption()
    {
        foreach (UpgradeUI upgradeOption in upgradeUIOptions)
        {
            if (upgradeUIOptions.Count == 0)
            {
                Debug.LogError("RemoveAndApplyUpgrades: 沒有升級選項，但仍然嘗試存取！");
                return;
            }
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgradeUI(upgradeOption); //在添加升級前呼叫取消UI的方法
        }
    }

    public void RemoveAndApplyUpgrades()
    {
        RemoveUpgradeOption();
        ApplyUpgradeOptions();
    }

    void DisableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(false);
    }

    void EnableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(true);
    }

}
