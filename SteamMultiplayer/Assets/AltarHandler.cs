using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using Mirror;
public class AltarHandler : NetworkBehaviour
{
    public GameObject particle;
    public VisualEffect visualEffect;
    public Light particleLight;
    public Color particleColor;

    [SyncVar (hook = nameof(OnActiveChange))]
    public bool isActive = false;

    void OnActiveChange(bool oldValue, bool newValue)
    {
        if (oldValue)      //if is active
        {
            Debug.Log("Disable Smoke");
            particleLight.enabled = false;
            visualEffect.Stop();

        }
        else
        {
            Debug.Log("Enable Smoke");
            visualEffect.SetVector4("SmokeColor", particleColor);
            particleLight.color = particleColor;
            particleLight.enabled = true;
            visualEffect.Play();
        }
    }

    public void Start()
    {
        visualEffect.Stop();
        particleLight.enabled = false;
    }
}
