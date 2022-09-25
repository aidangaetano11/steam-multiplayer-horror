using UnityEngine;
using UnityEngine.VFX;
using Mirror;
public class AltarHandler : NetworkBehaviour
{
    public GameObject particle;
    public VisualEffect visualEffect;
    public Light particleLight;
    public Color particleColor;

    public bool isActive = false;

    public void Start()
    {
        visualEffect.Stop();
        particleLight.enabled = false;
    }

    public void EnableParticle() 
    {
        visualEffect.SetVector4("SmokeColor", particleColor);
        particleLight.color = particleColor;
        particleLight.enabled = true;
        visualEffect.Play();
        isActive = true;
    }

    public void DisableParticle() 
    {
        particleLight.enabled = false;
        visualEffect.Stop();
        isActive = false;
    }
}
