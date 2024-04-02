using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootProjectile : MonoBehaviour
{
    // Start is called before the first frame update

    public float projectileSpeed;
    public GameObject projectile;
    public AudioClip projectileSFX;
    GameObject projectileSource;


    void Start()
    {
        projectileSource = GameObject.FindGameObjectWithTag("ProjectileSource");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShootSlashProjectile()
    {
        GameObject swordSlash = Instantiate(projectile, projectileSource.transform.position + projectileSource.transform.forward, projectileSource.transform.rotation);
        
        //Rigidbody rb = swordSlash.GetComponent<Rigidbody>();

        //rb.AddForce(projectileSource.transform.forward * projectileSpeed, ForceMode.VelocityChange);

        //projectile.transform.SetParent(GameObject.FindGameObjectWithTag("ProjectileParent").transform);

        AudioSource.PlayClipAtPoint(projectileSFX, transform.position);
    }

}
