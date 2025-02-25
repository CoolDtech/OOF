using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> terrainChunks;
    public GameObject player;
    public float checkerRadius;
    public LayerMask terrainMask;
    public GameObject currentChunk;
    Vector3 playerLastPosition;
    
    
    [Header("Optimization")]
    public List<GameObject> spawnedChunks;
    GameObject latestChunk;
    public float maxOpDist; 
    float opDist;
    float optimizerCooldown;
    public float optimizerCooldownDur;


    void Start()
    {
        playerLastPosition = player.transform.position;
    }

    
    // Update is called once per frame
    void Update()
    {
        ChunkChecker();
        ChunkOptimizer();
    }
    void ChunkChecker()
    {
        if(!currentChunk)
        {
            return;
        }

        Vector3 moveDir = player.transform.position - playerLastPosition;
        playerLastPosition = player.transform.position;

        string directionName = GetDirectionName(moveDir);

        CheckAndSpawnChunk(directionName);

        if(directionName.Contains("Up"))
        {
            CheckAndSpawnChunk("Up");
        }
        if(directionName.Contains("Down"))
        {
            CheckAndSpawnChunk("Down");
        }
        if(directionName.Contains("Right"))
        {
            CheckAndSpawnChunk("Right");
        }
        if(directionName.Contains("Left"))
        {
            CheckAndSpawnChunk("Left");
        }

    }

    void CheckAndSpawnChunk(string direction)
    {
        if (!Physics2D.OverlapCircle(currentChunk.transform.Find(direction).position, checkerRadius, terrainMask))
        {
            SpawnChunk(currentChunk.transform.Find(direction).position);
        }
    }

    string GetDirectionName(Vector3 direction)
    {
        direction = direction.normalized;

        if(Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            //水平方向移動大於垂直方向
            if(direction.y > 0.5f)
            {
                return direction.x > 0 ? "RightUp" : "LeftUp";
            }
            else if(direction.y < -0.5f)
            {
                return direction.x > 0 ? "RightDown" : "LeftDown";
            }
            else
            {
                return direction.x > 0 ? "Right" : "Left";
            }
        }
        else
        {
            //垂直方向移動大於水平方向
            if(direction.x > 0.5f)
            {
                return direction.y > 0 ? "RightUp" : "RightDown";
            }
            else if(direction.x < -0.5f)
            {
                return direction.y > 0 ? "LeftUp" : "LeftDown";
            }
            else
            {
                return direction.y > 0 ? "Up" : "Down";
            }
        }

    }

    void SpawnChunk(Vector3 spawnPotision)
    {
        int rand = Random.Range(0, terrainChunks.Count);
        latestChunk = Instantiate(terrainChunks[rand], spawnPotision, Quaternion.identity);
        spawnedChunks.Add(latestChunk);
    }

    void ChunkOptimizer()
    {
        optimizerCooldown -= Time.deltaTime;
        if (optimizerCooldown < 0f)
        {
            optimizerCooldown = optimizerCooldownDur;
        }
        else
        {
            return;
        }

        foreach(GameObject chunk in spawnedChunks)
        {
            opDist = Vector3.Distance(player.transform.position, chunk.transform.position);
            if(opDist > maxOpDist)
            {
                chunk.SetActive(false);
            }
            else
            {
                chunk.SetActive(true);
            }
        }
    }
}
