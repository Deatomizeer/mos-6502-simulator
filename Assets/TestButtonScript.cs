using UnityEngine;
using TMPro;

public class TestButtonScript : MonoBehaviour
{
    // Input area reference.
    public TMP_InputField code_area;
    // Test text reference.
    public TMP_Text test_text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        string s = code_area.text;
        test_text.text = s;
    }
}
