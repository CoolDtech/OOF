using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneColtroller : WeaponController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        StartCoroutine(SpawnBones());
    }

    IEnumerator SpawnBones()
    {
        for (int i = 0; i < weaponData.MultiHits; i++)
        {
            GameObject spawnedBone = Instantiate(weaponData.Prefab);
            spawnedBone.transform.position = transform.position;

            // 根據最近的敵人來設置方向
            GameObject closestEnemy = FindClosestEnemy();
            if (closestEnemy != null)
            {
                Vector3 directionToEnemy = (closestEnemy.transform.position - spawnedBone.transform.position).normalized;
                spawnedBone.GetComponent<BoneBehaviour>().SetDirection(directionToEnemy);
            }
            
            yield return new WaitForSeconds(0.2f);
        }
    }

    
    GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }
}