using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : WeaponController
{
    float subCooldown;
    protected override void Start()
    {
        base.Start();
        
    }

    protected override void Attack()
    {
        base.Attack();
        StartCoroutine(SpawnKnivesWithDelay());
        //GameObject spawnedKnife = Instantiate(weaponData.Prefab);
        //spawnedKnife.transform.position = transform.position; //Assign the position to be the same as this object which is parented to the player
        //spawnedKnife.GetComponent<KnifeBehaviour>().DirectionChecker(pm.lastMovedVector);   //Reference and set the direction
                
            
        
        
        

    }

    IEnumerator SpawnKnivesWithDelay()
{
    for (int i = 0; i < weaponData.MultiHits; i++)
    {
        GameObject spawnedKnife = Instantiate(weaponData.Prefab);
        spawnedKnife.transform.position = transform.position; // 設定位置
        spawnedKnife.GetComponent<KnifeBehaviour>().DirectionChecker(pm.lastMovedVector); // 設定方向
        
        yield return new WaitForSeconds(0.1f); // 每次生成之間延遲
    }
}
}