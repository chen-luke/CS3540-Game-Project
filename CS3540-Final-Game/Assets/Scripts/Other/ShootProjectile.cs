using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootProjectile : MonoBehaviour
{
    // Start is called before the first frame update

    public float projectileSpeed;
    public GameObject projectile;
    GameObject projectileSource;


    void Start()
    {
        projectileSource = GameObject.FindGameObjectWithTag("ProjectileSource");
    }

    public void ShootSlashProjectile()
    {
        Instantiate(projectile, projectileSource.transform.position + projectileSource.transform.forward, projectileSource.transform.rotation);
    }

}
