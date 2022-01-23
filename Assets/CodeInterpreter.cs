using UnityEngine;
using UnityEngine.UI;

// A class that holds a reference to all opcode objects, using their methods to manipulate memory as it reads processed code.
public class CodeInterpreter : MonoBehaviour
{
    // TODO: create and assign buttons.
    public Button stepButton;
    public Button runButton;
    public Button resetButton;
    
    public SimulationState simulation;

    // Start is called before the first frame update
    void Start()
    {
        simulation = GameObject.Find("GameManager").GetComponent<SimulationState>();

        stepButton.onClick.AddListener(simulation.SimulateStep);
        runButton.onClick.AddListener(simulation.RunProgram);
        resetButton.onClick.AddListener(simulation.ResetSimulation);
    }

}
