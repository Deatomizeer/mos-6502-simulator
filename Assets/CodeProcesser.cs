using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;    // Since input area uses a special UI text object class.

public class CodeProcesser : MonoBehaviour
{
    public Button assembleButton;       // Prepare the code to be executed.
    public Button disassembleButton;    // Translate the code into its hex representation, prior assembly required.
    public TMP_Text userInput;          // Raw text input by the user.

    // A reference to the "state" object to deliver the processed code to.
    public SimulationState simulation;

    // Start is called before the first frame update
    void Start()
    {
        simulation = this.GetComponent<SimulationState>();

        assembleButton.onClick.AddListener(ProcessCode);
        //disassembleButton.onClick.AddListener(Hexdump);
    }
    
    // Take the user-written code and prepare it for execution.
    public void ProcessCode()
    {
        simulation.step = 0;
        simulation.processedCode.Clear();

        List<string> codeLines = new List<string>(userInput.text.Split('\n'));
        foreach( string line in codeLines )
        {
            // Ignore lines that contain only whitespace.
            if( !Regex.IsMatch(line, @"^\s*$") )
            {
                // Replace whitespaces with singular spaces to ensure the line splits into words correctly.
                string modifiedLine = Regex.Replace(line, @"\s+", " ");
                List<string> splitLine = new List<string>(modifiedLine.Split(' '));
                simulation.processedCode.Add(splitLine);
            }
        }
    }

    // Iterate through processed code and translate assembly to machine code in hexadecimal.
    public void Hexdump()
    {
        // TODO: Maybe iterate over processedCode and feed it to opcode objects, one line at a time.
        // That would require a reference to that dictionary though.
    }
}
