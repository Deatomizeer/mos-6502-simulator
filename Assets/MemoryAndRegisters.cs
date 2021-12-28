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
        

    }

    public void InitializeMemoryGUI()
    {

    }
}
