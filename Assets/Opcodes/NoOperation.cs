using System.Collections.Generic;

public class NoOperation : GenericOperation
{
    public NoOperation(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType>{ };
    }

    public override void Execute(List<string> codeLine)
    {
        if (codeLine.Count != 1)
        {
            throw new BadOperandCountException("Bad operand count: " + string.Join(" ", codeLine));
        }

        return;
    }
}
