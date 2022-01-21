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
    public Dictionary<string, int> branchToStep = new Dictionary<string, int>();
    // Simulated address of the labels, to compare with the branch opcode and make sure the difference fits in one signed byte.
    public Dictionary<string, int> branchToBytes = new Dictionary<string, int>();

    // Maybe think of a better name?
    private Dictionary<string, GenericOperation> getOperation;

    // A variable similar to PC, it's declared here so that jump operations can alter it.
    public int step = 0;
    // Pretty much PC, but with an offset simulating its place in memory after RAM.
    public int bytesProcessed = 0x300;
    // Set to false only with the BRK opcode.
    public bool running = true;

    // Start is called before the first frame update
    void Start()
    {
        memory = this.GetComponent<MemoryAndRegisters>();

        getOperation = new Dictionary<string, GenericOperation>{
            { "TEST", new TestOpcode(this) },
            { "ADC", new AddWithCarry(this) },
            { "AND", new AndWithAccumulator(this) },
            { "ASL", new ArithmeticShiftLeft(this) },
            { "BCC", new BranchOnCarryClear(this) },
            { "BCS", new BranchOnCarrySet(this) },
            { "BEQ", new BranchOnEqual(this) },
            { "BIT", new Bit(this) },
            { "BMI", new BranchOnMinus(this) },
            { "BNE", new BranchOnNotEqual(this) },
            { "BPL", new BranchOnPlus(this) },
            { "BRK", new Break(this) },
            { "BVC", new BranchOnOverflowClear(this) },
            { "BVS", new BranchOnOverflowSet(this) }
        };
    }

    public void SimulateStep()
    {
        if (step < processedCode.Count && running)
        {
            List<string> line = processedCode[step];
            step++;
            bytesProcessed += line.Count;
            getOperation[line[0]].Execute(line);
            memory.SetRegisterValue("PC", bytesProcessed);
        }
    } 
}
