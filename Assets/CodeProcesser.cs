using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;    // Since input area uses a special UI text object class.
using System;

public class CodeProcesser : MonoBehaviour
{
    public Button assembleButton;       // Prepare the code to be executed.
    public Button disassembleButton;    // Translate the code into its hex representation, prior assembly required.
    public Button stepButton;
    public Button runButton;
    public Button resetButton;
    public TMP_Text userInput;          // Raw text input by the user.

    // A reference to the "state" object to deliver the processed code to.
    public SimulationState simulation;

    // Start is called before the first frame update
    void Start()
    {
        simulation = this.GetComponent<SimulationState>();

        assembleButton.onClick.AddListener(ProcessCode);
        disassembleButton.onClick.AddListener(Hexdump);
        disassembleButton.interactable = false;
        stepButton.onClick.AddListener(simulation.SimulateStep);
        stepButton.interactable = false;
        runButton.onClick.AddListener(simulation.RunProgram);
        runButton.interactable = false;
        resetButton.onClick.AddListener(simulation.ResetSimulation);
        resetButton.interactable = false;
    }

    // Take the user-written code and prepare it for execution.
    public void ProcessCode()
    {
        // Reset.
        simulation.ResetSimulation();
        simulation.step = 0;
        simulation.bytesProcessed = 0x300;
        simulation.processedCode.Clear();
        simulation.branchToBytes.Clear();
        simulation.branchToStep.Clear();

        int localStep = simulation.step;  // Keep track of where branch labels are in the code.
        int localBytes = simulation.bytesProcessed;

        List<string> codeLines = new List<string>(userInput.text.Split('\n'));
        foreach( string l in codeLines )
        {
            string line = l;
            char commentSign = ';';
            int commentIndex = line.IndexOf(commentSign);
            // If there is a comment on this line, ignore all text in it.
            if (commentIndex != -1)
            {
                line = line.Substring(0, commentIndex);
            }
            line = line.Trim();

            // Ignore lines that contain only whitespace.
            if( !(Regex.IsMatch(line, @"^[\s*\u200b]$") || line == "") )
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
                    localBytes += GenericOperation.LineSizeInBytes(splitLine);
                }
            }
        }
        // Make the buttons accessible/inaccessible.
        assembleButton.interactable = false;
        disassembleButton.interactable = true;
        stepButton.interactable = true;
        runButton.interactable = true;
        resetButton.interactable = true;
    }

    // Iterate through processed code and translate assembly to machine code in hexadecimal.
    public void Hexdump()
    {
        string fullMachineCode = "";
        int saveBytes = simulation.bytesProcessed; // Temporarily store the value here and overwrite the original one.
        simulation.bytesProcessed = 0x300;
        foreach(List<string> line in simulation.processedCode)
        {
            string lineMachineCode = "";
            try
            {
                lineMachineCode = simulation.getOperation[line[0].ToUpper()].LineToMachineCode(line);
            }
            catch( Exception ex ) when (
                ex is BranchOutOfBoundsException ||
                ex is BadOperandTypeException
            )
            {
                Debug.LogWarning(ex.Message);
                simulation.bytesProcessed = saveBytes;
                break;
            }
            catch( Exception )
            {
                Debug.LogWarning("An unknown error occured while processing line " + string.Join(" ", line));
                simulation.bytesProcessed = saveBytes;
                break;
            }

            fullMachineCode = string.Concat(fullMachineCode, "\n", lineMachineCode);
            simulation.bytesProcessed += GenericOperation.LineSizeInBytes(line);
        }
        // Once done or after an error, restore the original value.
        simulation.bytesProcessed = saveBytes;

        Debug.Log(fullMachineCode);
    }

    // Whenever the text area is edited, force the user to assemble the program afterwards.
    public void OnTextChangedHandler()
    {
        // Make the buttons accessible/inaccessible.
        assembleButton.interactable = true;
        disassembleButton.interactable = false;
        stepButton.interactable = false;
        runButton.interactable = false;
        resetButton.interactable = false;
    }
}
