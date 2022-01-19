using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OperandType
{
    Accumulator,    // Concerning the AC register.
    Immediate,      // Raw hexadecimal value, such as #$B2.
    ZeroPage,       // Stored in one byte, for example $66.
    ZeroPageX,      // Indexed: $44,X.
    ZeroPageY,      // $44,Y.
    Absolute,       // Specifying a memory location: $4032.
    AbsoluteX,      // Again, indexed: $1010,X.
    AbsoluteY,      // $1010,Y.
    Indirect,       // Reserved only for the JMP opcode: ($1000).
    IndirectX,      // ($44,X).
    IndirectY,      // ($44),Y.
    Error           // Likely a label: JSR subroutine.
}
