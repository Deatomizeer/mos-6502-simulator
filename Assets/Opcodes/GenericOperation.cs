using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;


public class GenericOperation
{
    public SimulationState sim;
    public List<OperandType> allowedTypes;  // For checking if a given opcode can be used in conjunction with a type of operand.
    // The byte representing opcode depends on the operand's addressing mode.
    // To simplify the algorithm, implied addressing type will have a {OperandType.Error, byte} pair.
    public Dictionary<OperandType, string> addrModeToOpcodeByte;

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
                    + (sim.memory.memory[(operandVal + sim.memory.register["X"] + 1) & 0xFF] << 8)
                );
                result = sim.memory.memory[deref];
                break;
            case OperandType.IndirectY:
                deref = (
                    sim.memory.memory[operandVal]
                    + (sim.memory.memory[operandVal + 1] << 8)
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

    // Determine how many bytes does the instruction and operand take.
    public static int LineSizeInBytes(List<string> line)
    {
        List<OperandType> twoByteOperands = new List<OperandType>
        {
            OperandType.Absolute, OperandType.AbsoluteX, OperandType.AbsoluteY,
            OperandType.Indirect, OperandType.Error
        };

        if (line.Count == 1)
        {
            // The opcode takes one byte and takes no operand.
            return 1;
        }
        else
        {
            OperandType ot = GetOperandType(line[1]);
            // Accumulator counts as an implicit target in machine code.
            if (ot == OperandType.Accumulator)
            {
                return 1;
            }
            else if (twoByteOperands.Exists(item => item == ot))
            {
                return 3;
            }
            // Most operands are one byte long.
            else
            {
                return 2;
            }
        }

    }

    public string LineToMachineCode(List<string> line)
    {
        string machineCode;
        // The first byte is determined by the opcode.
        // If the addressing mode is implied, simply return the opcode's value.
        if( line.Count == 1 || (line.Count == 2 && GetOperandType(line[1]) == OperandType.Accumulator))
        {
            machineCode = addrModeToOpcodeByte[OperandType.Error];
        }
        else
        // Determine the machine code for the opcode and operand.
        {
            line[0] = line[0].ToUpper();
            OperandType ot = GetOperandType(line[1]);
            // If the operand is a raw value, the conversion is easy enough.
            if( ot != OperandType.Error )
            {
                machineCode = addrModeToOpcodeByte[ot];
                machineCode = string.Concat(machineCode, " ", OperandToMachineCode(line[1]));
            }
            // If it's a label, the machine code will vary depending on the instruction.
            else
            {
                int targetInt = sim.branchToBytes[line[1]];
                // For jumps, label would be translated to a two-byte absolute address.
                if ( line[0] == "JMP" || line[0] == "JSR" )
                {
                    machineCode = addrModeToOpcodeByte[OperandType.Absolute];
                    string targetString = $"${targetInt:X4}";
                    machineCode = string.Concat(machineCode, " ", OperandToMachineCode(targetString));
                }
                // Branches require one byte representing the displacement. Other than that, they only have one possible hex representation.
                else if( line[0].StartsWith("B") && (line[0] != "BIT" && line[0] != "BRK") ) {
                    machineCode = addrModeToOpcodeByte[OperandType.Error];
                    int displacement = (sim.branchToBytes[line[1]] - sim.bytesProcessed + 256) & 0xFF ;   // Make it fit between 0 and 255, mapping -128 to 128.
                    if( displacement < 0 || displacement > 255)
                    {
                        throw new BranchOutOfBoundsException("Branch out of bounds: " + string.Join(" ", line));
                    }
                    string displacementString = $"{displacement:X2}";
                    machineCode = string.Concat(machineCode, " ", displacementString);
                }
                // Incorrectly used label.
                else
                {
                    throw new BadOperandTypeException("Incorrectly used label: " + string.Join(" ", line));
                }
            }
        }

        return machineCode;
    }

    // Take a string either 2 or 4 characters long and turn it into machine code.
    public string OperandToMachineCode(string operand)
    {
        string hexString = OperandToHexString(operand);
        // If it's just one byte, further conversion is not necessary.
        if( hexString.Length == 2 )
        {
            return hexString;
        }
        // If it's two bytes long, respect the little-endian rule.
        else
        {
            return string.Concat(
                hexString.Substring(2, 2), " ", hexString.Substring(0, 2)
            );
        }
    }
}
