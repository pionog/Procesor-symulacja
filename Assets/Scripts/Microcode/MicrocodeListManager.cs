using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MicrocodeListManager : MonoBehaviour
{
    public Transform content; // Referencja do Content w ScrollRect
    public GameObject mnemonicButtonPrefab; // Prefab przycisku mnemonika
    public GameObject optionButtonPrefab; // Prefab przycisku opcji

    private List<string> mnemonics = new List<string>(); // Lista mnemonik�w
    private Dictionary<string, List<GameObject>> activeOptionButtons = new Dictionary<string, List<GameObject>>(); // Mapowanie mnemonik -> przyciski opcji

    void Start()
    {
        // Przyk�adowe mnemoniki
        mnemonics.Add("LOAD");
        mnemonics.Add("STORE");
        mnemonics.Add("ADD");
        mnemonics.Add("SUB");
        mnemonics.Add("STEPW7C");

        RefreshList();
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
        foreach (var mnemonic in mnemonics)
        {
            AddMnemonicButton(mnemonic);
        }
    }

    private void AddMnemonicButton(string mnemonic)
    {
        // Tworzenie kontenera dla wiersza
        GameObject rowContainer = new GameObject("RowContainer");
        rowContainer.transform.SetParent(content);
        RectTransform rectTransform = rowContainer.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(0, 50); // Ustawienie sta�ego rozmiaru
        rectTransform.anchorMin = new Vector2(0, 1); // G�rny-lewy r�g
        rectTransform.anchorMax = new Vector2(1, 1); // G�rny-prawy r�g
        rectTransform.pivot = new Vector2(0.5f, 1);  // Punkt odniesienia: g�rny �rodek

        HorizontalLayoutGroup rowLayout = rowContainer.AddComponent<HorizontalLayoutGroup>();
        rowLayout.childForceExpandWidth = false;
        rowLayout.childForceExpandHeight = false;
        rowLayout.childControlHeight = false;
        rowLayout.childControlWidth = false;
        rowLayout.childAlignment = TextAnchor.MiddleLeft;

        // Tworzenie przycisku mnemonika
        GameObject newButton = Instantiate(mnemonicButtonPrefab, rowContainer.transform);
        TMP_Text buttonText = newButton.transform.GetComponentInChildren<TMP_Text>();
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
        optionsTransform.sizeDelta = new Vector2(0, 50); // Ustawienie sta�ego rozmiaru
        optionsTransform.anchorMin = new Vector2(0, 1); // G�rny-lewy r�g
        optionsTransform.anchorMax = new Vector2(1, 1); // G�rny-prawy r�g
        optionsTransform.pivot = new Vector2(0.5f, 1);  // Punkt odniesienia: g�rny �rodek

        HorizontalLayoutGroup optionsRowLayout = optionContainer.AddComponent<HorizontalLayoutGroup>(); // Uk�ad opcji poziomo
        optionsRowLayout.childForceExpandWidth = false;
        optionsRowLayout.childForceExpandHeight = false;
        optionsRowLayout.childControlHeight = false;
        optionsRowLayout.childControlWidth = false;
        optionsRowLayout.childAlignment = TextAnchor.MiddleLeft;


        // Ukryj kontener opcji na pocz�tku
        optionContainer.SetActive(false);

        // Obs�uga klikni�cia przycisku mnemonika
        newButton.GetComponent<Button>().onClick.AddListener(() => ToggleOptions(mnemonic, optionContainer));
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

        TMP_Text deleteButtonText = deleteButton.transform.GetComponentInChildren<TMP_Text>();
        if (deleteButtonText != null)
        {
            deleteButtonText.text = "Usu�";
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
    }

    public void RemoveMnemonic(string mnemonic)
    {
        Debug.Log($"Usuwanie mnemonika: {mnemonic}");
        mnemonics.Remove(mnemonic);

        // Usu� przyciski opcji, je�li istniej�
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
}
