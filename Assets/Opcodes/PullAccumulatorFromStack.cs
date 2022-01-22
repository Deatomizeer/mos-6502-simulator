using System.Collections.Generic;

public class PullAccumulatorFromStack : GenericOperation
{
    public PullAccumulatorFromStack(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType> { };
    }

    public override void Execute(List<string> codeLine)
    {
        if (codeLine.Count != 1)
        {
            throw new BadOperandCountException("Bad operand count: " + string.Join(" ", codeLine));
        }

        int val = sim.memory.PopStack();
        sim.memory.SetRegisterValue("AC", val);

        sim.memory.SetFlagValue('N', val > 127);
        sim.memory.SetFlagValue('Z', val == 0);

    }
}
