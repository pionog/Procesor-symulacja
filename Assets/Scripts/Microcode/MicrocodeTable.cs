using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MicrocodeTable
{
    private List<MicrocodeRow> rows = new List<MicrocodeRow>();
    private int MicrocodeType = 0; // 0 - R type,   1 - I Type, 2 - J Type
    private int RegistersNumber = 2;
    private bool Removable = true;
    private bool Editable = true;

    public void AddRow(MicrocodeRow row)
    {
        rows.Add(row);
    }

    public MicrocodeRow GetRow(int address)
    {
        return rows.FirstOrDefault(r => r.Address == address);
    }
    public int GetMicrocodeType() {
        Debug.Log("Odczyta³em nastêpuj¹c¹ wartoœæ typu mnemonika: " + MicrocodeType.ToString());
        return MicrocodeType;
    }
    public void SetMicrocodeType(int type) {
        MicrocodeType = type;
        Debug.Log("Zapisujê nastêpuj¹c¹ wartoœæ typu mnemonika: " + MicrocodeType.ToString());
    }
    public int GetRegistersNumber()
    {
        return RegistersNumber;
    }
    public void SetRegistersNumber(int registersNumber)
    {
        RegistersNumber = registersNumber;
    }
    public bool GetRemovable()
    {
        return Removable;
    }
    public void SetRemovable(bool Removable)
    {
        this.Removable = Removable;
    }
    public bool GetEditable()
    {
        return Editable;
    }
    public void SetEditable(bool Editable)
    {
        this.Editable = Editable;
    }

    public IEnumerable<MicrocodeRow> GetAllRows()
    {
        return rows;
    }
    public int Count() { 
        return rows.Count;
    }

    public void Clear()
    {
        rows.Clear();
    }

    public override string ToString()
    {
        return string.Join("\n", rows.Select(r => r.ToString()));
    }
}
