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
    public Text InteractTipText;
    public string emptyText = "";

    public Vector2 iconSize;

    public ItemSpawning itemSpawning;

    [SyncVar(hook =nameof(onHasKey))]
    public bool hasKey = false;

    [Header("Object Prefabs")]
    public GameObject RedPotion;

    [Header("Other Variables")]
    public Transform dropPoint;
    public float dropForce;
    public float dropUpForce;

    [Header("Sounds")]
    public AudioSource pickupSound;
    public AudioClip emptyAltarInteractSound;
    public AudioClip itemAltarInteractSound;
    public AudioSource keySound;

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

    void onHasKey(bool oldValue, bool newValue)   //makes key obtained on all clients
    {
        hasKey = newValue;
        Debug.Log("HAS KEY CALLED");
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

    public void PlayFootstep() 
    {
        if (hasAuthority)
        {
            gameObject.GetComponentInChildren<PlayerAudioHelper>().AuthorityPlaySound();
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
            if (hit.collider.GetComponent<AltarHandler>() && !currentItemInHand)
            {
                InteractTipText.text = "You need an item to interact with the altar";
            }
            else if (hit.collider.GetComponent<ItemTesterHandler>() && !currentItemInHand)
            {
                InteractTipText.text = "You need an item to interact with the item tester";
            }
            else if (hit.collider.GetComponent<ItemManager>() && currentItemInHand)
            {
                InteractTipText.text = "You are already carrying an item";
            }
            else if (hit.collider.GetComponent<OfficeDoorManager>() && !hasKey) 
            {
                InteractTipText.text = "The door is locked";
            }
            else InteractTipText.text = hit.collider.GetComponent<Interactable>().InteractableHintMessage;


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
                    pickupSound.Play();
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
                if (Input.GetKeyDown(KeyCode.F)) 
                {
                    HandleItemTester(hit.collider.gameObject);                  
                }
            }

            //Handles office key interactable
            if (hit.collider.GetComponent<OfficeKeyManager>() != false)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    CheckKey();
                    keySound.Play();
                    hit.collider.GetComponent<OfficeKeyManager>().DisableKey();      //Adds key to inventory key manager
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
                InteractTipText.text = emptyText;
            }
        }

        if (Input.GetKeyDown(KeyCode.G) && currentItemInHand) 
        {
            HandleItemWhenDropped(currentItemInHand);
        }
    }

    public void CheckKey() 
    {
        if (isServer)
        {
            hasKey = true;
        }
        else CmdCheckKey();
    }

    [Command]
    public void CmdCheckKey() 
    {
        CheckKey();
    }

    public void HandleItemTester(GameObject IT)     //calls test item function in item tester script
    {
        ItemTesterHandler ITHandler = IT.GetComponent<ItemTesterHandler>();

        if (currentItemInHand) 
        {
            ItemManager itemManager = currentItemInHand.GetComponent<ItemManager>();

            if (isServer)                //if we are the server then we will test the item
            {
                ITHandler.TestItem(itemManager.itemName);
                currentItemInHand = null;
            }
            else CmdHandleItemTester(IT); //if we are client we will call command to tell server to test the item
        }        
    }

    [Command]
    public void CmdHandleItemTester(GameObject IT) 
    {
        HandleItemTester(IT);
    }

    public void HandleAltar(GameObject altar) 
    {
        AltarHandler altarHandler = altar.GetComponent<AltarHandler>();


        if (currentItemInHand)
        {
            ItemManager itemInHandManager = currentItemInHand.GetComponent<ItemManager>();
            altarHandler.PlayInteractSound(itemAltarInteractSound);   //play interact sound

            if (isServer)
            {
                if (!altarHandler.isActive)
                {
                    altarHandler.particleColor = itemInHandManager.interactorColor;
                    altarHandler.particleLight.color = itemInHandManager.interactorColor;
                    altarHandler.isActive = true;

                    altarHandler.CheckItem(currentItemInHand);

                    currentItemInHand = null;
                }
                else
                {
                    altarHandler.isActive = false;
                }
            }
            else CmdHandleAltar(altar);
        }
        else altarHandler.PlayInteractSound(emptyAltarInteractSound);    //play empty handed interact sound

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
