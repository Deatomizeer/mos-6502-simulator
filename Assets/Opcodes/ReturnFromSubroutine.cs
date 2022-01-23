using System;
using System.Collections.Generic;

public class ReturnFromSubroutine : GenericOperation
{
    public ReturnFromSubroutine(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType>{ };
        addrModeToOpcodeByte = new Dictionary<OperandType, string>{
            { OperandType.Error, "60" }
        };
    }

    public override void Execute(List<string> codeLine)
    {
        if (codeLine.Count != 1)
        {
            throw new BadOperandCountException("Bad operand count: " + string.Join(" ", codeLine));
        }
        // First, leave the current PC on the stack.
        int returnAddressHi = sim.memory.PopStack();
        int returnAddressLo = sim.memory.PopStack();

        // Prepare to find the target code line.
        int dest = (returnAddressHi << 8) + returnAddressLo;
        int currBytes = sim.bytesProcessed;
        int d = Math.Sign(dest - currBytes);    // Jump forward or back.
        // Iterate over code lines until the current byte count matches the destination.
        while (currBytes != dest)
        {
            sim.step += d;
            // Instead of checking if we jumped over the destination,
            // keep iterating and let the step variable fall out of bounds.
            if (sim.step < 0 || sim.step >= sim.processedCode.Count)
            {
                throw new BadJumpAddressException("Incorrect jump address: " + string.Join(" ", codeLine));
            }
            currBytes += d * LineSizeInBytes(sim.processedCode[sim.step]);
        }
        sim.bytesProcessed = currBytes;
    }
}
