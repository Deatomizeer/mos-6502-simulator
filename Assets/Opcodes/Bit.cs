using System.Collections.Generic;

public class Bit : GenericOperation
{
    public Bit(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType>{
            OperandType.ZeroPage, OperandType.Absolute
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
        int m7 = (val & 1 << 7) >> 7;
        int m6 = (val & 1 << 6) >> 6;
        sim.memory.SetFlagValue('N', m7 == 1);
        sim.memory.SetFlagValue('V', m6 == 1);
        int ac = sim.memory.register["AC"];
        int conjunction = val & ac;
        sim.memory.SetFlagValue('Z', conjunction == 0);
    }
}
