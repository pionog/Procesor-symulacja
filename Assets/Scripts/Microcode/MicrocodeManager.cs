using System;
using System.Collections.Generic;
using UnityEngine;

public class MicrocodeManager : MonoBehaviour
{
    // Odwo�anie do tabeli mikrokod�w
    public MicrocodeTable microcodeTable;

    // Odwo�anie do zarz�dcy rejestr�w
    private RegisterManager registerManager;

    // Odwo�anie do wykonawcy mikrokodu
    private MicrocodeExecutor microcodeExecutor;

    private void Awake()
    {
        // Inicjalizacja obiekt�w
        registerManager = RegisterManager.Instance;
        microcodeTable = new MicrocodeTable(); // Mo�esz ustawi� w edytorze Unity lub stworzy� dynamicznie
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

    // Mo�esz doda� metod� do czyszczenia tabeli mikrokod�w
    public void ClearMicrocodeTable()
    {
        microcodeTable = new MicrocodeTable();
    }

    // Funkcja do testowania dodawania mikrokod�w i ich wy�wietlania
    public void DebugMicrocodes()
    {
        Debug.Log(microcodeTable.ToString());
    }
}
