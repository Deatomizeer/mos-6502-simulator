using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationState : MonoBehaviour
{
    // Memory reference to allow opcodes to read and modify it.
    public MemoryAndRegisters memory;
    // User's code, split into singular words for convenience.
    public List<List<string>> processedCode = new List<List<string>>();
    // A branch-to-address map for various purposes (replace labels with raw addresses).
    public Dictionary<string, int> branchToAddress = new Dictionary<string, int>();

    // Maybe think of a better name?
    private Dictionary<string, GenericOperation> getOperation;

    // A variable similar to PC, it's declared here so that jump operations can alter it.
    public int step = 0;

    // Start is called before the first frame update
    void Start()
    {
        memory = this.GetComponent<MemoryAndRegisters>();

        getOperation = new Dictionary<string, GenericOperation>{
            { "TEST", new TestOpcode(this) },
            { "ADC", new AddWithCarry(this) }
        };
    }

    public void SimulateStep()
    {
        if (step < processedCode.Count)
        {
            List<string> line = processedCode[step];
            step++;
            getOperation[line[0]].Execute(line);
        }
    } 
}
