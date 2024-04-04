using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutChildren : MonoBehaviour
{
    public float timeToFade = 1.0f;

    private bool canFade;
    private Color alphaColor;

    private Color startColor;

    private MeshRenderer[] renderers;
    private float elapsedTime = 0;

    public void Start()
    {
        renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        canFade = false;
        alphaColor = renderers[0].material.color;
        startColor = renderers[0].material.color;
        alphaColor.a = 0;
        
    }
    public void Update()
    {
        if (canFade)
        {
            print(renderers[0].sharedMaterial.color);
            //print(Time.deltaTime);
            elapsedTime += Time.deltaTime;
            // renderers[0].sharedMaterial.SetColor("_Color", Color.Lerp(startColor, alphaColor, elapsedTime/timeToFade));
            foreach (MeshRenderer rend in renderers)
            {
                rend.material.SetColor("_Color", Color.Lerp(startColor, alphaColor, elapsedTime/timeToFade));
                //rend.material.color = Color.Lerp(startColor, alphaColor, timeToFade * Time.deltaTime);
                //rend.material.SetColor
            }
        }
    }

    public void StartFade()
    {
        canFade = true;
    }
}
