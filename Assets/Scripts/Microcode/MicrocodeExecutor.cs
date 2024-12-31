using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrocodeExecutor : MonoBehaviour
{
    private RegisterManager registerManager;
    private MicrocodeTable microcodeTable;

    public MicrocodeExecutor(RegisterManager regManager, MicrocodeTable microTable)
    {
        registerManager = regManager;
        microcodeTable = microTable;
    }

    public void Execute()
    {
        int currentAddress = 0;

        while (true)
        {
            var row = microcodeTable.GetRow(currentAddress);
            if (row == null)
                break;

            Debug.Log($"Executing uAR {row.Address:X4}");

            // Obs³uga ALU
            if (!string.IsNullOrEmpty(row.ALU))
            {
                int s1 = !string.IsNullOrEmpty(row.S1) ? (row.S1 == "Const" ? (int)row.Const : registerManager.GetRegisterValue(row.S1)) : 0;
                int s2 = !string.IsNullOrEmpty(row.S2) ? (row.S2 == "Const" ? (int)row.Const : registerManager.GetRegisterValue(row.S2)) : 0;

                int result = row.ALU switch
                {
                    "ADD" => s1 + s2,
                    "SUB" => s1 - s2,
                    "MUL" => s1 * s2,
                    "DIV" => s1 / s2,
                    "XOR" => s1 ^ s2,
                    "OR" => s1 | s2,
                    "AND" => s1 & s2,
                    "SRL" => s1 >> s2,
                    "SLL" => s1 << s2,
                    "S1" => s1,
                    "S2" => s2,
                    _ => throw new Exception($"Unknown ALU operation: {row.ALU}")
                };

                if (!string.IsNullOrEmpty(row.Dest))
                    registerManager.SetRegisterValue(row.Dest, result);
            }

            // Obs³uga skoków
            if (!string.IsNullOrEmpty(row.JCond) && !string.IsNullOrEmpty(row.Adr))
            {
                bool conditionMet = row.JCond switch
                {
                    "True" => true,
                    "GT" => registerManager.GetRegisterValue(row.S1) > 0,
                    "LT" => registerManager.GetRegisterValue(row.S1) < 0,
                    "GE" => registerManager.GetRegisterValue(row.S1) >= 0,
                    "LE" => registerManager.GetRegisterValue(row.S1) <= 0,
                    "EQ" => registerManager.GetRegisterValue(row.S1) == 0,
                    _ => false
                };

                if (conditionMet)
                {
                    currentAddress = int.Parse(row.Adr, System.Globalization.NumberStyles.HexNumber);
                    continue;
                }
            }
            // Obs³uga Mem
            if (!string.IsNullOrEmpty(row.Mem) && !string.IsNullOrEmpty(row.MAdr) && !string.IsNullOrEmpty(row.MDest)) { 
                
            }

            // Obs³uga Regs
            if (!string.IsNullOrEmpty(row.Regs)) {
                
            }


            currentAddress++;
        }
    }
}

