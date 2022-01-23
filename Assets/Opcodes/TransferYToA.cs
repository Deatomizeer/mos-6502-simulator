using System.Collections.Generic;

public class TransferYToA : GenericOperation
{
    public TransferYToA(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType> { };
        addrModeToOpcodeByte = new Dictionary<OperandType, string>{
            { OperandType.Error, "98" }
        };
    }

    public override void Execute(List<string> codeLine)
    {
        if (codeLine.Count != 1)
        {
            throw new BadOperandCountException("Bad operand count: " + string.Join(" ", codeLine));
        }

        int y = sim.memory.register["Y"];

        sim.memory.SetFlagValue('N', y > 127);
        sim.memory.SetFlagValue('Z', y == 0);

        sim.memory.SetRegisterValue("AC", y);
    }
}
