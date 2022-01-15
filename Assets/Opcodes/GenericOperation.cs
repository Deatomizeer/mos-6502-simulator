using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GenericOperation
{
    public MemoryAndRegisters mem;

    public GenericOperation(MemoryAndRegisters memoryReference)
    {
        this.mem = memoryReference;
        Debug.Log("generic construct");
    }

    public virtual void Execute( List<string> codeLine ) { }
}
