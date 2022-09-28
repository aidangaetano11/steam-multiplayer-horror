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

    [SyncVar (hook=nameof(OnQuestItemChange))]
    public string questItemName;

    [SyncVar (hook=nameof(OnColorChange))]
    public Color particleColor;

    [SyncVar (hook = nameof(OnActiveChange))]
    public bool isActive = false;

    public List<GameObject> itemList;

    void OnColorChange(Color oldValue, Color newValue) 
    {
        Debug.Log("COLOR CHANGE: " + newValue.ToString());
        particleColor = newValue;
    }

    void OnActiveChange(bool oldValue, bool newValue)
    {
        if (oldValue)      //if is active
        {
            Debug.Log("Disable Smoke");
            particleLight.enabled = false;
            visualEffect.Stop();

        }
        else
        {
            Debug.Log("Enable Smoke");
            visualEffect.SetVector4("SmokeColor", particleColor);
            particleLight.color = particleColor;
            particleLight.enabled = true;
            visualEffect.Play();
        }
    }

    void OnQuestItemChange(string oldValue, string newValue)
    {
        questItemName = newValue;
        Debug.Log("Alter " + number + " Quest Item: " + questItemName);
    }

    public void Start()
    {
        visualEffect.Stop();
        particleLight.enabled = false;

        HandleQuestItem();
    }

    public GameObject PickQuestObject() 
    {
        int randomIndex = Random.Range(0, itemList.Count);
        return itemList[randomIndex];      
    }

    public void HandleQuestItem() 
    {
        if (isServer)
        {
            questItemName = PickQuestObject().GetComponent<ItemManager>().itemName;
        }
        else CmdHandleQuestItem();
    }

    [Command]
    public void CmdHandleQuestItem() 
    {
        HandleQuestItem();
    }
}
