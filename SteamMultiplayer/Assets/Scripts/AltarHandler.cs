using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Mirror;
public class AltarHandler : NetworkBehaviour
{
    [Header("Altar Variables")]
    public string number;
    public GameObject particle;
    public VisualEffect visualEffect;
    public Light particleLight;
    public Interactable interactable;

    [Header("Altar Sounds")]
    public AudioSource altarInteractSound;
    public AudioSource altarActiveSound;

    [SyncVar (hook=nameof(OnQuestItemChange))]
    public string questItemName;

    [SyncVar (hook=nameof(OnColorChange))]
    public Color particleColor;

    [SyncVar (hook = nameof(OnActiveChange))]
    public bool isActive = false;

    public bool correctItem = false;

    public List<GameObject> itemList;

    void OnColorChange(Color oldValue, Color newValue) 
    {
        particleColor = newValue;
    }

    void OnActiveChange(bool oldValue, bool newValue)
    {
        if (oldValue)      //if is active
        {
            particleLight.enabled = false;
            visualEffect.Stop();
            correctItem = false;
            altarActiveSound.Stop();
        }
        else
        {
            visualEffect.SetVector4("SmokeColor", particleColor);
            particleLight.color = particleColor;
            particleLight.enabled = true;
            visualEffect.Play();
            altarActiveSound.Play();
        }
    }

    void OnQuestItemChange(string oldValue, string newValue)
    {
        questItemName = newValue;
        //Debug.Log("Alter " + number + " Quest Item: " + questItemName);
    }

    public void Start()
    {
        interactable = GetComponent<Interactable>();
        visualEffect.Stop();
        particleLight.enabled = false;

        HandleQuestItem();
    }

    public void CheckItem(GameObject item)   //called from interactor script
    {
        if (item.GetComponent<ItemManager>().itemName == questItemName) 
        {
            correctItem = true;
        }
    }

    public void PlayInteractSound(AudioClip clip)   //called from interactor script
    {
        altarInteractSound.clip = clip;
        altarInteractSound.Play();
    }

    public GameObject PickQuestObject() 
    {
        int randomIndex = Random.Range(0, itemList.Count);
        return itemList[randomIndex];      
    }

    [Server]
    public void HandleQuestItem()   //server should set quest item for that altar
    {
         questItemName = PickQuestObject().GetComponent<ItemManager>().itemName;
    }
}
