using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewcodeGlobalManager : MonoBehaviour
{
    private List<string> instructionsToDelete = new List<string>();

    public void addInstructionToDelete(string instruction) {
        instructionsToDelete.Add(instruction);
    }

    public void clearInstructionsToDelete() {
        instructionsToDelete.Clear();
    }

    public List<string> getInstructionsToDelete() {
        return instructionsToDelete;
    }
}
