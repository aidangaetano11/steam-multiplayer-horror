using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterCosmetics : MonoBehaviour
{
    public int currentColorIndex = 0;
    public Material[] playerColors;
    public Image currentColorImage;
    public Text currentColorText;

    public GameObject LocalPlayerObject;
    public PlayerObjectController LocalPlayerController;

    private void Start()
    {
        currentColorIndex = PlayerPrefs.GetInt("currentColorIndex", 0);           //collects previously used color and settings to use every time the game opens
        currentColorImage.color = playerColors[currentColorIndex].color;
        currentColorText.text = playerColors[currentColorIndex].name;
        FindLocalPlayer();
    }

    public void NextColor() 
    {
        if (currentColorIndex < playerColors.Length - 1) 
        {
            currentColorIndex++;
            PlayerPrefs.SetInt("currentColorIndex", currentColorIndex);
            currentColorImage.color = playerColors[currentColorIndex].color;
            currentColorText.text = playerColors[currentColorIndex].name;
            LocalPlayerController.CmdUpdatePlayerColor(currentColorIndex);
        }
    }

    public void PreviousColor()
    {
        if (currentColorIndex > 0) 
        {
            currentColorIndex--;
            PlayerPrefs.SetInt("currentColorIndex", currentColorIndex);
            currentColorImage.color = playerColors[currentColorIndex].color;
            currentColorText.text = playerColors[currentColorIndex].name;
            LocalPlayerController.CmdUpdatePlayerColor(currentColorIndex);
        }
        
    }

    public void FindLocalPlayer()
    {
        LocalPlayerObject = GameObject.Find("LocalGamePlayer");
        LocalPlayerController = LocalPlayerObject.GetComponent<PlayerObjectController>();
        LocalPlayerController.CmdUpdatePlayerColor(currentColorIndex);  //The command was added here so it runs at the start, that way the last color used is automatically applied.
        Debug.Log("FindLocalPlayer called");
    }
}
