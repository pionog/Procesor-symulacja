using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RegisterSet : MonoBehaviour
{
    private Dictionary<string, Register> registers = new Dictionary<string, Register>();

    public RegisterSet(string[] registerNames)
    {
        foreach (var name in registerNames)
        {
            registers[name] = new Register(name);
        }
    }

    public Register GetRegister(string name)
    {
        if (registers.ContainsKey(name))
        {
            return registers[name];
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
        return string.Join(", ", registers.Values.Select(r => r.ToString()));
    }
}
