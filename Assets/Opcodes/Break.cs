using System.Collections.Generic;

public class Break : GenericOperation
{
    public Break(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType>{ };
        addrModeToOpcodeByte = new Dictionary<OperandType, string>{
            { OperandType.Error, "00" }
        };
    }


    public override void Execute(List<string> codeLine)
    {
        if (codeLine.Count != 1)
        {
            throw new BadOperandCountException("Bad operand count: " + string.Join(" ", codeLine));
        }

        // Since the entire simulation will halt after this, we don't push the address on the stack.
        sim.memory.SetFlagValue('I', true);
        sim.running = false;
    }
}
