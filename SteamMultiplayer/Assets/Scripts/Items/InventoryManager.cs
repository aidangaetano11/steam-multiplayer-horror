using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mirror;
public class InventoryManager : NetworkBehaviour
{
    [SyncVar]
    public bool officeKeyObtained = false;
    [SyncVar]
    public List<GameObject> Inventory = new List<GameObject>();

    [SyncVar]
    public int pickedUpItems = 0;

    public ItemSpawning itemSpawning;
    public EndGameTest endGameTest;

    private void Start()
    {
        itemSpawning = GetComponent<ItemSpawning>();
    }

    private void Update()
    {
        if (pickedUpItems >= itemSpawning.maxSpawnedItems)     //checks if all spawned item prefabs have been picked up
        {
            endGameTest.CmdChangeSphereColor();        //if they have, then it will change color from red to green on test sphere in game scene
            //Debug.Log("Picked up all items");
        }

        if (!officeKeyObtained)                        //if we have found key then we will not keep searching inventory
        {
            for (int i = 0; i < Inventory.Count; i++)         //keeps searching inventory until we have picked up key
            {
                if (Inventory[i].tag == "OfficeKey")
                {
                    officeKeyObtained = true; 
                }
            }
        }
    }
}
