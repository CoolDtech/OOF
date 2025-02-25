using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CharacterSelector : MonoBehaviour
{
    public static CharacterSelector instance;
    public CharacterData characterData;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("EXTRA" + this + "DELETE");
            Destroy(gameObject);
        }
    }
    public static CharacterData GetData()
    {
        if(instance && instance.characterData)
            return instance.characterData;
        else
        {
            //如果沒有選擇角色，就隨機挑一個
            #if UNITY_EDITOR
            string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
            List<CharacterData> characters = new List<CharacterData>();
            foreach(string assetPath in allAssetPaths)
            {
                if (assetPath.EndsWith(".asset"))
                {
                    CharacterData characterData = AssetDatabase.LoadAssetAtPath<CharacterData>(assetPath);
                    if( characterData != null)
                    {
                        characters.Add(characterData);
                    }
                }
            }
            //選擇隨機角色
            if(characters.Count > 0) return characters[Random.Range(0, characters.Count)];

            #endif
        }
        return null;
        
    }

    public void SelectCharacter(CharacterData character)
    {
        characterData = character;
    }

    public void DestorySingleton()
    {
        instance = null;
        Destroy(gameObject);
    }
}
