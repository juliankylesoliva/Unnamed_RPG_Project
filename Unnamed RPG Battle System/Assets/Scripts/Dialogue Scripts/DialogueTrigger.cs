using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueNode startNode; // This trigger will always start with this node

    private DialogueNode currentNode = null; // What node we are currently in.
    public void setCurrentNode(DialogueNode node)
    {
        currentNode = node;
        currentNode.initNode();
    }

    private bool isActive = false; // Is the dialogue supposed to be on-screen?

    public void turnOnDialogue()
    {
        isActive = true;
    }

    public void turnOffDialogue()
    {
        isActive = false;
    }

    // Extract these from a trigger
    private CharData playerStats = null;
    private CharCtrl playerCtrl = null;

    public void doActionOnTrigger()
    {
        if (!isActive)
        {
            if(playerStats != null && playerCtrl != null)
            {
                turnOnDialogue();
                playerCtrl.enableMovement = false;
                setCurrentNode(startNode);
                currentNode.doDialogue(playerStats, playerCtrl);
            }
        }
        else
        {
            if (playerStats != null && playerCtrl != null)
            {
                currentNode.doDialogue(playerStats, playerCtrl);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            playerStats = other.gameObject.GetComponent<CharData>();
            playerCtrl = other.gameObject.GetComponent<CharCtrl>();
        }
    }

    /*
    // OnTriggerStay does NOT check for input every frame, this functionality has been moved to the Update function.
    private void OnTriggerStay(Collider other)
    {
        if((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0)) && other.tag == "Player" && playerStats != null && playerCtrl != null)
        {
            doActionOnTrigger();
        }
    }
    */

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerStats = null;
            playerCtrl = null;
        }
    }

    private void Update()
    {
        if (playerStats != null && playerCtrl != null && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0)))
        {
            doActionOnTrigger();
        }
    }
}
