using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;


// This class contains variables related to the simulation's state: registers, flags and memory.
public class MemoryAndRegisters : MonoBehaviour
{
    // References to display text for all the values.
    public Text acText;
    public Text xText;
    public Text yText;
    public Text pcText;
    public Text flagsText;

    // Processor registers.
    public Dictionary<string, int> register = new Dictionary<string, int>();
    // Random Access Memory.
    int memorySize = 100;
    public sbyte[] memory;
    // Flags register, with each bit corresponding to a specific flag: NV-BDIZC.
    public sbyte flags;

    // Variables for handling memory panel.
    public GameObject memoryViewContent;
    public GameObject cellPrefab;
    public GameObject headerPrefab; // Leftmost column and top row, 
    List<Text> memoryText = new List<Text>();


    void Start()
    {
        // Initialize the accumulator, X, Y, and program count registers.
        register.Add("AC", 0);
        register.Add("X", 0);
        register.Add("Y", 0);
        register.Add("PC", 0);
        // Initialize the RAM (200 address spaces for now).
        memory = new sbyte[memorySize];
        for(int i=0; i<memorySize; i++)
        {
            memory[i] = 0;
        }
        // Initialize the flags.
        flags = 0b00110000;

        // Set the default text for all data.
        acText.text = "AC: " + register["AC"];
        xText.text = "X: " + register["X"];
        yText.text = "Y: " + register["Y"];
        pcText.text = "PC: " + register["PC"];

        // Convert flags to binary format, filling leading zeroes as needed.
        string flagsBinary = Convert.ToString(flags, 2);
        flagsBinary = flagsBinary.PadLeft(8, '0');   // What a nice function!
        flagsText.text = "NV-BDIZC\n" + flagsBinary;

        Transform memoryCellParent = memoryViewContent.GetComponent<Transform>();
        InitializeMemoryGUI(memoryCellParent);

        //SetMemoryValue(0, 42);
    }

    // This method populates the memory table with cells containing data to show to the user.
    // To lower CPU usage, the parent's component is loaded once earlier and passed as an argument.
    public void InitializeMemoryGUI(Transform memoryCellParent)
    {
        int firstAddress = 0;
        int cellsInOneRow = 16;
        int rowsPopulated = 0;
        while( rowsPopulated * cellsInOneRow < memorySize )
        {
            // Place the memory address indicator as the leftmost item.
            GameObject hdr = Instantiate<GameObject>(headerPrefab, memoryCellParent);
            hdr.GetComponentInChildren<Text>().text = (firstAddress + rowsPopulated*cellsInOneRow).ToString();

            // Then populate the row as normal.
            for (int i = 0; i < cellsInOneRow && (rowsPopulated*cellsInOneRow + i < memorySize); i++)
            {
                GameObject btn = Instantiate<GameObject>(cellPrefab, memoryCellParent);
                // Add text component reference for later as well as match the representation with actual data.
                memoryText.Add(btn.GetComponentInChildren<Text>());
                memoryText[memoryText.Count - 1].text = memory[memoryText.Count - 1].ToString();
            }
            rowsPopulated++;
        }
    }

    public int GetMemoryValue(int address)
    {
        return memory[address];
    }

    public void SetMemoryValue(int address, sbyte value)
    {
        memory[address] = value;
        memoryText[address].text = value.ToString();
    }
}
