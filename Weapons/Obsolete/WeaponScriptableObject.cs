using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("這個會被WeaponData取代")]
[CreateAssetMenu(fileName = "WeaponScriptableObject", menuName = "ScriptableObjects/Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    [SerializeField]
    GameObject prefab;
    public GameObject Prefab { get => prefab; private set => prefab = value; }

    //Base stats for the weapon
    [SerializeField]
    float damage;
    public float Damage { get => damage; private set => damage = value; }

    [SerializeField]
    float speed;
    public float Speed { get => speed; private set => speed = value; }

    [SerializeField]
    float cooldownDuration;
    public float CooldownDuration { get => cooldownDuration; private set => cooldownDuration = value; }

    [SerializeField]
    int pierce;
    public int Pierce { get => pierce; private set => pierce = value; }

    [SerializeField]
    int multiHits;
    public int MultiHits { get => multiHits; private set => multiHits = value; }
    
    [SerializeField]
    int level; //道具的等級，不是拿經驗直升級的那個
    public int Level { get => level; private set => level = value; }
    
    [SerializeField]
    GameObject nextlevelPrefab; //昇級版道具的樣子
    public GameObject NextlevelPrefab { get => nextlevelPrefab; private set => nextlevelPrefab = value; }

    [SerializeField]
    new string name;
    public string Name { get => name; private set => name = value; }
    [SerializeField]
    string description;
    public string Description { get => description; private set => description = value; }

    [SerializeField]
    Sprite icon;
    public Sprite Icon { get => icon; private set => icon = value; }

    [SerializeField]
    int evolveUpgradeToRemove; //道具的等級，不是拿經驗直升級的那個
    public int EvolveUpgradeToRemove { get => evolveUpgradeToRemove; private set => evolveUpgradeToRemove = value; }




}