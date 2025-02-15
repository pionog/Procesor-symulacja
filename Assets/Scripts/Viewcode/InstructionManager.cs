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
    private bool NoMoreLeft = false;
    // Start is called before the first frame update

    public List<string[]> getInstructionList() {
        return instructionList;
    }

    /// <summary>
    /// Ustawianie listy instrukcji
    /// </summary>
    /// <param name="instructions">
    /// Lista instrukcji
    /// </param>
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
    /// <summary>
    /// Ustawianie informacji czy zostały jeszcze instrukcje do wykonania
    /// </summary>
    /// <param name="noMoreLeft">
    /// 
    /// </param>
    public void SetNoMoreLeft(bool noMoreLeft) { 
        NoMoreLeft = noMoreLeft;
    }

    /// <summary>
    /// Uzyskanie informacji czy zostały jeszcze instrukcje do wykonania
    /// </summary>
    /// <returns>
    /// <c>bool</c> informacja o tym, czy zostały jeszcze instrukcje do wykonania
    /// </returns>
    public bool GetNoMoreLeft() { 
        return NoMoreLeft;
    }

    /// <summary>
    /// Dodawanie nowej instrukcji na liście instrukcji
    /// </summary>
    /// <param name="instruction">
    /// Instrukcja do dodania na listę instrukcji
    /// </param>
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
        GameManager.Instance.SetActualInstructions(GameManager.Instance.GetActualInstructions());
        GameManager.Instance.SetDefinedInstructions(GameManager.Instance.GetDefinedInstructions());
    }

    /// <summary>
    /// Usuwanie wskazanej instrukcji
    /// </summary>
    /// <param name="instruction">
    /// Instrukcja do usunięcia z listy instrukcji
    /// </param>
    public void RemoveInstruction(string[] instruction) { 
        int index = Instance.getInstructionList().IndexOf(instruction);
        Instance.getInstructionList().Remove(instruction);
        registersList.RemoveAt(index);
        UpdateIR();
        GameManager.Instance.SetActualInstructions(instructionList.Count);
    }

    /// <summary>
    /// Usuwanie wszystkich instrukcji związanych z danym mnemonikiem
    /// </summary>
    /// <param name="mnemonic">
    /// Mnemonik, którego tyczą się dane instrukcje
    /// </param>
    public void RemoveInstructionList(string mnemonic) {
        Instance.getInstructionList().RemoveAll(i => i[0] == mnemonic);
        var instructions = Instance.getInstructionList()
            .Where(array => array.Length > 2)
            .Select(array => array[0])
            .ToList();
        int[] indexes = instructions
            .Select((value, index) => new { value, index }) // Sparowanie warto�ci z indeksami
            .Where(pair => pair.value == mnemonic)       // Filtrujemy elementy, kt�re pasuj� do instruction
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
        GameManager.Instance.SetActualInstructions(instructionList.Count);
    }
    /// <summary>
    /// Pobieranie instrukcji na podstawie jej pozycji na liście instrukcji
    /// </summary>
    /// <param name="index">
    /// Numer indeksu szukanej instrukcji
    /// </param>
    /// <returns>
    /// <c>string[]</c> Wskazana instrukcja
    /// </returns>
    public string[] GetInstruction(int index)
    {
        try
        {
            return instructionList[index];
        }
        catch {
            NoMoreLeft = true;
            return null;
        }
    }

    /// <summary>
    /// Zmienianie pozycjami poszczególnych instrukcji
    /// </summary>
    /// <param name="first">
    /// Numer indeksu pierwszej instrukcji na liście instrukcji
    /// </param>
    /// <param name="second">
    /// Numer indeksu drugiej instrukcji na liście instrukcji
    /// </param>
    /// <exception cref="Exception">
    /// Numer indeksu wychodzący poza możliwy obszar na liście
    /// </exception>
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

    /// <summary>
    /// Aktualizowanie wartości IR dla każdej instrukcji na liście instrukcji
    /// </summary>
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
                bool parsed = int.TryParse(numberToParse, out IR); // decimal number
                if (!parsed)
                {
                    IR = Int32.Parse(numberToParse.Remove(0, 2), System.Globalization.NumberStyles.HexNumber); // hex number
                }
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

    /// <summary>
    /// Obliczanie wartości potrzebnej do prawidłowego rozpoznania instrukcji przez procesor
    /// </summary>
    /// <param name="label">
    /// Etykieta instrukcji, do której ma nastąpić skok
    /// </param>
    /// <param name="originIndex">
    /// Numer indeksu instrukcji na liście instrukcji, od której ma zostać wykonany skok i dla której obliczana jest wartość
    /// </param>
    /// <returns>
    /// <c>int</c> wartość IR
    /// </returns>
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

    /// <summary>
    /// Reprezentacja tekstowa poszczególnych instrukcji
    /// </summary>
    /// <returns>
    /// <c>string</c> z wypisanymi po kolei instrukcjami
    /// </returns>
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
