using System.Collections.Generic;

public class IncrementMemory : GenericOperation
{
    public IncrementMemory(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType>{
            OperandType.ZeroPage, OperandType.ZeroPageX,
            OperandType.Absolute, OperandType.AbsoluteX
        };
    }

    public override void Execute(List<string> codeLine)
    {
        if (codeLine.Count != 2)
        {
            throw new BadOperandCountException("Bad operand count: " + string.Join(" ", codeLine));
        }
        string operand = codeLine[1];
        if (IsIllegalOperandType(GetOperandType(operand)))
        {
            throw new BadOperandTypeException("Bad addressing mode (" + GetOperandType(operand).ToString() + "): " + string.Join(" ", codeLine));
        }

        int val = OperandToReferencedValue(operand);
        val = (val + 1) & 0xFF;

        sim.memory.SetFlagValue('Z', val == 0);
        sim.memory.SetFlagValue('N', val > 127);

        // Determine the variable address first, then store the incremented value.
        int addr = OperandToInt(operand);
        switch (GetOperandType(operand))
        {
            case OperandType.ZeroPage:
            case OperandType.Absolute:
                break;
            case OperandType.ZeroPageX:
                addr = (addr + sim.memory.register["X"]) & 0xFF;
                break;
            case OperandType.AbsoluteX:
                addr = addr + sim.memory.register["X"];
                break;
        }
        sim.memory.SetMemoryValue(addr, val);
    }
}
