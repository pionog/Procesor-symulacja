using System;
using System.Collections.Generic;
using UnityEngine;

public class MicrocodeManager : MonoBehaviour
{
    // Odwo³anie do tabeli mikrokodów
    public MicrocodeTable microcodeTable;

    // Odwo³anie do zarz¹dcy rejestrów
    private RegisterManager registerManager;

    // Odwo³anie do wykonawcy mikrokodu
    private MicrocodeExecutor microcodeExecutor;

    private void Awake()
    {
        // Inicjalizacja obiektów
        registerManager = RegisterManager.Instance;
        microcodeTable = new MicrocodeTable(); // Mo¿esz ustawiæ w edytorze Unity lub stworzyæ dynamicznie
        microcodeExecutor = new MicrocodeExecutor(registerManager, microcodeTable);
    }

    // Dodawanie nowego wiersza mikrokodu
    public void AddMicrocodeRow(MicrocodeRow row)
    {
        microcodeTable.AddRow(row);
    }

    // Uruchamianie mikrokodu
    public void ExecuteMicrocode()
    {
        microcodeExecutor.Execute();
    }

    // Mo¿esz dodaæ metodê do czyszczenia tabeli mikrokodów
    public void ClearMicrocodeTable()
    {
        microcodeTable = new MicrocodeTable();
    }

    // Funkcja do testowania dodawania mikrokodów i ich wyœwietlania
    public void DebugMicrocodes()
    {
        Debug.Log(microcodeTable.ToString());
    }
}
