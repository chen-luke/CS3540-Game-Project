using UnityEngine;

// behavior to fade out any meshes that are children of this gameobject
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
            elapsedTime += Time.deltaTime;
            foreach (MeshRenderer rend in renderers)
            {
                rend.material.SetColor("_Color", Color.Lerp(startColor, alphaColor, elapsedTime/timeToFade)); // lerp alpha to 0
            }
        }
    }

    public void StartFade()
    {
        canFade = true;
    }
}
