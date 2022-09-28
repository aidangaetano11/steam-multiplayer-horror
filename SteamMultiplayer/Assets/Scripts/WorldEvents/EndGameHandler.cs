using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
public class EndGameHandler : NetworkBehaviour
{
    [SyncVar (hook = nameof(OnAltarsComplete))]
    public bool altarsComplete = false;

    public List<AltarHandler> altars;

    public void OnAltarsComplete(bool oldValue, bool newValue) 
    {
        altarsComplete = newValue;
    }

    public void Update()
    {
        if (!altarsComplete)
        {
            CheckAltars();
        }      
    }

    public void CheckAltars() 
    {
        if (isServer)
        {
            int count = 0;

            for (int i = 0; i < altars.Count; i++)
            {
                if (altars[i].correctItem)
                {
                    count++;
                }
            }


            if (count == altars.Count)
            {
                altarsComplete = true;
            }
            else altarsComplete = false;
        }
        else CmdCheckAltars();      
    }

    [Command]
    public void CmdCheckAltars() 
    {
        CheckAltars();
    }
}
