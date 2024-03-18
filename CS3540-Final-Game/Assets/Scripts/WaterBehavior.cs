using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBehavior : MonoBehaviour
{
    public AudioClip splashEnterSFX;

    public AudioClip splashExitSFX;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other) {
        AudioSource.PlayClipAtPoint(splashEnterSFX, Camera.main.transform.position);
        print(other);
    }

     void OnTriggerExit(Collider other) {
        AudioSource.PlayClipAtPoint(splashExitSFX, Camera.main.transform.position);
        print(other);
    }
}
