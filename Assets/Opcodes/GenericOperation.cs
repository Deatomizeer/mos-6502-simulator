using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;


public class GenericOperation
{
    public SimulationState sim;

    public GenericOperation(SimulationState sim)
    {
        this.sim = sim;
        Debug.Log("generic constructor");
    }

    public virtual void Execute( List<string> codeLine ) { }

    public OperandType GetOperandType(string operand)
    {
        // U+200B is a zero-width space that the text area appends to the end sometimes.
        operand = operand.Trim('\u200b');
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
}
