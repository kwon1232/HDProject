using System.Collections.Generic;
using UnityEngine;

public class MonsterSkillInfo
{
    public int IDX;
    public string IDXSkill;
    public string MonsterID;
    public string SkillName;
    public string ImagePath;
    public string SkillDescription_ko;
    public string EffectType;
    public int DamageAmount;
    public int EffectTime;
    public string Overlapping;
}

public static class MonsterSkillInfoData
{
    public static readonly MonsterSkillInfo[] Items = new MonsterSkillInfo[]
    {
        new MonsterSkillInfo
        {
            IDX = 1,
            IDXSkill = "SKILL0001",
            MonsterID = "MON0001",
            SkillName = "버섯의 독",
            ImagePath = "",
            SkillDescription_ko = "2초간 -2HP",
            EffectType = "Damage",
            DamageAmount = 2,
            EffectTime = 3,
            Overlapping = "F",
        },
        new MonsterSkillInfo
        {
            IDX = 2,
            IDXSkill = "SKILL0002",
            MonsterID = "MON0002",
            SkillName = "분노의 화살",
            ImagePath = "",
            SkillDescription_ko = "2초간 -2HP",
            EffectType = "Damage",
            DamageAmount = 2,
            EffectTime = 3,
            Overlapping = "F",
        },
        new MonsterSkillInfo
        {
            IDX = 3,
            IDXSkill = "SKILL0003",
            MonsterID = "MON0003",
            SkillName = "독 분비물",
            ImagePath = "",
            SkillDescription_ko = "2초간 -2HP",
            EffectType = "Damage",
            DamageAmount = 2,
            EffectTime = 3,
            Overlapping = "F",
        },
        new MonsterSkillInfo
        {
            IDX = 4,
            IDXSkill = "SKILL0004",
            MonsterID = "MON0004",
            SkillName = "할퀴기",
            ImagePath = "",
            SkillDescription_ko = "2초간 -2HP",
            EffectType = "Damage",
            DamageAmount = 2,
            EffectTime = 3,
            Overlapping = "F",
        },
    };
}
