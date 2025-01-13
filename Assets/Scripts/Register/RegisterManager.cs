using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RegisterManager : MonoBehaviour
{
    public static RegisterManager Instance { get; set; }
    public Dictionary<string, Register> SingleRegisters { get; set; }
    public RegisterGroup GeneralPurposeRegisters { get; set; }

    private void Awake()
    {
        // Ustawienie instancji singletona
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Zapobiega istnieniu wielu instancji
            return;
        }
        Instance = this;

        // Inicjalizacja rejestrów
        InitializeRegisters();
    }

    /// <summary>
    /// Inicjalizuje rejestry.
    /// </summary>
    public void InitializeRegisters()
    {
        // Inicjalizacja indywidualnych rejestrów
        SingleRegisters = new Dictionary<string, Register>
        {
            { "A", new Register("A", 0) },
            { "B", new Register("B", 0) },
            { "C", new Register("C", 0) },
            { "TMP1", new Register("TMP1", 0) },
            { "TMP2", new Register("TMP2", 0) },
            { "PC", new Register("PC", 0) },
            { "MAR", new Register("MAR", 0) },
            { "MDR", new Register("MDR", 0) },
            { "uAR", new Register("uAR", 0) },
            { "IR", new Register("IR", 0) }
        };

        // Inicjalizacja grupy R0-R32
        GeneralPurposeRegisters = new RegisterGroup("R", 32);

    }

    /// <summary>
    /// Pobiera rejestr na podstawie nazwy.
    /// </summary>
    public Register GetRegister(string name)
    {
        if (SingleRegisters.ContainsKey(name))
        {
            return SingleRegisters[name];
        }
        if (name.StartsWith("R"))
        {
            return GeneralPurposeRegisters.GetRegister(name);
        }
        throw new ArgumentException($"Register {name} does not exist.");
    }

    /// <summary>
    /// Ustawia wartoœæ w rejestrze.
    /// </summary>
    public void SetRegisterValue(string name, int value)
    {
        GetRegister(name).Value = value;
    }

    /// <summary>
    /// Pobiera wartoœæ z rejestru.
    /// </summary>
    public int GetRegisterValue(string name)
    {
        return GetRegister(name).Value;
    }

    /// <summary>
    /// Pobiera listê wszystkich nazw rejestrów (indywidualnych i grupowych).
    /// </summary>
    public List<string> GetAllRegisterNames()
    {
        // Pobiera nazwy z indywidualnych rejestrów oraz grupy R
        var singleRegisterNames = SingleRegisters.Keys.ToList();
        var generalRegisterNames = GeneralPurposeRegisters.GetRegisterNames();

        // £¹czy obie listy w jedn¹
        return singleRegisterNames.Concat(generalRegisterNames).ToList();
    }

    /// <summary>
    /// Pobiera ³añcuch znaków opisuj¹cy wszystkie rejestry (na potrzeby debugowania).
    /// </summary>
    public override string ToString()
    {
        var singleRegs = string.Join(", ", SingleRegisters.Values.Select(r => r.ToString()));
        var generalRegs = GeneralPurposeRegisters.ToString();
        return $"{singleRegs}, {generalRegs}";
    }
}
