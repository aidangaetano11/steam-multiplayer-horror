using UnityEngine;
using Mirror;
public class OfficeKeyManager : NetworkBehaviour
{
    public InventoryManager inventoryManager;
    public GameObject keyPrefab;

    public void Start()
    {
        inventoryManager = InventoryManager.FindObjectOfType<InventoryManager>();
    }


    public void DisableKey()  //function is called from interactor script, from raycast collision
    {
        //inventoryManager.Inventory.Add(keyPrefab);        //adds key to inventory.
        NetworkServer.Destroy(keyPrefab);
    }

}
