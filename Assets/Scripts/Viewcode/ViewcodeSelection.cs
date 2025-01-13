using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewBehaviourScript : MonoBehaviour
{
    private List<string> mnemonics = new List<string>(); // Lista mnemonik�w

    public GameObject ScrollView;

    [SerializeField] private TMP_Dropdown dropdown; 

    void Awake() {
        mnemonics = ScrollView.GetComponent<MicrocodeListManager>().getMnemonics();
    }

    //This gets called everytime object is set to active
    void OnEnable() {
        dropdown.options.Clear();

        foreach (var mnemonic in mnemonics.ToList())
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(
                mnemonic, null
            ));
        }

        dropdown.RefreshShownValue();
    }
}
