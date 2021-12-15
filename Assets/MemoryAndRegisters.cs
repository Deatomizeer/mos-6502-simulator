using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class 
public class MemoryAndRegisters : MonoBehaviour
{
    // Processor registers
    Dictionary<string, int> register = new Dictionary<string, int>();
    void Start()
    {
        // Initialize the registers
        register.Add("AC", 0);
        register.Add("X", 0);
        register.Add("Y", 0);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
