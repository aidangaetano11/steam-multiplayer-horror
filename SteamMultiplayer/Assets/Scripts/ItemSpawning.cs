using UnityEngine;
using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
public class ItemSpawning : NetworkBehaviour
{
    public int maxSpawnedItems = 3;
    public int maxSpawnedRedPotions = 3;


    public Transform Objects;

    [Header("Spawnpoint Lists")]
    public List<Transform> ItemSpawnPoints = new List<Transform>();
    public List<Transform> TotalItemSpawnPoints = new List<Transform>();    //this is used for resetting map
    public List<Transform> KeySpawnPoints = new List<Transform>();

    [Header("Item Pool")]
    public List<GameObject> ItemPool = new List<GameObject>(); 

    [Header("Item Prefabs")]
    public GameObject SpawnPrefab;
    public GameObject KeyPrefab;
    public GameObject RedPotionPrefab;

    [Header("Item Prefab Index List")]
    public List<GameObject> MapItemList = new List<GameObject>();

    private CustomNetworkManager manager;

    public List<GameObject> objectsInScene;

    [SyncVar (hook = nameof(OnHasKey))]
    public bool hasKey = false;

    Vector3 currentItemSpawnPoint;
    Vector3 currentKeySpawnPoint;


    void OnHasKey(bool oldValue, bool newValue) 
    {
        hasKey = newValue;
    }

    void Start()
    {
        ChooseRandomPoint();  
        ChooseRandomKeySpawnPoint();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V)) 
        {
            RestartGameItems();
        }
    }

    void ChooseRandomPoint()     //this will handle spawning random items, red potion, yellow potion, blue potion
    {
        for (int i = 0; i < maxSpawnedItems; i++)                           //loop and spawn items as much as maxSpawnedItems
        {
            if (ItemPool != null) 
            {
                int randomSpawn = Random.Range(0, ItemSpawnPoints.Count);           //randomly loop all spawnpoints
                currentItemSpawnPoint = ItemSpawnPoints[randomSpawn].position;         //save position of that spawn point
                GameObject obj = Instantiate(ChooseRandomItem(), currentItemSpawnPoint, Quaternion.identity);
                NetworkServer.Spawn(obj);                               //spawn item prefab on spawn point
                ItemSpawnPoints.Remove(ItemSpawnPoints[randomSpawn]);          //delete spawn point from list, so we cant try and spawn another object on that point
            }           
        }     
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
        }        
    }

    public GameObject ChooseRandomItem() 
    {
        int randomRange = Random.Range(0, ItemPool.Count);
        GameObject currentItem = ItemPool[randomRange];
        return currentItem;
    }

    public void RestartGameItems() 
    {
        foreach (ItemManager p in FindObjectsOfType<ItemManager>())   //searches every item with itemmanager in the scene
        {
            objectsInScene.Add(p.gameObject);         //adds it to new list
        }

        foreach (GameObject g in objectsInScene)      //destroys every object in the new list
        {
            NetworkServer.Destroy(g);
        }

        objectsInScene.Clear();          //clear the new list

        ItemSpawnPoints.Clear();           //clear the spawn points list

        foreach (Transform t in TotalItemSpawnPoints)     //take the unchanged spawn points list
        {
            ItemSpawnPoints.Add(t);           //and takes each spawn point from that list and re adds it to other spawnpoint list
        }

        ChooseRandomPoint();   //re spawns all items again

        if (hasKey) 
        {
            hasKey = false;
        }
        else NetworkServer.Destroy(FindObjectOfType<OfficeKeyManager>().gameObject);


        FindObjectOfType<OfficeDoorManager>().anim.SetBool("DoorOpen", false);   //play animation to close door
        FindObjectOfType<OfficeDoorManager>().doorState = DoorState.Locked;    //we will relock door
        
        ChooseRandomKeySpawnPoint();    //spawn new key 
    }

}
