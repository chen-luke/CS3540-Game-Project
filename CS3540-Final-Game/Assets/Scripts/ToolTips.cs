using TMPro;
using UnityEngine;

public class ToolTips : MonoBehaviour
{

    public GameObject titleObj;
    public GameObject toolTipObj;
    public GameObject panel;

    public void MovementTip() {
        titleObj.GetComponent<TextMeshProUGUI>().text = "Movement Controls";
        toolTipObj.GetComponent<TextMeshProUGUI>().text = "Use the W, A, S, D keys to move and the spacebar to jump. Hold shift while moving to jog. To attack, use the left mouse button. Hit ESC to pause the game.";
        PauseGame();
        //Invoke("PauseGame", 2f);
    }

    public void InteractionTip() {
        titleObj.GetComponent<TextMeshProUGUI>().text = "Interactions";
        toolTipObj.GetComponent<TextMeshProUGUI>().text = "To converse with an NPC, move close to the NPC and press X.";
        PauseGame();
        //Invoke("PauseGame", 2f);
    }

    public void PotionTip() {
        titleObj.GetComponent<TextMeshProUGUI>().text = "Potions";
        toolTipObj.GetComponent<TextMeshProUGUI>().text = "Pick up potions by walking into them. To drink a health potion, press Q. To drink a mana potion, press E.";
        PauseGame();
    }

    public void SuperJump() {
        titleObj.GetComponent<TextMeshProUGUI>().text = "Super Jump";
        toolTipObj.GetComponent<TextMeshProUGUI>().text = "To super jump, hold shift while pressing the spacebar.";
        PauseGame();
    }

    public void SuperAttack() {
        titleObj.GetComponent<TextMeshProUGUI>().text = "Super Attack";
        toolTipObj.GetComponent<TextMeshProUGUI>().text = "To do a super attack, press F.";
        PauseGame();
    }

    void PauseGame() {
        panel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame() {
        panel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
}
