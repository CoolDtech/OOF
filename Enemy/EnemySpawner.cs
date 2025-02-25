using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
        public class Wave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups; //該波次生成的怪物集團
        public int waveQuota; //該波次怪物總數
        public float spawmInterval; //怪物生成間隔
        public int spawnCount; //該波次已生成怪物數量
    }
    
    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName;
        public int enemyCount;//該波次已生成怪物
        public int spawnCount;//該種類怪物在此波次已生成數量
        public GameObject enemyPrefab;
    }

    public List<Wave> waves; //所有波次的清單
    public int currentWaveCount; //顯示當前波次[從0開始]

    [Header("Spawner Attributer")]
    float spwanTimer; //計時器用來計算何時生成下一隻怪物
    public int enemiesAlive;
    public int maxEnemiesAllowed;//地圖怪物上限
    public bool maxEnemiesReached = false;//檢查地圖怪物是否達到上限
    public float waveInterval; //波次間隔
    bool isWaveActive = false;

    [Header("Spawn Positions")]
    public List<Transform> relativeSpawnPoints;



    Transform player;

        void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
        CalculateWaveQuota();
    }

    void Update()
    {

        //檢查本波次結束並開始下一波次
        if(currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount == 0 && !isWaveActive)
        {
            StartCoroutine(BeginNextWave());
        }

        spwanTimer += Time.deltaTime;

        //檢查甚麼時候生成下一隻怪物
        if(spwanTimer >= waves[currentWaveCount].spawmInterval)
        {
            spwanTimer = 0f;
            SpawnEnemies();
        }

    }

    IEnumerator BeginNextWave()
    {
        isWaveActive = true;

        //根據波次間隔在下波開始前等待的秒數
        yield return new WaitForSeconds(waveInterval);

        //時間到了如果還有下一波就開始下一波
        if(currentWaveCount < waves.Count - 1 )
        {
            isWaveActive = false;
            currentWaveCount++;
            CalculateWaveQuota();            
        }
    }


    void CalculateWaveQuota()
    {
        int currentWaveQuota = 0;
        foreach(var enemyGroups in waves[currentWaveCount].enemyGroups)
        {
            currentWaveQuota += enemyGroups.enemyCount;
        }

        waves[currentWaveCount].waveQuota = currentWaveQuota;
        Debug.LogWarning(currentWaveQuota);
    }
    ///<summary>
    ///該方法會在當怪物達到生成上限時阻止生成
    ///該方法只會在特定波次生成敵人直到下一波次的敵人生成
    ///<summary>
    void SpawnEnemies()
    {
        //檢查該波次怪物是否生成完畢
        if(waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota && !maxEnemiesReached)
        {
            //生成每種種類怪物直到達到上限
            foreach(var enemyGroup in waves[currentWaveCount].enemyGroups)
            {
                //檢查該種類怪物是否生成完畢
                if(enemyGroup.spawnCount < enemyGroup.enemyCount)
                {
                    //將怪物生成在遠離玩家的隨機點
                    Instantiate(enemyGroup.enemyPrefab, player.position + relativeSpawnPoints[Random.Range(0, relativeSpawnPoints.Count)].position, Quaternion.identity);
                    
                    enemyGroup.spawnCount++;
                    waves[currentWaveCount].spawnCount++;
                    enemiesAlive++;

                    //限制怪物最大生成數量
                    if(enemiesAlive >= maxEnemiesAllowed)
                    {
                        maxEnemiesReached = true;
                        return;
                    }

                }                
            }
        }
    }

    //當怪物被擊殺時呼叫這條
    public void OnEnemyKilled()
    {
        //檢查還有多少怪物
        enemiesAlive--;

        //反正就是低於上限就可以重新生怪
        if(enemiesAlive < maxEnemiesAllowed)
        {
            maxEnemiesReached = false;
        }
    }
}
