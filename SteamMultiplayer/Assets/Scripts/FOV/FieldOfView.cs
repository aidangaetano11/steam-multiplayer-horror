using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float viewRadius;

    [Range(0, 360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();          //create a list of targets we can see

    public float meshResolution;

    public MeshFilter viewMeshFilter;
    Mesh viewMesh;

    private void Start()
    {
        StartCoroutine("FindTargetsWithDelay", .2f);       //calls the IEnumerator function every 0.2 seconds
    }
    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)                                         //while coroutine is active
        {
            yield return new WaitForSeconds(delay);         //we will return every .2 seconds/delay seconds
            FindVisibleTargets();                           //call findvisibletargets function
        }
    }

    private void Update()
    {
        DrawFieldOfView();
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();                              //clears list every time function is called
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);          //creates radius in which we check if targets are there

        for (int i = 0; i < targetsInViewRadius.Length; i++)            //loops through every collider in array
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;        //gets direction from self to target that is visible
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)              //if target is in sphere,
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);      //then we get the distance of the target

                if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleMask))  //and check if there are no obstacles in the way of targets (with an arc raycast)
                {
                    visibleTargets.Add(target);                   //add to list of visible targets if there is no obstacles in the way
                }
            }
        }
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;;
            Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * viewRadius, Color.red);   //draw line from monster to target if in LOS
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)          //converts global andle to radians
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

}
