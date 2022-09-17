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


    [Command (requiresAuthority = false)]
    public void CmdDisableKey()  //function is called from interactor script, from raycast collision
    {
        inventoryManager.Inventory.Add(keyPrefab);        //adds key to inventory.
        gameObject.SetActive(false);
        NetworkServer.UnSpawn(gameObject);        //unspawns key from game
    }

}
