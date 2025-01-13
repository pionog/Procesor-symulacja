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
    private bool StartMnemonic = true;
    private int CurrentInstruction = 0;

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
    public void SetStartBool(bool start) {
        this.StartMnemonic = start;
    }
    public bool GetStartBool()
    {
        return StartMnemonic;
    }
    public int GetCurrentInstruction() { 
        return this.CurrentInstruction;
    }
    public void SetCurrentInstruction(int index) { 
        this.CurrentInstruction = index;
    }

    public bool Execute(int currentAddress, string[] args, int[] argsType)
    {
        
        var row = MicrocodeTable.GetRow(currentAddress);
        if (row == null)
        {
            Debug.Log("Pusty wiersz");
            return false;
        }
        Debug.Log(row.ToString());

        // Obs�uga ALU
        if (!string.IsNullOrEmpty(row.ALU))
        {
            int s1 = !string.IsNullOrEmpty(row.S1) ? (row.S1 == "Const" ? (int)row.Const : RegisterManager.Instance.GetRegisterValue(row.S1)) : 0;
            int s2 = !string.IsNullOrEmpty(row.S2) ? (row.S2 == "Const" ? (int)row.Const : RegisterManager.Instance.GetRegisterValue(row.S2)) : 0;

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
                if(row.ALU == "MUL") Debug.Log(result.ToString());
                RegisterManager.Instance.SetRegisterValue(row.Dest, result);
                //Debug.Log("Po wykonaniu ALU: row.Dest = " + RegisterManager.Instance.GetRegisterValue(row.Dest).ToString());
            }
        }

        // Obs�uga skok�w
        if (!string.IsNullOrEmpty(row.JCond))
        {
            int value = row.ALU switch {
                "S1" => RegisterManager.Instance.GetRegisterValue(row.S1),
                "S2" => RegisterManager.Instance.GetRegisterValue(row.S2),
                _ => 0
            };
            //Debug.Log(value.ToString());

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
                //Debug.Log("Wykonuje skok!");
                StartMnemonic = !StartMnemonic;
            }
            else {
                //Debug.Log("Falsz!");
            }
        }
        // Obs�uga Mem
        if (!string.IsNullOrEmpty(row.Mem) && !string.IsNullOrEmpty(row.MAdr) && !string.IsNullOrEmpty(row.MDest)) {
            int memoryIndex;
            int value;
            switch (row.Mem)
            {
                case "Read":
                    
                    memoryIndex = RegisterManager.Instance.GetRegisterValue(row.MAdr);
                    Debug.Log("memoryIndex: " + memoryIndex.ToString());
                    value = MemoryManager.ReadInt(memoryIndex);
                    Debug.Log("value: " + value.ToString());
                    RegisterManager.Instance.SetRegisterValue(row.MDest, value);
                    if (StartMnemonic) { 
                        Debug.Log("Zapisuje w IR liczbe: " + Instance.CurrentInstruction.ToString());
                        Instance.CurrentInstruction = memoryIndex;
                    }
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

        // Obs�uga Regs
        if (!string.IsNullOrEmpty(row.Regs))
        {
            List<string> regs = new List<string>();
            string registers = "";
            for (int i = 0; i < args.Length; i++)
            {
                if (argsType[i] == 0)
                {
                    regs.Add(args[i]);
                    registers += args[i] + " ";
                }
                if (argsType[i] == 3) {
                    string[] splitText = args[i].Split("(");
                    string register = splitText[1].Remove(splitText.Length);
                    Debug.Log("Wyodrebniony rejestr: " + register);
                    regs.Insert(0, register);
                    registers = register + " " + registers; 

                }
            }

            regs.Add("R0");
            regs.Add("R0");
            regs.Add("R0"); //in case if user forgets to write registers in instruction input field


            switch (row.Regs)
            {
                case "RR":
                    Debug.Log(registers);
                    RegisterManager.Instance.SetRegisterValue("A", RegisterManager.Instance.GetRegisterValue(regs[0]));
                    RegisterManager.Instance.SetRegisterValue("B", RegisterManager.Instance.GetRegisterValue(regs[1]));
                    break;
                case "RAF3":
                    RegisterManager.Instance.SetRegisterValue("A", argsType[2] == 0 ? RegisterManager.Instance.GetRegisterValue(regs[2]) : 0);
                    break;
                case "RAF4":
                    RegisterManager.Instance.SetRegisterValue("A", argsType[3] == 0 ? RegisterManager.Instance.GetRegisterValue(regs[3]) : 0);
                    break;
                case "RBF3":
                    RegisterManager.Instance.SetRegisterValue("B", argsType[2] == 0 ? RegisterManager.Instance.GetRegisterValue(regs[2]) : 0);
                    break;
                case "RBF4":
                    RegisterManager.Instance.SetRegisterValue("B", argsType[3] == 0 ? RegisterManager.Instance.GetRegisterValue(regs[3]) : 0);
                    break;
                case "WF1":
                    RegisterManager.Instance.SetRegisterValue(regs[0], RegisterManager.Instance.GetRegisterValue("C"));
                    break;
                case "WF2":
                    RegisterManager.Instance.SetRegisterValue(regs[1], RegisterManager.Instance.GetRegisterValue("C"));
                    break;
                case "WF3":
                    Debug.Log(RegisterManager.Instance.GetRegisterValue("TMP1").ToString());
                    Debug.Log(regs[2]);
                    RegisterManager.Instance.SetRegisterValue(regs[2], RegisterManager.Instance.GetRegisterValue("C"));
                    break;
                case "WF4":
                    RegisterManager.Instance.SetRegisterValue(regs[3], RegisterManager.Instance.GetRegisterValue("C"));
                    break;
                default:
                    break;
            }
        }
        return true;
        
    }
}

