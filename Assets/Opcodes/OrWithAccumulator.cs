using System.Collections.Generic;

public class OrWithAccumulator : GenericOperation
{
    public OrWithAccumulator(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType>{
            OperandType.Immediate, OperandType.ZeroPage, OperandType.ZeroPageX,
            OperandType.Absolute, OperandType.AbsoluteX, OperandType.AbsoluteY,
            OperandType.IndirectX, OperandType.IndirectY
        };
        addrModeToOpcodeByte = new Dictionary<OperandType, string>{
            { OperandType.Immediate, "09" },
            { OperandType.ZeroPage, "05" },
            { OperandType.ZeroPageX, "15" },
            { OperandType.Absolute, "0D" },
            { OperandType.AbsoluteX, "1D" },
            { OperandType.AbsoluteY, "19" },
            { OperandType.IndirectX, "01" },
            { OperandType.IndirectY, "11" }
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
        int alternative = val | ac;

        sim.memory.SetFlagValue('N', alternative > 127);
        sim.memory.SetFlagValue('Z', alternative == 0);

        sim.memory.SetRegisterValue("AC", alternative);
    }
}
