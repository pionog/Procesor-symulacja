using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RegisterGroup : MonoBehaviour
{
    private Dictionary<string, Register> registers = new Dictionary<string, Register>();

    public RegisterGroup(string groupName, int count)
    {
        for (int i = 0; i < count; i++)
        {
            string registerName = $"{groupName}{i}";
            registers[registerName] = new Register(registerName);
        }
    }

    public Register GetRegister(string name)
    {
        if (registers.ContainsKey(name))
        {
            return registers[name];
        }
        throw new ArgumentException($"Register {name} does not exist in this group.");
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

