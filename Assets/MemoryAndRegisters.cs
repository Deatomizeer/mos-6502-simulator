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
    public Text stackText;
    public Text flagsText;

    // Processor registers.
    public Dictionary<string, int> register = new Dictionary<string, int> {
        {"AC", 0}, {"X", 0}, {"Y", 0}, {"PC", 0x300}, {"SP", 0x1FF} 
    };
    public Dictionary<string, Text> nameToRegisterUI = new Dictionary<string, Text>();

    // Random Access Memory.
    int memorySize = 0x400;
    public int[] memory;
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

        // Populate the register map.
        nameToRegisterUI.Add("AC", acText);
        nameToRegisterUI.Add("X", xText);
        nameToRegisterUI.Add("Y", yText);
        nameToRegisterUI.Add("PC", pcText);
        nameToRegisterUI.Add("SP", stackText);
        // Set the default text for all data.
        foreach( string reg in nameToRegisterUI.Keys ) {
            SetRegisterValue(reg, register[reg]);
        }


        //SetMemoryValue(0, 42);
        //Debug.Log("Right: " + GetFlagValue('-') + ", Wrong: " + GetFlagValue('N'));
        //SetFlagValue('N', true);
    }

    // This method populates the memory table with cells containing data to show to the user.
    // To lower CPU usage, the parent's component is loaded once earlier and passed as an argument.
    public void InitializeMemoryGUI(Transform memoryCellParent)
    {
        int cellsInOneRow = 16;
        int rowsPopulated = 0;
        int cells = 0;
        while( cells < memorySize )
        {
            // Place the memory address indicator as the leftmost item.
            GameObject hdr = Instantiate<GameObject>(headerPrefab, memoryCellParent);
            hdr.GetComponentInChildren<Text>().text = cells.ToString();

            // Then populate the row as normal.
            for (int i = 0; i < cellsInOneRow && (rowsPopulated*cellsInOneRow + i < memorySize); i++)
            {
                GameObject btn = Instantiate<GameObject>(cellPrefab, memoryCellParent);
                // Add text component reference for later as well as match the representation with actual data.
                memoryText.Add(btn.GetComponentInChildren<Text>());
                memoryText[memoryText.Count - 1].text = memory[memoryText.Count - 1].ToString();
                cells++;
            }
            rowsPopulated++;
        }
    }

    public int GetFlagValue(char flag)
    {
        int index = flagToShift[flag];
        int flagValue = (flags & (1 << index)) >> index;  // Single out the flag bit, then shift it so it's either 1 or 0.
        return flagValue;
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

    public void SetRegisterValue(string reg, int value)
    {
        register[reg] = value;
        if (reg == "PC")
        {
            // The program counter register gets two bytes of memory instead of one.
            nameToRegisterUI[reg].text = reg + $": ${register[reg]:X4}";
        }
        else
        {
            nameToRegisterUI[reg].text = reg + $": ${register[reg]:X2}";
        }
        
    }

    // Push a single byte worth of data on the stack.
    public void PushStack(int val)
    {
        int stackPointer = register["SP"];
        SetMemoryValue(stackPointer, val);
        SetRegisterValue("SP", stackPointer-1);
        // Wrap the stack pointer around.
        if ( stackPointer == 0xFF)
        {
            SetRegisterValue("SP", 0x1FF);
        }
    }

    // Pull one byte from the stack.
    public int PopStack()
    {
        int stackPointer = register["SP"];
        // If the stack is empty, throw an exception.
        if ( stackPointer == 0x1FF )
        {
            throw new EmptyStackException("Trying to pop an empty stack");
        }
        int val = GetMemoryValue(stackPointer+1);
        SetRegisterValue("SP", stackPointer+1);
        return val;
    }

    // Reset memory, registers and flags.
    public void ResetMemory()
    {
        for (int i = 0; i < memorySize; i++)
        {
            SetMemoryValue(i, 0);
        }
        foreach (string reg in new List<string>{"AC", "X", "Y"} )
        {
            SetRegisterValue(reg, 0);
        }
        SetRegisterValue("PC", 0x300);
        SetRegisterValue("SP", 0x1FF);
        flags = 0b00110000;
        RefreshFlagValueUI();
    }
}
