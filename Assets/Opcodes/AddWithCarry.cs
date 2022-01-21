using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddWithCarry : GenericOperation
{
    public AddWithCarry(SimulationState sim) : base(sim) {
        allowedTypes = new List<OperandType>{
            OperandType.Immediate, OperandType.ZeroPage, OperandType.ZeroPageX,
            OperandType.Absolute, OperandType.AbsoluteX, OperandType.AbsoluteY,
            OperandType.IndirectX, OperandType.IndirectY
        };
    }

    public override void Execute(List<string> codeLine)
    {
        if ( codeLine.Count != 2 )
        {
            Debug.LogWarning("Exception on line " + sim.step + ". Bad operand count.");
            return;
        }
        string operand = codeLine[1];
        if ( IsIllegalOperandType(GetOperandType(operand)) )
        {
            Debug.LogWarning("Exception on line " + sim.step + ". Bad operand type.");
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
