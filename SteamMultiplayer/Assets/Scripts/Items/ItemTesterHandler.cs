using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class ItemTesterHandler : NetworkBehaviour
{
    public List<AltarHandler> altars = new List<AltarHandler>();
    public List<TextMeshPro> wallNumbers = new List<TextMeshPro>();

    public AudioSource itemTesterSound;

    public MonsterAI monster;
    public EndGameHandler endGame;

    [SyncVar (hook = nameof(OnTriggered))]
    public bool isTriggered = false;

    [Header("1 Out of __ Chance to Trigger.")]
    public int MonsterTriggerChance = 2;

    public void OnTriggered(bool oldValue, bool newValue) 
    {
        if (isTriggered == newValue) 
        {
            itemTesterSound.Play();
            isTriggered = oldValue;
        }
        
    }

    public void OnTriggerEnter(Collider other)   //if monster targets the item tester, and he will continue to patroll if he touches item tester
    {
        if (other.gameObject.GetComponent<MonsterAI>()) 
        {
            Debug.Log("Monster Entered");
            other.gameObject.GetComponent<MonsterAI>().hasNewTarget = false;           
        }
    }

    public void TestItem(string itemName)
    {
        RevertItemColors();      
        ChangeItemColors(itemName);
        CheckIfMonsterTriggered();
        if (isClient) 
        {
            itemTesterSound.Play();
        }
        
    }

    public void CheckIfMonsterTriggered() 
    {
        int randomIndex = Random.Range(0, MonsterTriggerChance);
        Debug.Log(randomIndex);
        if (randomIndex == 0) 
        {
            endGame.altarsCompleteSound.Play();
            monster.RunToSpecificTarget(transform.position);
        }
    }


    public void RevertItemColors()
    {
        for (int i = 0; i < wallNumbers.Count; i++)   //resets colors of wallnumbers back to red
        {
            wallNumbers[i].GetComponent<WallNumberHandler>().numColor = Color.red;
        }
    }


    public void ChangeItemColors(string itemName)
    {
        for (int i = 0; i < altars.Count; i++)      //changes wallnumbers to green if matching object with quest item altar
        {
            if (altars[i].questItemName == itemName)
            {
                wallNumbers[i].GetComponent<WallNumberHandler>().numColor = Color.green;
            }
        }
    }
}
