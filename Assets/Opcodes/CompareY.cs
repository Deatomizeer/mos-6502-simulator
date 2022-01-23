using System.Collections.Generic;

public class CompareY : GenericOperation
{
    public CompareY(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType>{
            OperandType.Immediate, OperandType.ZeroPage, OperandType.Absolute,
        };
        addrModeToOpcodeByte = new Dictionary<OperandType, string>{
            { OperandType.Immediate, "C0" },
            { OperandType.ZeroPage, "C4" },
            { OperandType.Absolute, "CC" }
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
        int y = sim.memory.register["Y"];
        int comparison = (y - val + 256) & 0xFF;

        sim.memory.SetFlagValue('Z', comparison == 0);
        sim.memory.SetFlagValue('C', y >= val);
        sim.memory.SetFlagValue('N', comparison > 127);
    }
}