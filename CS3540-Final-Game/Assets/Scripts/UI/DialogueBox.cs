using System;
using TMPro;
using UnityEngine;

// behavior for dialogue box
public class DialogueBox : MonoBehaviour
{
    public GameObject nameObj;
    public GameObject dialogueObj;
    public GameObject panel;
    public GameObject next;
    public GameObject prev;
    public GameObject closeBtn;
    public AudioClip npcSFX;

    private string[] npc1_dialogue;
    private string[] npc2_dialogue;
    private string[] npc3_dialogue;
    private string[] currentDialogue = new string[0];
    private string currentNPC = "";
    private int currentIndx;


    // Start is called before the first frame update
    void Start()
    {
        PopulateNPC1();
        PopulateNPC2();
        PopulateNPC3();
        SetNPC(1);
    }

    // go to next panel of dialogue
    public void NextPanel()
    {
        if (currentIndx < currentDialogue.Length - 1)
        {
            currentIndx += 1;
            SetPanel();
        }
    }

    // go to previous panel of dialogue
    public void PreviousPanel()
    {
        if (currentIndx > 0)
        {
            currentIndx -= 1;
            SetPanel();
        }
    }

    // pause game and allow player to interact with buttons
    public void PauseGame()
    {
        SetPanel();
        panel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // resume game
    public void ResumeGame()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    // set which NPC's dialogue will show
    public void SetNPC(int NPC)
    {
        if (!panel.activeSelf)
        {
            switch (NPC)
            {
                case 1:
                    currentDialogue = npc1_dialogue;
                    currentIndx = 0;
                    currentNPC = "Fundys";
                    break;
                case 2:
                    currentDialogue = npc2_dialogue;
                    currentIndx = 0;
                    currentNPC = "Ood";
                    break;
                case 3:
                    currentDialogue = npc3_dialogue;
                    currentIndx = 0;
                    currentNPC = "Al-Go";
                    break;
            }
        }

    }

    // set the currently displayed dialogue and buttons to go to next/prev panel or close panel 
    private void SetPanel()
    {
        gameObject.GetComponent<AudioSource>().ignoreListenerPause = true;
        gameObject.GetComponent<AudioSource>().Play();
        nameObj.GetComponent<TextMeshProUGUI>().text = currentNPC;
        dialogueObj.GetComponent<TextMeshProUGUI>().text = currentDialogue[currentIndx];
        if (currentIndx < currentDialogue.Length - 1)
        {
            next.SetActive(true);
            closeBtn.SetActive(false);
        }
        else
        {
            next.SetActive(false);
            closeBtn.SetActive(true);
        }

        if (currentIndx > 0)
        {
            prev.SetActive(true);
        }
        else
        {
            prev.SetActive(false);
        }
    }

    private void PopulateNPC1()
    {
        String panel1 = "Hello, explorer! You had quite a crash there. I'm so glad to see that you're okay. My name is Fundys and I live here on Khoury.";
        String panel2 = "I reckon you probably want to get back to your spaceship up there on that top island. Well, you'll need some help to do that.";
        String panel3 = "You're going to need to get up to the second island and you won't be able to do it without a pair of Super Boots.";
        String panel4 = "Lucky for you, there is a pair of Super Boots that will help you jump super high just along this path here.";
        String panel5 = "But, please be careful! There are machines around the island that spawn creatures meant to protect Khoury. For a long time, we lived in harmony with them, but something has corrupted the machines and now the creatures attack anything that moves!";
        String panel6 = "You can defeat the creatures, but you'll have to destroy the machines to keep more from coming.";
        String panel7 = "Good luck, explorer!";
        npc1_dialogue = new string[] { panel1, panel2, panel3, panel4, panel5, panel6, panel7 };
    }

    private void PopulateNPC2()
    {
        String panel1 = "Hello, explorer! I saw that crash, earlier. Oh, and it looks like you spoke to Fundys already. I'm Ood and I live on Khoury, too.";
        String panel2 = "I'm sure you want to get to your ship on that neighboring island, but there is a big wall of stone in the way.";
        String panel3 = "A normal attack won't make a dent in that wall, but there is a Super Glove on that platform floating above the lake that may be of use. It will allow you to do stronger attacks that can bring down that wall.";
        String panel4 = "You'll have to be cautious, though! I'm assuming Fundys told you about the corrupted spawners on the islands. Well, the spawners on this island have been corrupted, too. There are a bunch of angry creatures on this island that like to shoot fireballs.";
        String panel5 = "Good luck, explorer!";
        npc2_dialogue = new string[] { panel1, panel2, panel3, panel4, panel5 };
    }

    private void PopulateNPC3()
    {
        String panel1 = "H-h-h-hello, explorer! That m-m-must be your sh-sh-ship that crashed, earlier. I'm sure y-you want to g-g-get it back, but there's a bit of a p-p-problem.";
        String panel2 = "The c-c-corruption that infected the sp-spawners seems to have gotten to your sh-sh-ship too. It's not moving now, but y-y-you should have seen it when it got corrupted! Oh, it was a-a-a-awful.";
        String panel3 = "If you want to g-get your ship back, you'll n-n-need to defeat it first. It's not going to be easy, b-but I believe in you!";
        String panel4 = "Oh, and I f-forgot to mention. M-my name is Al-Go. G-g-good luck, explorer!";
        npc3_dialogue = new string[] { panel1, panel2, panel3, panel4 };
    }

    private void PlayAudio()
    {
        AudioSource.PlayClipAtPoint(npcSFX, Camera.main.transform.position);

    }
}
