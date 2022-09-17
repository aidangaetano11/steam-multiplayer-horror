using UnityEngine;
using Mirror;
public class RedPotionManager : NetworkBehaviour
{
    public InventoryManager inventoryManager;
    public GameObject redPotionPrefab;


    public void Start()
    {
        inventoryManager = InventoryManager.FindObjectOfType<InventoryManager>();      //finds itemmanager script 
    }

    //[Command(requiresAuthority = false)]
    //public void CmdPickupRedPotion(Transform handPos)
    //{
    //    inventoryManager.Inventory.Add(redPotionPrefab);   //adds picked up item to inventory 
    //    gameObject.transform.position = handPos.position;
    //    //inventoryManager.pickedUpItems++;     //adds count to inventory manager script
    //    //gameObject.SetActive(false);
    //    //NetworkServer.UnSpawn(gameObject);     //unspawns it from server
    //}
}
