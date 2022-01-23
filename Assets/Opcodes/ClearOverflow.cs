using System.Collections.Generic;

public class ClearOverflow : GenericOperation
{
    public ClearOverflow(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType> { };
        addrModeToOpcodeByte = new Dictionary<OperandType, string>{
            { OperandType.Error, "B8" }
        };
    }

    public override void Execute(List<string> codeLine)
    {
        if (codeLine.Count != 1)
        {
            throw new BadOperandCountException("Bad operand count: " + string.Join(" ", codeLine));
        }

        sim.memory.SetFlagValue('V', false);
    }
}
