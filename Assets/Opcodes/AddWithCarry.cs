using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddWithCarry : GenericOperation
{
    public AddWithCarry(SimulationState sim) : base(sim) { }

    public override void Execute(List<string> codeLine)
    {
        if ( codeLine.Count != 2 )
        {
            Debug.Log("Exception on line " + sim.step + ". Bad operand count.");
            return;
        }

        string operand = codeLine[1];
        int val = OperandToInt(operand);
        int ac = sim.memory.register["AC"];
        int carry = sim.memory.GetFlagValue('C');
        int deref;
        int sum;

        switch (GetOperandType(operand))
        {
            case OperandType.Immediate:
                sum = ac + val + carry;
                break;
            case OperandType.ZeroPage:
            case OperandType.Absolute:
                sum = ac + sim.memory.memory[val] + carry;
                break;
            case OperandType.ZeroPageX:
            case OperandType.AbsoluteX:
                sum = ac + sim.memory.memory[val + sim.memory.register["X"]] + carry;
                break;
            case OperandType.AbsoluteY:
                sum = ac + sim.memory.memory[val + sim.memory.register["Y"]] + carry;
                break;
            case OperandType.IndirectX:
                deref = (
                    sim.memory.memory[(val + sim.memory.register["X"]) & 0xFF]
                    + sim.memory.memory[(val + sim.memory.register["X"] + 1) & 0xFF] << 8
                );
                sum = ac + sim.memory.memory[deref] + carry;
                break;
            case OperandType.IndirectY:
                deref = (
                    sim.memory.memory[val]
                    + sim.memory.memory[val + 1] << 8
                    + sim.memory.register["Y"]
                );
                sum = ac + sim.memory.memory[deref] + carry;
                break;
            default:
                Debug.Log("ADC error " + val + " " + GetOperandType(operand));
                Debug.Log(operand.Length);
                sum = 0;
                break;
        }
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
