using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;

public enum BattleState {Start, PlayerPhase, EnemyPhase, Win, Lose}
public enum MenuState {Inactive, TopMenu, Attack, Guard, Skill, Items, Tactics}
public enum TargetMode {None, AnyEnemy, AnyAlly, AllEnemies, AllAllies, AllUnits, Self}

public class BattleSystem : MonoBehaviour
{
    // Master lists of players and enemies to spawn in
    public GameObject[] playerList;
    public GameObject[] enemyList;

    public int[] partyLineupRef;

    // Locations for players and enemies
    public Transform[] playerStations;
    public Transform[] enemyStations;

    // References to each units' data
    public CharacterInfo[] playerPartyData = new CharacterInfo[4];
    public CharacterInfo[] enemyPartyData = new CharacterInfo[5];

    // Maximum amount of time per phase.
    int playerTime = 0;
    int enemyTime = 0;
    public int timePerPartyMember;
    public int timePerEnemy;

    int numActionsTaken = 0;
    public int getNumActionsTaken()
    {
        return numActionsTaken;
    }

    // The status panel grid, prefab, and script reference.
    public GameObject partyStatusGrid;
    public GameObject partyStatusPanel;
    public StatusPanelUI[] statusPanelRefs = new StatusPanelUI[4];

    // Camera controls
    public Camera cam;
    public CinemachineVirtualCamera[] vcams;
    int camPosition = -1;

    // Healthbar parent, prefab, and script references.
    public GameObject enemyHealthbarRef;
    public GameObject enemyHealthbarPrefab;
    public EnemySliderUI[] healthbarRefs = new EnemySliderUI[5];

    public GameObject playerHealthbarRef;
    public GameObject playerHealthbarPrefab;
    public PlayerSliderUI[] playerHealthbarRefs = new PlayerSliderUI[4];

    // Skill button parent and prefab
    public GameObject skillMenu;
    public GameObject skillButtonPrefab;

    // Textbox for battle info
    public TextMeshProUGUI infoText;

    // Set the starting state before the battle begins
    public BattleState startingState;

    public BattleState currentState;    // Overall state of the battle sequence
    public MenuState menuState;         // Current state of the battle menu
    public TargetMode targetMode;       // Current targeting mode
    public int currentTurn;             // Used as an array index for player and enemy data

    public DamageCalculator damageCalculator;   // Reference to the battle calculator class
    public SkillRefs masterSkillList;           // Reference to the collection of all skills
    SkillScript selectedSkill;                  // Skill currently in use

    // Basic skills accessible from the top of the battle menu
    SkillScript attackSkill;
    SkillScript guardSkill;

    public StatusContainer statusList;  // Reference to the collection of all status conditions

    public Inventory inventory;         // Reference to the current collection of items
    public GameObject itemMenu;         // Item button parent
    public GameObject itemButtonPrefab; // Item button prefab
    ItemScript selectedItem;            // Item currently in use

    public TimeMeter meter; // Reference to the time meter

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        // Initialize and start setting up the battle scene
        inventory.InitInventory();

        currentState = BattleState.Start;
        menuState = MenuState.Inactive;
        targetMode = TargetMode.None;
        currentTurn = -1;

        attackSkill = masterSkillList.LookForSkill("Attack");
        guardSkill = masterSkillList.LookForSkill("Guard");

        ChangeToCamPosition(10);
        StartCoroutine(Setup());
    }

    IEnumerator Setup()
    {
        // Setup units and UI
        SetupPlayers();
        SetupEnemies();
        SetupStatusPanels();
        SetupEnemyHealthbars();
        SetupPlayerHealthbars();

        // Announce the enemies and determine starting phase.
        infoText.SetText($"{enemyPartyData[0].Name} draws near!");
        yield return new WaitForSeconds(2.0f);
        StartCoroutine(SetStartingPhase());
    }

    /* TODO: May need optimizations -- Store SkillScripts directly? */
    void SetupPlayers()
    {
        for (int i = 0; i < partyLineupRef.Length; ++i)
        {
            int lineupIndex = partyLineupRef[i];
            if((lineupIndex >= 0 && lineupIndex <= playerList.Length) && playerList[lineupIndex] != null)
            {
                GameObject playerDataTemp = Instantiate(playerList[lineupIndex], playerStations[i]);
                playerPartyData[i] = playerDataTemp.GetComponent<CharacterInfo>();
                playerPartyData[i].UnitPosition = i;

                foreach (string skl in playerPartyData[i].equippedSkills)
                {
                    SkillScript skillTemp = masterSkillList.LookForSkill(skl);
                    if (skillTemp != null)
                    {
                        playerPartyData[i].battleSkills.Add(skillTemp);
                    }
                }

                playerPartyData[i].initCharacter();
            }
        }
    }

    /* TODO: Not final -- Might need enemy variety and skills */
    void SetupEnemies()
    {
        int numSpawns = Random.Range(1, 6);

        for(int i = 0; i < numSpawns; ++i)
        {
            GameObject enemyDataTemp = Instantiate(enemyList[0], enemyStations[i]);
            enemyPartyData[i] = enemyDataTemp.GetComponent<CharacterInfo>();
            enemyPartyData[i].UnitPosition = i + 4;
            enemyPartyData[i].initCharacter();
        }
        
    }

    void SetupStatusPanels()
    {
        // For each available party member, create a status panel and keep a reference to each one
        for (int i = 0; i < playerPartyData.Length; ++i)
        {
            if (playerPartyData[i] != null)
            {
                GameObject panelTemp = Instantiate(partyStatusPanel, partyStatusGrid.transform);
                statusPanelRefs[i] = panelTemp.GetComponent<StatusPanelUI>();
                statusPanelRefs[i].setHUD(playerPartyData[i]);
            }
        }
    }

    void SetupEnemyHealthbars()
    {
        // For each available enemy, create a healthbar and keep a reference to each one
        for(int i = 0; i < enemyStations.Length; ++i)
        {
            if(enemyStations[i] != null && enemyPartyData[i] != null)
            {
                GameObject sliderTemp = Instantiate(enemyHealthbarPrefab, enemyHealthbarRef.transform);
                healthbarRefs[i] = sliderTemp.GetComponent<EnemySliderUI>();
                healthbarRefs[i].target = enemyStations[i];
                healthbarRefs[i].setHUD(enemyPartyData[i]);
                healthbarRefs[i].setNametag(enemyPartyData[i]);
                healthbarRefs[i].hideNametag();
                healthbarRefs[i].hideHealthbar();
                healthbarRefs[i].hideDamage();
                healthbarRefs[i].hideHeader();
            }
        }
    }

    void SetupPlayerHealthbars()
    {
        // Same as enemy healthbars
        for(int i = 0; i < playerStations.Length; ++i)
        {
            if(playerStations[i] != null && playerPartyData[i] != null)
            {
                GameObject sliderTemp = Instantiate(playerHealthbarPrefab, playerHealthbarRef.transform);
                playerHealthbarRefs[i] = sliderTemp.GetComponent<PlayerSliderUI>();
                playerHealthbarRefs[i].target = playerStations[i];
                playerHealthbarRefs[i].setHUD(playerPartyData[i]);
                playerHealthbarRefs[i].hideHealthbar();
                playerHealthbarRefs[i].hideDamage();
                playerHealthbarRefs[i].hideHeader();
            }
        }
    }

    IEnumerator SetStartingPhase()
    {
        // Set the starting phase according to the editor (defaults to Player Phase)
        if (startingState == BattleState.PlayerPhase || startingState == BattleState.EnemyPhase)
        {
            currentState = startingState;
            if(startingState == BattleState.PlayerPhase)
            {
                currentTurn = 0;
            }
            else
            {
                currentTurn = 4;
            }
        }
        else
        {
            currentState = BattleState.PlayerPhase;
        }

        // Announce the current phase while setting the total phase time
        CalculateMaxTimers();
        meter.ShowMeter();
        if (currentState == BattleState.PlayerPhase)
        {
            meter.timeMeter.maxValue = playerTime;
            currentState = BattleState.PlayerPhase;
            infoText.SetText("PLAYER PHASE");
            ChangeToCamPosition(9);
        }
        else if (currentState == BattleState.EnemyPhase)
        {
            meter.timeMeter.maxValue = enemyTime;
            currentState = BattleState.EnemyPhase;
            infoText.SetText("ENEMY PHASE");
            ChangeToCamPosition(10);
        }
        else
        {
            yield break;
        }
        meter.RefillTimer();
        yield return new WaitForSeconds(2.0f);
        
        // Start the first turn
        ChangeToCamPosition(currentTurn);
        meter.meterRate = numActionsTaken;
        
        if (currentState == BattleState.PlayerPhase)
        {
            meter.SetMeterState(MeterStates.Draining);
            PlayerPhase();
        }
        else if (currentState == BattleState.EnemyPhase)
        {
            meter.SetMeterState(MeterStates.Stopped);
            StartCoroutine(EnemyPhase());
        }
        else
        {
            yield break;
        }
    }

    // Announces Player Phase
    void PlayerPhase()
    {
        infoText.SetText($"It's {playerPartyData[currentTurn].Name}'s turn...");
        menuState = MenuState.TopMenu;
    }

    // Activate the currently selected skill
    public IEnumerator DoSkillCR(int targetPos, TargetGroup group)
    {
        CharacterInfo source;
        CharacterInfo destination;

        meter.SetMeterState(MeterStates.Stopped);
        menuState = MenuState.Inactive;
        targetMode = TargetMode.None;

        // Must have a skill selected; Must be in player phase; Must be a valid player ID
        if (selectedSkill == null || currentState != BattleState.PlayerPhase || (currentTurn < 0 || currentTurn > 3))
        {
            yield break;
        }

        source = playerPartyData[currentTurn];

        selectedSkill.DoTimeCost();
        selectedSkill.DoMPCost(source);
        selectedSkill.DoHPCost(source);

        // Verify that TargetGroup matches the appropriate unit ID before executing the skill
        switch (group)
        {
            case TargetGroup.Players:
                if(targetPos < 0 || targetPos > 3)
                {
                    yield break;
                }
                destination = playerPartyData[targetPos];
                yield return StartCoroutine(selectedSkill.DoSkill(source, destination));
                break;
            case TargetGroup.Enemies:
                if (targetPos < 4 || targetPos > 8)
                {
                    yield break;
                }
                destination = enemyPartyData[targetPos - 4];
                yield return StartCoroutine(selectedSkill.DoSkill(source, destination));
                break;
            default:
                break;
        }
        
        StartCoroutine(EndOfPhase());
    }

    // Function to be called by a skill menu button
    void SkillButtonClicked(int menuPos)
    {
        // menuPos corresponds to the index of the unit's skill array
        selectedSkill = playerPartyData[currentTurn].battleSkills[menuPos];

        if(selectedSkill != null) {
            int mpC = selectedSkill.info.mpCost;
            int hpCP = selectedSkill.info.hpCostPercent;
            CharacterInfo dataTemp = playerPartyData[currentTurn];

            // Verify HP Costs and MP Costs
            // Cannot have both costs at once
            if (mpC > 0 && hpCP == 0 && dataTemp.checkMPCost(mpC))
            {
                CloseSkillMenu();
                selectedSkill.PrepareSkill();
            }
            else if(hpCP > 0 && mpC == 0 && dataTemp.checkHPCost(hpCP))
            {
                CloseSkillMenu();
                selectedSkill.PrepareSkill();
            }
            else if(mpC == 0 && hpCP == 0)
            {
                CloseSkillMenu();
                selectedSkill.PrepareSkill();
            }
            else
            {
                selectedSkill = null;

                if (!dataTemp.checkMPCost(mpC))
                {
                    infoText.SetText("Not enough MP!");
                }

                if(!dataTemp.checkHPCost(hpCP))
                {
                    infoText.SetText("Not enough HP!");
                }
            }
        }
    }

    // Creates the skill menu by spawning buttons
    void OpenSkillMenu()
    {
        infoText.SetText("Select a skill!");
        menuState = MenuState.Skill;

        CharacterInfo dataTemp = playerPartyData[currentTurn];

        for(int i = 0; i < dataTemp.battleSkills.Count; ++i)
        {
            SkillScript skl = dataTemp.battleSkills[i];

            GameObject goTemp = Instantiate(skillButtonPrefab, skillMenu.transform);

            // Send the skill information to the UI Script
            SkillButtonUI buttonUITemp = goTemp.GetComponent<SkillButtonUI>();
            buttonUITemp.setButtonText(skl.info, dataTemp, numActionsTaken);
            buttonUITemp.skillMenuPosition = i;

            // Get the button and add the listener to the skill selection function
            Button buttonTemp = goTemp.GetComponent<Button>();
            buttonTemp.onClick.AddListener(() => SkillButtonClicked(buttonUITemp.skillMenuPosition));
        }
    }

    // Deletes the buttons from the menu parent
    void CloseSkillMenu()
    {
        int buttons = skillMenu.transform.childCount;
        for(int i = 0; i < buttons; ++i)
        {
            GameObject.Destroy(skillMenu.transform.GetChild(i).gameObject);
        }
    }

    // Similar to DoSkillCR as ItemScripts inherit the same base SkillScript class
    public IEnumerator DoItemCR(int targetPos, TargetGroup group)
    {
        CharacterInfo source;
        CharacterInfo destination;

        meter.SetMeterState(MeterStates.Stopped);
        menuState = MenuState.Inactive;
        targetMode = TargetMode.None;

        if (selectedItem == null || currentState != BattleState.PlayerPhase || (currentTurn < 0 || currentTurn > 3))
        {
            yield break;
        }

        source = playerPartyData[currentTurn];

        switch (group)
        {
            case TargetGroup.Players:
                if (targetPos < 0 || targetPos > 3)
                {
                    yield break;
                }
                destination = playerPartyData[targetPos];
                yield return StartCoroutine(selectedItem.DoSkill(source, destination));
                break;
            case TargetGroup.Enemies:
                if (targetPos < 4 || targetPos > 8)
                {
                    yield break;
                }
                destination = enemyPartyData[targetPos - 4];
                yield return StartCoroutine(selectedItem.DoSkill(source, destination));
                break;
            default:
                break;
        }

        StartCoroutine(EndOfPhase());
    }

    // Selects the item to be used like skills
    void ItemButtonClicked(string name)
    {
        selectedItem = inventory.DoGetItem(name);

        if(selectedItem != null)
        {
            CloseInventory();
            selectedItem.PrepareSkill();
        }
    }

    // Similar to the Skill menu, it instantiates buttons showing item info
    void OpenInventory()
    {
        infoText.SetText("Choose an Item!");
        menuState = MenuState.Items;

        inventory.InitInventory();

        foreach(string str in inventory.items.Keys)
        {
            ItemScript item = inventory.itemRef.LookForItem(str);

            GameObject goTemp = Instantiate(itemButtonPrefab, itemMenu.transform);

            ItemButtonUI buttonUITemp = goTemp.GetComponent<ItemButtonUI>();
            buttonUITemp.setButtonText(item.info, inventory.items[str]);

            Button buttonTemp = goTemp.GetComponent<Button>();
            buttonTemp.onClick.AddListener(() => ItemButtonClicked(str));
        }
    }

    // Deletes buttons from the menu parent
    void CloseInventory()
    {
        int buttons = itemMenu.transform.childCount;
        for (int i = 0; i < buttons; ++i)
        {
            GameObject.Destroy(itemMenu.transform.GetChild(i).gameObject);
        }
    }

    /* TODO: Add better enemy AI */
    IEnumerator EnemyPhase()
    {
        CharacterInfo attackerTemp = enemyPartyData[currentTurn - 4];
        CharacterInfo targetTemp = playerPartyData[Random.Range(0, playerList.Length)];

        attackSkill.DoTimeCost();
        yield return StartCoroutine(attackSkill.DoSkill(attackerTemp, targetTemp));

        StartCoroutine(EndOfPhase());
    }

    // Toggles between Player Phase and Enemy Phase
    public IEnumerator ChangePhases()
    {
        if (currentState == BattleState.PlayerPhase)
        {
            meter.timeMeter.maxValue = enemyTime;
            currentState = BattleState.EnemyPhase;
            infoText.SetText("ENEMY PHASE");
            ChangeToCamPosition(10);
        }
        else if (currentState == BattleState.EnemyPhase)
        {
            meter.timeMeter.maxValue = playerTime;
            currentState = BattleState.PlayerPhase;
            infoText.SetText("PLAYER PHASE");
            ChangeToCamPosition(9);
            resetPlayerWeaknessStates();
        }
        else
        {
            yield break;
        }

        meter.RefillTimer();
        yield return new WaitForSeconds(2.0f);
    }
    
    // Procedures to be done at the end of a unit's turn
    public IEnumerator EndOfPhase()
    {
        // Disable every player menu
        targetMode = TargetMode.None;
        menuState = MenuState.Inactive;
        CloseInventory();
        CloseSkillMenu();

        // Activate the scripts for any active Burn, Poison, etc.
        yield return StartCoroutine(CheckDamageStatuses());

        yield return new WaitForSeconds(0.75f);

        // If win/lose condition is not met, check for a phase change.
        if (areAllPlayersDefeated())
        {
            currentState = BattleState.Lose;
            EndBattle();
            yield break;
        }
        else if (areAllEnemiesDefeated())
        {
            currentState = BattleState.Win;
            EndBattle();
            yield break;
        }
        else if (meter.timeMeter.value <= 0)
        {
            numActionsTaken = 0;

            if (currentState == BattleState.PlayerPhase)
            {
                if(!findNextActivePlayer())
                {
                    currentTurn = 3;
                    findNextActiveEnemy();
                    yield return StartCoroutine(ChangePhases());
                    CalculateMaxTimers();
                    meter.timeMeter.maxValue = enemyTime;
                    meter.RefillTimer();
                }
                else
                {
                    CalculateMaxTimers();
                    meter.timeMeter.maxValue = playerTime;
                    meter.RefillTimer();
                }
            }
            else if (currentState == BattleState.EnemyPhase)
            {
                if(!findNextActiveEnemy())
                {
                    currentTurn = -1;
                    findNextActivePlayer();
                    yield return StartCoroutine(ChangePhases());
                    CalculateMaxTimers();
                    meter.timeMeter.maxValue = playerTime;
                    meter.RefillTimer();
                }
                else
                {
                    CalculateMaxTimers();
                    meter.timeMeter.maxValue = enemyTime;
                    meter.RefillTimer();
                }
            }
            else { }
        }
        else
        {
            numActionsTaken++;
        }

        // Update timer rate
        meter.meterRate = numActionsTaken;

        if (currentState == BattleState.PlayerPhase)
        {
            /* IN PLAYER PHASE */

            ChangeToCamPosition(currentTurn);
            yield return StartCoroutine(CheckStatusTimers()); // Passive status timers tick down every time it gets to a unit's turn

            if (currentTurn >= 0 && currentTurn <= 3)
            {
                playerPartyData[currentTurn].IsGuarding = false;
                meter.SetMeterState(MeterStates.Draining);
                currentState = BattleState.PlayerPhase;
                PlayerPhase();
            }
            else { }
        }
        else if (currentState == BattleState.EnemyPhase)
        {
            /* IN ENEMY PHASE */

            ChangeToCamPosition(currentTurn);
            yield return StartCoroutine(CheckStatusTimers()); // Passive status timers tick down every time it gets to a unit's turn

            if (currentTurn >= 4 && currentTurn <= 8)
            {
                meter.SetMeterState(MeterStates.Stopped);
                currentState = BattleState.EnemyPhase;
                StartCoroutine(EnemyPhase());
            }
            else { }
        }
        else { }
    }

    bool findNextActivePlayer()
    {
        do
        {
            currentTurn++;
            if (currentTurn > 3)
            {
                return false;
            }
        }
        while (playerPartyData[currentTurn] == null || playerPartyData[currentTurn].CurrentHP <= 0); // Continue until a valid and live unit is found.

        return true;
    }

    bool findNextActiveEnemy()
    {
        do
        {
            currentTurn++;
            if (currentTurn > 8)
            {
                return false;
            }
        }
        while (enemyPartyData[currentTurn - 4] == null || enemyPartyData[currentTurn - 4].CurrentHP <= 0); // Continue until a valid and live unit is found.

        return true;
    }

    // Used for status conditions such as Burn, Poison, etc.
    IEnumerator CheckDamageStatuses()
    {
        CharacterInfo charTemp;

        if (currentTurn < 4)
        {
            if(playerPartyData[currentTurn] == null)
            {
                yield break;
            }
            charTemp = playerPartyData[currentTurn];
        }
        else
        {
            if (enemyPartyData[currentTurn - 4] == null)
            {
                yield break;
            }
            charTemp = enemyPartyData[currentTurn - 4];
        }

        if (charTemp.containsStatus(StatusCondition.Burn))
        {
            StatusScript statTemp = statusList.LookForStatus(StatusCondition.Burn);
            yield return StartCoroutine(statTemp.DoStatus(charTemp));
        }
        else if(charTemp.containsStatus(StatusCondition.Poison))
        {
            StatusScript statTemp = statusList.LookForStatus(StatusCondition.Poison);
            yield return StartCoroutine(statTemp.DoStatus(charTemp));
        }
    }

    // Tick down and remove status conditions when a unit's turn starts
    IEnumerator CheckStatusTimers()
    {
        CharacterInfo charTemp;

        if(currentTurn < 4)
        {
            if (playerPartyData[currentTurn] == null)
            {
                yield break;
            }
            charTemp = playerPartyData[currentTurn];
        }
        else
        {
            if (enemyPartyData[currentTurn - 4] == null)
            {
                yield break;
            }
            charTemp = enemyPartyData[currentTurn - 4];
        }

        foreach (StatusScript st in statusList.listOfStatuses)
        {
            if(charTemp.countdownStatus(st.statusName))
            {
                yield return StartCoroutine(st.StatusCleared(charTemp));
                charTemp.removeStatus(st.statusName);
            }
        }
    }

    void resetPlayerWeaknessStates()
    {
        foreach (CharacterInfo c in playerPartyData)
        {
            c.DidHitWeakness = false;
        }
    }

    // Win condition
    public bool areAllEnemiesDefeated()
    {
        foreach (CharacterInfo enemy in enemyPartyData)
        {
            if(enemy != null)
            {
                if(enemy.CurrentHP > 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    // Lose condition
    public bool areAllPlayersDefeated()
    {
        foreach (CharacterInfo player in playerPartyData)
        {
            if(player != null)
            {
                if(player.CurrentHP > 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    // Resets the maximum timers for Player and Enemy phases
    void CalculateMaxTimers()
    {
        playerTime = 0;
        enemyTime = 0;

        switch(currentState)
        {
            case BattleState.PlayerPhase:
                if (currentTurn >= 0 && currentTurn <= 3)
                {
                    playerTime = playerPartyData[currentTurn].Speed;
                }
                break;
            case BattleState.EnemyPhase:
                if (currentTurn >= 4 && currentTurn <= 8)
                {
                    enemyTime = enemyPartyData[currentTurn - 4].Speed;
                }
                break;
            default:
                break;
        }
    }

    // Announce the endgame state
    void EndBattle()
    {
        if (currentState == BattleState.Win)
        {
            infoText.SetText("YOU WIN!");
            ChangeToCamPosition(9);
        }
        else if (currentState == BattleState.Lose)
        {
            infoText.SetText("You lost the battle...");
        }
        else { }
    }

    // Changes the priority of Cinemachine virtual cameras
    public void ChangeToCamPosition(int vc)
    {
        if (camPosition != -1 && camPosition != vc)
        {
            vcams[camPosition].m_Priority = 0;
        }

        camPosition = vc;
        vcams[camPosition].m_Priority = 10;
    }

    // State machine for the battle menu
    void Update()
    {
        switch(menuState)
        {
            case MenuState.TopMenu:
                if (Input.GetKeyDown(KeyCode.Space)) // ATTACK
                {
                    selectedSkill = attackSkill;
                    attackSkill.PrepareSkill();
                }
                else if (Input.GetKeyDown(KeyCode.LeftShift)) // GUARD
                {
                    selectedSkill = guardSkill;
                    guardSkill.PrepareSkill();
                }
                else if (Input.GetKeyDown(KeyCode.W)) // SKILL
                {
                    OpenSkillMenu();
                }
                else if (Input.GetKeyDown(KeyCode.A)) // ITEM
                {
                    OpenInventory();
                }
                else if(Input.GetKeyDown(KeyCode.S)) // TACTICS
                {
                    infoText.SetText("Stop and think...");
                    ChangeToCamPosition(9);
                    menuState = MenuState.Tactics;
                    meter.SetMeterState(MeterStates.Stopped);
                }
                else { }
                break;
            case MenuState.Attack:
                if (Input.GetKeyDown(KeyCode.LeftShift)) // Cancel Attack
                {
                    ChangeToCamPosition(currentTurn);
                    PlayerPhase();
                }
                break;
            case MenuState.Guard:
                if (Input.GetKeyDown(KeyCode.LeftShift)) // Cancel Guard
                {
                    PlayerPhase();
                }
                else if (Input.GetKeyDown(KeyCode.Space)) // Confirm Guard
                {
                    StartCoroutine(DoSkillCR(currentTurn, TargetGroup.Players));
                }
                else { }
                break;
            case MenuState.Skill:
                if (Input.GetKeyDown(KeyCode.LeftShift)) // Cancel Skill while...
                {
                    if (targetMode == TargetMode.None) // In Skill Menu
                    {
                        CloseSkillMenu();
                        PlayerPhase();
                    }
                    else // While targeting with a skill
                    {
                        targetMode = TargetMode.None;
                        ChangeToCamPosition(currentTurn);
                        OpenSkillMenu();
                    }
                }
                else { }
                break;
            case MenuState.Items:
                if (Input.GetKeyDown(KeyCode.LeftShift)) // Cancel Item while...
                {
                    if (targetMode == TargetMode.None) // In Item menu
                    {
                        CloseInventory();
                        PlayerPhase();
                    }
                    else // While targeting with an item
                    {
                        targetMode = TargetMode.None;
                        ChangeToCamPosition(currentTurn);
                        OpenInventory();
                    }
                }
                else { }
                break;
            case MenuState.Tactics:
                if(Input.GetKeyDown(KeyCode.LeftShift))
                {
                    ChangeToCamPosition(currentTurn);
                    meter.SetMeterState(MeterStates.Draining);
                    PlayerPhase();
                }
                break;
            default:
                break;
        }
    }
}
