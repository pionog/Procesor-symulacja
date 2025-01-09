using UnityEngine;

public class MemoryManager : MonoBehaviour
{
    public int memorySize = 1024;
    private byte[] memory;
    public static MemoryManager Instance;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance of GameManager exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensure this object persists across scenes
            InitializeMemory();
        }
    }

    public void InitializeMemory()
    {
        memory = new byte[memorySize];
        Debug.Log("Pamiêæ zosta³a zainicjalizowana.");
    }

    public int GetMemorySize()
    {
        return memory.Length;
    }

    // Przyk³adowa metoda do zapisu i odczytu
    public void WriteByte(int address, byte value)
    {
        if (address < 0 || address >= memory.Length)
        {
            Debug.LogError("Adres poza zakresem pamiêci!");
            return;
        }
        memory[address] = value;
    }

    public byte ReadByte(int address)
    {
        if (address < 0 || address >= memory.Length)
        {
            Debug.LogError("Adres poza zakresem pamiêci!");
            return 0;
        }
        return memory[address];
    }

    // Zapisuje int do pamiêci (4 bajty)
    public void WriteInt(int address, int value)
    {
        if (address < 0 || address + 3 >= memory.Length)
        {
            Debug.LogError("Adres poza zakresem pamiêci");
            return;
        }

        memory[address] = (byte)(value & 0xFF);           // Bajt 0 (LSB)
        memory[address + 1] = (byte)((value >> 8) & 0xFF); // Bajt 1
        memory[address + 2] = (byte)((value >> 16) & 0xFF); // Bajt 2
        memory[address + 3] = (byte)((value >> 24) & 0xFF); // Bajt 3 (MSB)
    }

    public int ReadInt(int address)
    {
        if (address < 0 || address + 3 >= memory.Length)
        {
            Debug.LogError("Adres poza zakresem pamiêci!");
            return 0;
        }

        // Sk³adanie 4 bajtów w jeden int (little-endian)
        return memory[address] |
               (memory[address + 1] << 8) |
               (memory[address + 2] << 16) |
               (memory[address + 3] << 24);
    }
}
