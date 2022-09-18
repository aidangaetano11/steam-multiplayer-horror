using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;
using Steamworks;

public class Interactor : NetworkBehaviour
{
    public float interactRange;
    public LayerMask interactMask;
    public Camera cam;
    Interactable interactable;
    public Vector2 defaultIconSize;
    public Vector2 defaultInteractIconSize;
    public Image interactImage;
    public Sprite interactIcon;
    public Sprite defaultIcon;
    public Sprite defaultInteractIcon;

    public Vector2 iconSize;

    public ItemSpawning itemSpawning;

    [Header("Other Variables")]
    public Transform dropPoint;
    public float dropForce;
    public float dropUpForce;

    [Header("Hand Variables")]
    public Transform hand;
    public GameObject currentItemInHand;


    public void Start()
    {
        
    }

    private void Update()
    {
        if (itemSpawning == null) 
        {
            itemSpawning = ItemSpawning.FindObjectOfType<ItemSpawning>();
        }
        
        RaycastHit hit;
        if (hasAuthority) 
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, interactRange, interactMask))
            {
                //handles resizing interact icon and showing it
                interactImage.sprite = interactIcon;
                if (iconSize == Vector2.zero)
                {
                    interactImage.rectTransform.sizeDelta = defaultInteractIconSize;
                }
                else
                {
                    interactImage.rectTransform.sizeDelta = iconSize;
                }

                //Handles test item interactable
                if (hit.collider.GetComponent<ItemTestManager>() != false)
                {
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        hit.collider.GetComponent<ItemTestManager>().CmdPickupTestItem();   //adds item to inventory and calls function in prefab script
                    }
                }

                //Handles red potion interactable
                if (hit.collider.GetComponent<RedPotionManager>() != false) 
                {
                    if (Input.GetKeyDown(KeyCode.F)) 
                    {
                        currentItemInHand = hit.collider.gameObject;
                        if (isClient) { CmdChangeItemInHand(hit); }                                                                            
                    }
                }

                //Handles office key interactable
                if (hit.collider.GetComponent<OfficeKeyManager>() != false)
                {
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        hit.collider.GetComponent<OfficeKeyManager>().CmdDisableKey();      //Adds key to inventory key manager
                    }
                }

                //Handles office door interactable
                if (hit.collider.GetComponent<OfficeDoorManager>() != false)
                {
                    HandleDoor(hit);
                }
            }
            else
            {
                if (interactImage.sprite != defaultIcon)
                {
                    interactImage.sprite = defaultIcon;
                    interactImage.rectTransform.sizeDelta = defaultIconSize;
                }
            }
        }

        CmdItemInHand(currentItemInHand);
    }

    [Command (requiresAuthority = false)]
    public void CmdChangeItemInHand(RaycastHit hit) 
    {
        currentItemInHand = hit.collider.gameObject;
    }


    [Command (requiresAuthority = false)]
    public void CmdItemInHand(GameObject itemToHold) 
    {
        if (itemToHold != null) 
        {
            itemToHold.transform.position = hand.position;
            Collider col = itemToHold.GetComponent<Collider>();
            Rigidbody rb = itemToHold.GetComponent<Rigidbody>();
            col.enabled = false;
            rb.isKinematic = true;

            if (hasAuthority)
            {
                if (Input.GetKeyDown(KeyCode.G))    //Drop item in hand
                {
                    Debug.Log("Dropped item: " + currentItemInHand.name);
                    DropItem(rb, col);
                    currentItemInHand = null;
                }
            }
        }
    }

    public void DropItem(Rigidbody rb, Collider col) 
    {
        rb.isKinematic = false;
        rb.AddForce(dropPoint.forward * dropForce + Vector3.up * dropUpForce, ForceMode.Impulse);
        col.enabled = true;
    }


    [Client]
    public void HandleDoor(RaycastHit hit) 
    {
        var officeDoor = hit.collider.GetComponent<OfficeDoorManager>();
        if (Input.GetKeyDown(KeyCode.F))
        {
            officeDoor.CmdSetDoorState();
        }
    }
}
