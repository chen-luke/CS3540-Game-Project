using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public float speed = 10f;
    private Vector3 direction;

    public int damage = 30;

    GameObject projectileSource;
    // Start is called before the first frame update
    void Start()
    {
        projectileSource = GameObject.FindGameObjectWithTag("ProjectileSource");
        direction = projectileSource.transform.forward;

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Enemy") || other.CompareTag("Boss")) {
            other.GetComponent<EnemyHealth>().TakeDamage(damage);
        }
    }
}
