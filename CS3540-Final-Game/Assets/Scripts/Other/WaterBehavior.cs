using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBehavior : MonoBehaviour
{
    public AudioClip splashEnterSFX;
    public AudioClip splashExitSFX;

    void OnTriggerEnter(Collider other) {
        AudioSource.PlayClipAtPoint(splashEnterSFX, other.transform.position, .5f);
    }

     void OnTriggerExit(Collider other) {
        AudioSource.PlayClipAtPoint(splashExitSFX, other.transform.position, .5f);
    }
}
