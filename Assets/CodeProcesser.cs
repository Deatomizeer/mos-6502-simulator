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
        // Reset.
        simulation.step = 0;
        simulation.bytesProcessed = 0x300;
        simulation.processedCode.Clear();
        simulation.running = true;

        int localStep = 0;  // Keep track of where branch labels are in the code.
        int localBytes = 0x300;
        List<OperandType> twoByteOperands = new List<OperandType>
        {
            OperandType.Absolute, OperandType.AbsoluteX, OperandType.AbsoluteY
        };

        List<string> codeLines = new List<string>(userInput.text.Split('\n'));
        foreach( string line in codeLines )
        {
            // Ignore lines that contain only whitespace.
            if( !Regex.IsMatch(line, @"^[\s*\u200b]$") )
            {
                // Replace whitespaces with singular spaces to ensure the line splits into words correctly.
                string modifiedLine = Regex.Replace(line, @"\s+", " ");
                List<string> splitLine = new List<string>(modifiedLine.Split(' '));
                // U+200B is a zero-width space that the text area appends to the end of a line sometimes.
                splitLine[splitLine.Count-1] = splitLine[splitLine.Count - 1].TrimEnd('\u200b');

                // If the line is a label, make note of it.
                if (splitLine.Count == 1 && splitLine[0].EndsWith(":"))
                {
                    string label = splitLine[0].Substring(0, splitLine[0].Length - 1);
                    simulation.branchToStep.Add(label, localStep);
                    simulation.branchToBytes.Add(label, localBytes);
                }
                else
                {
                    simulation.processedCode.Add(splitLine);
                    localStep++;
                    // Determine how many bytes did the instruction and operand take.
                    if (splitLine.Count == 1)
                    {
                        // The opcode takes one byte and takes no operand.
                        localBytes++;
                    }
                    else
                    {
                        OperandType ot = GenericOperation.GetOperandType(splitLine[1]);
                        // Accumulator counts as an implicit target in machine code.
                        if (ot == OperandType.Accumulator )
                        {
                            localBytes++;
                        }
                        else if (twoByteOperands.Exists(item => item == ot))
                        {
                            localBytes += 3;
                        }
                        // Most operands are one byte long.
                        else
                        {
                            localBytes += 2;
                        }
                    }
                    
                }
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
