using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
public class BallController : MonoBehaviour
{
    public GameObject BallModel;

    private void Start()
    {
        BallModel.SetActive(false);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game") 
        {
            if (BallModel.activeSelf == false) 
            {
                BallModel.SetActive(true);
                //SetBallPosition();
            }
        }
    }

    public void SetBallPosition() 
    {
        transform.position = new Vector3(0f, 0f, 0f);
    }
}
