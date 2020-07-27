using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueBoxHandler : MonoBehaviour
{
    public GameObject handlerParent; // DialogueBoxHandler

    public TextMeshProUGUI mainTextbox; // Where the main dialogue is displayed.

    public TextMeshProUGUI nametagText; // Where the name of the character who's talking is displayed.
    public Image nametagPanel; // Where the name is displayed.

    public Transform choiceGrid; // The grid where the choice buttons will be stored as children.
    public GameObject buttonPrefab; // Prefab of the choice option button.

    public void openDialogueBox()
    {
        for (int i = 0; i < handlerParent.transform.childCount; ++i)
        {
            handlerParent.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void setDialogueText(string s)
    {
        mainTextbox.SetText(s);
    }

    public void setNametag(bool enable, string nameText, Color textColor, Color panelColor)
    {
        nametagPanel.color = panelColor;
        nametagText.color = textColor;
        nametagText.SetText(nameText);

        nametagPanel.gameObject.SetActive(enable);
        nametagText.gameObject.SetActive(enable);
    }

    public void closeDialogueBox()
    {
        for (int i = 0; i < handlerParent.transform.childCount; ++i)
        {
            handlerParent.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void createChoiceButtons(string[] choices, DialogueNode[] postChoiceNodes, DialogueTrigger trigger, DialogueNode srcNode)
    {
        int i = 0;

        foreach (string s in choices)
        {
            GameObject objTemp = Instantiate(buttonPrefab, choiceGrid);

            ChoiceButton choiceBtnTemp = objTemp.GetComponent<ChoiceButton>();
            choiceBtnTemp.setChoiceText(s);

            Button clickBtnTemp = objTemp.GetComponent<Button>();

            int tempIndex = i;
            clickBtnTemp.onClick.AddListener(() => doChoice(postChoiceNodes[tempIndex], trigger, srcNode));

            i++;
        }
    }

    public void closeChoices()
    {
        int numChildren = choiceGrid.childCount;
        for (int i = 0; i < numChildren; ++i)
        {
            GameObject.Destroy(choiceGrid.GetChild(i).gameObject);
        }
    }

    public bool areChoicesOpen()
    {
        return choiceGrid.childCount > 0;
    }

    public void doChoice(DialogueNode dstNode, DialogueTrigger trigger, DialogueNode srcNode)
    {
        trigger.setCurrentNode(dstNode);
        srcNode.initNode();
        closeChoices();
        trigger.doActionOnTrigger();
    }
}
