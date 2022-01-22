using System.Collections.Generic;

public class StoreX : GenericOperation
{
    public StoreX(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType>{
            OperandType.ZeroPage, OperandType.ZeroPageY,
            OperandType.Absolute
        };
    }

    public override void Execute(List<string> codeLine)
    {
        if (codeLine.Count != 2)
        {
            throw new BadOperandCountException("Bad operand count: " + string.Join(" ", codeLine));
        }
        string operand = codeLine[1];
        if (IsIllegalOperandType(GetOperandType(operand)))
        {
            throw new BadOperandTypeException("Bad addressing mode (" + GetOperandType(operand).ToString() + "): " + string.Join(" ", codeLine));
        }

        int val = sim.memory.register["X"];

        int addr = OperandToInt(operand);

        switch (GetOperandType(operand))
        {
            case OperandType.ZeroPage:
            case OperandType.Absolute:
                break;
            case OperandType.ZeroPageY:
                addr = (addr + sim.memory.register["Y"]) & 0xFF;
                break;
        }
        sim.memory.SetMemoryValue(addr, val);
    }
}
