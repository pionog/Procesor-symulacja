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
    /// <summary>
    /// Uzyskiwanie menad¿era rejestrów
    /// </summary>
    /// <returns>
    /// Menad¿er rejestrów
    /// </returns>
    public RegisterManager GetRegisterManager() { 
        return RegisterManager;
    }
    /// <summary>
    /// Ustawianie menad¿era rejestrów
    /// </summary>
    /// <param name="RegisterManager">
    /// Menad¿er rejestrów
    /// </param>
    public void SetRegisterManager(RegisterManager RegisterManager) { 
        this.RegisterManager = RegisterManager;
    }
    /// <summary>
    /// Uzyskiwanie menad¿era tabel mikrokodów
    /// </summary>
    /// <returns>
    /// Menad¿er tabel mikrokodów
    /// </returns>
    public MicrocodeTable GetMicrocodeTable()
    {
        return MicrocodeTable;
    }
    /// <summary>
    /// Ustawianie menad¿era tabel mikrokodów
    /// </summary>
    /// <param name="MicrocodeTable">
    /// Menad¿er tabel mikrokodów
    /// </param>
    public void SetMicrocodeTable(MicrocodeTable MicrocodeTable)
    {
        this.MicrocodeTable = MicrocodeTable;
    }
    /// <summary>
    /// Uzyskiwanie menad¿era pamiêci
    /// </summary>
    /// <returns>
    /// Menad¿er pamiêci
    /// </returns>
    public MemoryManager GetMemoryManager()
    {
        return MemoryManager;
    }
    /// <summary>
    /// Ustawianie menad¿era pamiêci
    /// </summary>
    /// <param name="MemoryManager">
    /// Menad¿er pamiêci
    /// </param>
    public void SetMemoryManager(MemoryManager MemoryManager)
    {
        this.MemoryManager = MemoryManager;
    }
    /// <summary>
    /// Ustawianie informacji o tym, czy program aktualnie wykonuje operacjê prze³¹czania siê miêdzy instrukcjami
    /// </summary>
    /// <param name="start"></param>
    public void SetStartBool(bool start) {
        this.StartMnemonic = start;
    }

    /// <summary>
    /// Pobieranie informacji, czy program obecnie wykonuje mechanizm prze³¹czania siê miêdzy instrukcjami
    /// </summary>
    /// <returns>
    /// <c>bool</c> czy program wykonuje mnemonik <c>"START"</c>
    /// </returns>
    public bool GetStartBool()
    {
        return StartMnemonic;
    }

    /// <summary>
    /// Pobieranie numeru instrukcji aktualnie rozpatrywanej przez program
    /// </summary>
    /// <returns>
    /// <c>int</c> numer indeksu rozpatrywanej instrukcji
    /// </returns>
    public int GetCurrentInstruction() { 
        return this.CurrentInstruction;
    }
    /// <summary>
    /// Wpisywanie numeru indeksu aktualnie rozpatrywanej instrukcji
    /// </summary>
    /// <param name="index">
    /// Numer indeksu rozpatrywanej instrukcji
    /// </param>
    public void SetCurrentInstruction(int index) { 
        this.CurrentInstruction = index;
    }

    /// <summary>
    /// Wykonywanie instrukcji zawartej w tabli mikrokodów. Wykonanie kroku w grze
    /// </summary>
    /// <param name="currentAddress">
    /// Adres wiersza w aktualnej tabeli mikrokodów
    /// </param>
    /// <param name="args">
    /// Lista argumentów instrukcji
    /// </param>
    /// <param name="argsType">
    /// Lista typów argumentów zawartych w zmiennej <c>args</c>
    /// </param>
    /// <returns>
    /// <c>bool</c> Czy program wykona³ operacje zawarte w danym wierszu tabeli mikrokodu
    /// </returns>
    /// <exception cref="Exception"></exception>
    public bool Execute(int currentAddress, string[] args, int[] argsType)
    {
        
        var row = MicrocodeTable.GetRow(currentAddress);
        if (row == null)
        {
            Debug.Log("Pusty wiersz");
            return false;
        }
        Debug.Log(row.ToString());

        // Obs³uga ALU
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
                if(row.ALU == "MUL") Debug.Log(result.ToString());
                RegisterManager.Instance.SetRegisterValue(row.Dest, result);
            }
        }

        // Obs³uga skoków
        if (!string.IsNullOrEmpty(row.JCond))
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
                StartMnemonic = !StartMnemonic;
            }
            else {
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
                    if (StartMnemonic) { 
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

        // Obs³uga Regs
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

