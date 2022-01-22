using System.Collections.Generic;

public class StoreAccumulator : GenericOperation
{
    public StoreAccumulator(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType>{
            OperandType.ZeroPage, OperandType.ZeroPageX,
            OperandType.Absolute, OperandType.AbsoluteX, OperandType.AbsoluteY,
            OperandType.IndirectX, OperandType.IndirectY
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

        int val = sim.memory.register["AC"];

        int addr = OperandToInt(operand);
        
        switch (GetOperandType(operand))
        {
            case OperandType.ZeroPage:
            case OperandType.Absolute:
                break;
            case OperandType.ZeroPageX:
                addr = (addr + sim.memory.register["X"]) & 0xFF;
                break;
            case OperandType.AbsoluteX:
                addr = addr + sim.memory.register["X"];
                break;
            case OperandType.AbsoluteY:
                addr = addr + sim.memory.register["Y"];
                break;
            case OperandType.IndirectX:
                addr = (
                    sim.memory.memory[(addr + sim.memory.register["X"]) & 0xFF]
                    + (sim.memory.memory[(addr + sim.memory.register["X"] + 1) & 0xFF] << 8)
                );
                break;
            case OperandType.IndirectY:
                addr = (
                    sim.memory.memory[addr]
                    + (sim.memory.memory[addr + 1] << 8)
                    + sim.memory.register["Y"]
                );
                break;
        }
        sim.memory.SetMemoryValue(addr, val);
    }
}
