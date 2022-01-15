using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;    // Since input area uses a special UI text object class.

public class CodeProcesser : MonoBehaviour
{
    public Button assembleButton;
    public TMP_Text userInput;
    public List<List<string>> processedCode = new List<List<string>>();

    // Start is called before the first frame update
    void Start()
    {
        assembleButton.onClick.AddListener(ProcessCode);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // Take the user-written code and prepare it for execution and deassembly.
    public void ProcessCode()
    {
        processedCode.Clear();
        List<string> codeLines = new List<string>(userInput.text.Split('\n'));
        foreach( string line in codeLines )
        {
            // Ignore lines that contain only whitespace.
            if( !Regex.IsMatch(line, @"^\s*$") )
            {
                // Replace whitespaces with singular spaces to ensure the line splits into words correctly.
                string modifiedLine = Regex.Replace(line, @"\s+", " ");
                List<string> splitLine = new List<string>(modifiedLine.Split(' '));
                processedCode.Add(splitLine);
            }
        }
    }
}
