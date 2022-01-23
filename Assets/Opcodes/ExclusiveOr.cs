using System.Collections.Generic;

public class ExclusiveOr : GenericOperation
{
    public ExclusiveOr(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType>{
            OperandType.Immediate, OperandType.ZeroPage, OperandType.ZeroPageX,
            OperandType.Absolute, OperandType.AbsoluteX, OperandType.AbsoluteY,
            OperandType.IndirectX, OperandType.IndirectY
        };
        addrModeToOpcodeByte = new Dictionary<OperandType, string>{
            { OperandType.Immediate, "49" },
            { OperandType.ZeroPage, "45" },
            { OperandType.ZeroPageX, "55" },
            { OperandType.Absolute, "4D" },
            { OperandType.AbsoluteX, "5D" },
            { OperandType.AbsoluteY, "59" },
            { OperandType.IndirectX, "41" },
            { OperandType.IndirectY, "51" }
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
        int product = val ^ ac;

        sim.memory.SetFlagValue('N', product > 127);
        sim.memory.SetFlagValue('Z', product == 0);

        sim.memory.SetRegisterValue("AC", product);
    }
}
