using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBarBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject mainCamera;
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(mainCamera.transform);
    }
}