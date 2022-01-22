using System.Collections.Generic;

public class RotateLeft : GenericOperation
{
    public RotateLeft(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType>{
            OperandType.Accumulator, OperandType.ZeroPage, OperandType.ZeroPageX,
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
        OperandType ot = GetOperandType(operand);
        if (IsIllegalOperandType(ot))
        {
            throw new BadOperandTypeException("Bad addressing mode (" + GetOperandType(operand).ToString() + "): " + string.Join(" ", codeLine));
        }

        int val = OperandToReferencedValue(operand);
        int carry = sim.memory.GetFlagValue('C');

        int shiftedVal = val << 1;
        sim.memory.SetFlagValue('C', shiftedVal > 255);
        shiftedVal &= 0xFF;
        shiftedVal |= carry;

        sim.memory.SetFlagValue('N', shiftedVal > 127);
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
