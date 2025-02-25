using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbingAnimation : MonoBehaviour
{ 
    public float frequency; //動畫頻率
    public float magnitude; //動畫範圍
    public Vector3 direction; //動畫方向
    Vector3 initialPosition; 
    Pickup pickup;

    void Start()
    {
        pickup = GetComponent<Pickup>();
        //儲存物件位置
        initialPosition = transform.position;
    }

    void Update()
    {
        if(pickup && !pickup.hasBeenCollected)
        //三角函數
        transform.position = initialPosition + direction * Mathf.Sin(Time.time * frequency) * magnitude;
    }
}
