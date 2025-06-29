using System.Collections.Generic;
using UnityEngine;

public class Descript
{
    public int IDX;
    public string SystemType;
    public bool KO;
    public bool EN;
    public bool JP;
    public bool CH;
}

public static class DescriptData
{
    public static readonly Descript[] Items = new Descript[]
    {
        new Descript
        {
            IDX = 1,
            SystemType = "Dialogue",
            KO = true,
            EN = false,
            JP = false,
            CH = false,
        },
        new Descript
        {
            IDX = 2,
            SystemType = "Quest",
            KO = true,
            EN = false,
            JP = false,
            CH = false,
        },
    };
}
