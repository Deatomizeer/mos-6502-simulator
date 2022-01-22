using System.Collections.Generic;

public class DecrementY : GenericOperation
{
    public DecrementY(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType> { };
    }

    public override void Execute(List<string> codeLine)
    {
        if (codeLine.Count != 1)
        {
            throw new BadOperandCountException("Bad operand count: " + string.Join(" ", codeLine));
        }
        int newY = (sim.memory.register["Y"] - 1 + 256) & 0xFF;
        sim.memory.SetRegisterValue("Y", newY);

        sim.memory.SetFlagValue('Z', newY == 0);
        sim.memory.SetFlagValue('N', newY > 127);
    }
}
