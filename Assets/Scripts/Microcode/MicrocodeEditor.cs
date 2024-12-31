using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MicrocodeEditor : MonoBehaviour
{
    public Transform tableParent; // Kontener dla wierszy
    public GameObject rowPrefab; // Prefab wiersza tabeli
    private MicrocodeTable microcodeTable = new MicrocodeTable();

    public void AddRow()
    {
        int address = microcodeTable.GetAllRows().Count();
        var newRow = new MicrocodeRow(address);
        microcodeTable.AddRow(newRow);

        var rowUI = Instantiate(rowPrefab, tableParent);
        var inputs = rowUI.GetComponentsInChildren<InputField>();

        // Powi¹¿ pola z danymi
        inputs[0].text = address.ToString("X4"); // uAR
        inputs[1].onEndEdit.AddListener(value => newRow.Label = value);
        inputs[2].onEndEdit.AddListener(value => newRow.ALU = value);
        inputs[3].onEndEdit.AddListener(value => newRow.S1 = value);
        inputs[4].onEndEdit.AddListener(value => newRow.S2 = value);
        inputs[5].onEndEdit.AddListener(value => newRow.Dest = value);
        inputs[6].onEndEdit.AddListener(value => newRow.ExtIR = value);
        inputs[7].onEndEdit.AddListener(value => newRow.Const = int.TryParse(value, out int result) ? result : (int?)null);
        inputs[8].onEndEdit.AddListener(value => newRow.JCond = value);
        inputs[9].onEndEdit.AddListener(value => newRow.Adr = value);
        inputs[10].onEndEdit.AddListener(value => newRow.Mem = value);
        inputs[11].onEndEdit.AddListener(value => newRow.MAdr = value);
        inputs[12].onEndEdit.AddListener(value => newRow.MDest = value);
        inputs[13].onEndEdit.AddListener(value => newRow.Regs = value);
    }

    public void PrintTable()
    {
        Debug.Log(microcodeTable.ToString());
    }
}
