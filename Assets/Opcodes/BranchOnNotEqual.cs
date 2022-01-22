using System.Collections.Generic;

public class BranchOnNotEqual : GenericOperation
{
    public BranchOnNotEqual(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType>{
            OperandType.Error
        };
    }

    public override void Execute(List<string> codeLine)
    {
        if (codeLine.Count != 2)
        {
            throw new BadOperandCountException("Bad operand count: " + string.Join(" ", codeLine));
        }
        string targetLabel = codeLine[1];
        OperandType ot = GetOperandType(targetLabel);
        if (IsIllegalOperandType(ot))
        {
            throw new BadOperandTypeException("Bad addressing mode (" + ot.ToString() + "): " + string.Join(" ", codeLine));
        }
        // The branch displacement needs to be anywhere between -128 and 127 or it won't fit into one signed byte.
        int displacement = sim.branchToBytes[targetLabel] - sim.bytesProcessed;
        if (displacement < -128 || displacement > 127)
        {
            throw new BranchOutOfBoundsException("Branch out of bounds: " + string.Join(" ", codeLine));
        }

        int targetStep = sim.branchToStep[targetLabel];
        if (sim.memory.GetFlagValue('Z') == 0)
        {
            // Set both the internal step counter as well as the simulated PC.
            sim.step = targetStep;
            sim.bytesProcessed = sim.branchToBytes[targetLabel];
        }
    }
}