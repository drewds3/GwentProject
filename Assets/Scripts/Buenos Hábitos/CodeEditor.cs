using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CodeEditor : MonoBehaviour
{
    //Entrada de texto para escribir el código
    public static TMPro.TMP_InputField inputField;

    void Start()
    {
        //Se inicializa la variable
        inputField = GetComponent<TMPro.TMP_InputField>();
    }

    //Método para saltar a la siguiente línea al presionar "enter"
    public void JumpToNextLine()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            inputField.text += "\n";
            inputField.MoveTextEnd(false);
            inputField.ActivateInputField(); 
            inputField.Select();
            int caretPosition = inputField.caretPosition;
            inputField.caretPosition = caretPosition + 1;
            inputField.ForceLabelUpdate();
        }
    }
}

