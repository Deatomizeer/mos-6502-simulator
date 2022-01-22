using System.Collections.Generic;

public class LoadX : GenericOperation
{
    public LoadX(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType>{
            OperandType.Immediate, OperandType.ZeroPage, OperandType.ZeroPageY,
            OperandType.Absolute, OperandType.AbsoluteY
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
        sim.memory.SetRegisterValue("X", val);

        sim.memory.SetFlagValue('N', val > 127);
        sim.memory.SetFlagValue('Z', val == 0);
    }
}
