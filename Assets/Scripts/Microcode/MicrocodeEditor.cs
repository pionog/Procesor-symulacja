using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MicrocodeEditor : MonoBehaviour
{
    public Transform TableParent; // Kontener dla wierszy
    public GameObject RowPrefab; // Prefab wiersza tabeli
    public GameObject RowAliases;
    public MicrocodeTable MicrocodeTable;
    public GameObject AddNewRowButton;
    public GameObject SaveButton;
    public GameObject ChangeTypeButton;
    private MicrocodeTable tempMicrocodeTable; // Tymczasowa tabela
    public string Mnemonic { get; set; }
    public TMP_Text MnemonicTitle;
    public ToggleGroup mnemonicToggleGroup;
    public TMP_Text mnemonicRegistersNumber;


    public void AddRow()
    {
        int address = tempMicrocodeTable.GetAllRows().Count();
        Debug.Log("Adres lokalny to: " + address);
        var newRow = new MicrocodeRow();
        tempMicrocodeTable.AddRow(newRow);

        var rowUI = Instantiate(RowPrefab, TableParent);
        RectTransform rowRectTransform = rowUI.GetComponent<RectTransform>();
        rowRectTransform.sizeDelta = new Vector2(TableParent.GetComponent<RectTransform>().rect.width, rowRectTransform.sizeDelta.y);

        AddNewRowButton.transform.SetAsLastSibling();


        var Const = rowUI.GetComponentInChildren<TMP_InputField>();
        var dropdowns = rowUI.GetComponentsInChildren<TMP_Dropdown>();

        // Ustaw listenery dla Dropdownów
        dropdowns[0].onValueChanged.AddListener(index => newRow.ALU = dropdowns[0].options[index].text);        //0
        dropdowns[1].onValueChanged.AddListener(index => newRow.S1 = dropdowns[1].options[index].text);         //1
        dropdowns[2].onValueChanged.AddListener(index => newRow.S2 = dropdowns[2].options[index].text);         //2
        dropdowns[3].onValueChanged.AddListener(index => newRow.Dest = dropdowns[3].options[index].text);       //3
                    Const.onEndEdit.AddListener(value => newRow.Const = int.Parse(Const.text));                 //4
        dropdowns[4].onValueChanged.AddListener(index => newRow.JCond = dropdowns[4].options[index].text);      //5
        dropdowns[5].onValueChanged.AddListener(index => newRow.Mem = dropdowns[5].options[index].text);        //6
        dropdowns[6].onValueChanged.AddListener(index => newRow.MAdr = dropdowns[6].options[index].text);       //7
        dropdowns[7].onValueChanged.AddListener(index => newRow.MDest = dropdowns[7].options[index].text);      //8
        dropdowns[8].onValueChanged.AddListener(index => newRow.Regs = dropdowns[8].options[index].text);       //9

    }

    //public void LoadMicrocodeTable()
    //{
    //    // Najpierw usuñ istniej¹ce wiersze w tabeli, jeœli s¹
    //    foreach (Transform child in tableParent)
    //    {
    //        if (child.gameObject != AddNewRowButton) // Nie usuwaj przycisku dodawania
    //        {
    //            Destroy(child.gameObject);
    //        }
    //    }

    //    // Iteruj przez istniej¹ce wiersze w microcodeTable
    //    foreach (var row in microcodeTable.GetAllRows())
    //    {
    //        // Utwórz nowy prefab wiersza
    //        var rowUI = Instantiate(rowPrefab, tableParent);
    //        RectTransform rowRectTransform = rowUI.GetComponent<RectTransform>();
    //        rowRectTransform.sizeDelta = new Vector2(tableParent.GetComponent<RectTransform>().rect.width, rowRectTransform.sizeDelta.y);

    //        // Pobierz komponenty dropdownów i input field
    //        var dropdowns = rowUI.GetComponentsInChildren<TMP_Dropdown>();
    //        var ConstField = rowUI.GetComponentInChildren<TMP_InputField>();

    //        // Ustaw wartoœci dropdownów i pola Const na podstawie danych w obiekcie MicrocodeRow
    //        dropdowns[0].value = dropdowns[0].options.FindIndex(option => option.text == row.ALU);   //0
    //        dropdowns[1].value = dropdowns[1].options.FindIndex(option => option.text == row.S1);    //1
    //        dropdowns[2].value = dropdowns[2].options.FindIndex(option => option.text == row.S2);    //2
    //        dropdowns[3].value = dropdowns[3].options.FindIndex(option => option.text == row.Dest);  //3
    //        ConstField.text = row.Const.HasValue ? row.Const.Value.ToString() : string.Empty;        //4
    //        dropdowns[4].value = dropdowns[4].options.FindIndex(option => option.text == row.JCond); //5
    //        dropdowns[5].value = dropdowns[5].options.FindIndex(option => option.text == row.Mem);   //6
    //        dropdowns[6].value = dropdowns[6].options.FindIndex(option => option.text == row.MAdr);  //7
    //        dropdowns[7].value = dropdowns[7].options.FindIndex(option => option.text == row.MDest); //8
    //        dropdowns[8].value = dropdowns[8].options.FindIndex(option => option.text == row.Regs);  //9

    //        // Dodaj listenery, aby synchronizowaæ UI z obiektem MicrocodeRow
    //        dropdowns[0].onValueChanged.AddListener(index => row.ALU = dropdowns[0].options[index].text);
    //        dropdowns[1].onValueChanged.AddListener(index => row.S1 = dropdowns[1].options[index].text);
    //        dropdowns[2].onValueChanged.AddListener(index => row.S2 = dropdowns[2].options[index].text);
    //        dropdowns[3].onValueChanged.AddListener(index => row.Dest = dropdowns[3].options[index].text);
    //        ConstField.onEndEdit.AddListener(value => row.Const = int.TryParse(value, out int result) ? result : (int?)null);
    //        dropdowns[4].onValueChanged.AddListener(index => row.JCond = dropdowns[4].options[index].text);
    //        dropdowns[5].onValueChanged.AddListener(index => row.Mem = dropdowns[5].options[index].text);
    //        dropdowns[6].onValueChanged.AddListener(index => row.MAdr = dropdowns[6].options[index].text);
    //        dropdowns[7].onValueChanged.AddListener(index => row.MDest = dropdowns[7].options[index].text);
    //        dropdowns[8].onValueChanged.AddListener(index => row.Regs = dropdowns[8].options[index].text);
    //    }

    //    // Upewnij siê, ¿e przycisk dodawania wiersza znajduje siê na koñcu
    //    AddNewRowButton.transform.SetAsLastSibling();
    //}


    public void LoadMicrocodeTable()
    {
        MnemonicTitle.text = Mnemonic;

        // Stwórz kopiê tabeli mikrokodu
        tempMicrocodeTable = new MicrocodeTable();
        foreach (var row in MicrocodeTable.GetAllRows())
        {
            tempMicrocodeTable.AddRow(new MicrocodeRow
            {
                Address = row.Address,
                Label = row.Label,
                ALU = row.ALU,
                S1 = row.S1,
                S2 = row.S2,
                Dest = row.Dest,
                ExtIR = row.ExtIR,
                Const = row.Const,
                JCond = row.JCond,
                Adr = row.Adr,
                Mem = row.Mem,
                MAdr = row.MAdr,
                MDest = row.MDest,
                Regs = row.Regs
            });
        }


        // Wype³nij UI na podstawie tempMicrocodeTable
        foreach (Transform child in TableParent)
        {
            if (child.gameObject != AddNewRowButton)
            {
                Destroy(child.gameObject);
            }
        }
        if (!MicrocodeTable.GetEditable())
        {
            ChangeTypeButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
            SaveButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
            AddNewRowButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
        }
        else {
            ChangeTypeButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
            SaveButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
            AddNewRowButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
        }

        var rowAliases = Instantiate(RowAliases, TableParent);
        RectTransform rowAliasesRectTransform = rowAliases.GetComponent<RectTransform>();
        rowAliasesRectTransform.sizeDelta = new Vector2(TableParent.GetComponent<RectTransform>().rect.width, rowAliasesRectTransform.sizeDelta.y);

        foreach (var row in tempMicrocodeTable.GetAllRows())
        {
            var rowUI = Instantiate(RowPrefab, TableParent);
            RectTransform rowRectTransform = rowUI.GetComponent<RectTransform>();
            rowRectTransform.sizeDelta = new Vector2(TableParent.GetComponent<RectTransform>().rect.width, rowRectTransform.sizeDelta.y);

            var dropdowns = rowUI.GetComponentsInChildren<TMP_Dropdown>();
            var ConstField = rowUI.GetComponentInChildren<TMP_InputField>();

            dropdowns[0].value = dropdowns[0].options.FindIndex(option => option.text == row.ALU);
            dropdowns[1].value = dropdowns[1].options.FindIndex(option => option.text == row.S1);
            dropdowns[2].value = dropdowns[2].options.FindIndex(option => option.text == row.S2);
            dropdowns[3].value = dropdowns[3].options.FindIndex(option => option.text == row.Dest);
            ConstField.text = row.Const.HasValue ? row.Const.Value.ToString() : string.Empty;
            dropdowns[4].value = dropdowns[4].options.FindIndex(option => option.text == row.JCond);
            dropdowns[5].value = dropdowns[5].options.FindIndex(option => option.text == row.Mem);
            dropdowns[6].value = dropdowns[6].options.FindIndex(option => option.text == row.MAdr);
            dropdowns[7].value = dropdowns[7].options.FindIndex(option => option.text == row.MDest);
            dropdowns[8].value = dropdowns[8].options.FindIndex(option => option.text == row.Regs);

            // Synchronizuj dane tylko w tymczasowej tabeli
            dropdowns[0].onValueChanged.AddListener(index => row.ALU = dropdowns[0].options[index].text);
            dropdowns[1].onValueChanged.AddListener(index => row.S1 = dropdowns[1].options[index].text);
            dropdowns[2].onValueChanged.AddListener(index => row.S2 = dropdowns[2].options[index].text);
            dropdowns[3].onValueChanged.AddListener(index => row.Dest = dropdowns[3].options[index].text);
            ConstField.onEndEdit.AddListener(value => row.Const = int.TryParse(value, out int result) ? result : (int?)null);
            dropdowns[4].onValueChanged.AddListener(index => row.JCond = dropdowns[4].options[index].text);
            dropdowns[5].onValueChanged.AddListener(index => row.Mem = dropdowns[5].options[index].text);
            dropdowns[6].onValueChanged.AddListener(index => row.MAdr = dropdowns[6].options[index].text);
            dropdowns[7].onValueChanged.AddListener(index => row.MDest = dropdowns[7].options[index].text);
            dropdowns[8].onValueChanged.AddListener(index => row.Regs = dropdowns[8].options[index].text);
        }

        AddNewRowButton.transform.SetAsLastSibling();
        

        mnemonicRegistersNumber.text = MicrocodeTable.GetRegistersNumber().ToString();
        int toggleNumber = MicrocodeTable.GetMicrocodeType();
        Debug.Log(toggleNumber.ToString());
        SetToggleSelectedValue(toggleNumber.ToString());

    }

    public void SaveChanges()
    {
        MicrocodeTable.Clear();
        int address = 0;
        foreach (var row in tempMicrocodeTable.GetAllRows())
        {
            var currentRow = new MicrocodeRow
            {
                Address = address,
                Label = row.Label,
                ALU = row.ALU,
                S1 = row.S1,
                S2 = row.S2,
                Dest = row.Dest,
                ExtIR = row.ExtIR,
                Const = row.Const,
                JCond = row.JCond,
                Adr = row.Adr,
                Mem = row.Mem,
                MAdr = row.MAdr,
                MDest = row.MDest,
                Regs = row.Regs
            };
            MicrocodeTable.AddRow(currentRow);
            address++;
        }
        PrintTable();
        Debug.Log("Zmiany zosta³y zapisane.");
    }

    public void CancelChanges()
    {
        Debug.Log("Zmiany zosta³y anulowane.");
        // Mo¿esz tutaj dodaæ logikê powrotu do poprzedniego ekranu
    }

    public string GetToggleSelectedValue()
    {
        UnityEngine.UI.Toggle activeToggle = mnemonicToggleGroup.ActiveToggles().FirstOrDefault();

        if (activeToggle != null)
        {
            return activeToggle.name; // U¿yj pola "name" jako wartoœci
        }

        Debug.LogWarning("Nie wybrano ¿adnego Toggle w grupie!");
        return null;
    }

    public void SetToggleSelectedValue(string value)
    {
        // ZnajdŸ Toggle w grupie, który ma nazwê pasuj¹c¹ do wartoœci
        bool changed = false;
        var group = mnemonicToggleGroup.GetComponentsInChildren<UnityEngine.UI.Toggle>();
        foreach (var toggle in group)
        {
            if (toggle.name == value)
            {
                toggle.isOn = true; // Ustaw jako aktywny
                changed = true;
            }
            else toggle.isOn = false;
        }
        if (!changed) {
            group[0].isOn = true;
        }

        Debug.LogWarning($"Nie znaleziono Toggle o nazwie: {value}");
    }

    public void SetMnemonicType() { 
        int type = int.Parse(GetToggleSelectedValue());
        int registersNumber = int.Parse(mnemonicRegistersNumber.text);
        MicrocodeTable.SetRegistersNumber(registersNumber);
        MicrocodeTable.SetMicrocodeType(type);
    }
    public void changeResgistersNumber(bool add) {
        int number = add ? 1 : -1;
        int registersNumber = int.Parse(mnemonicRegistersNumber.text) + number;
        if (0 <= registersNumber && registersNumber < 5)
        {
            mnemonicRegistersNumber.text = (registersNumber).ToString();
        }
    }


    public void PrintTable()
    {
        Debug.Log(MicrocodeTable.ToString());
    }
}
