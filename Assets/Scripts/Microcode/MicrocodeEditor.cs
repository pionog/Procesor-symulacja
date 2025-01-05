using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MicrocodeEditor : MonoBehaviour
{
    public Transform tableParent; // Kontener dla wierszy
    public GameObject rowPrefab; // Prefab wiersza tabeli
    public MicrocodeTable microcodeTable;
    public GameObject AddNewRowButton;

    public void AddRow()
    {
        int address = microcodeTable.GetAllRows().Count();
        Debug.Log("Adres lokalny to: " + address);
        var newRow = gameObject.AddComponent<MicrocodeRow>();
        microcodeTable.AddRow(newRow);

        var rowUI = Instantiate(rowPrefab, tableParent);
        RectTransform rowRectTransform = rowUI.GetComponent<RectTransform>();
        rowRectTransform.sizeDelta = new Vector2(tableParent.GetComponent<RectTransform>().rect.width, rowRectTransform.sizeDelta.y);

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

    public void PrintTable()
    {
        Debug.Log(microcodeTable.ToString());
    }
}
