using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehavior : MonoBehaviour
{

    public GameObject dialoguePanel;
    public int npcNumber;
    public bool canTalk = false;
    private DialogueBox dialogue;

    // Start is called before the first frame update
    void Start()
    {
        dialogue = dialoguePanel.GetComponent<DialogueBox>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dialogue.SetNPC(npcNumber);
            canTalk = true;
            //dialogue.PauseGame();

            //Destroy(gameObject.GetComponent<CapsuleCollider>());
        }

    }

    public void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player")) {
           canTalk = false; 
        }
    }
}
