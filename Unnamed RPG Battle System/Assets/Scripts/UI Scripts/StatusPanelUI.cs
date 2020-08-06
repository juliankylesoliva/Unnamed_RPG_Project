using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusPanelUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;

    public TextMeshProUGUI hpText;
    public Slider hpSlider;

    public TextMeshProUGUI mpText;
    public Slider mpSlider;

    public void setHUD(CharacterInfo data)
    {
        setName(data.Name);
        setHP(data.CurrentHP, data.MaxHP);
        setMP(data.CurrentMP, data.MaxMP);
    }

    public IEnumerator drainHPBar(CharacterInfo data)
    {
        int goal = data.CurrentHP;

        while (hpSlider.value > goal)
        {
            hpSlider.value--;
            hpText.SetText($"{hpSlider.value} / {hpSlider.maxValue}");
            yield return new WaitForSeconds(0.001f);
        }

        if(hpSlider.value < goal)
        {
            hpSlider.value = goal;
            hpText.SetText($"{hpSlider.value} / {hpSlider.maxValue}");
        }

        yield return new WaitForSeconds(0.0f);
    }

    public IEnumerator fillHPBar(CharacterInfo data)
    {
        int goal = data.CurrentHP;

        while (hpSlider.value < goal)
        {
            hpSlider.value++;
            hpText.SetText($"{hpSlider.value} / {hpSlider.maxValue}");
            yield return new WaitForSeconds(0.001f);
        }

        if(hpSlider.value > goal)
        {
            hpSlider.value = goal;
            hpText.SetText($"{hpSlider.value} / {hpSlider.maxValue}");
        }

        yield return new WaitForSeconds(0.0f);
    }

    public IEnumerator drainMPBar(CharacterInfo data)
    {
        int goal = data.CurrentMP;

        while (mpSlider.value > goal)
        {
            mpSlider.value--;
            mpText.SetText($"{mpSlider.value} / {mpSlider.maxValue}");
            yield return new WaitForSeconds(0.001f);
        }

        if(mpSlider.value < goal)
        {
            mpSlider.value = goal;
            mpText.SetText($"{mpSlider.value} / {mpSlider.maxValue}");
        }

        yield return new WaitForSeconds(0.0f);
    }

    public IEnumerator fillMPBar(CharacterInfo data)
    {
        int goal = data.CurrentMP;

        while (mpSlider.value < goal)
        {
            mpSlider.value++;
            mpText.SetText($"{mpSlider.value} / {mpSlider.maxValue}");
            yield return new WaitForSeconds(0.001f);
        }

        if(mpSlider.value > goal)
        {
            mpSlider.value = goal;
            mpText.SetText($"{mpSlider.value} / {mpSlider.maxValue}");
        }

        yield return new WaitForSeconds(0.0f);
    }

    public void setName(string name)
    {
        nameText.SetText(name);
    }

    public void setHP(int hp, int max)
    {
        hpText.SetText($"{hp} / {max}");
        hpSlider.maxValue = max;
        hpSlider.value = hp;
    }

    public void setMP(int mp, int max)
    {
        mpText.SetText($"{mp} / {max}");
        mpSlider.maxValue = max;
        mpSlider.value = mp;
    }
}
