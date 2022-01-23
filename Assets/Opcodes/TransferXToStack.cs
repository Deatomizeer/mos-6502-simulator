using System.Collections.Generic;

public class TransferXToStack : GenericOperation
{
    public TransferXToStack(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType>{ };
        addrModeToOpcodeByte = new Dictionary<OperandType, string>{
            { OperandType.Error, "9A" }
        };
    }

    public override void Execute(List<string> codeLine)
    {
        if (codeLine.Count != 1)
        {
            throw new BadOperandCountException("Bad operand count: " + string.Join(" ", codeLine));
        }

        int x = sim.memory.register["X"] | 0x100;
        sim.memory.SetRegisterValue("SP", x);
    }
}
