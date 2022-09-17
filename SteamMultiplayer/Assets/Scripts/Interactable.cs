using UnityEngine;
using UnityEngine.Events;
using Mirror;
using Steamworks;
public class Interactable : MonoBehaviour
{
    public UnityEvent onInteract;
    public Sprite interactIcon;
    public int ID;
    public Vector2 iconSize;
    void Start()
    {
        ID = Random.Range(0, 99999);
        
    }

    void Update()
    {
        
    }
}
