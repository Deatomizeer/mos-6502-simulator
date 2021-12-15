using System.Collections.Generic;
using UnityEngine;

// This class contains variables related to the simulation's state: registers, flags and memory.
public class MemoryAndRegisters : MonoBehaviour
{
    // Processor registers.
    public Dictionary<string, int> register = new Dictionary<string, int>();
    // Random Access Memory.
    int memorySize = 200;
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
    }
}
