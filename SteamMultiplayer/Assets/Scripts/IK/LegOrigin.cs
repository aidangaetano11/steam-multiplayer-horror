using UnityEngine;
public class LegOrigin : MonoBehaviour
{
    public Transform legOrigin;
    public Transform LegEndPiece;


    void Update()
    {
        LegEndPiece.position = legOrigin.position;
    }
}
