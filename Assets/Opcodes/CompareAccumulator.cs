using System.Collections.Generic;

public class CompareAccumulator : GenericOperation
{
    public CompareAccumulator(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType>{
            OperandType.Immediate, OperandType.ZeroPage, OperandType.ZeroPageX,
            OperandType.Absolute, OperandType.AbsoluteX, OperandType.AbsoluteY,
            OperandType.IndirectX, OperandType.IndirectY
        };
        addrModeToOpcodeByte = new Dictionary<OperandType, string>{
            { OperandType.Immediate, "C9" },
            { OperandType.ZeroPage, "C5" },
            { OperandType.ZeroPageX, "D5" },
            { OperandType.Absolute, "CD" },
            { OperandType.AbsoluteX, "DD" },
            { OperandType.AbsoluteY, "D9" },
            { OperandType.IndirectX, "C1" },
            { OperandType.IndirectY, "D1" }
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
        int ac = sim.memory.register["AC"];
        int comparison = (ac - val + 256) & 0xFF;

        sim.memory.SetFlagValue('Z', comparison == 0);
        sim.memory.SetFlagValue('C', ac >= val);
        sim.memory.SetFlagValue('N', comparison > 127);
    }
}
