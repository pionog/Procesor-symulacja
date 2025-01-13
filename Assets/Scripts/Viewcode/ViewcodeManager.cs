using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewcodeManager : MonoBehaviour
{
    private List<string[]> instructionList = new List<string[]>();
    public InstructionManager InstructionManager;

    public Transform content; // Referencja do Content w ScrollRect
    public GameObject mnemonicButtonPrefab; // Prefab przycisku mnemonika
    public GameObject InstructionPrefab; // Prefab definiowania instrukcji
    public GameObject optionButtonPrefab; // Prefab przycisku opcji
    public GameObject ViewcodeGlobalManager;
    public TMP_Dropdown dropdown;

    /// <summary>
    /// Pobieranie listy instrukcji
    /// </summary>
    /// <returns>
    /// Lista instrukcji składająca się z tablic stringów
    /// </returns>
    public List<string[]> getInstructionList() {
        return instructionList;
    }

    void OnEnable() {
        List<string> instructionsToDelete = ViewcodeGlobalManager.GetComponent<ViewcodeGlobalManager>().getInstructionsToDelete();

        foreach(var instruction in instructionsToDelete) {
            RemoveAllInstructions(instruction);
        }

        ViewcodeGlobalManager.GetComponent<ViewcodeGlobalManager>().clearInstructionsToDelete();
    }

    /// <summary>
    /// Odświeżanie widoku instrukcji
    /// </summary>
    public void createListOfInstructions()
    {
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        int index = 0;
        

        foreach (var instruction in InstructionManager.Instance.getInstructionList())
        {
            GameObject rowContainer = new GameObject("RowContainer");
            rowContainer.transform.SetParent(content, false);

            RectTransform rowContainerTransform = rowContainer.AddComponent<RectTransform>();
            rowContainerTransform.sizeDelta = new Vector2(500, 30);

            HorizontalLayoutGroup rowLayout = rowContainer.AddComponent<HorizontalLayoutGroup>();
            rowLayout.childControlWidth = false;
            rowLayout.childControlHeight = false;
            rowLayout.childForceExpandWidth = false;
            rowLayout.childForceExpandHeight = false;

            GameObject instructionRow = Instantiate(InstructionPrefab, rowContainer.transform);

            // szuaknie komponentow w prefabie InstructionRow
            TMP_InputField labelInputField = instructionRow.transform.Find("LabelInputField").GetComponent<TMP_InputField>();
            Button mnemonicButton = instructionRow.transform.Find("MnemonicButton").GetComponent<Button>();
            TMP_InputField instructionInputField = instructionRow.transform.Find("InstructionInputField").GetComponent<TMP_InputField>();

            if (labelInputField == null || mnemonicButton == null || instructionInputField == null)
            {
                Debug.LogError("Prefab InstructionRow nie zawiera wszystkich wymaganych komponentów!");
                continue;
            }


            labelInputField.text = $"{instruction[2]}";
            instructionInputField.text = $"{instruction[1]}";

            mnemonicButton.GetComponentInChildren<TMP_Text>().text = $"{instruction[0]}";

            labelInputField.onEndEdit.AddListener(newText =>
            {
                instruction[2] = newText;
            });

            instructionInputField.onEndEdit.AddListener(newText =>
            {
                List<string> list = new List<string>();
                foreach (string[] s in InstructionManager.Instance.getInstructionList()) {
                    list.Add(s[2]);
                }
                newText = TextParser.IndicateErrors(newText, list);
                instruction[1] = newText;
                instructionInputField.text = newText;
                InstructionManager.Instance.UpdateIR();
            });


            GameObject optionContainer = new GameObject("OptionContainer");
            optionContainer.transform.SetParent(rowContainer.transform, false);

            RectTransform optionsTransform = optionContainer.AddComponent<RectTransform>();
            optionsTransform.sizeDelta = new Vector2(200, 30);

            HorizontalLayoutGroup optionsRowLayout = optionContainer.AddComponent<HorizontalLayoutGroup>();
            optionsRowLayout.childForceExpandWidth = false;
            optionsRowLayout.childForceExpandHeight = true;
            optionsRowLayout.childControlWidth = true;
            optionsRowLayout.childControlHeight = true;

            optionContainer.SetActive(false);

            int currentIndex = index;
            mnemonicButton.onClick.AddListener(() => ToggleOptions(instruction, optionContainer, currentIndex));

            index++;
        }
    }

    /// <summary>
    /// Przełączanie widoczności opcji danej instrukcji
    /// </summary>
    /// <param name="instruction">
    /// Instrukcja, której tyczą się dane opcje
    /// </param>
    /// <param name="optionContainer">
    /// Kontener opcji wraz z przyciskami
    /// </param>
    /// <param name="index">
    /// Numer pozycji instrukcji na liście instrukcji
    /// </param>
    private void ToggleOptions(string[] instruction, GameObject optionContainer, int index)
    {
        if (optionContainer.activeSelf){
            optionContainer.SetActive(false);
            return;
        }

        foreach (Transform child in optionContainer.transform){
            Destroy(child.gameObject);
        }

        GameObject deleteButton = Instantiate(optionButtonPrefab, optionContainer.transform);

        TMP_Text deleteButtonText = deleteButton.transform.GetComponentInChildren<TMP_Text>();

        if (deleteButtonText != null){
            deleteButtonText.text = "Usuń";
        }
        else{
            Debug.LogError("Prefab przycisku nie ma komponentu TMP_Text jako dziecko!");
        }

        deleteButton.GetComponent<Button>().onClick.AddListener(() => RemoveInstruction(instruction, optionContainer));

        GameObject upButton = Instantiate(optionButtonPrefab, optionContainer.transform);

        TMP_Text upButtonText = upButton.transform.GetComponentInChildren<TMP_Text>();

        if (upButtonText != null){
            upButtonText.text = "↑";
        }
        else{
            Debug.LogError("Prefab przycisku nie ma komponentu TMP_Text jako dziecko!");
        }

        upButton.GetComponent<Button>().onClick.AddListener(() => pushInstructionUp(index));

        GameObject downButton = Instantiate(optionButtonPrefab, optionContainer.transform);

        TMP_Text downButtonText = downButton.transform.GetComponentInChildren<TMP_Text>();

        if (downButtonText != null){
            downButtonText.text = "↓";
        }
        else{
            Debug.LogError("Prefab przycisku nie ma komponentu TMP_Text jako dziecko!");
        }

        downButton.GetComponent<Button>().onClick.AddListener(() => pushInstructionDown(index));

        optionContainer.SetActive(true);
    }

    /// <summary>
    /// Usuwanie wskazanej instrukcji
    /// </summary>
    /// <param name="instruction">
    /// Usuwana instrukcja
    /// </param>
    /// <param name="optionContainer">
    /// Kontener opcji danej instrukcji (przyciski usuwania i przenoszenia pozycji)
    /// </param>
    public void RemoveInstruction(string[] instruction, GameObject optionContainer)
    {
        Debug.Log($"Usuwanie instrukcji: {instruction}");

        if (InstructionManager.Instance.getInstructionList().Contains(instruction)){
            InstructionManager.Instance.RemoveInstruction(instruction);
        }
        else{
            Debug.LogWarning($"Instrukcja '{instruction}' nie istnieje na li�cie.");
            return;
        }

        Destroy(optionContainer);
        createListOfInstructions();
    }

    /// <summary>
    /// Usuwanie wszystkich wystąpień danego mnemonika na liście instrukcji
    /// </summary>
    /// <param name="mnemonic">
    /// Mnemonik, którego tyczą się usuwane instrukcje
    /// </param>
    public void RemoveAllInstructions(string mnemonic)
    {
        InstructionManager.Instance.RemoveInstructionList(mnemonic);
        dropdown.value = 0; // prevents dropdown value to be out of range
        createListOfInstructions();
    }

    /// <summary>
    /// Dodawanie nowej instrukcji na listę instrukcji
    /// </summary>
    /// <param name="mnemonic">
    /// Mnemonik, którego tyczy się dodawana instrukcja
    /// </param>
    public void AddNewInstruction(string mnemonic)
    {
        string[] newInstruction = new string[] { mnemonic, "", "" };
        InstructionManager.Instance.AddInstruction(newInstruction);
        createListOfInstructions();
    }

    /// <summary>
    /// Przesuwanie w górę instrukcji na liście
    /// </summary>
    /// <param name="index">
    /// Numer indeksu instrukcji na liście instrukcji
    /// </param>
    void pushInstructionUp(int index){
        List<string[]> lista = InstructionManager.Instance.getInstructionList();
        if (index <= 0 || index >= lista.Count)
        {
            Debug.Log("Swap up not possible. Index out of range." + index.ToString());
            return;
        }

        InstructionManager.Instance.Swap(index - 1, index);

        createListOfInstructions();
    }

    /// <summary>
    /// Przesuwanie w dół instrukcji na liście
    /// </summary>
    /// <param name="index">
    /// Numer indeksu instrukcji na liście instrukcji
    /// </param>
    void pushInstructionDown(int index){
        List<string[]> lista = InstructionManager.Instance.getInstructionList();
        if (index < 0 || index >= lista.Count - 1)
        {
            Debug.Log("Swap down not possible. Index out of range.");
            return;
        }

        InstructionManager.Instance.Swap(index, index + 1);

        createListOfInstructions();
    }
}
