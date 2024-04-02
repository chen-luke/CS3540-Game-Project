using UnityEngine;
using UnityEngine.UI;

public class PlayerStr : MonoBehaviour
{
    // public int startingStr = 100;
    // public AudioClip deadSFX;

    // public AudioClip drinkPotionSFX;
    // public Slider strengthBar;
    // int currentStr;
    // int maxStr = 100;

    // private const string STR_POTION_AMT_ICON = "StrPotionIcon";



    // // Start is called before the first frame update
    // void Start()
    // {
    //     currentStr = startingStr;
    //     strengthBar.value = currentStr;
    // }

    // // Update is called once per frame
    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.E)) {
    //         Addstr(ManaPotionBehavior.manaAmt);
    //     }
    // }

    // public void Addstr(int manaAmt) {

    //     if (currentStr < maxStr && LevelManager.manaPotionAmt > 0) {
    //         currentStr += manaAmt;
    //         strengthBar.value = currentStr;
    //         LevelManager.manaPotionAmt --;
    //         AudioSource.PlayClipAtPoint(drinkPotionSFX, Camera.main.transform.position);
    //         FindObjectOfType<LevelManager>().UpdatePotionCountUI(STR_POTION_AMT_ICON, LevelManager.strPotionAmt);
    //     }
    // }
}
