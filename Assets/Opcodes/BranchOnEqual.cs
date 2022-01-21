using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchOnEqual : GenericOperation
{
    public BranchOnEqual(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType>{
            OperandType.Error
        };
    }

    public override void Execute(List<string> codeLine)
    {
        if (codeLine.Count != 2)
        {
            Debug.LogWarning("Exception on line " + sim.step + ". Bad operand count.");
            return;
        }
        string targetLabel = codeLine[1];
        OperandType ot = GetOperandType(targetLabel);
        if (IsIllegalOperandType(ot))
        {
            Debug.LogWarning("Exception on line " + sim.step + ". Bad operand type.");
        }
        // The branch displacement needs to be anywhere between -128 and 127 or it won't fit into one signed byte.
        int displacement = sim.branchToBytes[targetLabel] - sim.bytesProcessed;
        if (displacement < -128 || displacement > 127)
        {
            Debug.LogWarning("Exception on line " + sim.step + ". Branch out of bounds.");
        }

        int targetStep = sim.branchToStep[targetLabel];
        if (sim.memory.GetFlagValue('Z') == 1)
        {
            // Set both the internal step counter as well as the simulated PC.
            sim.step = targetStep;
            sim.bytesProcessed = sim.branchToBytes[targetLabel];
        }
    }
}
