using System.Collections.Generic;
using UnityEngine;

public class MonsterInfo
{
    public int IDX;
    public string MonsterID;
    public string CharacterID;
    public string Name;
    public string MapID;
    public int HP;
    public int ATK;
    public int DEF;
    public string SkillName1;
    public string SkillDescription1;
    public string Skill1_IDX;
    public string SkillName2;
    public string SkillDescription2;
    public string Skill2_IDX;
    public string Description;
}

public static class MonsterInfoData
{
    public static readonly MonsterInfo[] Items = new MonsterInfo[]
    {
        new MonsterInfo
        {
            IDX = 1,
            MonsterID = "MON0001",
            CharacterID = "CH0002",
            Name = "중독된 버섯",
            MapID = "RM0001",
            HP = 120,
            ATK = 10,
            DEF = 5,
            SkillName1 = "버섯의 독",
            SkillDescription1 = "2초간 -2HP",
            Skill1_IDX = "SKILL0001",
            SkillName2 = "",
            SkillDescription2 = "",
            Skill2_IDX = "",
            Description = "",
        },
        new MonsterInfo
        {
            IDX = 2,
            MonsterID = "MON0002",
            CharacterID = "CH0003",
            Name = "화난 요정",
            MapID = "RM0001",
            HP = 80,
            ATK = 8,
            DEF = 2,
            SkillName1 = "분노의 화살",
            SkillDescription1 = "2초간 -2HP",
            Skill1_IDX = "SKILL0002",
            SkillName2 = "",
            SkillDescription2 = "",
            Skill2_IDX = "",
            Description = "",
        },
        new MonsterInfo
        {
            IDX = 3,
            MonsterID = "MON0003",
            CharacterID = "CH0004",
            Name = "중독된 쥐",
            MapID = "RM0001",
            HP = 160,
            ATK = 12,
            DEF = 8,
            SkillName1 = "독 분비물",
            SkillDescription1 = "2초간 -2HP",
            Skill1_IDX = "SKILL0003",
            SkillName2 = "",
            SkillDescription2 = "",
            Skill2_IDX = "",
            Description = "",
        },
        new MonsterInfo
        {
            IDX = 4,
            MonsterID = "MON0004",
            CharacterID = "CH0005",
            Name = "작은 늑대",
            MapID = "RM0002",
            HP = 160,
            ATK = 15,
            DEF = 5,
            SkillName1 = "할퀴기",
            SkillDescription1 = "2초간 -2HP",
            Skill1_IDX = "SKILL0004",
            SkillName2 = "",
            SkillDescription2 = "",
            Skill2_IDX = "",
            Description = "",
        },
    };
}
