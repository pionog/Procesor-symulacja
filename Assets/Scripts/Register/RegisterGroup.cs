using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RegisterGroup
{
    private readonly string prefix;
    private readonly Dictionary<string, Register> registers;

    public RegisterGroup(string prefix, int count)
    {
        this.prefix = prefix;
        registers = new Dictionary<string, Register>();
        registers[$"{prefix}0"] = new Register($"{prefix}0", 0);

        for (int i = 1; i < count; i++)
        {
            string registerName = $"{prefix}{i}";
            registers[registerName] = new Register(registerName);
        }
    }

    public Register GetRegister(string name)
    {
        if (registers.ContainsKey(name))
        {
            return registers[name];
        }
        throw new KeyNotFoundException($"Register {name} not found in group {prefix}.");
    }

    public List<Register> GetAllRegisters()
    {
        return registers.Values.ToList();
    }

    public List<string> GetRegisterNames()
    {
        return registers.Keys.ToList();
    }

    public override string ToString()
    {
        return string.Join(", ", registers.Values.Select(r => r.ToString()));
    }
}
