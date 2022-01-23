using System.Collections.Generic;

public class StoreY : GenericOperation
{
    public StoreY(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType>{
            OperandType.ZeroPage, OperandType.ZeroPageY,
            OperandType.Absolute
        };
        addrModeToOpcodeByte = new Dictionary<OperandType, string>{
            { OperandType.ZeroPage, "84" },
            { OperandType.ZeroPageY, "94" },
            { OperandType.Absolute, "8C" }
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

        int val = sim.memory.register["Y"];

        int addr = OperandToInt(operand);

        switch (GetOperandType(operand))
        {
            case OperandType.ZeroPage:
            case OperandType.Absolute:
                break;
            case OperandType.ZeroPageX:
                addr = (addr + sim.memory.register["X"]) & 0xFF;
                break;
        }
        sim.memory.SetMemoryValue(addr, val);
    }
}
