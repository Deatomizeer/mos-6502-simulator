using System;
using System.Collections.Generic;

public class JumpToSubroutine : GenericOperation
{
    public JumpToSubroutine(SimulationState sim) : base(sim)
    {
        allowedTypes = new List<OperandType>{
            OperandType.Absolute, OperandType.Error
        };
        addrModeToOpcodeByte = new Dictionary<OperandType, string>{
            { OperandType.Absolute, "20" },
            { OperandType.Error, "20" }
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
        // First, leave the current PC on the stack.
        int currentAddressLo = sim.bytesProcessed & 0xFF;
        int currentAddressHi = (sim.bytesProcessed & 0xFF00) >> 8;
        // The low byte goes first.
        sim.memory.PushStack(currentAddressLo);
        sim.memory.PushStack(currentAddressHi);

        // If the jump address is a label, do it.
        if (GetOperandType(operand) == OperandType.Error)
        {
            int targetStep = sim.branchToStep[operand];
            sim.step = targetStep;
            sim.bytesProcessed = sim.branchToBytes[operand];
            return;
        }
        // Otherwise prepare to find the target code line.
        int dest = OperandToInt(operand);

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

