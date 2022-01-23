using System.Collections.Generic;

public class LoadAccumulator: GenericOperation
{
    public LoadAccumulator(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType>{
            OperandType.Immediate, OperandType.ZeroPage, OperandType.ZeroPageX,
            OperandType.Absolute, OperandType.AbsoluteX, OperandType.AbsoluteY,
            OperandType.IndirectX, OperandType.IndirectY
        };
        addrModeToOpcodeByte = new Dictionary<OperandType, string>{
            { OperandType.Immediate, "A9" },
            { OperandType.ZeroPage, "A5" },
            { OperandType.ZeroPageX, "B5" },
            { OperandType.Absolute, "AD" },
            { OperandType.AbsoluteX, "BD" },
            { OperandType.AbsoluteY, "B9" },
            { OperandType.IndirectX, "A1" },
            { OperandType.IndirectY, "B1" }
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
        sim.memory.SetRegisterValue("AC", val);

        sim.memory.SetFlagValue('N', val > 127);
        sim.memory.SetFlagValue('Z', val == 0);
    }
}
