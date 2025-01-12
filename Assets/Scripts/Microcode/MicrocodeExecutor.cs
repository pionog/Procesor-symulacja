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
    private bool StartMnemonic = true;
    private bool JumpToLabel = false;

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
    public bool GetJumpBool() {
        return JumpToLabel;
    }
    public void SetStartBool(bool start) {
        this.StartMnemonic = start;
    }
    public bool GetStartBool()
    {
        return StartMnemonic;
    }
    public void SetJumpBool(bool jump)
    {
        this.JumpToLabel = jump;
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
            int s1 = !string.IsNullOrEmpty(row.S1) ? (row.S1 == "Const" ? (int)row.Const : row.S1 == "IR" ? 0 : RegisterManager.Instance.GetRegisterValue(row.S1)) : 0;
            int s2 = !string.IsNullOrEmpty(row.S2) ? (row.S2 == "Const" ? (int)row.Const : row.S2 == "IR" ? 0 : RegisterManager.Instance.GetRegisterValue(row.S2)) : 0;

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
                //Debug.Log("Przed wykonaniem ALU: row.Dest = " + RegisterManager.Instance.GetRegisterValue(row.Dest).ToString());
                RegisterManager.Instance.SetRegisterValue(row.Dest, result);
                lastDest = row.Dest;
                //Debug.Log("Po wykonaniu ALU: row.Dest = " + RegisterManager.Instance.GetRegisterValue(row.Dest).ToString());
            }
        }

        // Obs³uga skoków
        if (!string.IsNullOrEmpty(row.JCond) && !string.IsNullOrEmpty(row.Adr))
        {
            int value = row.ALU switch {
                "S1" => RegisterManager.Instance.GetRegisterValue(row.S1),
                "S2" => RegisterManager.Instance.GetRegisterValue(row.S2),
                _ => 0
            };


            bool conditionMet = row.JCond switch
            {
                "True" => true,
                "GT" => value > 0,
                "LT" => value < 0,
                "GE" => value >= 0,
                "LE" => value <= 0,
                "EQ" => value == 0,
                _ => false
            };

            if (conditionMet)
            {
                StartMnemonic = true;
                //currentAddress = int.Parse(row.Adr, System.Globalization.NumberStyles.HexNumber);
            }
            else {
                JumpToLabel = false;
            }
        }
        // Obs³uga Mem
        if (!string.IsNullOrEmpty(row.Mem) && !string.IsNullOrEmpty(row.MAdr) && !string.IsNullOrEmpty(row.MDest)) {
            int memoryIndex;
            int value;
            switch (row.Mem)
            {
                case "Read":
                    memoryIndex = RegisterManager.Instance.GetRegisterValue(row.MAdr);
                    value = MemoryManager.ReadInt(memoryIndex);
                    RegisterManager.Instance.SetRegisterValue(row.MDest, value);
                    break;
                case "Write":
                    memoryIndex = RegisterManager.Instance.GetRegisterValue(row.MAdr);
                    value = RegisterManager.Instance.GetRegisterValue(row.MDest);
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
                    RegisterManager.Instance.SetRegisterValue("A", RegisterManager.Instance.GetRegisterValue(args[0]));
                    RegisterManager.Instance.SetRegisterValue("B", RegisterManager.Instance.GetRegisterValue(args[1]));
                    break;
                case "RAF3":
                    RegisterManager.Instance.SetRegisterValue("A", RegisterManager.Instance.GetRegisterValue(args[2]));
                    break;
                case "RAF4":
                    RegisterManager.Instance.SetRegisterValue("A", RegisterManager.Instance.GetRegisterValue(args[3]));
                    break;
                case "RBF3":
                    RegisterManager.Instance.SetRegisterValue("B", RegisterManager.Instance.GetRegisterValue(args[2]));
                    break;
                case "RBF4":
                    RegisterManager.Instance.SetRegisterValue("B", RegisterManager.Instance.GetRegisterValue(args[3]));
                    break;
                case "WF1":
                    RegisterManager.Instance.SetRegisterValue(args[0], RegisterManager.Instance.GetRegisterValue(lastDest));
                    break;
                case "WF2":
                    RegisterManager.Instance.SetRegisterValue(args[1], RegisterManager.Instance.GetRegisterValue(lastDest));
                    break;
                case "WF3":
                    RegisterManager.Instance.SetRegisterValue(args[2], RegisterManager.Instance.GetRegisterValue(lastDest));
                    break;
                case "WF4":
                    RegisterManager.Instance.SetRegisterValue(args[3], RegisterManager.Instance.GetRegisterValue(lastDest));
                    break;
                default:
                    break;
            }
        }
        return true;
        
    }
}

