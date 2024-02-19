using Unity.VisualScripting;
using UnityEngine;

public class PotionBehavior : MonoBehaviour
{
    public int rotationAmount = 45;

    public float bobbingHeight = 0.5f; // The maximum height the object will bob
    public float bobbingSpeed = 1.0f; // The speed of the bobbing movement

    private float startY; // The initial y-position of the object
    protected const string HP_POTION_AMT_ICON = "HpPotionIcon";
    protected const string STR_POTION_AMT_ICON = "StrPotionIcon";
    protected LevelManager lvlManager;

    protected virtual void Start()
    {
        lvlManager = FindObjectOfType<LevelManager>();
        startY = transform.position.y; // Store the initial y-position of the object
    }

    protected virtual void Update() {
        potionAnimation();
    }
    protected void UpdatePotionCountUI(string iconTag, int potionAmt)
    {
        lvlManager.UpdatePotionCountUI(iconTag, potionAmt);
    }

    private void potionAnimation() {
        float newY = startY + Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight;
        transform.Rotate(Vector3.up * rotationAmount * Time.deltaTime); // Rotation Animation
        transform.position = new Vector3(transform.position.x, newY, transform.position.z); // Bobbing Animation
    }
}