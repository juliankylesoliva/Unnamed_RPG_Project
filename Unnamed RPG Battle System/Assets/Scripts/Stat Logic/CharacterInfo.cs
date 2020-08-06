using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    /* BASE STATS AND GROWTHS */
    [Header("Base Stats and Growth Ratio Data")]
    public BasesAndGrowths baseGrowths;
    // Bonus stats here

    /* BASIC CHARACTER INFO */
    [Header("Basic Character Info")]
    [SerializeField]
    private string _name;
    public string Name
    {
        get
        {
            return _name;
        }

        set
        {
            _name = value;
        }
    }

    [SerializeField]
    private int _level;
    public int Level {
        get
        {
            return _level;
        }
        
        private set
        {
            int temp = _level;

            if(value >= 1 && value <= 100)
            {
                _level = value;
                
            }
            else if(value < 1)
            {
                _level = 1;
            }
            else // value > 100
            {
                _level = 100;
            }

            refreshStats();
            learnNewSkills();
        }
    }

    public void levelUp()
    {
        this.Level++;
    }

    [SerializeField]
    private int _expPts;
    public int ExperiencePoints
    {
        get
        {
            return _expPts;
        }

        private set
        {
            if(value > 0)
            {
                _expPts = value;

                initExpGoals();

                while (_level >= 1 && _level <= 99 && _expPts >= expGoals[_level - 1])
                {
                    levelUp();
                }

                getExpPtsToNextLevel();
            }
        }
    }

    public int getExpPtsToNextLevel()
    {
        if(_level >= 1)
        {
            initExpGoals();
            return expGoals[_level - 1] - _expPts;
        }
        return -1;
    }

    public void giveEXP(int xp)
    {
        if(_level < 1)
        {
            initCharacter();
        }

        if(_level >= 1 && _level <= 99 && xp > 0)
        {
            this.ExperiencePoints += xp;
        }
    }

    private List<int> expGoals = null;
    private void initExpGoals()
    {
        if(expGoals != null)
        {
            return;
        }

        expGoals = new List<int>();
        int previousGoal = 0;

        for(int lv = 1; lv <= 99; ++lv)
        {
            int goalTemp = (int)((float)baseGrowths.baseExpGoal * Mathf.Pow((float)(baseGrowths.baseExpGoalGrowth), (float)(lv - 1)));
            if(goalTemp > baseGrowths.maxExpGoal)
            {
                goalTemp = baseGrowths.maxExpGoal;
            }

            goalTemp += previousGoal;

            expGoals.Add(goalTemp);
            previousGoal = goalTemp;
        }
    }

    /* PHYSICAL STATS */
    [Header("Physical Stats")]
    [SerializeField]
    private int _maxHP;
    public int MaxHP
    {
        get
        {
            return _maxHP;
        }

        private set
        {
            if(value >= 1 && value <= 999)
            {
                _maxHP = value;
            }
            else if(value < 1)
            {
                _maxHP = 1;
            }
            else
            {
                _maxHP = 999;
            }
        }
    }

    [SerializeField]
    private int _currentHP;
    public int CurrentHP
    {
        get
        {
            return _currentHP;
        }

        private set
        {
            if(value >= 0 && value <= this._maxHP)
            {
                _currentHP = value;
            }
            else if(value < 0)
            {
                _currentHP = 0;
            }
            else
            {
                _currentHP = this._maxHP;
            }
        }
    }

    public bool dealDamage(int dmg)
    {
        if (dmg > 0)
        {
            CurrentHP -= dmg;
        }

        return CurrentHP == 0;
    }

    public void healDamage(int heal)
    {
        if(heal > 0)
        {
            CurrentHP += heal;
        }
    }

    public int getHPPercent(int costPercent)
    {
        return (int)(_maxHP * (costPercent / 100.0f));
    }

    public bool checkHPCost(int costPercent)
    {
        if(getHPPercent(costPercent) > _currentHP - 1)
        {
            return false;
        }

        return true;
    }

    public void spendHP(int costPercent)
    {
        _currentHP -= getHPPercent(costPercent);

        if(_currentHP < 1)
        {
            _currentHP = 1;
        }
    }

    [SerializeField]
    private int _attack;
    public int Attack
    {
        get
        {
            return _attack;
        }

        private set
        {
            if(value >= 1 && value <= 999)
            {
                _attack = value;
            }
            else if(value < 1)
            {
                _attack = 1;
            }
            else
            {
                _attack = 999;
            }
        }
    }

    [SerializeField]
    private int _defense;
    public int Defense
    {
        get
        {
            return _defense;
        }

        private set
        {
            if (value >= 1 && value <= 999)
            {
                _defense = value;
            }
            else if (value < 1)
            {
                _defense = 1;
            }
            else
            {
                _defense = 999;
            }
        }
    }

    [SerializeField]
    private int _speed;
    public int Speed
    {
        get
        {
            return _speed;
        }

        private set
        {
            if (value >= 1 && value <= 999)
            {
                _speed = value;
            }
            else if (value < 1)
            {
                _speed = 1;
            }
            else
            {
                _speed = 999;
            }
        }
    }

    /* MENTAL STATS */
    [Header("Mental Stats")]
    [SerializeField]
    private int _maxMP;
    public int MaxMP
    {
        get
        {
            return _maxMP;
        }

        private set
        {
            if (value >= 1 && value <= 999)
            {
                _maxMP = value;
            }
            else if (value < 1)
            {
                _maxMP = 1;
            }
            else
            {
                _maxMP = 999;
            }
        }
    }

    [SerializeField]
    private int _currentMP;
    public int CurrentMP
    {
        get
        {
            return _currentMP;
        }

        private set
        {
            if (value >= 0 && value <= this._maxMP)
            {
                _currentMP = value;
            }
            else if (value < 0)
            {
                _currentMP = 0;
            }
            else
            {
                _currentMP = this._maxMP;
            }
        }
    }

    public bool checkMPCost(int cost)
    {
        if(cost > _currentMP)
        {
            return false;
        }

        return true;
    }

    public void spendMP(int cost)
    {
        if(cost > 0)
        {
            CurrentMP -= cost;
        }
    }

    public void restoreMP(int heal)
    {
        if(heal > 0)
        {
            CurrentMP += heal;
        }
    }

    [SerializeField]
    private int _magic;
    public int Magic
    {
        get
        {
            return _magic;
        }

        private set
        {
            if (value >= 1 && value <= 999)
            {
                _magic = value;
            }
            else if (value < 1)
            {
                _magic = 1;
            }
            else
            {
                _magic = 999;
            }
        }
    }

    [SerializeField]
    private int _resistance;
    public int Resistance
    {
        get
        {
            return _resistance;
        }

        private set
        {
            if (value >= 1 && value <= 999)
            {
                _resistance = value;
            }
            else if (value < 1)
            {
                _resistance = 1;
            }
            else
            {
                _resistance = 999;
            }
        }
    }

    [SerializeField]
    private int _precision;
    public int Precision
    {
        get
        {
            return _precision;
        }

        private set
        {
            if (value >= 1 && value <= 999)
            {
                _precision = value;
            }
            else if (value < 1)
            {
                _precision = 1;
            }
            else
            {
                _precision = 999;
            }
        }
    }

    /* LIFESTYLE STATS */
    [Header("Lifestyle Stats")]
    [SerializeField]
    private int _bravery;
    public int Bravery
    {
        get
        {
            return _bravery;
        }

        private set
        {
            if (value >= 1 && value <= 999)
            {
                _bravery = value;
            }
            else if (value < 1)
            {
                _bravery = 1;
            }
            else
            {
                _bravery = 999;
            }
        }
    }

    [SerializeField]
    private int _charisma;
    public int Charisma
    {
        get
        {
            return _charisma;
        }

        private set
        {
            if (value >= 1 && value <= 999)
            {
                _charisma = value;
            }
            else if (value < 1)
            {
                _charisma = 1;
            }
            else
            {
                _charisma = 999;
            }
        }
    }

    [SerializeField]
    private int _compassion;
    public int Compassion
    {
        get
        {
            return _compassion;
        }

        private set
        {
            if (value >= 1 && value <= 999)
            {
                _compassion = value;
            }
            else if (value < 1)
            {
                _compassion = 1;
            }
            else
            {
                _compassion = 999;
            }
        }
    }

    [SerializeField]
    private int _skill;
    public int Skill
    {
        get
        {
            return _skill;
        }

        private set
        {
            if (value >= 1 && value <= 999)
            {
                _skill = value;
            }
            else if (value < 1)
            {
                _skill = 1;
            }
            else
            {
                _skill = 999;
            }
        }
    }

    /* BONUS STATS */
    [Header("Bonus Stats")]

    [SerializeField]
    private int _bonusHP;
    public int BonusHP
    {
        get
        {
            return _bonusHP;
        }

        set
        {
            if(value >= 0 && value <= 500)
            {
                _bonusHP = value;
            }
            else if(value < 0)
            {
                _bonusHP = 0;
            }
            else
            {
                _bonusHP = 500;
            }

            refreshStats();
        }
    }

    [SerializeField]
    private int _bonusATK;
    public int BonusATK
    {
        get
        {
            return _bonusATK;
        }

        set
        {
            if (value >= 0 && value <= 500)
            {
                _bonusATK = value;
            }
            else if (value < 0)
            {
                _bonusATK = 0;
            }
            else
            {
                _bonusATK = 500;
            }

            refreshStats();
        }
    }

    [SerializeField]
    private int _bonusDEF;
    public int BonusDEF
    {
        get
        {
            return _bonusDEF;
        }

        set
        {
            if (value >= 0 && value <= 500)
            {
                _bonusDEF = value;
            }
            else if (value < 0)
            {
                _bonusDEF = 0;
            }
            else
            {
                _bonusDEF = 500;
            }

            refreshStats();
        }
    }

    [SerializeField]
    private int _bonusSPD;
    public int BonusSPD
    {
        get
        {
            return _bonusSPD;
        }

        set
        {
            if (value >= 0 && value <= 500)
            {
                _bonusSPD = value;
            }
            else if (value < 0)
            {
                _bonusSPD = 0;
            }
            else
            {
                _bonusSPD = 500;
            }

            refreshStats();
        }
    }

    [SerializeField]
    private int _bonusMP;
    public int BonusMP
    {
        get
        {
            return _bonusMP;
        }

        set
        {
            if (value >= 0 && value <= 500)
            {
                _bonusMP = value;
            }
            else if (value < 0)
            {
                _bonusMP = 0;
            }
            else
            {
                _bonusMP = 500;
            }

            refreshStats();
        }
    }

    [SerializeField]
    private int _bonusMAG;
    public int BonusMAG
    {
        get
        {
            return _bonusMAG;
        }

        set
        {
            if (value >= 0 && value <= 500)
            {
                _bonusMAG = value;
            }
            else if (value < 0)
            {
                _bonusMAG = 0;
            }
            else
            {
                _bonusMAG = 500;
            }

            refreshStats();
        }
    }

    [SerializeField]
    private int _bonusRES;
    public int BonusRES
    {
        get
        {
            return _bonusRES;
        }

        set
        {
            if (value >= 0 && value <= 500)
            {
                _bonusRES = value;
            }
            else if (value < 0)
            {
                _bonusRES = 0;
            }
            else
            {
                _bonusRES = 500;
            }

            refreshStats();
        }
    }

    [SerializeField]
    private int _bonusPRC;
    public int BonusPRC
    {
        get
        {
            return _bonusPRC;
        }

        set
        {
            if (value >= 0 && value <= 500)
            {
                _bonusPRC = value;
            }
            else if (value < 0)
            {
                _bonusPRC = 0;
            }
            else
            {
                _bonusPRC = 500;
            }

            refreshStats();
        }
    }

    [SerializeField]
    private int _bonusBRV;
    public int BonusBRV
    {
        get
        {
            return _bonusBRV;
        }

        set
        {
            if (value >= 0 && value <= 500)
            {
                _bonusBRV = value;
            }
            else if (value < 0)
            {
                _bonusBRV = 0;
            }
            else
            {
                _bonusBRV = 500;
            }

            refreshStats();
        }
    }

    [SerializeField]
    private int _bonusCHA;
    public int BonusCHA
    {
        get
        {
            return _bonusCHA;
        }

        set
        {
            if (value >= 0 && value <= 500)
            {
                _bonusCHA = value;
            }
            else if (value < 0)
            {
                _bonusCHA = 0;
            }
            else
            {
                _bonusCHA = 500;
            }

            refreshStats();
        }
    }

    [SerializeField]
    private int _bonusCOM;
    public int BonusCOM
    {
        get
        {
            return _bonusCOM;
        }

        set
        {
            if (value >= 0 && value <= 500)
            {
                _bonusCOM = value;
            }
            else if (value < 0)
            {
                _bonusCOM = 0;
            }
            else
            {
                _bonusCOM = 500;
            }

            refreshStats();
        }
    }

    [SerializeField]
    private int _bonusSKL;
    public int BonusSKL
    {
        get
        {
            return _bonusSKL;
        }

        set
        {
            if (value >= 0 && value <= 500)
            {
                _bonusSKL = value;
            }
            else if (value < 0)
            {
                _bonusSKL = 0;
            }
            else
            {
                _bonusSKL = 500;
            }

            refreshStats();
        }
    }

    /* BATTLE DATA */
    [Header("Battle Data")]

    [SerializeField]
    private int _unitPosition = -1; // Players: 0, 1, 2, 3. Enemies: 4, 5, 6, 7, 8.
    public int UnitPosition
    {
        get
        {
            return _unitPosition;
        }

        set
        {
            if(value >= 0 && value <= 8)
            {
                _unitPosition = value;
            }
        }
    }

    public bool IsGuarding { get; set; }

    public bool DidHitWeakness { get; set; }

    [SerializeField]
    private int _buffModATK = 0;
    public int BuffModATK
    {
        get
        {
            return _buffModATK;
        }

        set
        {
            if(value >= -1 && value <= 1)
            {
                _buffModATK = value;
            }
            else if(value < -1)
            {
                _buffModATK = -1;
            }
            else
            {
                _buffModATK = 1;
            }
        }
    }

    [SerializeField]
    private int _buffModDEF = 0;
    public int BuffModDEF
    {
        get
        {
            return _buffModDEF;
        }

        set
        {
            if (value >= -1 && value <= 1)
            {
                _buffModDEF = value;
            }
            else if (value < -1)
            {
                _buffModDEF = -1;
            }
            else
            {
                _buffModDEF = 1;
            }
        }
    }

    [SerializeField]
    private int _buffModMAG = 0;
    public int BuffModMAG
    {
        get
        {
            return _buffModMAG;
        }

        set
        {
            if (value >= -1 && value <= 1)
            {
                _buffModMAG = value;
            }
            else if (value < -1)
            {
                _buffModMAG = -1;
            }
            else
            {
                _buffModMAG = 1;
            }
        }
    }

    [SerializeField]
    private int _buffModRES = 0;
    public int BuffModRES
    {
        get
        {
            return _buffModRES;
        }

        set
        {
            if (value >= -1 && value <= 1)
            {
                _buffModRES = value;
            }
            else if (value < -1)
            {
                _buffModRES = -1;
            }
            else
            {
                _buffModRES = 1;
            }
        }
    }

    [SerializeField]
    private int _buffModSPD = 0;
    public int BuffModSPD
    {
        get
        {
            return _buffModSPD;
        }

        set
        {
            if (value >= -1 && value <= 1)
            {
                _buffModSPD = value;
            }
            else if (value < -1)
            {
                _buffModSPD = -1;
            }
            else
            {
                _buffModSPD = 1;
            }
        }
    }

    [SerializeField]
    private int _buffModPRC = 0;
    public int BuffModPRC
    {
        get
        {
            return _buffModPRC;
        }

        set
        {
            if (value >= -1 && value <= 1)
            {
                _buffModPRC = value;
            }
            else if (value < -1)
            {
                _buffModPRC = -1;
            }
            else
            {
                _buffModPRC = 1;
            }
        }
    }

    private Dictionary<StatusCondition, int> currentStatuses;
    public void initCurrentStatuses()
    {
        if(currentStatuses != null)
        {
            return;
        }

        currentStatuses = new Dictionary<StatusCondition, int>();
    }

    public bool containsStatus(StatusCondition stat)
    {
        initCurrentStatuses();
        return currentStatuses.ContainsKey(stat);
    }

    public void giveStatus(StatusCondition stat, int turns)
    {
        initCurrentStatuses();
        currentStatuses.Add(stat, turns);
    }

    public void extendStatus(StatusCondition stat, int turns)
    {
        initCurrentStatuses();
        if(currentStatuses.ContainsKey(stat))
        {
            currentStatuses[stat] += turns;
        }
    }

    public void removeStatus(StatusCondition stat)
    {
        initCurrentStatuses();
        if(currentStatuses.ContainsKey(stat))
        {
            currentStatuses.Remove(stat);
        }
    }
    
    public bool countdownStatus(StatusCondition stat)
    {
        initCurrentStatuses();
        if(!currentStatuses.ContainsKey(stat))
        {
            return false;
        }

        currentStatuses[stat]--;

        if(currentStatuses[stat] == 0)
        {
            return true;
        }

        return false;
    }

    public List<ElementType> weakness = new List<ElementType>();
    public List<ElementType> resists = new List<ElementType>();
    public List<ElementType> nullifies = new List<ElementType>();
    public List<ElementType> absorbs = new List<ElementType>();
    public List<ElementType> reflects = new List<ElementType>();

    /* SKILL SETS */
    [System.Serializable]
    public struct SkillsLearnedOnLevelUp
    {
        public int levelLearned;
        public string skillName;
    }

    [Header("Skill Sets")]
    public SkillsLearnedOnLevelUp[] skillsLearnedOnLevelUp; // Define within Editor
    
    public List<KeyValuePair<int, string>> levelUpSkills = null;
    public void initLevelUpSkills() // Populates the above list with the level and skill names provided in the Editor
    {
        if(levelUpSkills != null)
        {
            return;
        }

        levelUpSkills = new List<KeyValuePair<int, string>>();

        foreach(SkillsLearnedOnLevelUp skl in skillsLearnedOnLevelUp)
        {
            levelUpSkills.Add(new KeyValuePair<int, string>(skl.levelLearned, skl.skillName));
        }
    }

    public List<string> learnedSkills; // Skills that are able to be equipped are stored here

    public List<string> equippedSkills; // Skills that are currently equipped go here

    public List<SkillScript> battleSkills; // List of skills to be populated and used by the battle system.

    /* FUNCTIONS */
    public void initCharacter()
    {
        learnedSkills.Clear();
        equippedSkills.Clear();
        initLevelUpSkills();
        initExpGoals();
        this.Level = 1;
        this.ExperiencePoints = 0;
    }

    private void learnNewSkills()
    {
        initLevelUpSkills();

        var tempLookup = levelUpSkills.ToLookup(kvp => kvp.Key, kvp => kvp.Value);

        for(int i = 1; i <= this.Level; ++i)
        {
            foreach (string s in tempLookup[i])
            {
                if(!learnedSkills.Contains(s))
                {
                    learnedSkills.Add(s);
                }
            }
        }
    }

    public void refreshStats()
    {
        int temp = _maxHP;
        _calcStat(ref _maxHP, baseGrowths.baseHP, baseGrowths.growthHP, _bonusHP);
        _currentHP += (_maxHP - temp);
        _calcStat(ref _attack, baseGrowths.baseATK, baseGrowths.growthATK, _bonusATK);
        _calcStat(ref _defense, baseGrowths.baseDEF, baseGrowths.growthDEF, _bonusDEF);
        _calcStat(ref _speed, baseGrowths.baseSPD, baseGrowths.growthSPD, _bonusSPD);

        temp = _maxMP;
        _calcStat(ref _maxMP, baseGrowths.baseMP, baseGrowths.growthMP, _bonusMP);
        _currentMP += (_maxMP - temp);
        _calcStat(ref _magic, baseGrowths.baseMAG, baseGrowths.growthMAG, _bonusMAG);
        _calcStat(ref _resistance, baseGrowths.baseRES, baseGrowths.growthRES, _bonusRES);
        _calcStat(ref _precision, baseGrowths.basePRC, baseGrowths.growthPRC, _bonusPRC);

        _calcStat(ref _bravery, baseGrowths.baseBRV, baseGrowths.growthBRV, _bonusBRV);
        _calcStat(ref _charisma, baseGrowths.baseCHA, baseGrowths.growthCHA, _bonusCHA);
        _calcStat(ref _compassion, baseGrowths.baseCOM, baseGrowths.growthCOM, _bonusCOM);
        _calcStat(ref _skill, baseGrowths.baseSKL, baseGrowths.growthSKL, _bonusSKL);
    }

    private void _calcStat(ref int stat, int baseStat, float growth, int bonus)
    {
        stat = baseStat + (int)(growth * (_level - 1));
        stat += (int)((bonus / 5) * ((_level - 1) / 99.0f));
    }
}
