using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MicrocodeToggleHandler : MonoBehaviour
{
    [SerializeField] private ToggleGroup toggleGroup; // Odno�nik do Toggle Group

    public string GetSelectedValue()
    {
        // Znajd� aktywny Toggle
        Toggle activeToggle = toggleGroup.ActiveToggles().FirstOrDefault();

        if (activeToggle != null)
        {
            // Pobierz warto�� przypisan� do aktywnego Toggle
            ToggleValue toggleValue = activeToggle.GetComponent<ToggleValue>();
            if (toggleValue != null)
            {
                return toggleValue.value;
            }
            else
            {
                Debug.LogWarning("Toggle nie ma przypisanego skryptu ToggleValue!");
                return null;
            }
        }

        Debug.LogWarning("Nie wybrano �adnego Toggle w grupie!");
        return null;
    }
}
