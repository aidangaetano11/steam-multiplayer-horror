using UnityEngine;
using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
public class ItemSpawning : NetworkBehaviour
{
    public int maxSpawnedItems = 3;

    [Header("Spawnpoint Lists")]
    public List<Transform> ItemSpawnPoints = new List<Transform>();
    public List<Transform> KeySpawnPoints = new List<Transform>();

    [Header("Item Prefabs")]
    public GameObject SpawnPrefab;
    public GameObject KeyPrefab;
    public GameObject RedPotionPrefab;

    [Header("Item Prefab Index List")]
    public List<GameObject> MapItemList = new List<GameObject>();

    private CustomNetworkManager manager;

    Vector3 currentItemSpawnPoint;
    Vector3 currentKeySpawnPoint;
    Vector3 currentRedSpawnPoint;


    private void Awake()
    {
       
    }

    [Server]
    void Start()
    {
        ChooseRandomPoint();  
        ChooseRandomKeySpawnPoint();
    }

    void ChooseRandomPoint()     //this will handle spawning random items, red potion, yellow potion, blue potion
    {
        for (int i = 0; i < maxSpawnedItems; i++)                           //loop and spawn items as much as maxSpawnedItems
        {
            if (SpawnPrefab != null) 
            {
                int randomSpawn = Random.Range(0, ItemSpawnPoints.Count);           //randomly loop all spawnpoints
                currentItemSpawnPoint = ItemSpawnPoints[randomSpawn].position;         //save position of that spawn point
                GameObject testObject = Instantiate(SpawnPrefab, currentItemSpawnPoint, Quaternion.identity);
                NetworkServer.Spawn(testObject);                               //spawn item prefab on spawn point
                ItemSpawnPoints.Remove(ItemSpawnPoints[randomSpawn]);          //delete spawn point from list, so we cant try and spawn another object on that point
            }           
        }

        if (RedPotionPrefab != null) 
        {
            int randomRedSpawn = Random.Range(0, ItemSpawnPoints.Count);
            currentRedSpawnPoint = ItemSpawnPoints[randomRedSpawn].position;
            GameObject redPotionObject = Instantiate(RedPotionPrefab, currentRedSpawnPoint, Quaternion.identity);
            NetworkServer.Spawn(redPotionObject);
            ItemSpawnPoints.Remove(ItemSpawnPoints[randomRedSpawn]);
        }
    }

    public void SpawnDroppedPrefab(GameObject currentPrefab, Vector3 spawnPoint, Transform playerDirection, float dropForce)
    {
        GameObject itemToDrop = Instantiate(currentPrefab, spawnPoint, Quaternion.identity);
        NetworkServer.Spawn(itemToDrop);
        Rigidbody rb = itemToDrop.GetComponent<Rigidbody>();
        rb.AddForce(playerDirection.forward * dropForce + Vector3.up * 5, ForceMode.Impulse);
    }

    public void ChooseRandomKeySpawnPoint() 
    {      
        int randomSpawn = Random.Range(0, KeySpawnPoints.Count);           //randomly loop all spawnpoints
        currentKeySpawnPoint = KeySpawnPoints[randomSpawn].position;         //save position of that spawn point
        CmdSpawnKey(currentKeySpawnPoint, randomSpawn);    
    }

    public void CmdSpawnKey(Vector3 spawnPoint, int index) 
    {
        if (KeyPrefab != null) 
        {            
            GameObject keyObject = Instantiate(KeyPrefab, spawnPoint, Quaternion.identity);
            NetworkServer.Spawn(keyObject);                             //spawn item prefab on spawn point
            KeySpawnPoints.Remove(KeySpawnPoints[index]);          //delete spawn point from list, so we cant try and spawn another object on that point
        }        
    }

}
