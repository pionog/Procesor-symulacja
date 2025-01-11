using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class InstructionManager : MonoBehaviour
{
    public static InstructionManager Instance { get; set; }
    private List<string[]> instructionList = new List<string[]>();  // 0 - mnemonic name, 1 - formal registers/const numbers/labels to which is jump, 2 - label
    private List<int[]> registersList = new List<int[]>();  //0 - PC, 1 - IR
    private const int stringsInArray = 3 - 1;
    // Start is called before the first frame update

    private void Awake()
    {
        // Singleton pattern to ensure only one instance of GameManager exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            instructionList = new List<string[]>();
            DontDestroyOnLoad(gameObject); // Ensure this object persists across scenes
        }
    }
    public void AddInstruction(string[] instruction) { 
        instructionList.Add(instruction);
        int originIndex = instructionList.IndexOf(instruction);
        int IR = 0;
        string[] strings = TextParser.SplitText(instruction[1]);
        int[] types = TextParser.AnalyzeWords(strings);

        if (types.Contains(2)) {
            int labelIndex = Array.IndexOf(types, 2);
            IR = CalculateIR(strings[labelIndex], originIndex);
        }
        registersList.Add(new int[] { originIndex * 4, IR });
    }
    public void RemoveInstruction(string[] instruction) { 
        int index = instructionList.IndexOf(instruction);
        instructionList.Remove(instruction);
        registersList.RemoveAt(index);
        UpdateIR();
    }
    public void RemoveInstructionList(string instruction) { 
        instructionList.RemoveAll(i => i[0] == instruction);
        var instructions = instructionList
            .Where(array => array.Length > 2)
            .Select(array => array[0])
            .ToList();
        int[] indexes = instructions
            .Select((value, index) => new { value, index }) // Sparowanie wartoœci z indeksami
            .Where(pair => pair.value == instruction)       // Filtrujemy elementy, które pasuj¹ do instruction
            .Select(pair => pair.index)                    // Wybieramy indeksy
            .ToArray();
        foreach (var index in indexes.OrderByDescending(i => i)) // Sortowanie malej¹co
        {
            if (index >= 0 && index < registersList.Count) // Sprawdzanie, czy indeks jest w zakresie
            {
                registersList.RemoveAt(index); // Usuniêcie elementu na danym indeksie
            }
        }
        UpdateIR();
    }

    

    public string[] GetInstruction(int index) { 
        return instructionList[index];
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

    public void UpdateIR()
    {
        for (int i = 0; i < instructionList.Count; i++)
        {
            string[] strings = TextParser.SplitText(instructionList[i][1]); // rozdzielanie slow po przecinku
            int[] types = TextParser.AnalyzeWords(strings); // oznaczanie typow poszczegolnych slow
            if (types.Contains(2))
            { // jesli instrukcja zawiera etykiete
                int labelIndex = Array.IndexOf(types, 2); // indeks etykiety w porozdzielanych slowach
                int IR = CalculateIR(strings[labelIndex], i); // obliczanie nowego IR
                registersList[i][1] = IR; // akutalizowanie wartosci IR
            }
        }
    }

    public int CalculateIR(string label, int originIndex) {
        var LabelsList = instructionList
            .Where(array => array.Length > 2)
            .Select(array => array[2])
            .ToList();
        int destinationIndex = LabelsList.IndexOf(label);
        if (destinationIndex != -1)
        {
            return 0;
        }
        else {
            originIndex *= 4;
            destinationIndex *= 4;
            return destinationIndex - (originIndex + 4);
        }
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
