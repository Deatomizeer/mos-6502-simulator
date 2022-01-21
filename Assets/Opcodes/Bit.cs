using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            Debug.LogWarning("Exception on line " + sim.step + ". Bad operand count.");
            return;
        }
        string operand = codeLine[1];
        if (IsIllegalOperandType(GetOperandType(operand)))
        {
            Debug.LogWarning("Exception on line " + sim.step + ". Bad operand type.");
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
