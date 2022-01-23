using System.Collections.Generic;

public class CompareX : GenericOperation
{
    public CompareX(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType>{
            OperandType.Immediate, OperandType.ZeroPage, OperandType.Absolute,
        };
        addrModeToOpcodeByte = new Dictionary<OperandType, string>{
            { OperandType.Immediate, "E0" },
            { OperandType.ZeroPage, "E4" },
            { OperandType.Absolute, "EC" }
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
        int x = sim.memory.register["X"];
        int comparison = (x - val + 256) & 0xFF;

        sim.memory.SetFlagValue('Z', comparison == 0);
        sim.memory.SetFlagValue('C', x >= val);
        sim.memory.SetFlagValue('N', comparison > 127);
    }
}

