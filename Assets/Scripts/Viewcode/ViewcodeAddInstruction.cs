using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ViewcodeAddInstruction : MonoBehaviour
{

    public GameObject InstructionDisplay;

    [SerializeField] private TMP_Dropdown dropdown; 

    public void addNewInstruction(){
        if (dropdown.options.Count > 0)
        {
            InstructionDisplay.GetComponent<ViewcodeManager>().AddNewInstruction(dropdown.options[dropdown.value].text);
        }
        else
        {
            throw new System.Exception("There is no mnemonic that can be added. Create one.");
        }
    }
}
