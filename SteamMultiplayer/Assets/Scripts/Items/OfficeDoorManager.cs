using UnityEngine;
using Mirror;

public enum DoorState : byte 
{
    Open, Closed, Locked
}
public class OfficeDoorManager : NetworkBehaviour
{
    public InventoryManager inventoryManager;
    private Animator anim;

    [SyncVar]
    public DoorState doorState;

    private void Start()
    {
        anim = GetComponent<Animator>();
        doorState = DoorState.Locked;              //door will be initially locked
    }

    [Command(requiresAuthority = false)]
    public void CmdSetDoorState() 
    {
        bool hasDoorKey = inventoryManager.officeKeyObtained;   //bool to detect if we have picked up key or not

        if (doorState == DoorState.Locked && !hasDoorKey)         //if door is locked and we dont have key, make sure door stays closed
        {
            Debug.Log("Door is Locked");
            anim.SetBool("DoorOpen", false);
            return;
        }

        if (doorState == DoorState.Locked && hasDoorKey)    //if door is locked, but we have key, then door becomes open
        {
            doorState = DoorState.Open;
            anim.SetBool("DoorOpen", true);
            return;
        }

        if (doorState == DoorState.Closed)      //if door is closed, we will open the door
        {
            doorState = DoorState.Open;
            anim.SetBool("DoorOpen", true);
            return;
        }

        if (doorState == DoorState.Open)     //if door is open, we will close door
        {
            doorState = DoorState.Closed;
            anim.SetBool("DoorOpen", false);
            return;
        }
        
    }

}
