using System.Collections.Generic;

public class AddWithCarry : GenericOperation
{
    public AddWithCarry(SimulationState sim) : base(sim) {
        allowedTypes = new List<OperandType>{
            OperandType.Immediate, OperandType.ZeroPage, OperandType.ZeroPageX,
            OperandType.Absolute, OperandType.AbsoluteX, OperandType.AbsoluteY,
            OperandType.IndirectX, OperandType.IndirectY
        };
        addrModeToOpcodeByte = new Dictionary<OperandType, string>{
            { OperandType.Immediate, "69" },
            { OperandType.ZeroPage, "65" },
            { OperandType.ZeroPageX, "75" },
            { OperandType.Absolute, "6D" },
            { OperandType.AbsoluteX, "7D" },
            { OperandType.AbsoluteY, "79" },
            { OperandType.IndirectX, "61" },
            { OperandType.IndirectY, "71" }
        };
    }

    public override void Execute(List<string> codeLine)
    {
        if ( codeLine.Count != 2 )
        {
            throw new BadOperandCountException("Bad operand count: " + string.Join(" ", codeLine));
        }
        string operand = codeLine[1];
        if ( IsIllegalOperandType(GetOperandType(operand)) )
        {
            throw new BadOperandTypeException("Bad addressing mode (" + GetOperandType(operand).ToString() + "): " + string.Join(" ", codeLine));
        }

        int val = OperandToReferencedValue(operand);
        int ac = sim.memory.register["AC"];
        int carry = sim.memory.GetFlagValue('C');
        int sum = ac + val + carry;

        // Set appropriate flags.
        sim.memory.SetFlagValue('C', sum > 256);
        // Truncate value to fit in one byte.
        sum &= 0xFF;
        sim.memory.SetFlagValue('N', sum > 127);
        bool overflow = (ac < 128 && sum > 127) || (ac > 127 && sum < 128);
        sim.memory.SetFlagValue('V', overflow);
        sim.memory.SetFlagValue('Z', sum == 0);
        // Finally, do what the opcode says.
        sim.memory.SetRegisterValue("AC", sum);
    }
}

