using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySFX : MonoBehaviour
{
    public AudioClip SFX;

    public void PlayClip() {
        AudioSource.PlayClipAtPoint(SFX, Camera.main.transform.position);
    }
}
