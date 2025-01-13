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
                startRow = new MicrocodeRow() { Address = 1, ALU = "ADD", S1 = "PC", S2 = "Const", Dest = "PC", Const = 4, JCond="True", Regs = "RR" };
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

                GameManager.Instance.SetActualMnemonics(GameManager.Instance.GetActualMnemonics());
                GameManager.Instance.SetDefinedMnemonics(GameManager.Instance.GetDefinedMnemonics());
            }


        }
    }

    /// <summary>
    /// Uzyskiwanie słownika tabel mikrokodów
    /// </summary>
    /// <returns>
    /// Słownik tabel mikrokodów
    /// </returns>
    public Dictionary<string, MicrocodeTable> getmicrocodeTables()
    {
        return microcodeTables;
    }


    /// <summary>
    /// Ustawianie tabel mikrokodów
    /// </summary>
    /// <param name="tables">
    /// Słownik tabel mikrokodów
    /// </param>
    public void setMicrocodeTables(Dictionary<string, MicrocodeTable> tables)
    {
        microcodeTables = tables;
    }

    /// <summary>
    /// Dodawanie istniejącej tabeli mikrokodów do słownika
    /// </summary>
    /// <param name="mnemonic">
    /// Mnemonik, którego tabela mikrokodów ma zostać dodana do słownika
    /// </param>
    /// <param name="table">
    /// Tabela mikrokodów danego mnemonika
    /// </param>
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

    /// <summary>
    /// Pobieranie tabeli mikrokodów dla danego mnemonika
    /// </summary>
    /// <param name="mnemonic">
    /// Mnemonik, którego tabela mikrokodów ma zostać pobrana
    /// </param>
    /// <returns>
    /// <c>MicrocodeTable</c> tabela mikrokodów wskazanego mnemonika
    /// </returns>
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

    /// <summary>
    /// Pobieranie długości wskazanej tabeli mikrokodów
    /// </summary>
    /// <param name="mnemonic">
    /// Mnemonik, którego długość tabeli ma zostać uzyskana
    /// </param>
    /// <returns>
    /// <c>int</c> długość tabeli
    /// </returns>
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

    /// <summary>
    /// Debugowanie menadżera mikrokodów w Unity
    /// </summary>
    public void DebugAllMicrocodes()
    {
        foreach (var kvp in microcodeTables)
        {
            Debug.Log($"Mnemonik: {kvp.Key}, Mikrokody:\n{kvp.Value}");
        }
    }
}
