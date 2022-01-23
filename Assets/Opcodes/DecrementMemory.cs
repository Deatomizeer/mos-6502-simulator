using System.Collections.Generic;

public class DecrementMemory : GenericOperation
{
    public DecrementMemory(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType>{
            OperandType.ZeroPage, OperandType.ZeroPageX,
            OperandType.Absolute, OperandType.AbsoluteX
        };
        addrModeToOpcodeByte = new Dictionary<OperandType, string>{
            { OperandType.ZeroPage, "C6" },
            { OperandType.ZeroPageX, "D6" },
            { OperandType.Absolute, "CE" },
            { OperandType.AbsoluteX, "DE" },
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
        val = (val - 1 + 256) & 0xFF;

        sim.memory.SetFlagValue('Z', val == 0);
        sim.memory.SetFlagValue('N', val > 127);

        // Determine the variable address first, then store the decremented value.
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
