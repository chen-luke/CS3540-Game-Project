using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerStr : MonoBehaviour
{
    public int startingStr = 100;
    public AudioClip deadSFX;
    public Slider strSlider;
    int currentStr;
    int maxStr = 100;

    private const string STR_POTION_AMT_ICON = "StrPotionIcon";



    // Start is called before the first frame update
    void Start()
    {
        currentStr = startingStr;
        strSlider.value = currentStr;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) {
            Addstr(StrPotionBehavior.healAmount);
        }
    }

    public void Addstr(int strAmt) {

        if (currentStr < maxStr && LevelManager.strPotionAmt > 0) {
            currentStr += strAmt;
            strSlider.value = currentStr;
            LevelManager.strPotionAmt --;
            FindObjectOfType<LevelManager>().UpdatePotionCountUI(STR_POTION_AMT_ICON, LevelManager.strPotionAmt);
        }
    }


}
