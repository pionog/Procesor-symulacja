using System;
using System.Collections.Generic;
using UnityEngine;

public class MicrocodeManager : MonoBehaviour
{
    // S�ownik przechowuj�cy tabele mikrokod�w dla r�nych mnemonik�w
    private Dictionary<string, MicrocodeTable> microcodeTables;

    private void Awake()
    {
        // Inicjalizacja s�ownika
        microcodeTables = new Dictionary<string, MicrocodeTable>();
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
            DebugAllMicrocodes();
            return table;
        }
        else
        {
            Debug.LogError($"Tabela mikrokod�w dla mnemonika '{mnemonic}' nie istnieje.");
            return null;
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
