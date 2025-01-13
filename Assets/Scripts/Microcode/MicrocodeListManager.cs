using System.Collections.Generic;
using System.Linq;
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
    public GameObject ViewcodeGlobalManager;

    private List<string> mnemonics = new List<string>(); // Lista mnemonik�w
    private Dictionary<string, List<GameObject>> activeOptionButtons = new Dictionary<string, List<GameObject>>(); // Mapowanie mnemonik -> przyciski opcji

    void Start()
    {
        foreach (string mnemonic in MicrocodeManager.Instance.getmicrocodeTables().Keys) {
            mnemonics.Add(mnemonic);
        }
        //// Przyk�adowe mnemoniki

        RefreshList();
    }

    public List<string> getMnemonics() {
        return mnemonics;
    }

    public void RefreshList()
    {
        // Usu� istniej�ce elementy
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        activeOptionButtons.Clear();

        // Dodaj przyciski mnemonik�w
        foreach (var mnemonic in mnemonics.ToList())
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
        newButton.GetComponent<Button>().onClick.AddListener(() => ToggleOptions(mnemonic, optionContainer));

        GameManager.Instance.SetDefinedMnemonics(GameManager.Instance.GetDefinedMnemonics());
        GameManager.Instance.SetActualMnemonics(GameManager.Instance.GetActualMnemonics());
    }



    private void ToggleOptions(string mnemonic, GameObject optionContainer)
    {
        // Je�li kontener opcji jest aktywny, ukryj go
        if (optionContainer.activeSelf)
        {
            optionContainer.SetActive(false);
            return;
        }

        // Wyczy�� poprzednie przyciski opcji
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
        if (!MicrocodeManager.Instance.GetMicrocodeTable(mnemonic).GetRemovable()) {
            deleteButton.GetComponent<Button>().interactable = false;
        }

        TMP_Text deleteButtonText = deleteButton.transform.GetComponentInChildren<TMP_Text>();
        if (deleteButtonText != null)
        {
            deleteButtonText.text = "Usuń";
        }
        else
        {
            Debug.LogError("Prefab przycisku nie ma komponentu TMP_Text jako dziecko!");
        }
        deleteButton.GetComponent<Button>().onClick.AddListener(() => RemoveMnemonic(mnemonic));

        // Poka� kontener opcji
        optionContainer.SetActive(true);
    }

    public void EditMnemonic(string mnemonic)
    {
        Debug.Log($"Edycja mnemonika: {mnemonic}");
        // Logika edycji
        MicrocodeEditor.MicrocodeTable = MicrocodeManager.Instance.GetMicrocodeTable(mnemonic);
        MicrocodeEditor.Mnemonic = mnemonic;
        Debug.Log(MicrocodeEditor.MicrocodeTable.ToString());
        
        MicrocodeDetails.SetActive(true);
        MicrocodeEditor.LoadMicrocodeTable();
        Parent.SetActive(false);
    }

    public void RemoveMnemonic(string mnemonic)
    {
        Debug.Log($"Usuwanie mnemonika: {mnemonic}");


        // Usu� mnemonik z listy
        if (mnemonics.Contains(mnemonic))
        {
            mnemonics.Remove(mnemonic);
        }
        else
        {
            Debug.LogWarning($"Mnemonik '{mnemonic}' nie istnieje na li�cie.");
            return;
        }

        // Usu� przyciski powi�zane z mnemonikiem
        if (activeOptionButtons.ContainsKey(mnemonic))
        {
            Debug.Log($"Usuwanie {activeOptionButtons[mnemonic].Count} przycisk�w powi�zanych z '{mnemonic}'");
            foreach (var button in activeOptionButtons[mnemonic])
            {
                Destroy(button);
            }
            activeOptionButtons.Remove(mnemonic);
        }

        ViewcodeGlobalManager.GetComponent<ViewcodeGlobalManager>().addInstructionToDelete(mnemonic);

        // Zaktualizuj list� w UI
        RefreshList();
    }

    public void AddNewMnemonic() {
        string mnemonic = inputField.text;
        if (mnemonic != null && mnemonic != "")
        {
            MicrocodeTable newMicrocodeTable = new MicrocodeTable();
            MicrocodeRow row = new MicrocodeRow();
            row.Label = mnemonic;
            newMicrocodeTable.AddRow(row);
            MicrocodeManager.Instance.AddMicrocodeTable(mnemonic, newMicrocodeTable);
            AddMnemonicButton(mnemonic);
            mnemonics.Add(mnemonic);
        }
    }

}
