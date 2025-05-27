using UnityEngine;

public class Descript
{
    public string _IDX_;
    public string _SystemType_;
    public string _KO_;
    public string _EN_;
    public string _JP_;
    public string _CH_;
}

public static class DescriptData
{
    public static readonly Descript[] Items = new Descript[]
    {
        new Descript { _IDX_ = "1", _SystemType_ = "Dialogue", _KO_ = "TRUE", _EN_ = "FALSE", _JP_ = "FALSE", _CH_ = "FALSE" },
        new Descript { _IDX_ = "2", _SystemType_ = "Quest", _KO_ = "TRUE", _EN_ = "FALSE", _JP_ = "FALSE", _CH_ = "FALSE" },
    };
}
