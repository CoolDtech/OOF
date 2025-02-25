using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DropRateManager : MonoBehaviour
{
    [System.Serializable]
    public class Drops
    {
        public string name;
        public GameObject itemPrefab;
        public float dropRate;

    }
    public List<Drops> drops;

    void OnDestroy()
    {
        if(!gameObject.scene.isLoaded)//停止產生遊戲結束時的錯誤訊息
        {
            return;
        }


        float randomNumber = UnityEngine.Random.Range(0f, 100f);

        foreach (Drops rate in drops)
        {
            if(randomNumber <= rate.dropRate)
            {
                Instantiate(rate.itemPrefab, transform.position, Quaternion.identity);
            }
        }
    }

}
