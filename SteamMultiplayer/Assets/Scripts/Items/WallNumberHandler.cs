using UnityEngine;
using TMPro;
using Mirror;
public class WallNumberHandler : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnColorChange))]
    public Color numColor;

    public TextMeshPro number;

    public void OnColorChange(Color oldColor, Color newColor) 
    {
        number.color = newColor;
    }

    public void Start()
    {
        number = GetComponent<TextMeshPro>();
        number.color = Color.red;
    }
}
