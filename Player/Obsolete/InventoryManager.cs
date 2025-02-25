using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public List<WeaponController> weaponSlots = new List<WeaponController>(6);
    public int[] weaponLevels = new int[6];
    public List<Image> weaponUISlots = new List<Image>(6);
    public List<PassiveItem> passiveItemSlots = new List<PassiveItem>(6);
    public int[] PassiveItemLevels = new int[6];
    public List<Image> passiveItemUISlots = new List<Image>(6);

    [System.Serializable]
    public class WeaponUpgrade
    {   
        public int weaponUpgradeIndex;
        public GameObject initialWeapon;
        public WeaponScriptableObject weaponData;
        
    }
    [System.Serializable]
    public class PassiveItemUpurade
    {
        public int passiveItemUpgradeIndex;
        public GameObject initialPassiveItem;
        public PassiveItemScriptableObject passiveItemData;
    }
    [System.Serializable]
    public class UpgradeUI
    {
        public TMP_Text upgradeNameDisplay;
        public TMP_Text upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button upgradeButton;
    }
    public List<WeaponUpgrade> weaponUpgradeOptions = new List<WeaponUpgrade>();//武器升級清單
    public List<PassiveItemUpurade> passiveItemUpgradeOptions = new List<PassiveItemUpurade>();//道具升級清單
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>();//升級選項UI清單

    public List<WeaponEvolutionBlueprint> weaponEvolutions = new List<WeaponEvolutionBlueprint>();

    PlayerStats player;

    void Start()
    {
        player = GetComponent<PlayerStats>();
    }


    //把新武器放進武器格
    public void AddWeapon(int slotIndex, WeaponController weapon)
    {
        weaponSlots[slotIndex] = weapon;
        weaponLevels[slotIndex] = weapon.weaponData.Level;
        weaponUISlots[slotIndex].enabled = true;
        weaponUISlots[slotIndex].sprite = weapon.weaponData.Icon; //加入武器圖標到UI

        if(GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }
    //把新道具放進道具格
    public void AddPassiveItem(int slotIndex, PassiveItem passiveItem)
    {
        passiveItemSlots[slotIndex] = passiveItem;
        PassiveItemLevels[slotIndex] = passiveItem.passiveItemData.Level;
        passiveItemUISlots[slotIndex].enabled = true;
        passiveItemUISlots[slotIndex].sprite = passiveItem.passiveItemData.Icon; //加入道具圖標到UI

        if(GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }  
    }

    public void LevelUpWeapon(int slotIndex, int upgradeIndex)
    {
        if(weaponSlots.Count > slotIndex) //檢查有沒有武器
        {
            WeaponController weapon = weaponSlots[slotIndex];
            if(!weapon.weaponData.NextlevelPrefab) //檢查有沒有下一級武器
            {
                Debug.LogError("這他媽的" + weapon.name + "沒有下一級啦");
                return;
            }
            
            GameObject upgradedWeapon = Instantiate(weapon.weaponData.NextlevelPrefab, transform.position, Quaternion.identity);
            upgradedWeapon.transform.SetParent(transform); //把升級過後的武器設定為子物件
            AddWeapon(slotIndex, upgradedWeapon.GetComponent<WeaponController>());//把升級的武器放同一格
            Destroy(weapon.gameObject);
            weaponLevels[slotIndex] = upgradedWeapon.GetComponent<WeaponController>().weaponData.Level; //確定武器等級正確

            weaponUpgradeOptions[upgradeIndex].weaponData = upgradedWeapon.GetComponent<WeaponController>().weaponData;

            if(GameManager.instance != null && GameManager.instance.choosingUpgrade)
            {
                GameManager.instance.EndLevelUp();
            }
        }
    }

    public void LevelUpPassiveItem(int slotIndex, int upgradeIndex)
    {
        if(passiveItemSlots.Count > slotIndex) //檢查有沒有武器
        {
            PassiveItem passiveItem = passiveItemSlots[slotIndex];
            if(!passiveItem.passiveItemData.NextlevelPrefab) //檢查有沒有下一級武器
            {
                Debug.LogError("這他媽的" + passiveItem.name + "沒有下一級啦");
                return;
            }

            GameObject upgradedPassiveItem = Instantiate(passiveItem.passiveItemData.NextlevelPrefab, transform.position, Quaternion.identity);
            upgradedPassiveItem.transform.SetParent(transform); //把升級過後的道具設定為子物件
            AddPassiveItem(slotIndex, upgradedPassiveItem.GetComponent<PassiveItem>());//把升級的道具放同一格
            Destroy(passiveItem.gameObject);
            PassiveItemLevels[slotIndex] = upgradedPassiveItem.GetComponent<PassiveItem>().passiveItemData.Level; //確定道具等級正確

            passiveItemUpgradeOptions[upgradeIndex].passiveItemData = upgradedPassiveItem.GetComponent<PassiveItem>().passiveItemData;

            if(GameManager.instance != null && GameManager.instance.choosingUpgrade)
            {
                GameManager.instance.EndLevelUp();
            }
        }
    }

    void ApplyUpgradeOptions()
    {

        List<WeaponUpgrade> avaliableWeaponUpgrades= new List<WeaponUpgrade>(weaponUpgradeOptions);
        List<PassiveItemUpurade> avaliablePassiveItemUpgrades= new List<PassiveItemUpurade>(passiveItemUpgradeOptions);
        foreach (var upgradeOption in upgradeUIOptions)
        {
            if(avaliableWeaponUpgrades.Count == 0 && avaliablePassiveItemUpgrades.Count == 0)
            {
                return;
            }
            int upgradeType;

            if(avaliableWeaponUpgrades.Count == 0)
            {
                upgradeType = 2;
            }
            else if(avaliablePassiveItemUpgrades.Count == 0)
            {
                upgradeType = 1;
            }
            else
            {
                upgradeType = Random.Range(1, 3); //武器跟道具之間選擇
            }

            if(upgradeType == 1)
            {
                WeaponUpgrade chosenWeaponUpgrade = avaliableWeaponUpgrades[Random.Range(0, avaliableWeaponUpgrades.Count)];

                avaliableWeaponUpgrades.Remove(chosenWeaponUpgrade);

                if(chosenWeaponUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOption);

                    bool newWeapon = false;
                    for (int i = 0; i < weaponSlots.Count; i++)
                    {
                        if(weaponSlots[i] != null && weaponSlots[i].weaponData == chosenWeaponUpgrade.weaponData)
                        {
                            newWeapon = false;

                            if(!newWeapon)
                            {
                                if(!chosenWeaponUpgrade.weaponData.NextlevelPrefab)
                                {
                                    DisableUpgradeUI(upgradeOption);
                                    break;
                                }
                                
                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i, chosenWeaponUpgrade.weaponUpgradeIndex)); //加入按鈕功能
                                //設定升級後的武器名稱及描述
                                upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.weaponData.NextlevelPrefab.GetComponent<WeaponController>().weaponData.Description;
                                upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.weaponData.NextlevelPrefab.GetComponent<WeaponController>().weaponData.Name;
                            }
                            break;
                        }
                        else
                        {
                            newWeapon = true;
                        }
                    }
                    if(newWeapon)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => player.SpwanWeapon(chosenWeaponUpgrade.initialWeapon));
                        //顯示初始名稱跟描述
                        upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.weaponData.Description;
                        upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.weaponData.Name;
                    }

                    upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.weaponData.Icon;
                }
            }
            else if(upgradeType == 2)
            {
                PassiveItemUpurade chosenPassiveItemUpgrade = avaliablePassiveItemUpgrades[Random.Range(0, avaliablePassiveItemUpgrades.Count)];
                
                avaliablePassiveItemUpgrades.Remove(chosenPassiveItemUpgrade);

                if(chosenPassiveItemUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOption);

                    bool newPassiveItem = false;
                    for (int i = 0; i < passiveItemSlots.Count; i++)
                    {
                        if(passiveItemSlots[i] != null && passiveItemSlots[i].passiveItemData == chosenPassiveItemUpgrade.passiveItemData)
                        {
                            newPassiveItem = false;

                            if(!newPassiveItem)
                            {
                                if(!chosenPassiveItemUpgrade.passiveItemData.NextlevelPrefab)
                                {
                                    DisableUpgradeUI(upgradeOption);
                                    break;
                                }

                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(i, chosenPassiveItemUpgrade.passiveItemUpgradeIndex));
                                //設定升級後的道具名稱及描述
                                upgradeOption.upgradeDescriptionDisplay.text = chosenPassiveItemUpgrade.passiveItemData.NextlevelPrefab.GetComponent<PassiveItem>().passiveItemData.Description;
                                upgradeOption.upgradeNameDisplay.text = chosenPassiveItemUpgrade.passiveItemData.NextlevelPrefab.GetComponent<PassiveItem>().passiveItemData.Name;
                            }
                            break;  
                        }
                        else
                        {
                            newPassiveItem = true;
                        }
                    }
                    if(newPassiveItem)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => player.SpwanPassiveItem(chosenPassiveItemUpgrade.initialPassiveItem));
                        //顯示初始名稱跟描述
                        upgradeOption.upgradeDescriptionDisplay.text = chosenPassiveItemUpgrade.passiveItemData.Description;
                        upgradeOption.upgradeNameDisplay.text = chosenPassiveItemUpgrade.passiveItemData.Name;
                    }
                    upgradeOption.upgradeIcon.sprite = chosenPassiveItemUpgrade.passiveItemData.Icon;
                }
            }
            
        }
    }

    void RemoveUpgradeOptions()
    {
        foreach (var upgradeOption in upgradeUIOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgradeUI(upgradeOption); 
        }
    }
    public void RemoveAndApplyUpgrades()
    {
        RemoveUpgradeOptions();
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

    public List<WeaponEvolutionBlueprint> GetPossibleEvolutions()
    {
        List<WeaponEvolutionBlueprint> possibleEvolutions = new List<WeaponEvolutionBlueprint>();
        foreach(WeaponController weapon in weaponSlots)
        {
            if(weapon != null)
            {
                foreach(PassiveItem Catalyst in passiveItemSlots)
                {
                    if(Catalyst != null)
                    {
                        foreach (WeaponEvolutionBlueprint evolution in weaponEvolutions)
                        {
                            if(weapon.weaponData.Level >= evolution.baseWeaponData.Level && Catalyst.passiveItemData.Level >= evolution.catalystPassiveItemData.Level)
                            {
                                possibleEvolutions.Add(evolution);
                            }
                        }
                    }
                }
            }
        }
        return possibleEvolutions;
    }

    public void EvolveWeapon(WeaponEvolutionBlueprint evloution)
    {
        for(int weaponSlotsIndex = 0; weaponSlotsIndex < weaponSlots.Count; weaponSlotsIndex++)
        {
            WeaponController weapon = weaponSlots[weaponSlotsIndex];

            if(!weapon)
            {
                continue;
            }

            for(int catalystSlotIndex = 0; catalystSlotIndex < passiveItemSlots.Count; catalystSlotIndex++)
            {
                PassiveItem catalyst = passiveItemSlots[catalystSlotIndex];

                if(!catalyst)
            {
                continue;
            }

                if (weapon && catalyst && 
                weapon.weaponData.Level >= evloution.baseWeaponData.Level && 
                catalyst.passiveItemData.Level >= evloution.catalystPassiveItemData.Level)
                {
                    GameObject evolvedWeapon = Instantiate(evloution.evolvedWeapon, transform.position, Quaternion.identity);
                    WeaponController evolvedWeaponController = evolvedWeapon.GetComponent<WeaponController>();

                    evolvedWeapon.transform.SetParent(transform); //把武器設定到玩家的子集
                    AddWeapon(weaponSlotsIndex, evolvedWeaponController);
                    Destroy(weapon.gameObject);

                    //更新等級跟圖標
                    weaponLevels[weaponSlotsIndex] = evolvedWeaponController.weaponData.Level;
                    weaponUISlots[weaponSlotsIndex].sprite = evolvedWeaponController.weaponData.Icon;

                    //更新升級選項
                    weaponUpgradeOptions.RemoveAt(evolvedWeaponController.weaponData.EvolveUpgradeToRemove) ;

                    Debug.LogWarning("歐津津化成功");

                }
            }
        }
    }

}

