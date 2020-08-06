using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetGroup {Players, Enemies}

public class CursorTargeting : MonoBehaviour
{
    public BattleSystem system;
    public Camera cam;

    bool isLeftMouseHeld = false;
    bool isLeftMousePressed = false;

    int selectedTarget = -1;
    int hoverTarget = -1;

    RaycastHit hit;
    Ray ray;

    void Update()
    {
        CheckForPress();

        ray = cam.ScreenPointToRay(Input.mousePosition);

        if (system.currentState == BattleState.PlayerPhase)
        {
            switch(system.menuState)
            {
                case MenuState.TopMenu:
                    HoverMode();
                    break;
                case MenuState.Attack:
                    HoverMode();
                    TargetEnemyMode();
                    break;
                case MenuState.Skill:
                    switch (system.targetMode)
                    {
                        case TargetMode.AnyEnemy:
                            HoverMode();
                            TargetEnemyMode();
                            break;
                        case TargetMode.AllEnemies:
                            HoverMode();
                            TargetEnemyMode();
                            break;
                        case TargetMode.AnyAlly:
                            TargetAllyMode();
                            break;
                        default:
                            break;
                    }
                    break;
                case MenuState.Items:
                    switch(system.targetMode)
                    {
                        case TargetMode.AnyAlly:
                            TargetAllyMode();
                            break;
                        default:
                            break;
                    }
                    break;
                case MenuState.Tactics:
                    HoverMode();
                    break;
                default:
                    break;
            }
        }

        HoverCancel();
        CheckForHold();
    }

    void HoverMode()
    {
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "Enemy")
            {
                CharacterInfo targetTemp = hit.collider.gameObject.GetComponentInParent<CharacterInfo>();
                hoverTarget = targetTemp.UnitPosition;

                system.healthbarRefs[hoverTarget - 4].showHealthbar();
                system.healthbarRefs[hoverTarget - 4].showNametag();
            }
        }
    }

    void HoverCancel()
    {
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag != "Enemy")
            {
                if (hoverTarget != -1 && hoverTarget > 3 && hoverTarget < 9)
                {
                    system.healthbarRefs[hoverTarget - 4].hideHealthbar();
                    system.healthbarRefs[hoverTarget - 4].hideNametag();
                    hoverTarget = -1;
                }
            }
        }
    }

    void TargetAllyMode()
    {
        if (isLeftMousePressed && !isLeftMouseHeld)
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Player" && selectedTarget == -1)
                {
                    CharacterInfo targetTemp = hit.collider.gameObject.GetComponentInParent<CharacterInfo>();
                    selectedTarget = targetTemp.UnitPosition;

                    if (system.menuState == MenuState.Skill)
                    {
                        StartCoroutine(system.DoSkillCR(selectedTarget, TargetGroup.Players));
                    }
                    else if (system.menuState == MenuState.Items)
                    {
                        StartCoroutine(system.DoItemCR(selectedTarget, TargetGroup.Players));
                    }
                    else { }
                }
            }
            selectedTarget = -1;
        }
    }

    void TargetEnemyMode()
    {
        if (isLeftMousePressed && !isLeftMouseHeld)
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Enemy" && selectedTarget == -1)
                {
                    CharacterInfo targetTemp = hit.collider.gameObject.GetComponentInParent<CharacterInfo>();
                    selectedTarget = targetTemp.UnitPosition;

                    if (system.menuState == MenuState.Skill || system.menuState == MenuState.Attack)
                    {
                        StartCoroutine(system.DoSkillCR(selectedTarget, TargetGroup.Enemies));
                    }
                    else if (system.menuState == MenuState.Items)
                    {
                        StartCoroutine(system.DoItemCR(selectedTarget, TargetGroup.Enemies));
                    }
                    else { }
                }
            }
            selectedTarget = -1;
        }
    }

    void CheckForPress()
    {
        isLeftMousePressed = Input.GetKey(KeyCode.Mouse0);
    }

    void CheckForHold()
    {
        isLeftMouseHeld = isLeftMousePressed;
    }
}
