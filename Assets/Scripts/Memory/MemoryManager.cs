using UnityEngine;

public class MemoryManager : MonoBehaviour
{
    public int memorySize = 1024;
    private byte[] memory;

    void Awake()
    {
        InitializeMemory();
    }

    public void InitializeMemory()
    {
        memory = new byte[memorySize];
        Debug.Log("Pami�� zosta�a zainicjalizowana.");
    }

    public int GetMemorySize()
    {
        return memory.Length;
    }

    // Przyk�adowa metoda do zapisu i odczytu
    public void WriteByte(int address, byte value)
    {
        if (address < 0 || address >= memory.Length)
        {
            Debug.LogError("Adres poza zakresem pami�ci!");
            return;
        }
        memory[address] = value;
    }

    public byte ReadByte(int address)
    {
        if (address < 0 || address >= memory.Length)
        {
            Debug.LogError("Adres poza zakresem pami�ci!");
            return 0;
        }
        return memory[address];
    }

    // Zapisuje int do pami�ci (4 bajty)
    public void WriteInt(int address, int value)
    {
        if (address < 0 || address + 3 >= memory.Length)
        {
            Debug.LogError("Adres poza zakresem pami�ci");
            return;
        }

        memory[address] = (byte)(value & 0xFF);           // Bajt 0
        memory[address + 1] = (byte)((value >> 8) & 0xFF); // Bajt 1
        memory[address + 2] = (byte)((value >> 16) & 0xFF); // Bajt 2
        memory[address + 3] = (byte)((value >> 24) & 0xFF); // Bajt 3
    }

    public int ReadInt(int address)
    {
        if (address < 0 || address + 3 >= memory.Length)
        {
            Debug.LogError("Adres poza zakresem pami�ci!");
            return 0;
        }

        // Sk�adanie 4 bajt�w w jeden int (big-endian)
        return (memory[address] << 24) |
               (memory[address + 1] << 16) |
               (memory[address + 2] << 8) |
                memory[address + 3];
    }
}
