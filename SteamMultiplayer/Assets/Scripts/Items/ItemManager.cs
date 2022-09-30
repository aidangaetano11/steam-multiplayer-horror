using UnityEngine;
using Mirror;
public class ItemManager : MonoBehaviour
{
    public InventoryManager inventoryManager;

    public string itemName;

    public Collider itemCollider;
    public AudioSource itemDropSound;

    public GameObject itemPrefab;
    public GameObject itemModel;

    [Header("Color")]
    public Color interactorColor;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Map") 
        {
            itemDropSound.Play();
        }       
    }
}
