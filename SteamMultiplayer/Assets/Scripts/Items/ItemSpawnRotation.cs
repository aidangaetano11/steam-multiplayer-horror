using UnityEngine;
using System.Collections;
public class ItemSpawnRotation : MonoBehaviour
{
    public Transform currentObject;
    void Start()
    {
        currentObject.Rotate(30, Random.Range(0, 360), 0);
    }
}
