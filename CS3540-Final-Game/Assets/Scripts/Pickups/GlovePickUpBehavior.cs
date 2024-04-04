using UnityEngine;

// behavior for the super glove pickup object
public class GlovePickUpBehavior : MonoBehaviour
{
    public AudioClip pickupSound;
    public int rotationAmount = 45;
    public float bobbingHeight = 0.1f; // The maximum height the object will bob
    public float bobbingSpeed = 1.0f; // The speed of the bobbing movement

    private float startY; // The initial y-position of the object
    private bool soundPlayed = false;
    
    // Start is called before the first frame update
    void Start()
    {   
        startY = transform.position.y; // Store the initial y-position of the object
        gameObject.SetActive(!LevelManager.glovePickedUp);
    }

    // Update is called once per frame
    void Update()
    {
        gauntletAnimation();
    }


    private void gauntletAnimation() {
        float newY = startY + Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight;
        transform.Rotate(Vector3.forward * rotationAmount * Time.deltaTime); // Rotation Animation
        transform.position = new Vector3(transform.position.x, newY, transform.position.z); // Bobbing Animation
    }
    
    private void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("Player")) {
            if (!soundPlayed) {
                AudioSource.PlayClipAtPoint(pickupSound, Camera.main.transform.position);
                soundPlayed = true;
            }
            Destroy(gameObject, 0.5f);
            LevelManager.glovePickedUp = true;
        }
    }

    
}