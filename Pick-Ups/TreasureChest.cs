using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        
        PlayerInventory p = col.GetComponent<PlayerInventory>();
        if(p)
        {
            bool randomBool = Random.Range(0, 2) == 0;

            OpenTreasureChest(p, randomBool);
            Destroy(gameObject);
        }
    }

    public void OpenTreasureChest(PlayerInventory inventory, bool isHigherTier)
    {

        //檢測所有武器的迴圈，無論是否可以進化
        foreach(PlayerInventory.Slot s in inventory.weaponSlots)
        {
            Weapon w = s.item as Weapon;
            if (w.data.evolutionData == null) continue; //忽略無法進化的武器

            //檢測可以進化的武器的迴圈
            foreach (ItemData.Evolution e in w.data.evolutionData)
            {
                //嘗試透過寶箱來進化武器
                if (e.condition == ItemData.Evolution.Condition.treasureChest)
                {
                    bool attempt = w.AttemptEvolution(e, 0);
                    if(attempt) return;
                }
            }
        }
    }
}
