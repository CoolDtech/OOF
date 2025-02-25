using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 所有武器和道具的基本class，使所有WeaponData跟PassiveItemData能夠在需要時被使用
/// </summary>

public abstract class ItemData : ScriptableObject
{
    public Sprite icon;
    public int maxLevel;

    [System.Serializable]
    public struct Evolution
    {
        public string name;

        public enum Condition { auto, treasureChest }
        public Condition condition;

        [System.Flags] public enum Consumption { passive = 1, weapons = 2}
        public Consumption consumes;

        public int evolutionLevel;
        public Config[] catalysts;
        public Config outcome;

        [System.Serializable]
        public struct Config
        {
            public ItemData itemType;
            public int level;
        }
    }

    public Evolution[] evolutionData;
}
