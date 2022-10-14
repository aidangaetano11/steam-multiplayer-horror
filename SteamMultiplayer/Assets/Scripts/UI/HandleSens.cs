using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HandleSens : MonoBehaviour
{

    public float sens;
    public float displaySens;
    private double sensValue;
    public TextMeshProUGUI displayText;
    public CameraController cam;

    public void Start()
    {
        SetLevel(250f);  //sets the sense to 500 off the rip
    }

    public void Update()
    {
        cam.mouseSens = sens;    //we change camera sens to new sens
        displaySens = sens / 100f;   //we have display sens which is meant for displaying to user to ex: 3.0 instead of 300
        sensValue = System.Math.Round(displaySens, 2);    //then we convert the number to round to 2 decimal places
        displayText.text = sensValue.ToString();   //and convert the double to a string to display on screen
    }
    public void SetLevel(float sliderValue)    //we set the value with this slider
    {
        sens = sliderValue;
    }
}
