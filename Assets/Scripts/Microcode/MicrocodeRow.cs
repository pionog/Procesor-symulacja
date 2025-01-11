using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MicrocodeRow
{
    [SerializeField] public int Address; // uAR
    [SerializeField] public string Label;
    [SerializeField] public string ALU;
    [SerializeField] public string S1;
    [SerializeField] public string S2;
    [SerializeField] public string Dest;
    [SerializeField] public string ExtIR;
    [SerializeField] public int? Const; // Nullable, can be left empty
    [SerializeField] public string JCond;
    [SerializeField] public string Adr;
    [SerializeField] public string Mem;
    [SerializeField] public string MAdr;
    [SerializeField] public string MDest;
    [SerializeField] public string Regs;

    public MicrocodeRow() { }

    public MicrocodeRow(int address)
    {
        Address = address;
    }

    public override string ToString()
    {
        return $"{Address:X4} | {Label ?? " "}, {ALU ?? " "}, {S1 ?? " "}, {S2 ?? " "}, {Dest ?? " "}, {ExtIR ?? " "}, {Const?.ToString() ?? " "}, {JCond ?? " "}, {Adr ?? " "}, {Mem ?? " "}, {MAdr ?? " "}, {MDest ?? " "}, {Regs ?? " "}";
    }
}


