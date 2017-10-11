using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class ActionUnitData 
{
    public string Name;
    public int Index;
    public bool Used;
    public float Value;

    public ActionUnitData(int index, string name, bool used)
    {
        Index = index;
        Name = name;
        Used = used;
        Value = 0f;
    }
}
