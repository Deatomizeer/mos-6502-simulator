using System.Collections.Generic;

public class SubtractWithCarry : GenericOperation
{
    public SubtractWithCarry(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType>{
            OperandType.Immediate, OperandType.ZeroPage, OperandType.ZeroPageX,
            OperandType.Absolute, OperandType.AbsoluteX, OperandType.AbsoluteY,
            OperandType.IndirectX, OperandType.IndirectY
        };
        addrModeToOpcodeByte = new Dictionary<OperandType, string>{
            { OperandType.Immediate, "E9" },
            { OperandType.ZeroPage, "E5" },
            { OperandType.ZeroPageX, "F5" },
            { OperandType.Absolute, "ED" },
            { OperandType.AbsoluteX, "FD" },
            { OperandType.AbsoluteY, "F9" },
            { OperandType.IndirectX, "E1" },
            { OperandType.IndirectY, "F1" }
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
        int negatedCarry = sim.memory.GetFlagValue('C') == 1 ? 0 : 1;
        int diff = (ac - val - negatedCarry + 256) & 0xFF;

        // If a borrow occured, the carry flag is cleared.
        sim.memory.SetFlagValue('C', !(ac < val - negatedCarry));

        sim.memory.SetFlagValue('N', diff > 127);
        bool overflow = ((ac < 128 && ac > 0 ) && diff > 127) || (ac > 127 && diff < 128);
        sim.memory.SetFlagValue('V', overflow);
        sim.memory.SetFlagValue('Z', diff == 0);

        sim.memory.SetRegisterValue("AC", diff);
    }
}
