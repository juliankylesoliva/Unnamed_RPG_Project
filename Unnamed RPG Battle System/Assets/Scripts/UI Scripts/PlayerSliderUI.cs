using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSliderUI : MonoBehaviour
{
    public Slider healthbar;
    public TextMeshProUGUI damageNum;
    public TextMeshProUGUI modHeader;
    public Transform target;

    void Update()
    {
        if (target != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(target.position + (Vector3.up * 2));
        }
    }

    public IEnumerator flashHUD(CharData data)
    {
        showHealthbar();
        showDamage();
        showHeader();
        yield return StartCoroutine(drainHealthbar(data));
        hideHealthbar();
        hideDamage();
        hideHeader();
    }

    public IEnumerator drainHealthbar(CharData data)
    {
        int goal = data.currentHP;

        while (healthbar.value > goal)
        {
            healthbar.value -= 1000 * Time.deltaTime;
            yield return new WaitForSeconds(0.001f);
        }

        if (healthbar.value < goal)
        {
            healthbar.value = goal;
        }

        yield return new WaitForSeconds(0.45f);
    }

    public IEnumerator fillHealthbar(CharData data)
    {
        int goal = data.currentHP;

        while (healthbar.value < goal)
        {
            healthbar.value += 1000 * Time.deltaTime;
            yield return new WaitForSeconds(0.001f);
        }

        if (healthbar.value > goal)
        {
            healthbar.value = goal;
        }

        yield return new WaitForSeconds(1.0f);
    }

    public void setHUD(CharData data)
    {
        healthbar.maxValue = data.maxHP;
        healthbar.value = data.currentHP;
    }

    public void setDamageNum(int dmg)
    {
        damageNum.SetText($"{dmg}");
    }

    public void setHeader(string head)
    {
        modHeader.SetText(head);
    }

    public void showHealthbar()
    {
        healthbar.gameObject.SetActive(true);
    }

    public void hideHealthbar()
    {
        healthbar.gameObject.SetActive(false);
    }

    public void showDamage()
    {
        damageNum.gameObject.SetActive(true);
    }

    public void hideDamage()
    {
        damageNum.gameObject.SetActive(false);
    }

    public void showHeader()
    {
        modHeader.SetText("");
        modHeader.gameObject.SetActive(true);
    }

    public void hideHeader()
    {
        modHeader.gameObject.SetActive(false);
    }
}
