using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSetting : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<Slider>().value = PlayerPrefs.GetFloat("volume", 0);

    }

    public void ChangeValue()
    {
        PlayerPrefs.SetFloat("volume", gameObject.GetComponent<Slider>().value);
        PlayerPrefs.Save();
        AudioListener.volume = PlayerPrefs.GetFloat("volume", 5)/10;
    }
}
