
using UnityEngine;

public class PotionBehavior : MonoBehaviour
{
    public int rotationAmount = 45;
    public float bobbingHeight = 0.1f; // The maximum height the object will bob
    public float bobbingSpeed = 1.0f; // The speed of the bobbing movement
    public AudioClip potionPickupSFX;
    private float startY; // The initial y-position of the object

    void Start()
    {
        startY = transform.position.y; // Store the initial y-position of the object
    }

    void Update()
    {
        potionAnimation();
    }

    private void potionAnimation() {
        float newY = startY + Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight;
        transform.Rotate(Vector3.up * rotationAmount * Time.deltaTime); // Rotation Animation
        transform.position = new Vector3(transform.position.x, newY, transform.position.z); // Bobbing Animation
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioSource.PlayClipAtPoint(potionPickupSFX, Camera.main.transform.position);
            Destroy(gameObject, 0.5f);
        }
    }
}