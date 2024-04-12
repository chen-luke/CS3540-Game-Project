using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBehavior : MonoBehaviour
{
    public AudioClip splashEnterSFX;
    public AudioClip splashExitSFX;

    AudioSource source;

    void Start() {
        source = GetComponent<AudioSource>();
        source.volume = .4f;
        source.minDistance = .1f;
        source.maxDistance = 10f;
    }

    void OnTriggerEnter(Collider other) {
        source.clip = splashEnterSFX;
        AudioSource.PlayClipAtPoint(splashEnterSFX, other.transform.position, .5f);
    }

     void OnTriggerExit(Collider other) {
        AudioSource.PlayClipAtPoint(splashExitSFX, other.transform.position, .5f);
    }
}
