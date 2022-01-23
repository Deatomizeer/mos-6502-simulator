using System.Collections.Generic;

public class SetCarry : GenericOperation
{
    public SetCarry(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType> { };
        addrModeToOpcodeByte = new Dictionary<OperandType, string>{
            { OperandType.Error, "38" }
        };
    }

    public override void Execute(List<string> codeLine)
    {
        if (codeLine.Count != 1)
        {
            throw new BadOperandCountException("Bad operand count: " + string.Join(" ", codeLine));
        }

        sim.memory.SetFlagValue('C', true);
    }
}
