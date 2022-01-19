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

    public int OperandToInt(string operand, OperandType ot)
    {
        int result = 0;
        switch(ot)
        {
            case OperandType.Immediate:
                string numberString = operand.TrimStart('#','$');
                result = int.Parse(numberString, System.Globalization.NumberStyles.HexNumber);
                break;
            default:
                Debug.Log("OperandToInt failed for operand " + operand + "!");
                break;
        }
        return result;
    }
}
