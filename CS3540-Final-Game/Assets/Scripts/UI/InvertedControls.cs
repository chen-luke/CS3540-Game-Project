using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class InvertedControls : MonoBehaviour
{
    void Start()
    {
        // no bool, so using int (0 = false, 1 = true)
        int isOn = PlayerPrefs.GetInt("invertedControls", 1);
        if (isOn == 0)
        {
            gameObject.GetComponent<Toggle>().isOn = false;
        }
        else if (isOn == 1)
        {
            gameObject.GetComponent<Toggle>().isOn = true;
        }

    }

    public void ChangeValue()
    {
        // no bool, so using int (0 = false, 1 = true)
        bool isOn = gameObject.GetComponent<Toggle>().isOn;
        if (isOn)
        {
            PlayerPrefs.SetInt("invertedControls", 1);
        }
        else 
        {
            PlayerPrefs.SetInt("invertedControls", 0);
        }
         PlayerPrefs.Save();
    }
}
