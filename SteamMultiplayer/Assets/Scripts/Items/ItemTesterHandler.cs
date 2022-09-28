using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class ItemTesterHandler : NetworkBehaviour
{
    public List<AltarHandler> altars = new List<AltarHandler>();
    public List<TextMeshPro> wallNumbers = new List<TextMeshPro>();

    public void TestItem(string itemName)
    {
        RevertItemColors();

        ChangeItemColors(itemName);      
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
