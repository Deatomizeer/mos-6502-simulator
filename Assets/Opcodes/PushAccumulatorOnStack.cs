using System.Collections.Generic;

public class PushAccumulatorOnStack : GenericOperation
{
    public PushAccumulatorOnStack(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType>{ };
        addrModeToOpcodeByte = new Dictionary<OperandType, string>{
            { OperandType.Error, "48" }
        };
    }

    public override void Execute(List<string> codeLine)
    {
        if (codeLine.Count != 1)
        {
            throw new BadOperandCountException("Bad operand count: " + string.Join(" ", codeLine));
        }

        int ac = sim.memory.register["AC"];
        sim.memory.PushStack(ac);

    }
}
