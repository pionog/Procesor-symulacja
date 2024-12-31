using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrocodeRow : MonoBehaviour
{
    public int Address { get; set; } // uAR
    public string Label { get; set; }
    public string ALU { get; set; }
    public string S1 { get; set; }
    public string S2 { get; set; }
    public string Dest { get; set; }
    public string ExtIR { get; set; }
    public int? Const { get; set; } // Nullable, bo mo¿e byæ puste
    public string JCond { get; set; }
    public string Adr { get; set; }
    public string Mem { get; set; }
    public string MAdr { get; set; }
    public string MDest { get; set; }
    public string Regs { get; set; }

    public MicrocodeRow(int address)
    {
        Address = address;
    }

    public override string ToString()
    {
        return $"{Address:X4} | {Label ?? " "}, {ALU ?? " "}, {S1 ?? " "}, {S2 ?? " "}, {Dest ?? " "}, {ExtIR ?? " "}, {Const?.ToString() ?? " "}, {JCond ?? " "}, {Adr ?? " "}, {Mem ?? " "}, {MAdr ?? " "}, {MDest ?? " "}, {Regs ?? " "}";
    }
}

