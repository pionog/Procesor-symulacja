using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MicrocodeTable
{
    private List<MicrocodeRow> rows = new List<MicrocodeRow>();

    public void AddRow(MicrocodeRow row)
    {
        rows.Add(row);
    }

    public MicrocodeRow GetRow(int address)
    {
        return rows.FirstOrDefault(r => r.Address == address);
    }

    public IEnumerable<MicrocodeRow> GetAllRows()
    {
        return rows;
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
