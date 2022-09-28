using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;
using Steamworks;


public class Interactor : NetworkBehaviour
{
    [Header("User Interface")]
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
    public Text DebugText;

    public Vector2 iconSize;

    public ItemSpawning itemSpawning;

    [Header("Object Prefabs")]
    public GameObject RedPotion;

    [Header("Other Variables")]
    public Transform dropPoint;
    public float dropForce;
    public float dropUpForce;

    [Header("Hand Variables")]
    public Transform hand;
    public Transform thirdPersonHand;
    public GameObject emptyHand;

    [SyncVar(hook = nameof(OnCreateItemInHand))]
    public GameObject currentItemInHand;

    void OnCreateItemInHand(GameObject oldItem, GameObject newItem)
    {
        //DebugText.text = "Function is ran from command.";
        StartCoroutine(CreateItemInHand(newItem));
    }

    IEnumerator CreateItemInHand(GameObject newItemInHand)
    {
        while (hand.gameObject.transform.childCount > 0)
        {
            Destroy(hand.transform.GetChild(0).gameObject);
            yield return null;
        }

        if (currentItemInHand)
        {
            Instantiate(newItemInHand.GetComponent<ItemManager>().itemModel, hand.transform.position, Quaternion.identity, hand);
        }
        else 
        {
            //Instantiate(newItemInHand, hand.transform.position, Quaternion.identity, hand);
        }
    }

    void Start()
    {
        currentItemInHand = null;
    }

    void Update()
    {
        if (itemSpawning == null) 
        {
            itemSpawning = ItemSpawning.FindObjectOfType<ItemSpawning>();
        }

        if (!isLocalPlayer) return;

        RaycastHit hit;
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

            //Handles Item interactable
            if (hit.collider.GetComponent<ItemManager>() != false) 
            {
                if (Input.GetKeyDown(KeyCode.F) && !currentItemInHand) 
                {
                    HandleItem(hit.collider.gameObject);
                }
            }

            //Handles altar interactable
            if (hit.collider.GetComponent<AltarHandler>() != false)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    HandleAltar(hit.collider.gameObject);
                }
            }

            //Handles item tester interactable
            if (hit.collider.GetComponent<ItemTesterHandler>() != false) 
            {
                if (Input.GetKeyDown(KeyCode.F) && currentItemInHand) 
                {
                    HandleItemTester(hit);                  
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

        if (Input.GetKeyDown(KeyCode.G) && currentItemInHand) 
        {
            HandleItemWhenDropped(currentItemInHand);
        }
    }

    public void HandleItemTester(RaycastHit hit) 
    {
        if (isServer)
        {
            hit.collider.GetComponent<ItemTesterHandler>().TestItem(currentItemInHand.GetComponent<ItemManager>().itemName);
            currentItemInHand = null;
        }
        else CmdHandleItemTester(hit);
    }

    [Command]
    public void CmdHandleItemTester(RaycastHit hit) 
    {
        HandleItemTester(hit);
    }

    public void HandleAltar(GameObject altar) 
    {
        AltarHandler altarHandler = altar.GetComponent<AltarHandler>();


        if (currentItemInHand)
        {
            ItemManager itemInHandManager = currentItemInHand.GetComponent<ItemManager>();

            if (isServer)
            {
                if (!altarHandler.isActive)
                {
                    altarHandler.particleColor = itemInHandManager.interactorColor;
                    altarHandler.particleLight.color = itemInHandManager.interactorColor;
                    altarHandler.isActive = true;

                    currentItemInHand = null;
                }
                else
                {
                    altarHandler.isActive = false;
                }
            }
            else CmdHandleAltar(altar);
        }
        
    }

    [Command]
    public void CmdHandleAltar(GameObject altar) 
    {
        HandleAltar(altar);
    }

    public void HandleItem(GameObject item)
    {
        if (isServer)
        {
            currentItemInHand = item;
            item.transform.position = new Vector3(0f, 13f, 0f);   //temporary: when object is picked up. object will be moved to roof to sell the idea that it moved to the players hand
            DebugText.text = "Server is handling.";
        }
        else CmdItemInHand(item);
    }


    [Command]
    public void CmdItemInHand(GameObject selectedItem)
    {
        HandleItem(selectedItem);
    }

    public void HandleItemWhenDropped(GameObject item) 
    {
        if (isServer)
        {
            currentItemInHand = null;

            item.transform.position = dropPoint.position;
            item.GetComponent<Rigidbody>().isKinematic = false;

            item.GetComponent<Rigidbody>().AddForce(dropPoint.forward * dropForce + Vector3.up * dropUpForce, ForceMode.Impulse);
        }
        else CmdDropItem(item);
    }

    [Command]
    void CmdDropItem(GameObject item)
    {
        HandleItemWhenDropped(item);
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
