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

    [Header("1 Out of __ Chance to Trigger.")]
    public int MonsterTriggerChance = 2;


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
        RevertItemColors();         //resets all number colors back to red
        ChangeItemColors(itemName);       //change specific numbers to green depending on item inputted
        CheckIfMonsterTriggered();    //check if monster was called to the item tester
        RpcPlayITSound();           //play item tester sound for all clients to hear
    }

    [ClientRpc]
    public void RpcPlayITSound()     //plays item tester sound on all clients
    {
        itemTesterSound.Play();
    }

    [ClientRpc]
    public void RpcPlayMonsterSound()       //plays monster sound on all clients
    {
        endGame.altarsCompleteSound.Play();
    }

    public void CheckIfMonsterTriggered() 
    {
        int randomIndex = Random.Range(0, MonsterTriggerChance);
        Debug.Log(randomIndex);
        if (randomIndex == 0) 
        {
            RpcPlayMonsterSound();
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
