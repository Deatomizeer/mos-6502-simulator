using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Break : GenericOperation
{
    public Break(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType>{ };
    }

    public override void Execute(List<string> codeLine)
    {
        if (codeLine.Count != 1)
        {
            Debug.LogWarning("Exception on line " + sim.step + ". Bad operand count.");
            return;
        }

        // Since the entire simulation will halt after this, we don't push the address on the stack.
        sim.memory.SetFlagValue('I', true);
        sim.running = false;
    }
}
