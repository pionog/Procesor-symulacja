using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RegisterManager : MonoBehaviour
{
    public Dictionary<string, Register> SingleRegisters { get; private set; }
    public RegisterGroup GeneralPurposeRegisters { get; private set; }

    public RegisterManager()
    {
        // Inicjalizacja indywidualnych rejestrów
        SingleRegisters = new Dictionary<string, Register>
        {
            { "A", new Register("A") },
            { "B", new Register("B") },
            { "C", new Register("C") },
            { "TMP1", new Register("TMP1") },
            { "TMP2", new Register("TMP2") },
            { "PC", new Register("PC") },
            { "MAR", new Register("MAR") },
            { "MDR", new Register("MDR") },
            { "uAR", new Register("uAR") }
        };

        // Inicjalizacja grupy R0-R32
        GeneralPurposeRegisters = new RegisterGroup("R", 33);
    }

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

    public void SetRegisterValue(string name, int value)
    {
        GetRegister(name).Value = value;
    }

    public int GetRegisterValue(string name)
    {
        return GetRegister(name).Value;
    }

    public override string ToString()
    {
        var singleRegs = string.Join(", ", SingleRegisters.Values.Select(r => r.ToString()));
        var generalRegs = GeneralPurposeRegisters.ToString();
        return $"{singleRegs}, {generalRegs}";
    }
}
