using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MicrocodeExecutor : MonoBehaviour
{
    public static MicrocodeExecutor Instance;
    private RegisterManager RegisterManager;
    private MicrocodeTable MicrocodeTable;
    private MemoryManager MemoryManager;
    private string lastDest;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance of GameManager exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensure this object persists across scenes
        }
    }

    public RegisterManager GetRegisterManager() { 
        return RegisterManager;
    }
    public void SetRegisterManager(RegisterManager RegisterManager) { 
        this.RegisterManager = RegisterManager;
    }
    public MicrocodeTable GetMicrocodeTable()
    {
        return MicrocodeTable;
    }
    public void SetMicrocodeTable(MicrocodeTable MicrocodeTable)
    {
        this.MicrocodeTable = MicrocodeTable;
    }
    public MemoryManager GetMemoryManager()
    {
        return MemoryManager;
    }
    public void SetMemoryManager(MemoryManager MemoryManager)
    {
        this.MemoryManager = MemoryManager;
    }

    public bool Execute(int currentAddress, string[] args, int[] argsType)
    {
        var row = MicrocodeTable.GetRow(currentAddress);
        if (row == null)
        {
            Debug.Log("Pusty wiersz");
            return false;
        }

        // Obs³uga ALU
        if (!string.IsNullOrEmpty(row.ALU))
        {
            int s1 = !string.IsNullOrEmpty(row.S1) ? (row.S1 == "Const" ? (int)row.Const : RegisterManager.GetRegisterValue(row.S1)) : 0;
            int s2 = !string.IsNullOrEmpty(row.S2) ? (row.S2 == "Const" ? (int)row.Const : RegisterManager.GetRegisterValue(row.S2)) : 0;

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
            {
                //Debug.Log("Przed wykonaniem ALU: row.Dest = " + RegisterManager.GetRegisterValue(row.Dest).ToString());
                RegisterManager.SetRegisterValue(row.Dest, result);
                lastDest = row.Dest;
                //Debug.Log("Po wykonaniu ALU: row.Dest = " + RegisterManager.GetRegisterValue(row.Dest).ToString());
            }
        }

        // Obs³uga skoków
        if (!string.IsNullOrEmpty(row.JCond) && !string.IsNullOrEmpty(row.Adr))
        {
            bool conditionMet = row.JCond switch
            {
                "True" => true,
                "GT" => RegisterManager.GetRegisterValue(row.S1) > 0,
                "LT" => RegisterManager.GetRegisterValue(row.S1) < 0,
                "GE" => RegisterManager.GetRegisterValue(row.S1) >= 0,
                "LE" => RegisterManager.GetRegisterValue(row.S1) <= 0,
                "EQ" => RegisterManager.GetRegisterValue(row.S1) == 0,
                _ => false
            };

            if (conditionMet)
            {
                Debug.Log("Nastapilby skok w tym miejscu");
                //currentAddress = int.Parse(row.Adr, System.Globalization.NumberStyles.HexNumber);
            }
        }
        // Obs³uga Mem
        if (!string.IsNullOrEmpty(row.Mem) && !string.IsNullOrEmpty(row.MAdr) && !string.IsNullOrEmpty(row.MDest)) {
            int memoryIndex;
            int value;
            switch (row.Mem)
            {
                case "Read":
                    memoryIndex = RegisterManager.GetRegisterValue(row.MAdr);
                    value = MemoryManager.ReadInt(memoryIndex);
                    RegisterManager.SetRegisterValue(row.MDest, value);
                    break;
                case "Write":
                    memoryIndex = RegisterManager.GetRegisterValue(row.MAdr);
                    value = RegisterManager.GetRegisterValue(row.MDest);
                    MemoryManager.WriteInt(memoryIndex, value);
                    break;
                default:
                    break;
            }
        }

        // Obs³uga Regs
        if (!string.IsNullOrEmpty(row.Regs))
        {
            switch (row.Regs)
            {
                case "RR":
                    RegisterManager.SetRegisterValue("A", RegisterManager.GetRegisterValue(args[0]));
                    RegisterManager.SetRegisterValue("B", RegisterManager.GetRegisterValue(args[1]));
                    break;
                case "RAF3":
                    RegisterManager.SetRegisterValue("A", RegisterManager.GetRegisterValue(args[2]));
                    break;
                case "RAF4":
                    RegisterManager.SetRegisterValue("A", RegisterManager.GetRegisterValue(args[3]));
                    break;
                case "RBF3":
                    RegisterManager.SetRegisterValue("B", RegisterManager.GetRegisterValue(args[2]));
                    break;
                case "RBF4":
                    RegisterManager.SetRegisterValue("B", RegisterManager.GetRegisterValue(args[3]));
                    break;
                case "WF1":
                    RegisterManager.SetRegisterValue(args[0], RegisterManager.GetRegisterValue(lastDest));
                    break;
                case "WF2":
                    RegisterManager.SetRegisterValue(args[1], RegisterManager.GetRegisterValue(lastDest));
                    break;
                case "WF3":
                    RegisterManager.SetRegisterValue(args[2], RegisterManager.GetRegisterValue(lastDest));
                    break;
                case "WF4":
                    RegisterManager.SetRegisterValue(args[3], RegisterManager.GetRegisterValue(lastDest));
                    break;
                default:
                    break;
            }
        }
        return true;
        
    }
}

