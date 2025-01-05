using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MicrocodeListManager : MonoBehaviour
{
    public Transform content; // Referencja do Content w ScrollRect
    public GameObject mnemonicButtonPrefab; // Prefab przycisku mnemonika
    public GameObject optionButtonPrefab; // Prefab przycisku opcji
    public MicrocodeManager MicrocodeManager;
    public MicrocodeEditor MicrocodeEditor;
    public TMP_InputField inputField;
    public GameObject Parent;
    public GameObject MicrocodeDetails;

    private List<string> mnemonics = new List<string>(); // Lista mnemoników
    private Dictionary<string, List<GameObject>> activeOptionButtons = new Dictionary<string, List<GameObject>>(); // Mapowanie mnemonik -> przyciski opcji

    void Start()
    {
        // Przyk³adowe mnemoniki
        mnemonics.Add("LOAD");
        mnemonics.Add("STORE");
        mnemonics.Add("ADD");
        mnemonics.Add("SUB");
        mnemonics.Add("STEPW7C");

        RefreshList();
    }

    public void RefreshList()
    {
        // Usuñ istniej¹ce elementy
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        activeOptionButtons.Clear();

        // Dodaj przyciski mnemoników
        foreach (var mnemonic in mnemonics)
        {
            AddMnemonicButton(mnemonic);
        }
    }

    private void AddMnemonicButton(string mnemonic)
    {
        // Tworzenie kontenera dla wiersza
        GameObject rowContainer = new GameObject("RowContainer");
        rowContainer.transform.SetParent(content, false); // false oznacza zachowanie lokalnej skali
        RectTransform rectTransform = rowContainer.AddComponent<RectTransform>();

        // Dopasowanie szerokoœci do Content
        rectTransform.anchorMin = new Vector2(0, 0); // Górny-lewy róg
        rectTransform.anchorMax = new Vector2(1, 1); // Górny-prawy róg
        rectTransform.pivot = new Vector2(0.5f, 1);  // Punkt odniesienia: górny œrodek
        rectTransform.sizeDelta = new Vector2(300, 30); // Wysokoœæ wiersza

        // Dodanie uk³adu poziomego i wymuszanie dopasowania rozmiarów
        HorizontalLayoutGroup rowLayout = rowContainer.AddComponent<HorizontalLayoutGroup>();
        rowLayout.childForceExpandWidth = false; // Brak rozci¹gania szerokoœci dzieci
        rowLayout.childForceExpandHeight = true; // Rozci¹ganie wysokoœci dzieci
        rowLayout.childControlWidth = true;     // Kontrolowanie szerokoœci dzieci
        rowLayout.childControlHeight = true;    // Kontrolowanie wysokoœci dzieci
        rowLayout.childAlignment = TextAnchor.UpperLeft;

        // Tworzenie przycisku mnemonika
        GameObject newButton = Instantiate(mnemonicButtonPrefab, rowContainer.transform);
        TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();
        if (buttonText != null)
        {
            buttonText.text = mnemonic;
        }
        else
        {
            Debug.LogError("Prefab przycisku nie ma komponentu TMP_Text jako dziecko!");
        }

        // Kontener na przyciski opcji
        GameObject optionContainer = new GameObject("OptionContainer");
        optionContainer.transform.SetParent(rowContainer.transform);

        RectTransform optionsTransform = optionContainer.AddComponent<RectTransform>();
        optionsTransform.anchorMin = new Vector2(0, 0); // Górny-lewy róg
        optionsTransform.anchorMax = new Vector2(1, 1); // Górny-prawy róg
        optionsTransform.pivot = new Vector2(0.5f, 1);  // Punkt odniesienia: górny œrodek
        optionsTransform.sizeDelta = new Vector2(200, 30); // Rozmiar opcji mo¿e byæ sta³y lub dynamiczny

        HorizontalLayoutGroup optionsRowLayout = optionContainer.AddComponent<HorizontalLayoutGroup>();
        optionsRowLayout.childForceExpandWidth = false;
        optionsRowLayout.childForceExpandHeight = true;
        optionsRowLayout.childControlWidth = true;
        optionsRowLayout.childControlHeight = true;
        optionsRowLayout.transform.localScale = new Vector3(1, 1, 1);


        // Ukryj kontener opcji na pocz¹tku
        optionContainer.SetActive(false);

        // Obs³uga klikniêcia przycisku mnemonika
        newButton.GetComponent<Button>().onClick.AddListener(() => ToggleOptions(mnemonic, optionContainer));
    }



    private void ToggleOptions(string mnemonic, GameObject optionContainer)
    {
        // Jeœli kontener opcji jest aktywny, ukryj go
        if (optionContainer.activeSelf)
        {
            optionContainer.SetActive(false);
            return;
        }

        // Wyczyœæ poprzednie przyciski opcji
        foreach (Transform child in optionContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // Dodaj przyciski opcji
        GameObject editButton = Instantiate(optionButtonPrefab, optionContainer.transform);

        TMP_Text buttonText = editButton.transform.GetComponentInChildren<TMP_Text>();
        if (buttonText != null)
        {
            Debug.Log("Dodano napis Edycja");
            buttonText.text = "Edytuj";
        }
        else
        {
            Debug.LogError("Prefab przycisku nie ma komponentu TMP_Text jako dziecko!");
        }

        editButton.GetComponent<Button>().onClick.AddListener(() => EditMnemonic(mnemonic));

        GameObject deleteButton = Instantiate(optionButtonPrefab, optionContainer.transform);

        TMP_Text deleteButtonText = deleteButton.transform.GetComponentInChildren<TMP_Text>();
        if (deleteButtonText != null)
        {
            deleteButtonText.text = "Usuñ";
        }
        else
        {
            Debug.LogError("Prefab przycisku nie ma komponentu TMP_Text jako dziecko!");
        }
        deleteButton.GetComponent<Button>().onClick.AddListener(() => RemoveMnemonic(mnemonic));

        // Poka¿ kontener opcji
        optionContainer.SetActive(true);
    }

    public void EditMnemonic(string mnemonic)
    {
        Debug.Log($"Edycja mnemonika: {mnemonic}");
        // Logika edycji
        MicrocodeEditor.microcodeTable = MicrocodeManager.GetMicrocodeTable(mnemonic);
        Debug.Log(MicrocodeEditor.microcodeTable.ToString());
        MicrocodeEditor.LoadMicrocodeTable();
        MicrocodeDetails.SetActive(true);
        Parent.SetActive(false);
    }

    public void RemoveMnemonic(string mnemonic)
    {
        Debug.Log($"Usuwanie mnemonika: {mnemonic}");
        mnemonics.Remove(mnemonic);

        // Usuñ przyciski opcji, jeœli istniej¹
        if (activeOptionButtons.ContainsKey(mnemonic))
        {
            foreach (var optionButton in activeOptionButtons[mnemonic])
            {
                Destroy(optionButton);
            }
            activeOptionButtons.Remove(mnemonic);
        }

        RefreshList();
    }

    public void AddNewMnemonic() {
        string mnemonic = inputField.text;
        if (mnemonic != null)
        {
            MicrocodeManager.AddMicrocodeTable(mnemonic, new MicrocodeTable());
            AddMnemonicButton(mnemonic);
        }
    }

}
