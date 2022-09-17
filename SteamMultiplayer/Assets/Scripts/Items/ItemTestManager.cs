using UnityEngine;
using Mirror;
public class ItemTestManager : NetworkBehaviour
{
    public InventoryManager inventoryManager;
    public GameObject testPrefab;


    public void Start()
    {
        inventoryManager = InventoryManager.FindObjectOfType<InventoryManager>();      //finds itemmanager script 
    }

    [Command(requiresAuthority = false)]
    public void CmdPickupTestItem()     
    {
        inventoryManager.Inventory.Add(testPrefab);   //adds picked up item to inventory 
        inventoryManager.pickedUpItems++;     //adds count to inventory manager script
        gameObject.SetActive(false);
        NetworkServer.UnSpawn(gameObject);     //unspawns it from server
    }
}
