using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationState : MonoBehaviour
{
    // Memory reference to allow opcodes to read and modify it.
    public MemoryAndRegisters memory;
    // Error text reference for printing logs.
    public Text errorLog;
    // User's code, split into singular words for convenience.
    public List<List<string>> processedCode = new List<List<string>>();
    // A branch-to-address map for various purposes (replace labels with raw addresses).
    public Dictionary<string, int> branchToStep = new Dictionary<string, int>();
    // Simulated address of the labels, to compare with the branch opcode and make sure the difference fits in one signed byte.
    public Dictionary<string, int> branchToBytes = new Dictionary<string, int>();

    // An opcode-to-object map for executing code lines.
    public Dictionary<string, GenericOperation> getOperation;

    // A variable similar to PC, it's declared here so that jump operations can alter it.
    public int step = 0;
    // Pretty much PC, but with an offset simulating its place in memory after RAM.
    public int bytesProcessed = 0x300;
    // Set to false only with the BRK opcode.
    public bool running = false;

    // Start is called before the first frame update
    void Start()
    {
        memory = this.GetComponent<MemoryAndRegisters>();
        errorLog = GameObject.Find("ErrorLog").GetComponent<Text>();

        getOperation = new Dictionary<string, GenericOperation>{
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
            { "BVS", new BranchOnOverflowSet(this) },
            { "CLC", new ClearCarry(this) },
            { "CLV", new ClearOverflow(this) },
            { "CMP", new CompareAccumulator(this) },
            { "CPX", new CompareX(this) },
            { "CPY", new CompareY(this) },
            { "DEC", new DecrementMemory(this) },
            { "DEX", new DecrementX(this) },
            { "DEY", new DecrementY(this) },
            { "EOR", new ExclusiveOr(this) },
            { "INC", new IncrementMemory(this) },
            { "INX", new IncrementX(this) },
            { "INY", new IncrementY(this) },
            { "JMP", new JumpToLocation(this) },
            { "JSR", new JumpToSubroutine(this) },
            { "LDA", new LoadAccumulator(this) },
            { "LDX", new LoadX(this) },
            { "LDY", new LoadY(this) },
            { "LSR", new LogicalShiftRight(this) },
            { "NOP", new NoOperation(this) },
            { "ORA", new OrWithAccumulator(this) },
            { "PLA", new PullAccumulatorFromStack(this) },
            { "PHA", new PushAccumulatorOnStack(this) },
            { "RTS", new ReturnFromSubroutine(this) },
            { "ROL", new RotateLeft(this) },
            { "ROR", new RotateRight(this) },
            { "SEC", new SetCarry(this) },
            { "STA", new StoreAccumulator(this) },
            { "STX", new StoreX(this) },
            { "STY", new StoreY(this) },
            { "SBC", new SubtractWithCarry(this) },
            { "TAX", new TransferAToX(this) },
            { "TAY", new TransferAToY(this) },
            { "TSX", new TransferStackToX(this) },
            { "TXA", new TransferXToA(this) },
            { "TXS", new TransferXToStack(this) },
            { "TYA", new TransferYToA(this) }
        };
    }

    public void SimulateStep()
    {
        // If just started, set running to true.
        if(step == 0 && !running)
        {
            running = true;
        }
        if (step < processedCode.Count && running)
        {
            try
            {
                List<string> line = processedCode[step];
                step++;
                bytesProcessed += GenericOperation.LineSizeInBytes(line);
                getOperation[line[0].ToUpper()].Execute(line);
                memory.SetRegisterValue("PC", bytesProcessed);
            }
            catch( Exception ex ) when (
                ex is BadOperandCountException ||
                ex is BadOperandTypeException ||
                ex is BranchOutOfBoundsException ||
                ex is BadJumpAddressException ||
                ex is EmptyStackException)
            {
                errorLog.text = ex.Message;
                running = false;
            }
            catch( KeyNotFoundException )
            {
                errorLog.text = "Unknown opcode: " + processedCode[step-1][0];
                running = false;
            }
            catch( Exception )
            {
                errorLog.text = "An error has occured: " + string.Join(" ", processedCode[step-1]);
                running = false;
            }
        }
    }

    public void RunProgram()
    {
        //Thread th = new Thread(RunProgramThread);
        //th.Start();
        RunProgramThread();
    }

    public void RunProgramThread()
    {
        if(step == 0)
        {
            running = true;
        }
        if(running)
        {
            while (step < processedCode.Count && running)
            {
                SimulateStep();
            }
        }
        else
        {
            running = false;
            //Thread.CurrentThread.Join()
        }
    }
    // Reset the memory as well as the simulation variables to their original state.
    public void ResetSimulation()
    {
        memory.ResetMemory();
        errorLog.text = "";
        step = 0;
        bytesProcessed = 0x300;
        running = false;
    }
}
