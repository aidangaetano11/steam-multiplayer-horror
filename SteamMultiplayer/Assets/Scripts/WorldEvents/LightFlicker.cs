using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class LightFlicker : MonoBehaviour
{
    public List<Light> spotlights;
    public MeshRenderer mesh;

    public bool lightsOn = true;
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        for (int i = 0; i < this.gameObject.transform.childCount-1; i++)
        {
            spotlights.Add(this.gameObject.transform.GetChild(i).GetComponent<Light>());
        }
        StartCoroutine("LightHandler", Random.Range(0,40));
    }

    public IEnumerator LightHandler(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            StopCoroutine("LightHandler");
            TurnOffLights();
        }
    }

    public void TurnOffLights() 
    {
        foreach (Light light in spotlights) 
        {
            light.enabled = false;
        }
        mesh.enabled = false;
        StartCoroutine("FlickLightsBackOn", Random.Range(1.0f, 20.0f));
    }

    public IEnumerator FlickLightsBackOn(float delay) 
    {
        while (true) 
        {
            yield return new WaitForSeconds(delay);
            StopCoroutine("FlickLightsBackOn");
            TurnOnLights();
        }
    }

    public void TurnOnLights()
    {
        foreach (Light light in spotlights)
        {
            light.enabled = true;
        }
        mesh.enabled = true;
        StartCoroutine("LightHandler", Random.Range(0,40));
    }

}
