using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MemoryViewer : MonoBehaviour
{
    [SerializeField] private GameObject memoryRowPrefab; // Prefab wiersza pami�ci
    [SerializeField] private Transform content; // Content w Scroll View
    [SerializeField] private MemoryManager memoryManager; // Odno�nik do MemoryManager

    public void DisplayMemory()
    {
        // Usu� poprzednie elementy
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        int totalBytes = memoryManager.GetMemorySize(); // Pobierz rozmiar pami�ci w bajtach
        int bytesPerRow = 32; // Ka�dy wiersz wy�wietla 32 bajty
        int intsPerRow = bytesPerRow / 4; // Liczba int�w na wiersz

        // Przechodzimy przez pami�� i wy�wietlamy kolejne wiersze
        for (int address = 0; address < totalBytes; address += bytesPerRow)
        {
            var rowInstance = Instantiate(memoryRowPrefab, content);

            // Wy�wietl adres wiersza
            var addressButton = rowInstance.transform.Find("Address").GetComponent<Button>();
            addressButton.GetComponentInChildren<TMP_Text>().text = $"{address:X4}";

            // Wy�wietl warto�ci w kom�rkach (po 4 bajty)
            for (int i = 0; i < intsPerRow; i++)
            {
                int byteAddress = address + (i * 4); // Adres pierwszego bajtu dla tego int
                var intButton = rowInstance.transform.Find($"{i}").GetComponent<Button>();

                // Pobierz int z pami�ci i wy�wietl jako liczb� szesnastkow�
                int value = memoryManager.ReadInt(byteAddress);
                intButton.GetComponentInChildren<TMP_Text>().text = value.ToString("X8");
            }
        }
    }
}
