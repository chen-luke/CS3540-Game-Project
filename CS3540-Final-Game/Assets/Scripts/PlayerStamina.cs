using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerStamina : MonoBehaviour
{
    public int startingStamina = 100;
    public AudioClip deadSFX;
    public Slider staminaSlider;
    int currentStamina;
    int maxStamina = 100;

    private const string STAMINA_POTION_AMT_ICON = "StamPotionIcon";



    // Start is called before the first frame update
    void Start()
    {
        currentStamina = startingStamina;
        staminaSlider.value = currentStamina;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) {
            AddStamina(StamPotionBehavior.healAmount);
        }
    }

    public void AddStamina(int staminaAmt) {

        if (currentStamina < maxStamina && LevelManager.staminaPotionAmt > 0) {
            currentStamina += staminaAmt;
            staminaSlider.value = currentStamina;
            LevelManager.staminaPotionAmt --;
            FindObjectOfType<LevelManager>().UpdatePotionCountUI(STAMINA_POTION_AMT_ICON, LevelManager.staminaPotionAmt);
        }
    }


}
