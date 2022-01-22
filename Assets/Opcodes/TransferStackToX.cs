using System.Collections.Generic;

public class TransferStackToX : GenericOperation
{
    public TransferStackToX(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType> { };
    }

    public override void Execute(List<string> codeLine)
    {
        if (codeLine.Count != 1)
        {
            throw new BadOperandCountException("Bad operand count: " + string.Join(" ", codeLine));
        }

        int sp = sim.memory.register["SP"] & 0xFF;
        sim.memory.SetRegisterValue("X", sp);
    }
}
