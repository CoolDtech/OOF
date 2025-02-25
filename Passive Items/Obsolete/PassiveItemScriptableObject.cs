using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

[CreateAssetMenu(fileName = "PassiveItemScriptableObject", menuName = "ScriptableObjects/Passive Item")]
public class PassiveItemScriptableObject : ScriptableObject
{
    [SerializeField]
    float multipler;
    public float Multipler { get => multipler; private set => multipler = value; }
    
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
}
