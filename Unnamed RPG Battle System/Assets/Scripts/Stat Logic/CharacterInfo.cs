using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    [Header("Base Stats and Growth Ratio Data")]
    public BasesAndGrowths baseGrowths;
    // Bonus stats here

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
            if(value >= 0)
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

    [Header("Physical Stats")]
    [SerializeField]
    private int _maxHP;
    public int MaxHP
    {
        get
        {
            return _maxHP;
        }

        set
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

        set
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

    [SerializeField]
    private int _attack;
    public int Attack
    {
        get
        {
            return _attack;
        }

        set
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

        set
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

        set
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

    [Header("Mental Stats")]
    [SerializeField]
    private int _maxMP;
    public int MaxMP
    {
        get
        {
            return _maxMP;
        }

        set
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

        set
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

    [SerializeField]
    private int _magic;
    public int Magic
    {
        get
        {
            return _magic;
        }

        set
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

        set
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

        set
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

    [Header("Lifestyle Stats")]
    [SerializeField]
    private int _bravery;
    public int Bravery
    {
        get
        {
            return _bravery;
        }

        set
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

        set
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

        set
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

        set
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

    /* BATTLE DATA */
    // public int unitPosition = -1; // Players: 0, 1, 2, 3. Enemies: 4, 5, 6, 7, 8.

    // public List<StatusCondition> = new List<StatusCondition>();
    // public void giveStatusCondition(StatusCondition stat) {} // If the status doesn't already exist, put it in the list and start it. Otherwise, extend the current instance.
    // public void removeStatusCondition(string statusName) {} // If the list contains an instance of the same name, then remove it from the list.
    // public void countdownStatuses() {} // When it is this unit's turn, reduce all status timers by one. If any timer hits zero, then that status gets removed.
    // public void activateStatuses() {} // After a unit's turn (when applicable) make the attached status do it's specified action.

    // public List<ElementType> weakness = new List<ElementType>();
    // public List<ElementType> resists = new List<ElementType>();
    // public List<ElementType> nullifies = new List<ElementType>();
    // public List<ElementType> absorbs = new List<ElementType>();
    // public List<ElementType> reflects = new List<ElementType>();

    /* SKILL SETS */
    [System.Serializable]
    public struct SkillsLearnedOnLevelUp
    {
        public int levelLearned;
        public string skillName;
    }

    [Header("Skill Sets")]
    public SkillsLearnedOnLevelUp[] skillsLearnedOnLevelUp; // Define within Editor
    
    public List<KeyValuePair<int, string>> levelUpSkills;
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
        _calcStat(ref _maxHP, baseGrowths.baseHP, baseGrowths.growthHP);
        _currentHP += (_maxHP - temp);
        _calcStat(ref _attack, baseGrowths.baseATK, baseGrowths.growthATK);
        _calcStat(ref _defense, baseGrowths.baseDEF, baseGrowths.growthDEF);
        _calcStat(ref _speed, baseGrowths.baseSPD, baseGrowths.growthSPD);

        temp = _maxMP;
        _calcStat(ref _maxMP, baseGrowths.baseMP, baseGrowths.growthMP);
        _currentMP += (_maxMP - temp);
        _calcStat(ref _magic, baseGrowths.baseMAG, baseGrowths.growthMAG);
        _calcStat(ref _resistance, baseGrowths.baseRES, baseGrowths.growthRES);
        _calcStat(ref _precision, baseGrowths.basePRC, baseGrowths.growthPRC);

        _calcStat(ref _bravery, baseGrowths.baseBRV, baseGrowths.growthBRV);
        _calcStat(ref _charisma, baseGrowths.baseCHA, baseGrowths.growthCHA);
        _calcStat(ref _compassion, baseGrowths.baseCOM, baseGrowths.growthCOM);
        _calcStat(ref _skill, baseGrowths.baseSKL, baseGrowths.growthSKL);
    }

    private void _calcStat(ref int stat, int baseStat, float growth)
    {
        stat = baseStat + (int)(growth * (_level - 1));
        // Add any increases from gear or other bonuses?
    }
}
