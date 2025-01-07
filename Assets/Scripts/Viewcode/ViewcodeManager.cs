using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewcodeManager : MonoBehaviour
{
    private List<string> instructionList = new List<string>();

    public Transform content; // Referencja do Content w ScrollRect
    public GameObject mnemonicButtonPrefab; // Prefab przycisku mnemonika
    public GameObject optionButtonPrefab; // Prefab przycisku opcji
    public GameObject ViewcodeGlobalManager;

    void OnEnable() {
        List<string> instructionsToDelete = ViewcodeGlobalManager.GetComponent<ViewcodeGlobalManager>().getInstructionsToDelete();

        foreach(var instruction in instructionsToDelete) {
            RemoveAllInstructions(instruction);
        }

        ViewcodeGlobalManager.GetComponent<ViewcodeGlobalManager>().clearInstructionsToDelete();
    }

    public void createListOfInstructions(){
        for (int i = content.childCount - 1; i >= 0; i--){
            Destroy(content.GetChild(i).gameObject);
        }

        int index = 0;

        foreach(var instruction in instructionList){
            GameObject rowContainer = new GameObject("RowContainer");

            int currentIndex = index;

            rowContainer.transform.SetParent(content, false); // false oznacza zachowanie lokalnej skali

            RectTransform rectTransform = rowContainer.AddComponent<RectTransform>();

            // Dopasowanie szeroko�ci do Content
            rectTransform.anchorMin = new Vector2(0, 0); // G�rny-lewy r�g
            rectTransform.anchorMax = new Vector2(1, 1); // G�rny-prawy r�g
            rectTransform.pivot = new Vector2(0.5f, 1);  // Punkt odniesienia: g�rny �rodek
            rectTransform.sizeDelta = new Vector2(300, 30); // Wysoko�� wiersza

            // Dodanie uk�adu poziomego i wymuszanie dopasowania rozmiar�w
            HorizontalLayoutGroup rowLayout = rowContainer.AddComponent<HorizontalLayoutGroup>();

            rowLayout.childForceExpandWidth = false; // Brak rozci�gania szeroko�ci dzieci
            rowLayout.childForceExpandHeight = true; // Rozci�ganie wysoko�ci dzieci
            rowLayout.childControlWidth = true;     // Kontrolowanie szeroko�ci dzieci
            rowLayout.childControlHeight = true;    // Kontrolowanie wysoko�ci dzieci
            rowLayout.childAlignment = TextAnchor.UpperLeft;

            // Tworzenie przycisku mnemonika
            GameObject newButton = Instantiate(mnemonicButtonPrefab, rowContainer.transform);
            TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();

            if (buttonText != null){
                buttonText.text = instruction;
            }
            else{
                Debug.LogError("Prefab przycisku nie ma komponentu TMP_Text jako dziecko!");
            }

            // Kontener na przyciski opcji
            GameObject optionContainer = new GameObject("OptionContainer");

            optionContainer.transform.SetParent(rowContainer.transform);

            RectTransform optionsTransform = optionContainer.AddComponent<RectTransform>();

            optionsTransform.anchorMin = new Vector2(0, 0); // G�rny-lewy r�g
            optionsTransform.anchorMax = new Vector2(1, 1); // G�rny-prawy r�g
            optionsTransform.pivot = new Vector2(0.5f, 1);  // Punkt odniesienia: g�rny �rodek
            optionsTransform.sizeDelta = new Vector2(200, 30); // Rozmiar opcji mo�e by� sta�y lub dynamiczny

            HorizontalLayoutGroup optionsRowLayout = optionContainer.AddComponent<HorizontalLayoutGroup>();

            optionsRowLayout.childForceExpandWidth = false;
            optionsRowLayout.childForceExpandHeight = true;
            optionsRowLayout.childControlWidth = true;
            optionsRowLayout.childControlHeight = true;
            optionsRowLayout.transform.localScale = new Vector3(1, 1, 1);


            // Ukryj kontener opcji na pocz�tku
            optionContainer.SetActive(false);

            // Obs�uga klikni�cia przycisku mnemonika
            newButton.GetComponent<Button>().onClick.AddListener(() => ToggleOptions(instruction, optionContainer, currentIndex));

            index++;
        }
    }

    private void ToggleOptions(string instruction, GameObject optionContainer, int index)
    {
        // Je�li kontener opcji jest aktywny, ukryj go
        if (optionContainer.activeSelf){
            optionContainer.SetActive(false);
            return;
        }

        // Wyczy�� poprzednie przyciski opcji
        foreach (Transform child in optionContainer.transform){
            Destroy(child.gameObject);
        }

        // Dodaj przyciski opcji

        //Przycisk edycji
        GameObject editButton = Instantiate(optionButtonPrefab, optionContainer.transform);

        TMP_Text buttonText = editButton.transform.GetComponentInChildren<TMP_Text>();

        if (buttonText != null){
            buttonText.text = "Edytuj";
        }
        else{
            Debug.LogError("Prefab przycisku nie ma komponentu TMP_Text jako dziecko!");
        }

        editButton.GetComponent<Button>().onClick.AddListener(() => EditInstructionArguments(instruction));

        //Przycisk usuwania
        GameObject deleteButton = Instantiate(optionButtonPrefab, optionContainer.transform);

        TMP_Text deleteButtonText = deleteButton.transform.GetComponentInChildren<TMP_Text>();

        if (deleteButtonText != null){
            deleteButtonText.text = "Usuń";
        }
        else{
            Debug.LogError("Prefab przycisku nie ma komponentu TMP_Text jako dziecko!");
        }

        deleteButton.GetComponent<Button>().onClick.AddListener(() => RemoveInstruction(instruction, optionContainer));

        //Przycisk shiftu instrukcji w górę
        GameObject upButton = Instantiate(optionButtonPrefab, optionContainer.transform);

        TMP_Text upButtonText = upButton.transform.GetComponentInChildren<TMP_Text>();

        if (upButtonText != null){
            upButtonText.text = "↑";
        }
        else{
            Debug.LogError("Prefab przycisku nie ma komponentu TMP_Text jako dziecko!");
        }

        upButton.GetComponent<Button>().onClick.AddListener(() => pushInstructionUp(index));

        //Przycisk shiftu instrukcji w dół
        GameObject downButton = Instantiate(optionButtonPrefab, optionContainer.transform);

        TMP_Text downButtonText = downButton.transform.GetComponentInChildren<TMP_Text>();

        if (downButtonText != null){
            downButtonText.text = "↓";
        }
        else{
            Debug.LogError("Prefab przycisku nie ma komponentu TMP_Text jako dziecko!");
        }

        downButton.GetComponent<Button>().onClick.AddListener(() => pushInstructionDown(index));

        // Pokaż kontener opcji
        optionContainer.SetActive(true);
    }

    public void EditInstructionArguments(string instruction)
    {
        Debug.Log($"Edycja instrukcji: {instruction}");
        //Logika edycji
        //To już chyba pozostawiam Tobie, edycje argumentów, tak jak mówiłeś? 
    }

    public void RemoveInstruction(string instruction, GameObject optionContainer)
    {
        Debug.Log($"Usuwanie instrukcji: {instruction}");

        // Usuń instrukcję z listy
        if (instructionList.Contains(instruction)){
            instructionList.Remove(instruction);
        }
        else{
            Debug.LogWarning($"Instrukcja '{instruction}' nie istnieje na li�cie.");
            return;
        }

        //Mozna chyba po prostu usuwać ten optionContainer, tak myślę
        Destroy(optionContainer);
        createListOfInstructions();
    }

    public void RemoveAllInstructions(string instruction){
        instructionList.RemoveAll(i => i == instruction);
        createListOfInstructions();
    }

    public void AddNewInstruction(string instruction){
        instructionList.Add(instruction);
        createListOfInstructions();
    }

    void pushInstructionUp(int index){
        if (index <= 0 || index >= instructionList.Count)
        {
            Debug.Log("Swap up not possible. Index out of range." + index.ToString());
            return;
        }

        string temp = instructionList[index];

        instructionList[index] = instructionList[index - 1];
        instructionList[index - 1] = temp;

        createListOfInstructions();
    }

    void pushInstructionDown(int index){
        if (index < 0 || index >= instructionList.Count - 1)
        {
            Debug.Log("Swap down not possible. Index out of range.");
            return;
        }

        string temp = instructionList[index];

        instructionList[index] = instructionList[index + 1];
        instructionList[index + 1] = temp;

        createListOfInstructions();
    }
}
