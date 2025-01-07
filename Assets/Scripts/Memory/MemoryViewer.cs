using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MemoryViewer : MonoBehaviour
{
    [SerializeField] private GameObject memoryRowPrefab; // Prefab wiersza pamiêci
    [SerializeField] private Transform content; // Content w Scroll View
    [SerializeField] private MemoryManager memoryManager; // Odnoœnik do MemoryManager

    public void DisplayMemory()
    {
        // Usuñ poprzednie elementy
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        int totalBytes = memoryManager.GetMemorySize(); // Pobierz rozmiar pamiêci w bajtach
        int bytesPerRow = 32; // Ka¿dy wiersz wyœwietla 32 bajty
        int intsPerRow = bytesPerRow / 4; // Liczba intów na wiersz

        // Przechodzimy przez pamiêæ i wyœwietlamy kolejne wiersze
        for (int address = 0; address < totalBytes; address += bytesPerRow)
        {
            var rowInstance = Instantiate(memoryRowPrefab, content);

            // Wyœwietl adres wiersza
            var addressButton = rowInstance.transform.Find("Address").GetComponent<Button>();
            addressButton.GetComponentInChildren<TMP_Text>().text = $"{address:X4}";

            // Wyœwietl wartoœci w komórkach (po 4 bajty)
            for (int i = 0; i < intsPerRow; i++)
            {
                int byteAddress = address + (i * 4); // Adres pierwszego bajtu dla tego int
                var intButton = rowInstance.transform.Find($"{i}").GetComponent<Button>();

                // Pobierz int z pamiêci i wyœwietl jako liczbê szesnastkow¹
                int value = memoryManager.ReadInt(byteAddress);
                intButton.GetComponentInChildren<TMP_Text>().text = value.ToString("X8");
            }
        }
    }
}
