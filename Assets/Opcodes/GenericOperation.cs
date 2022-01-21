using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;


public class GenericOperation
{
    public SimulationState sim;
    public List<OperandType> allowedTypes;  // For checking if a given opcode can be used in conjunction with a type of operand.

    public GenericOperation(SimulationState sim)
    {
        this.sim = sim;
    }

    public virtual void Execute( List<string> codeLine ) { }

    public static OperandType GetOperandType(string operand)
    {

        if (operand == "A")
        {
            return OperandType.Accumulator;
        }
        else if (Regex.IsMatch(operand, @"^\#\$[0-9A-Fa-f]{2}$"))
        {
            return OperandType.Immediate;
        }
        else if (Regex.IsMatch(operand, @"^\$[0-9A-Fa-f]{2}$"))
        {
            return OperandType.ZeroPage;
        }
        else if (Regex.IsMatch(operand, @"^\$[0-9A-Fa-f]{2},X$"))
        {
            return OperandType.ZeroPageX;
        }
        else if (Regex.IsMatch(operand, @"^\$[0-9A-Fa-f]{2},Y$"))
        {
            return OperandType.ZeroPageY;
        }
        else if (Regex.IsMatch(operand, @"^\$[0-9A-Fa-f]{4}$"))
        {
            return OperandType.Absolute;
        }
        else if (Regex.IsMatch(operand, @"^\$[0-9A-Fa-f]{4},X$"))
        {
            return OperandType.AbsoluteX;
        }
        else if (Regex.IsMatch(operand, @"^\$[0-9A-Fa-f]{4},Y$"))
        {
            return OperandType.AbsoluteY;
        }
        else if (Regex.IsMatch(operand, @"^\(\$[0-9A-Fa-f]{4}\)$"))
        {
            return OperandType.Indirect;
        }
        else if (Regex.IsMatch(operand, @"^\(\$[0-9A-Fa-f]{2},X\)$"))
        {
            return OperandType.IndirectX;
        }
        else if (Regex.IsMatch(operand, @"^\(\$[0-9A-Fa-f]{2}\),Y$"))
        {
            return OperandType.IndirectY;
        }
        else
        {
            return OperandType.Error;
        }
    }

    public bool IsIllegalOperandType(OperandType ot)
    {
        return !allowedTypes.Exists(item => item == ot);
    }

    public string OperandToHexString(string operand)
    {
        string result;

        OperandType ot = GetOperandType(operand);
        switch (ot)
        {
            case OperandType.Immediate:
                result = operand.TrimStart('#', '$');
                break;
            case OperandType.ZeroPage:
            case OperandType.Absolute:
                result = operand.TrimStart('$');
                break;
            case OperandType.ZeroPageX:
            case OperandType.AbsoluteX:
                result = operand.Trim('$', ',', 'X');
                break;
            case OperandType.ZeroPageY:
            case OperandType.AbsoluteY:
                result = operand.Trim('$', ',', 'Y');
                break;
            case OperandType.Indirect:
                result = operand.Trim('(', '$', ')');
                break;
            case OperandType.IndirectX:
                result = operand.Trim('(', '$', ',', 'X', ')');
                break;
            case OperandType.IndirectY:
                result = operand.Trim('(', '$', ',', 'Y', ')');
                break;
            default:
                Debug.Log("OperandToHexString failed for operand " + operand);
                return "Will throw exception later, maybe";
        }
        return result;
    }

    public int OperandToInt(string operand)
    {
        string hexString = OperandToHexString(operand);
        return int.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
    }

    // Return the actual value represented by the operand, for example $0200 will translate to the value stored in that memory byte.
    public int OperandToReferencedValue(string operand)
    {
        OperandType ot = GetOperandType(operand);
        if (ot == OperandType.Accumulator)
        {
            return sim.memory.register["AC"];
        }
        int operandVal = OperandToInt(operand);
        int deref;
        int result;

        switch(ot)
        {
            case OperandType.Immediate:
                result = operandVal;
                break;
            case OperandType.ZeroPage:
            case OperandType.Absolute:
                result = sim.memory.memory[operandVal];
                break;
            case OperandType.ZeroPageX:
                result = sim.memory.memory[(operandVal + sim.memory.register["X"]) & 0xFF];
                break;
            case OperandType.AbsoluteX:
                result = sim.memory.memory[operandVal + sim.memory.register["X"]];
                break;
            case OperandType.ZeroPageY:
                result = sim.memory.memory[(operandVal + sim.memory.register["Y"]) & 0xFF];
                break;
            case OperandType.AbsoluteY:
                result = sim.memory.memory[operandVal + sim.memory.register["Y"]];
                break;
            case OperandType.Indirect:
                deref = sim.memory.memory[operandVal];
                result = sim.memory.memory[deref];
                break;
            case OperandType.IndirectX:
                deref = (
                    sim.memory.memory[(operandVal + sim.memory.register["X"]) & 0xFF]
                    + sim.memory.memory[(operandVal + sim.memory.register["X"] + 1) & 0xFF] << 8
                );
                result = sim.memory.memory[deref];
                break;
            case OperandType.IndirectY:
                deref = (
                    sim.memory.memory[operandVal]
                    + sim.memory.memory[operandVal + 1] << 8
                    + sim.memory.register["Y"]
                );
                result = sim.memory.memory[deref];
                break;
            default:
                // Make it throw an exception later.
                Debug.LogWarning("OperandToReferenced error " + operandVal + " " + GetOperandType(operand));
                Debug.Log(operand.Length);
                result = 0;
                break;
        }
        return result;

    }
}
