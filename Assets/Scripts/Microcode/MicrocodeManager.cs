using System;
using System.Collections.Generic;
using UnityEngine;

public class MicrocodeManager : MonoBehaviour
{
    // Słownik przechowujący tabele mikrokodów dla różnych mnemoników
    private Dictionary<string, MicrocodeTable> microcodeTables;
    public static MicrocodeManager Instance;

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

            microcodeTables = new Dictionary<string, MicrocodeTable>();

            if (!LoadManager.Instance.gameFromSave)
            {
                //start mnemonic
                MicrocodeTable start = new MicrocodeTable();
                MicrocodeRow startRow = new MicrocodeRow() { Address = 0, Mem = "Read", MAdr = "PC", MDest = "IR" };
                start.AddRow(startRow);
                startRow = new MicrocodeRow() { Address = 1, ALU = "ADD", S1 = "PC", S2 = "Const", Dest = "PC", Const = 4, Regs = "RR" };
                start.AddRow(startRow);
                start.SetRemovable(false);
                start.SetEditable(false);
                microcodeTables.Add("START", start);

                //add mnemonic
                MicrocodeTable add = new MicrocodeTable();
                MicrocodeRow addRow = new MicrocodeRow() { Address = 0, ALU = "ADD", S1 = "A", S2 = "B", Dest = "C" };
                add.AddRow(addRow);
                addRow = new MicrocodeRow() { Address = 1, JCond = "True", Regs = "WF3" };
                add.AddRow(addRow);
                microcodeTables.Add("ADD", add);

                //sub mnemonic
                MicrocodeTable sub = new MicrocodeTable();
                MicrocodeRow subRow = new MicrocodeRow() { Address = 0, ALU = "SUB", S1 = "A", S2 = "B", Dest = "C" };
                sub.AddRow(subRow);
                subRow = new MicrocodeRow() { Address = 1, JCond = "True", Regs = "WF3" };
                sub.AddRow(subRow);
                microcodeTables.Add("SUB", sub);

                //load mnemonic
                MicrocodeTable load = new MicrocodeTable();
                MicrocodeRow loadRow = new MicrocodeRow() { Address = 0, ALU = "ADD", S1 = "IR", S2 = "B", Dest = "MAR" };
                load.AddRow(loadRow);
                loadRow = new MicrocodeRow() { Address = 1, Mem = "Read", MAdr = "MAR", MDest = "MDR" };
                load.AddRow(loadRow);
                loadRow = new MicrocodeRow() { Address = 2, ALU = "S1", S1 = "MDR", Dest = "C" };
                load.AddRow(loadRow);
                loadRow = new MicrocodeRow() { Address = 3, JCond = "True", Regs = "WF2" };
                load.AddRow(loadRow);
                microcodeTables.Add("LOAD", load);

                //store mnemonic
                MicrocodeTable store = new MicrocodeTable();
                MicrocodeRow storeRow = new MicrocodeRow() { Address = 0, ALU = "S2", S1 = "A", Dest = "MDR" };
                store.AddRow(storeRow);
                storeRow = new MicrocodeRow() { Address = 1, ALU = "ADD", S1 = "IR", S2 = "B", Dest = "MAR" };
                store.AddRow(storeRow);
                storeRow = new MicrocodeRow() { Address = 2, JCond = "True", Mem = "Write", MAdr = "MAR", MDest = "MDR" };
                store.AddRow(storeRow);
                microcodeTables.Add("STORE", store);

                //move mnemonic
                MicrocodeTable move = new MicrocodeTable();
                MicrocodeRow moveRow = new MicrocodeRow() { Address = 0, ALU = "ADD", S1 = "Const", S2 = "B", Dest = "C", Const = 0 };
                move.AddRow(moveRow);
                moveRow = new MicrocodeRow() { Address = 1, JCond = "True", Regs = "WF1" };
                move.AddRow(moveRow);
                microcodeTables.Add("MOV", move);
            }


        }
    }

    public Dictionary<string, MicrocodeTable> getmicrocodeTables()
    {
        return microcodeTables;
    }

    public void setMicrocodeTables(Dictionary<string, MicrocodeTable> tables)
    {
        microcodeTables = tables;
    }

    // Dodawanie istniej�cej tabeli mikrokod�w do s�ownika
    public void AddMicrocodeTable(string mnemonic, MicrocodeTable table)
    {
        if (!microcodeTables.ContainsKey(mnemonic))
        {
            microcodeTables[mnemonic] = table;
            Debug.Log($"Dodano tabel� mikrokod�w dla mnemonika: {mnemonic}");
        }
        else
        {
            Debug.LogWarning($"Tabela mikrokod�w dla mnemonika '{mnemonic}' ju� istnieje.");
        }
    }

    // Pobieranie tabeli mikrokod�w dla danego mnemonika
    public MicrocodeTable GetMicrocodeTable(string mnemonic)
    {
        if (microcodeTables.TryGetValue(mnemonic, out var table))
        {
            return table;
        }
        else
        {
            Debug.LogError($"Tabela mikrokod�w dla mnemonika '{mnemonic}' nie istnieje.");
            return null;
        }
    }
    public int GetMicrocodeTableLength(string mnemonic) {
        if (microcodeTables.TryGetValue(mnemonic, out var table))
        {
            return table.Count();
        }
        else
        {
            Debug.LogError($"Tabela mikrokod�w dla mnemonika '{mnemonic}' nie istnieje.");
            return 0;
        }
    }

    // Debugowanie wszystkich tabel mikrokod�w
    public void DebugAllMicrocodes()
    {
        foreach (var kvp in microcodeTables)
        {
            Debug.Log($"Mnemonik: {kvp.Key}, Mikrokody:\n{kvp.Value}");
        }
    }
}
