using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseSensitivity : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Slider>().value = PlayerPrefs.GetFloat("mouseSensitivity", 1);
        
    }

    public void ChangeValue()
    {
        PlayerPrefs.SetFloat("mouseSensitivity", gameObject.GetComponent<Slider>().value);
        PlayerPrefs.Save();
    }
}
