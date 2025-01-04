using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Register
{
    public string Name { get; private set; }
    public int Value { get; set; }

    public Register(string name, int? initialValue = null)
    {
        Name = name;
        Value = initialValue ?? UnityEngine.Random.Range(0, Int32.MaxValue); // Losowa wartoœæ, np. w zakresie 0–99,999,999
    }

    public override string ToString()
    {
        return $"{Name}: {Value}";
    }
}

