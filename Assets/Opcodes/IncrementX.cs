using System.Collections.Generic;

public class IncrementX : GenericOperation
{
    public IncrementX(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType> { };
        addrModeToOpcodeByte = new Dictionary<OperandType, string>{
            { OperandType.Error, "E8" }
        };
    }

    public override void Execute(List<string> codeLine)
    {
        if (codeLine.Count != 1)
        {
            throw new BadOperandCountException("Bad operand count: " + string.Join(" ", codeLine));
        }
        int newX = (sim.memory.register["X"] + 1) & 0xFF;
        sim.memory.SetRegisterValue("X", newX);

        sim.memory.SetFlagValue('Z', newX == 0);
        sim.memory.SetFlagValue('N', newX > 127);
    }
}
