using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemySliderUI : MonoBehaviour
{
    public Slider healthbar;
    public TextMeshProUGUI nametag;
    public TextMeshProUGUI damageNum;
    public TextMeshProUGUI modHeader;
    public Transform target;

    void Update()
    {
        if(target != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(target.position + (Vector3.up * 3));
        }
    }

    public IEnumerator flashHUD(CharacterInfo data)
    {
        showHealthbar();
        showDamage();
        showHeader();
        yield return StartCoroutine(drainHealthbar(data));
        hideHealthbar();
        hideDamage();
        hideHeader();
    }

    public IEnumerator drainHealthbar(CharacterInfo data)
    {
        int goal = data.CurrentHP;

        while(healthbar.value > goal)
        {
            healthbar.value -= 1000 * Time.deltaTime;
            yield return new WaitForSeconds(0.001f);
        }

        if(healthbar.value < goal)
        {
            healthbar.value = goal;
        }

        yield return new WaitForSeconds(0.65f);
    }

    public IEnumerator fillHealthbar(CharacterInfo data)
    {
        int goal = data.CurrentHP;

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

    public void setHUD(CharacterInfo data)
    {
        healthbar.maxValue = data.MaxHP;
        healthbar.value = data.CurrentHP;
    }

    public void setNametag(CharacterInfo data)
    {
        nametag.SetText($"{data.Name} (Lv. {data.Level})");
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

    public void showNametag()
    {
        nametag.enabled = true;
    }

    public void hideNametag()
    {
        nametag.enabled = false;
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
        modHeader.gameObject.SetActive(true);
    }

    public void hideHeader()
    {
        modHeader.SetText("");
        modHeader.gameObject.SetActive(false);
    }
}
