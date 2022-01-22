using System.Collections.Generic;

public class AndWithAccumulator : GenericOperation
{
    public AndWithAccumulator(SimulationState sim) : base(sim) {
        allowedTypes = new List<OperandType>{
            OperandType.Immediate, OperandType.ZeroPage, OperandType.ZeroPageX,
            OperandType.Absolute, OperandType.AbsoluteX, OperandType.AbsoluteY,
            OperandType.IndirectX, OperandType.IndirectY
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
        int conjunction = val & ac;

        sim.memory.SetFlagValue('N', conjunction > 127);
        sim.memory.SetFlagValue('Z', conjunction == 0);

        sim.memory.SetRegisterValue("AC", conjunction);
    }
}
