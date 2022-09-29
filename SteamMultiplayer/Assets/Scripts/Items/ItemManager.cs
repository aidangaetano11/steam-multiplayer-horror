using UnityEngine;
using Mirror;
public class ItemManager : MonoBehaviour
{
    public InventoryManager inventoryManager;

    public string itemName;

    public GameObject itemPrefab;
    public GameObject itemModel;

    [Header("Color")]
    public Color interactorColor;

}
