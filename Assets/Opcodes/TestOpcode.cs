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
        //int val = int.Parse(cleanValString);
        //sim.memory.SetMemoryValue(0, val);
        OperandType ot = GetOperandType(codeLine[1]);

        int val = OperandToInt(codeLine[1]);
        sim.memory.SetMemoryValue(0, val);

        Debug.Log("Operand type: " + ot.ToString());
    }

}
