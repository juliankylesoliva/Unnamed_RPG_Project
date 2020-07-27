using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum StatSetting {None, Check, Gain} // None: Don't do anything with stats. Check: Enforce a stat check in order to run this node's dialogue. Gain: Give the player a stat increase.
public enum ChosenStat {None, LV, HP, ATK, DEF, SPD, MP, MAG, RES, PRC, BRV, CHA, COM, SKL} // The stat used for the above stat setting.
public enum PostDialogueSetting {None, Choice, Link} // None: Stops the dialogue after this node. Choice: Presents the player with choices. Link: Links to another dialogue node.

public class DialogueNode : MonoBehaviour
{
    [Header("Drag & Drops")]
    public DialogueTrigger trigger; // All nodes within this dialogue tree must reference the same trigger.
    public DialogueBoxHandler handler; // DialogueBoxHandler is the main controller of dialogue-related UI elements.

    [Header("Stat-Related Settings")]
    public StatSetting statSetting;
    public ChosenStat statOfInterest; // Checks or Gains the selected stat.
    public int statAmount = 0;
    public DialogueNode checkFail; // Use when stat setting is set to Check.

    [Header("Nametag Settings")]
    public bool showNametag;
    public string nametag;
    public Color nametagPanelColor;
    public Color nametagTextColor;

    [Header("Dialogue Settings")]
    public bool enableScrolling;
    public float tickValue = 0.05f;
    public string[] dialogue; // The main dialogue shown.

    [Header("Post Dialogue Settings")]
    public PostDialogueSetting afterDialogue;
    public string[] choices; // Text used for choices after the main dialogue.
    public DialogueNode[] postChoiceDialogue; // Dialogue Nodes for after making certain choices.
    public DialogueNode linkedDialogue; // This node plays after this one when set to Link.


    /* PRIVATE VARIABLES */
    private int dialogueIndex = 0;
    private bool isTextScrolling = false;

    /* FUNCTIONS */
    public void initNode()
    {
        dialogueIndex = 0;
    }

    public void doDialogue(CharData playerStats, CharCtrl playerCtrl)
    {
        if (isTextScrolling)
        {
            isTextScrolling = false;
            return;
        }

        if(dialogueIndex == 0)
        {
            switch (statSetting)
            {
                case StatSetting.Check:
                    if (playerStats != null)
                    {
                        if (!checkStatRequirement(playerStats))
                        {
                            initNode();
                            if (checkFail != null)
                            {
                                trigger.setCurrentNode(checkFail);
                                trigger.doActionOnTrigger();
                            }
                            return;
                        }
                    }
                    break;
                case StatSetting.Gain:
                    if (playerStats != null)
                    {
                        gainStatAmount(playerStats);
                    }
                    break;
                default:
                    break;
            }
        }

        if(dialogueIndex < dialogue.Length)
        {
            handler.openDialogueBox();
            handler.setNametag(showNametag, nametag, nametagTextColor, nametagPanelColor);

            if(enableScrolling)
            {
                StartCoroutine(doTextScroll(dialogue[dialogueIndex], tickValue));
            }
            else
            {
                handler.setDialogueText(dialogue[dialogueIndex]);
            }
            
            dialogueIndex++;
        }
        else
        {
            switch(afterDialogue)
            {
                case PostDialogueSetting.Choice:
                    if(!handler.areChoicesOpen())
                    {
                        handler.createChoiceButtons(choices, postChoiceDialogue, trigger, this);
                    }
                    break;
                case PostDialogueSetting.Link:
                    if(linkedDialogue != null)
                    {
                        initNode();
                        trigger.setCurrentNode(linkedDialogue);
                        trigger.doActionOnTrigger();
                    }
                    else
                    {
                        goto default;
                    }
                    break;
                default:
                    handler.closeDialogueBox();
                    trigger.turnOffDialogue();
                    initNode();
                    if(playerCtrl != null)
                    {
                        playerCtrl.enableMovement = true;
                    }
                    break;
            }
        }
    }

    IEnumerator doTextScroll(string s, float tick)
    {
        isTextScrolling = true;
        string currentText = "";
        int textIndex = 0;
        int textLength = s.Length;

        while(isTextScrolling && textIndex < textLength)
        {
            currentText = $"{currentText}{s.Substring(textIndex, 1)}";
            handler.setDialogueText(currentText);
            textIndex++;
            yield return new WaitForSeconds(tickValue);
        }

        handler.setDialogueText(s);
        isTextScrolling = false;
        yield return null;
    }

    private bool checkStatRequirement(CharData stats)
    {
        switch(statOfInterest)
        {
            case ChosenStat.LV:
                return stats.charLvl >= statAmount;
            case ChosenStat.HP:
                return stats.maxHP >= statAmount;
            case ChosenStat.ATK:
                return stats.ATK >= statAmount;
            case ChosenStat.DEF:
                return stats.DEF >= statAmount;
            case ChosenStat.SPD:
                return stats.SPD >= statAmount;
            case ChosenStat.MP:
                return stats.maxMP >= statAmount;
            case ChosenStat.MAG:
                return stats.MAG >= statAmount;
            case ChosenStat.RES:
                return stats.RES >= statAmount;
            case ChosenStat.PRC:
                return stats.PRC >= statAmount;
            case ChosenStat.BRV:
                return stats.BRV >= statAmount;
            case ChosenStat.CHA:
                return stats.CHA >= statAmount;
            case ChosenStat.COM:
                return stats.COM >= statAmount;
            case ChosenStat.SKL:
                return stats.SKL >= statAmount;
            default:
                return true;
        }
    }

    private void gainStatAmount(CharData stats)
    {
        switch (statOfInterest)
        {
            case ChosenStat.LV:
                //stats.charLvl += statAmount;
                break;
            case ChosenStat.HP:
                stats.maxHP += statAmount;
                break;
            case ChosenStat.ATK:
                stats.ATK += statAmount;
                break;
            case ChosenStat.DEF:
                stats.DEF += statAmount;
                break;
            case ChosenStat.SPD:
                stats.SPD += statAmount;
                break;
            case ChosenStat.MP:
                stats.maxMP += statAmount;
                break;
            case ChosenStat.MAG:
                stats.MAG += statAmount;
                break;
            case ChosenStat.RES:
                stats.RES += statAmount;
                break;
            case ChosenStat.PRC:
                stats.PRC += statAmount;
                break;
            case ChosenStat.BRV:
                stats.BRV += statAmount;
                break;
            case ChosenStat.CHA:
                stats.CHA += statAmount;
                break;
            case ChosenStat.COM:
                stats.COM += statAmount;
                break;
            case ChosenStat.SKL:
                stats.SKL += statAmount;
                break;
            default:
                break;
        }
    }

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }
}
