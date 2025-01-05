using System;
using System.Collections.Generic;
using UnityEngine;

public class MicrocodeManager : MonoBehaviour
{
    // S³ownik przechowuj¹cy tabele mikrokodów dla ró¿nych mnemoników
    private Dictionary<string, MicrocodeTable> microcodeTables;

    private void Awake()
    {
        // Inicjalizacja s³ownika
        microcodeTables = new Dictionary<string, MicrocodeTable>();
    }

    // Dodawanie istniej¹cej tabeli mikrokodów do s³ownika
    public void AddMicrocodeTable(string mnemonic, MicrocodeTable table)
    {
        if (!microcodeTables.ContainsKey(mnemonic))
        {
            microcodeTables[mnemonic] = table;
            Debug.Log($"Dodano tabelê mikrokodów dla mnemonika: {mnemonic}");
        }
        else
        {
            Debug.LogWarning($"Tabela mikrokodów dla mnemonika '{mnemonic}' ju¿ istnieje.");
        }
    }

    // Pobieranie tabeli mikrokodów dla danego mnemonika
    public MicrocodeTable GetMicrocodeTable(string mnemonic)
    {
        if (microcodeTables.TryGetValue(mnemonic, out var table))
        {
            DebugAllMicrocodes();
            return table;
        }
        else
        {
            Debug.LogError($"Tabela mikrokodów dla mnemonika '{mnemonic}' nie istnieje.");
            return null;
        }
    }

    // Debugowanie wszystkich tabel mikrokodów
    public void DebugAllMicrocodes()
    {
        foreach (var kvp in microcodeTables)
        {
            Debug.Log($"Mnemonik: {kvp.Key}, Mikrokody:\n{kvp.Value}");
        }
    }
}
