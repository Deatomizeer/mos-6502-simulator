using System.Collections.Generic;

public class TransferXToA : GenericOperation
{
    public TransferXToA(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType> { };
        addrModeToOpcodeByte = new Dictionary<OperandType, string>{
            { OperandType.Error, "8A" }
        };
    }

    public override void Execute(List<string> codeLine)
    {
        if (codeLine.Count != 1)
        {
            throw new BadOperandCountException("Bad operand count: " + string.Join(" ", codeLine));
        }

        int x = sim.memory.register["X"];

        sim.memory.SetFlagValue('N', x > 127);
        sim.memory.SetFlagValue('Z', x == 0);

        sim.memory.SetRegisterValue("AC", x);
    }
}
