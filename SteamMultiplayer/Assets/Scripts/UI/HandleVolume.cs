using UnityEngine;
using UnityEngine.Audio;
public class HandleVolume : MonoBehaviour
{
    public AudioMixer mixer;

    public void SetLevel(float sliderValue)
    {
        mixer.SetFloat("MainVolume", Mathf.Log10(sliderValue) * 20);   //converts slider value into decibals
    }
}
