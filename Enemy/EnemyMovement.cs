using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    EnemyStats enemy;
    Transform player;

    Vector2 knockbackVelocity;
    float konckbackDuration;



    void Start()
    {   
        enemy = GetComponent<EnemyStats>();
        player = FindObjectOfType<PlayerMovement>().transform;
    }

    void Update()
    {
            //如果被擊退
        if(konckbackDuration > 0)
        {
            transform.position += (Vector3)knockbackVelocity * Time.deltaTime;
            konckbackDuration -= Time.deltaTime;
        }
        else
        {
            //正常的移動
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, enemy.currentMoveSpeed * Time.deltaTime);    //Constantly move the enemy towards the player
        }
    }

    public void Knockback(Vector2 velocity, float duration)
    {
        //如果擊退時間大於0(正在被擊退)
        if(konckbackDuration > 0) return;

        knockbackVelocity = velocity;
        konckbackDuration = duration;
    }
}