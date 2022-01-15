using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// A class that holds a reference to all opcode objects, using their methods to manipulate memory as it reads processed code.
public class CodeInterpreter : MonoBehaviour
{
    // TODO: create and assign buttons.
    public Button stepButton;
    public Button runButton;
    public CodeProcesser codeProcesser;
    public int step = 0;

    // Maybe think of a better name?
    private Dictionary<string, GenericOperation> getOperation;

    // Start is called before the first frame update
    void Start()
    {
        // Because the "Find" method is inaccessible from a regular constructor, it has to be used here, then passed to every opcode.
        codeProcesser = GameObject.Find("GameManager").GetComponent<CodeProcesser>();
        MemoryAndRegisters mem = GameObject.Find("GameManager").GetComponent<MemoryAndRegisters>();
        getOperation = new Dictionary<string, GenericOperation>{
            { "TEST", new TestOpcode(mem) }
        };
        stepButton.onClick.AddListener(SimulateStep);
    }

    void SimulateStep()
    {
        List<List<string>> code = codeProcesser.processedCode;
        if (step < code.Count) {
            List<string> line = code[step];
            step++;
            getOperation[line[0]].Execute(line);
        }
    }
}
