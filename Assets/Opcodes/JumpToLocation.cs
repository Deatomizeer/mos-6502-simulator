using System;
using System.Collections.Generic;

public class JumpToLocation : GenericOperation
{
    public JumpToLocation(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType>{
            OperandType.Absolute, OperandType.Indirect, OperandType.Error
        };
        addrModeToOpcodeByte = new Dictionary<OperandType, string>{
            { OperandType.Absolute, "4C" },
            { OperandType.Indirect, "6C" },
            { OperandType.Error, "4C" }
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

        // If the jump address is a label, do it.
        if( GetOperandType(operand) == OperandType.Error )
        {
            int targetStep = sim.branchToStep[operand];
            sim.step = targetStep;
            sim.bytesProcessed = sim.branchToBytes[operand];
            return;
        }
        // Otherwise prepare to find the target code line.
        int dest = 0;
        switch (GetOperandType(operand))
        {
            case OperandType.Absolute:
                dest = OperandToInt(operand);
                break;
            case OperandType.Indirect:
                dest = sim.memory.memory[OperandToInt(operand)]
                    + (sim.memory.memory[OperandToInt(operand)+1] << 8);
                break;
        }

        int currBytes = sim.bytesProcessed;
        int d = Math.Sign(dest - currBytes);    // Jump forward or back.
        // Iterate over code lines until the current byte count matches the destination.
        while( currBytes != dest )
        {
            sim.step += d;
            // Instead of checking if we jumped over the destination,
            // keep iterating and let the step variable fall out of bounds.
            if( sim.step < 0 || sim.step >= sim.processedCode.Count)
            {
                throw new BadJumpAddressException("Incorrect jump address: " + string.Join(" ", codeLine));
            }
            currBytes += d * LineSizeInBytes(sim.processedCode[sim.step]);
        }
        sim.bytesProcessed = currBytes;
    }

}
