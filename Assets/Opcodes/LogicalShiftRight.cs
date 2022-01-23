using System.Collections.Generic;

public class LogicalShiftRight : GenericOperation
{
    public LogicalShiftRight(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType>{
            OperandType.Accumulator, OperandType.ZeroPage, OperandType.ZeroPageX,
            OperandType.Absolute, OperandType.AbsoluteX
        };
        addrModeToOpcodeByte = new Dictionary<OperandType, string>{
            { OperandType.Accumulator, "4A" },
            { OperandType.ZeroPage, "46" },
            { OperandType.ZeroPageX, "56" },
            { OperandType.Absolute, "4E" },
            { OperandType.AbsoluteX, "5E" }
        };
    }

    public override void Execute(List<string> codeLine)
    {
        if (codeLine.Count != 2)
        {
            throw new BadOperandCountException("Bad operand count: " + string.Join(" ", codeLine));
        }
        string operand = codeLine[1];
        OperandType ot = GetOperandType(operand);
        if (IsIllegalOperandType(ot))
        {
            throw new BadOperandTypeException("Bad addressing mode (" + GetOperandType(operand).ToString() + "): " + string.Join(" ", codeLine));
        }

        int val = OperandToReferencedValue(operand);
        sim.memory.SetFlagValue('C', (val & 1) == 1);

        int shiftedVal = val >> 1;
        sim.memory.SetFlagValue('N', false);    // Since leftmost bit is guaranteed to be 0.
        sim.memory.SetFlagValue('Z', shiftedVal == 0);

        switch (ot)
        {
            case OperandType.Accumulator:
                sim.memory.SetRegisterValue("AC", shiftedVal);
                break;
            case OperandType.ZeroPage:
            case OperandType.Absolute:
                sim.memory.SetMemoryValue(OperandToInt(operand), shiftedVal);
                break;
            case OperandType.ZeroPageX:
                sim.memory.SetMemoryValue((OperandToInt(operand) + sim.memory.register["X"]) & 0xFF, shiftedVal);
                break;
            case OperandType.AbsoluteX:
                sim.memory.SetMemoryValue(OperandToInt(operand) + sim.memory.register["X"], shiftedVal);
                break;
        }

    }
}
