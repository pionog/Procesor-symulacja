using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionManager : MonoBehaviour
{
    private List<string[]> instructionList = new List<string[]>();
    private const int stringsInArray = 3 - 1;
    // Start is called before the first frame update
    private void Awake()
    {
        // Inicjalizacja s³ownika
        instructionList = new List<string[]>();
    }
    public void UpdateInstructionList(List<string[]> instructionList) {
        this.instructionList.Clear();
        this.instructionList = instructionList;
    }
    public void AddInstruction(string[] instruction) { 
        instructionList.Add(instruction);
    }
    public void RemoveInstruction(string[] instruction) { 
        instructionList.Remove(instruction);
    }
    public void RemoveInstructionList(string instruction) { 
        instructionList.RemoveAll(i => i[0] == instruction);
    }

    public void Swap(int first, int second) { 
        if (first == second) return;
        int len = instructionList.Count - 1;
        if (0 > first || first > len) {
            Debug.Log("Pierwszy indeks jest spoza zakresu!");
            throw new Exception($"{first} jest spoza mozliwego zakresu (0-{len})!");
        }
        if (0 > second || second > len)
        {
            Debug.Log("Drugi indeks jest spoza zakresu!");
            throw new Exception($"{second} jest spoza mozliwego zakresu (0-{len})!");
        }
        string[] temp = instructionList[second];
        instructionList[second] = instructionList[first];
        instructionList[first] = temp;
        Debug.Log("Pomyslnie zamieniono pozycje na liscie instrukcji.");
    }

    public override string ToString()
    {
        string result = "";
        int len = instructionList.Count - 1;
        foreach (string[] instruction in instructionList)
        {
            result += "{";
            foreach (string s in instruction) { 
                result += "\"" + s;
                if (s != instruction[stringsInArray]) { 
                    result += "\", ";
                }
                else
                {
                    result += "\"";
                }
            }
            if (instruction != instructionList[len])
            {
                result += "}, ";
            }
            else
            {
                result += "}";
            }
        }
        return base.ToString();
    }
}
