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
            if (isServer)
            {
                wallNumbers[i].GetComponent<WallNumberHandler>().numColor = Color.red;
            }
            else CmdRevertItemColors();
        }
    }

    [Command]
    public void CmdRevertItemColors()  
    {
        RevertItemColors();
    }

    public void ChangeItemColors(string itemName) 
    {
        for (int i = 0; i < altars.Count; i++)      //changes wallnumbers to green if matching object with quest item altar
        {
            if (altars[i].questItemName == itemName)
            {
                if (isServer)
                {
                    wallNumbers[i].GetComponent<WallNumberHandler>().numColor = Color.green;
                }
                else CmdChangeItemColors(itemName);
            }
        }
    }

    [Command]
    public void CmdChangeItemColors(string itemName) 
    {
        TestItem(itemName);
    }
}
