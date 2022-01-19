using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOpcode : GenericOperation
{
    public TestOpcode(SimulationState sim) : base(sim)
    {
        Debug.Log("test constructor");
    }
    
    public override void Execute( List<string> codeLine )
    {
        // U+200B is a zero-width space that the text area appends to the end sometimes.
        string cleanOperand = codeLine[1].Trim('\u200b');

        //int val = int.Parse(cleanValString);
        //sim.memory.SetMemoryValue(0, val);
        OperandType ot = GetOperandType(cleanOperand);
        switch( ot )
        {
            case OperandType.Immediate:
                int val = OperandToInt(cleanOperand, ot);
                sim.memory.SetMemoryValue(0, val);
                break;
            default:
                break;
        }
        Debug.Log("Operand type: " + ot.ToString());
    }

}
