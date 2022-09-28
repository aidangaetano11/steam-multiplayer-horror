using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
public class EndGameHandler : NetworkBehaviour
{
    [SyncVar (hook = nameof(OnAltarsComplete))]
    public bool altarsComplete = false;

    public List<AltarHandler> altars;

    public List<Transform> summonPoints;

    public GameObject summoningCircle;

    public void OnAltarsComplete(bool oldValue, bool newValue) 
    {
        altarsComplete = newValue;
        SpawnSummoningCircle();
    }

    public void SpawnSummoningCircle() 
    {
        Debug.Log("Circle Summoned");
        int randomSpawn = Random.Range(0, summonPoints.Count);

        GameObject summonObject = Instantiate(summoningCircle, summonPoints[randomSpawn].position, Quaternion.identity);
        summonObject.transform.SetPositionAndRotation(new Vector3(summonPoints[randomSpawn].position.x, -1.85f, summonPoints[randomSpawn].position.z), Quaternion.Euler(-90f,0f,0f));
        NetworkServer.Spawn(summonObject);      
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
