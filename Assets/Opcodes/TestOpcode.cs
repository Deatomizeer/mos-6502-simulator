using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOpcode : GenericOperation
{
    public TestOpcode(MemoryAndRegisters mem) : base(mem)
    {
        Debug.Log("test constructor");
    }
    
    public override void Execute( List<string> codeLine )
    {
        // U+200B is a zero-width space that the text area appends to the end sometimes.
        int val = int.Parse(codeLine[1].Trim('\u200b'));
        mem.SetMemoryValue(0, val);
        Debug.Log("Execute order: " + mem.GetMemoryValue(0));
    }

}
