using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehavior : MonoBehaviour
{
    public enum RobotFSMStates {
        Idle,
        Scared
    }

    public GameObject dialoguePanel;
    public int npcNumber;
    public bool canTalk = false;
    
    private Animator anim;
    public RobotFSMStates currentState = RobotFSMStates.Idle;
    private DialogueBox dialogue;

    // Start is called before the first frame update
    void Start()
    {
        dialogue = dialoguePanel.GetComponent<DialogueBox>();
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.X)) {
            if(canTalk) {
                dialogue.SetNPC(npcNumber);
                dialogue.PauseGame();
            }
        }

        switch(currentState) {
            case RobotFSMStates.Idle:
                UpdateIdleState();
                break;
            case RobotFSMStates.Scared:
                UpdateScaredState();
                break;
        }

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTalk = true;
        }

    }

    public void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player")) {
           canTalk = false; 
        }
    }

    private void UpdateIdleState() {
        anim.SetInteger("RobotAnimState", 0);
        currentState = RobotFSMStates.Idle;
    }

    private void UpdateScaredState() {
        anim.SetInteger("RobotAnimState", 1);
        currentState = RobotFSMStates.Scared;
    }
}
