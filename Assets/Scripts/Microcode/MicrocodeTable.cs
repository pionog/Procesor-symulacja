using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class MicrocodeTable
{
    [SerializeField] private List<MicrocodeRow> rows = new List<MicrocodeRow>();
    [SerializeField] private int MicrocodeType = 0; // 0 - R type, 1 - I Type, 2 - J Type
    [SerializeField] private int RegistersNumber = 2;
    [SerializeField] private bool Removable = true;
    [SerializeField] private bool Editable = true;

    public void AddRow(MicrocodeRow row)
    {
        rows.Add(row);
    }

    public MicrocodeRow GetRow(int address)
    {
        return rows.FirstOrDefault(r => r.Address == address);
    }

    public int GetMicrocodeType() => MicrocodeType;
    public void SetMicrocodeType(int type) => MicrocodeType = type;

    public int GetRegistersNumber() => RegistersNumber;
    public void SetRegistersNumber(int registersNumber) => RegistersNumber = registersNumber;

    public bool GetRemovable() => Removable;
    public void SetRemovable(bool removable) => Removable = removable;

    public bool GetEditable() => Editable;
    public void SetEditable(bool editable) => Editable = editable;

    public IEnumerable<MicrocodeRow> GetAllRows() => rows;
    
    public List<MicrocodeRow> GetAllRowsList()
    {
        return rows;
    }

    public int Count() => rows.Count;

    public void Clear() => rows.Clear();

    public override string ToString()
    {
        return string.Join("\n", rows.Select(r => r.ToString()));
    }
}

