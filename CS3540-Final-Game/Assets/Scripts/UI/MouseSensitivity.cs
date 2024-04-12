using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

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
        try
        {
            CinemachineFreeLook freeLook = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<CinemachineFreeLook>();
            freeLook.m_XAxis.m_MaxSpeed = 50 * PlayerPrefs.GetFloat("mouseSensitivity", 5);
            freeLook.m_YAxis.m_MaxSpeed = PlayerPrefs.GetFloat("mouseSensitivity", 5) * .4f;
        }
        catch
        {

        }
    }
}
