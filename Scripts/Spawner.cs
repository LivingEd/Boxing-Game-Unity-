using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] Objects;
    public int ObjectTypeInt = 1;
    public int iMinAmount = 1;
    public int iMaxAmount = 2;
    public int AmountToSpawn;
    public int AmountSpawned = 0;
    public float RateOfSpawn = 0;
    private bool HasBeginSpawn = false;
    void Start()
    {
        AmountToSpawn = Random.Range(iMinAmount, iMaxAmount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void SpawnNow()
    {
       Instantiate(Objects[Random.Range(0, ObjectTypeInt)], this.transform.position, Quaternion.identity);
        AmountSpawned += 1;
        if(AmountSpawned < AmountToSpawn)
        {
            StartCoroutine(TimeToSpawn());
        }
    }
    private IEnumerator TimeToSpawn()
    {
        yield return new WaitForSeconds(RateOfSpawn);
        SpawnNow();
    }
    
    private void OnTriggerEnter(Collider TrigInfo)
    {
        if(HasBeginSpawn == false)
        {
            if (TrigInfo.gameObject.CompareTag("Player") && AmountSpawned < AmountToSpawn)
            {
                SpawnNow();
                //StartCoroutine(TimeToSpawn());
                HasBeginSpawn = true;
            }
            
        }
       
    }
}
