using System;
using System.Collections.Generic;
using UnityEngine;

public class MicrocodeManager : MonoBehaviour
{
    // S³ownik przechowuj¹cy tabele mikrokodów dla ró¿nych mnemoników
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
            microcodeTables = new Dictionary<string, MicrocodeTable>();
            DontDestroyOnLoad(gameObject); // Ensure this object persists across scenes
        }
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
            return table;
        }
        else
        {
            Debug.LogError($"Tabela mikrokodów dla mnemonika '{mnemonic}' nie istnieje.");
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
            Debug.LogError($"Tabela mikrokodów dla mnemonika '{mnemonic}' nie istnieje.");
            return 0;
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
