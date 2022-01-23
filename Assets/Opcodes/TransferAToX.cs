using System.Collections.Generic;

public class TransferAToX : GenericOperation
{
    public TransferAToX(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType>{ };
        addrModeToOpcodeByte = new Dictionary<OperandType, string>{
            { OperandType.Error, "AA" }
        };
    }

    public override void Execute(List<string> codeLine)
    {
        if (codeLine.Count != 1)
        {
            throw new BadOperandCountException("Bad operand count: " + string.Join(" ", codeLine));
        }

        int ac = sim.memory.register["AC"];

        sim.memory.SetFlagValue('N', ac > 127);
        sim.memory.SetFlagValue('Z', ac == 0);

        sim.memory.SetRegisterValue("X", ac);
    }
}
