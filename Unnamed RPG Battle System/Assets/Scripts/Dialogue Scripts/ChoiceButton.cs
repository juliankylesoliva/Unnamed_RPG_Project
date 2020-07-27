using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;

    public void setChoiceText(string s)
    {
        buttonText.SetText(s);
    }
}
