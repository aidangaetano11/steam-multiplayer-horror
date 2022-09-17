using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GroundSearchManager : MonoBehaviour
{
    [Header("Ground Detection")]
    public LayerMask groundMask;
    public float maxDistanceFromBody = 1.0f;
    public float distanceFromRaycast;
    public bool hasMoved;

    public RaycastHit hit;

    public FastIKFabric ik;

    [Header("Footstep Audio")]
    public MonsterAudio monsterAudio;

    private void Update()
    {
        distanceFromRaycast = Vector3.Distance(ik.Target.position, hit.point);


        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, groundMask))  //vector3.right is essentially pointing it down
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
        }
    }

    public IEnumerator MoveTargetToRaycast(float delay) 
    {
        while (true) 
        {
            yield return new WaitForSeconds(delay);         //we will return every .2 seconds/delay seconds
            ik.Target.position = Vector3.Lerp(ik.Target.position, hit.point, 0.6f);
            hasMoved = true;
            monsterAudio.PlayFootsteps();
            StopCoroutine("MoveTargetToRaycast");
        }
    }
}
