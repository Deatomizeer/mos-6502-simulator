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
    public Dictionary<string, int> register = new Dictionary<string, int> {
        {"AC", 0}, {"X", 0}, {"Y", 0}, {"PC", 0} 
    };
    // Random Access Memory.
    int memorySize = 100;
    private int[] memory;
    // Flags register, with each bit corresponding to a specific flag: NV-BDIZC.
    private int flags;
    private Dictionary<char, int> flagToShift = new Dictionary<char, int>{
        {'N', 7}, {'V', 6}, {'-', 5}, {'B', 4}, {'D', 3}, {'I', 2}, {'Z', 1}, {'C', 0}
    };
    private string flagsTextPrefix = "NV-BDIZC\n";

    // Variables for handling memory panel.
    public GameObject memoryViewContent;
    public GameObject cellPrefab;
    public GameObject headerPrefab;             // Leftmost column and top row.
    List<Text> memoryText = new List<Text>();   // UI text components.


    void Start()
    {
        // Initialize the RAM (200 address spaces for now).
        memory = new int[memorySize];
        for( int i=0; i<memorySize; i++ )
        {
            memory[i] = 0;
        }
        Transform memoryCellParent = memoryViewContent.GetComponent<Transform>();
        InitializeMemoryGUI(memoryCellParent);

        // Initialize the flags and show them on the UI in binary format.
        flags = 0b00110000;
        RefreshFlagValueUI();

        // Set the default text for all data.
        acText.text = "AC: " + register["AC"];
        xText.text = "X: " + register["X"];
        yText.text = "Y: " + register["Y"];
        pcText.text = "PC: " + register["PC"];


        //SetMemoryValue(0, 42);
        //Debug.Log("Right: " + GetFlagValue('-') + ", Wrong: " + GetFlagValue('N'));
        //SetFlagValue('N', true);
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

    public bool GetFlagValue(char flag)
    {
        int index = flagToShift[flag];
        int flagValue = (flags & (1 << index)) >> index;  // Single out the flag bit, then shift it so it's either 1 or 0.
        return flagValue == 1 ? true : false;
    }

    public void SetFlagValue(char flag, bool value)
    {
        int index = flagToShift[flag];
        int switcher = 1 << index;
        if( value )
        {
            // Set flag to 1.
            flags |= switcher;
        }
        else
        {
            // Set flag to 0.
            switcher = ~switcher;   // Binary complement, negates all bits.
            flags &= switcher;
        }
        RefreshFlagValueUI();
    }

    private void RefreshFlagValueUI()
    {
        // Convert flags to binary format, filling leading zeroes as needed.
        string flagsBinary = Convert.ToString(flags, 2);
        flagsBinary = flagsBinary.PadLeft(8, '0');   // What a nice function!
        flagsText.text = flagsTextPrefix + flagsBinary;
    }

    public int GetMemoryValue(int address)
    {
        return memory[address];
    }

    public void SetMemoryValue(int address, int value)
    {
        memory[address] = value;
        memoryText[address].text = value.ToString();
    }
}
