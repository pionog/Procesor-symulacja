using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Collections;
using UnityEngine;

public class InstructionManager : MonoBehaviour
{
    public static InstructionManager Instance { get; set; }
    private List<string[]> instructionList = new List<string[]>();  // 0 - mnemonic name, 1 - formal registers/const numbers/labels to which is jump, 2 - label
    private List<int[]> registersList = new List<int[]>();  //0 - PC, 1 - IR
    private const int stringsInArray = 3 - 1;
    // Start is called before the first frame update

    public List<string[]> getInstructionList() {
        return instructionList;
    }

    public void setInstructions(List<string[]> instructions) {
        instructionList = instructions;
        int len = instructionList.Count;
        registersList = new List<int[]>();
        for (int i = 0; i < len; i++) {
            registersList.Add(new int[]{i * 4, 0 });
        }
        UpdateIR();
    }

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
            registersList = new List<int[]>();
            DontDestroyOnLoad(gameObject); // Ensure this object persists across scenes
        }
    }
    public void AddInstruction(string[] instruction) {
        Instance.getInstructionList().Add(instruction);
        int originIndex = Instance.getInstructionList().IndexOf(instruction);
        int IR = 0;
        string[] strings = TextParser.SplitText(instruction[1]);
        int[] types = TextParser.AnalyzeWords(strings);

        if (types.Contains(2)) {
            int labelIndex = Array.IndexOf(types, 2);
            IR = CalculateIR(strings[labelIndex], originIndex);
        }
        else if (types.Contains(1))
        {
            int labelIndex = Array.IndexOf(types, 1);
            string numberToParse = strings[labelIndex];
            IR = Int32.Parse(numberToParse.Remove(0, 2), System.Globalization.NumberStyles.HexNumber);
        }
        else if (types.Contains(3))
        {
            int labelIndex = Array.IndexOf(types, 3);
            string numberToParse = strings[labelIndex]; // e.g. "0x0020(R1)"
            string[] parts = numberToParse.Split('('); // [0] = "0x0020", [1] = "R1)"
            IR = Int32.Parse(parts[0].Remove(0, 2), System.Globalization.NumberStyles.HexNumber);
        }
        registersList.Add(new int[] { originIndex * 4, IR });
        //Debug.Log((originIndex * 4).ToString() + ", " +IR.ToString());
    }

    public void RemoveInstruction(string[] instruction) { 
        int index = Instance.getInstructionList().IndexOf(instruction);
        Instance.getInstructionList().Remove(instruction);
        registersList.RemoveAt(index);
        UpdateIR();
    }

    public void RemoveInstructionList(string instruction) {
        Instance.getInstructionList().RemoveAll(i => i[0] == instruction);
        var instructions = Instance.getInstructionList()
            .Where(array => array.Length > 2)
            .Select(array => array[0])
            .ToList();
        int[] indexes = instructions
            .Select((value, index) => new { value, index }) // Sparowanie warto�ci z indeksami
            .Where(pair => pair.value == instruction)       // Filtrujemy elementy, kt�re pasuj� do instruction
            .Select(pair => pair.index)                    // Wybieramy indeksy
            .ToArray();
        foreach (var index in indexes.OrderByDescending(i => i)) // Sortowanie malej�co
        {
            if (index >= 0 && index < registersList.Count) // Sprawdzanie, czy indeks jest w zakresie
            {
                registersList.RemoveAt(index); // Usuni�cie elementu na danym indeksie
            }
        }
        UpdateIR();
    }
    public string[] GetInstruction(int index) { 
        return instructionList[index];
    }


    public void Swap(int first, int second) { 
        if (first == second) return;
        int len = Instance.getInstructionList().Count - 1;
        if (0 > first || first > len) {
            Debug.Log("Pierwszy indeks jest spoza zakresu!");
            throw new Exception($"{first} jest spoza mozliwego zakresu (0-{len})!");
        }
        if (0 > second || second > len)
        {
            Debug.Log("Drugi indeks jest spoza zakresu!");
            throw new Exception($"{second} jest spoza mozliwego zakresu (0-{len})!");
        }
        List<string[]> lista = Instance.getInstructionList();
        string[] temp = lista[second];
        int[] tempInt = registersList[second];
        lista[second] = lista[first];
        lista[first] = temp;
        registersList[second] = registersList[first];
        registersList[first] = tempInt;
        UpdateIR();
        Debug.Log("Pomyslnie zamieniono pozycje na liscie instrukcji.");
    }

    public void UpdateIR()
    {
        if (MemoryManager.Instance == null)
        {
            MemoryManager.Instance = gameObject.AddComponent<MemoryManager>();
        }
        for (int i = 0; i < Instance.getInstructionList().Count; i++)
        {
            string[] strings = TextParser.SplitText(Instance.getInstructionList()[i][1]); // rozdzielanie slow po przecinku
            int[] types = TextParser.AnalyzeWords(strings); // oznaczanie typow poszczegolnych slow
            int IR = 0;
            if (types.Contains(2))
            { // jesli instrukcja zawiera etykiete
                int labelIndex = Array.IndexOf(types, 2); // indeks etykiety w porozdzielanych slowach
                IR = CalculateIR(strings[labelIndex], i); // obliczanie nowego IR
                registersList[i][1] = IR; // akutalizowanie wartosci IR
            }
            else if (types.Contains(1))
            {
                int labelIndex = Array.IndexOf(types, 1);
                string numberToParse = strings[labelIndex];
                IR = Int32.Parse(numberToParse.Remove(0,2), System.Globalization.NumberStyles.HexNumber);
                registersList[i][1] = IR;
            }
            else if (types.Contains(3))
            {
                int labelIndex = Array.IndexOf(types, 3);
                string numberToParse = strings[labelIndex]; // e.g. "0x0020(R1)"
                string[] parts = numberToParse.Split('('); // [0] = "0x0020", [1] = "R1)"
                IR = Int32.Parse(parts[0].Remove(0, 2), System.Globalization.NumberStyles.HexNumber);
                registersList[i][1] = IR;
            }
            MemoryManager.Instance.WriteInt(i * 4, IR);
            //Debug.Log("IR dla indeksu " + i.ToString() + ":\t" + registersList[i][1].ToString());
        }
    }

    public int CalculateIR(string label, int originIndex) {
        var LabelsList = Instance.getInstructionList()
            .Where(array => array.Length > 2)
            .Select(array => array[2])
            .ToList();

        int destinationIndex = LabelsList.IndexOf(label);
        if (destinationIndex == -1)
        {
            Debug.Log("Nie znaleziono takiej etykiety!");
            return 0;
        }
        else {
            //Debug.Log(originIndex.ToString() + " " + destinationIndex.ToString());
            originIndex *= 4;
            destinationIndex *= 4;
            int result = destinationIndex - (originIndex + 4);
            return result;
        }
    }

    public override string ToString()
    {
        string result = "";
        int len = Instance.getInstructionList().Count - 1;
        foreach (string[] instruction in Instance.getInstructionList())
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
            if (instruction != Instance.getInstructionList()[len])
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
