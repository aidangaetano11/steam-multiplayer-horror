using UnityEngine;
using Mirror;
public class EndGameTest : NetworkBehaviour
{
    public MeshRenderer mesh;

    public Color red;
    public Color green;

    private void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        mesh.material.color = red;
    }

    [Command (requiresAuthority = false)]
    public void CmdChangeSphereColor()          //called from inventory manager
    {
        mesh.material.color = green;    //upon all items picked up, it will change end game sphere from red to green
    }
}
