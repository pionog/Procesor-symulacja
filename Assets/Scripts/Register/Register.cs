using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Register : MonoBehaviour
{
    public string Name { get; private set; }
    public int Value { get; set; }

    public Register(string name, int initialValue = 0)
    {
        Name = name;
        Value = initialValue;
    }

    public override string ToString()
    {
        return $"{Name}: {Value}";
    }
}
