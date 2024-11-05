using MixedReality.Toolkit.UX;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using TMPro;
//using static MixedReality.Toolkit.Examples.Demos.InspectorDrivenDialog;

public class MyCalculatorUIManager : MonoBehaviour
{
    #region CUSTOM EVENTS AND DELEGATES
    public delegate void MyButtonElementPressed(MyButtonElement value);
    public static event MyButtonElementPressed OnMyButtonElementPressed;

    //public delegate void ButtonPressed(string value);
    //public static event ButtonPressed OnButtonPressed;
    #endregion

    public TMP_Text tmpCalculatorExpressionDisplay;
    public TMP_Text tmpCalculatorResultDisplay;

    public Transform GridLayoutRoot;
    public GameObject CalculatorButtonPrefab;

    public class MyButtonElement
    {
        public string Caption;
        public ButtonType Type;

        public MyButtonElement(string caption, ButtonType type)
        {
            Caption = caption;
            Type = type;
        }
    }

    #region NEW Way
    public Dictionary<int, MyButtonElement> buttonDictionary = new Dictionary<int, MyButtonElement>
    {
        {0, new MyButtonElement("%", ButtonType.Operation) },
        {1, new MyButtonElement("CE", ButtonType.Operation) },
        {2, new MyButtonElement("C", ButtonType.Operation) },
        {3, new MyButtonElement("←", ButtonType.Operation) },
        {4, new MyButtonElement("1/x", ButtonType.Operation) },
        {5, new MyButtonElement("X<sup>2", ButtonType.Operation) },
        {6, new MyButtonElement("√x", ButtonType.Operation) },
        {7, new MyButtonElement("/", ButtonType.Operation) },
        {8, new MyButtonElement("7", ButtonType.Numeric) },
        {9, new MyButtonElement("8", ButtonType.Numeric) },
        {10, new MyButtonElement("9", ButtonType.Numeric) },
        {11, new MyButtonElement("*", ButtonType.Operation) },
        {12, new MyButtonElement("4", ButtonType.Numeric) },
        {13, new MyButtonElement("5", ButtonType.Numeric) },
        {14, new MyButtonElement("6", ButtonType.Numeric) },
        {15, new MyButtonElement("-", ButtonType.Operation) },
        {16, new MyButtonElement("1", ButtonType.Numeric) },
        {17, new MyButtonElement("2", ButtonType.Numeric) },
        {18, new MyButtonElement("3", ButtonType.Numeric) },
        {19, new MyButtonElement("+", ButtonType.Operation) },
        {20, new MyButtonElement("+/-", ButtonType.Operation) },
        {21, new MyButtonElement("0", ButtonType.Numeric) },
        {22, new MyButtonElement(".", ButtonType.Operation) },
        {23, new MyButtonElement("=", ButtonType.Operation) }
    };
    #endregion

    #region OLD WAY
    public List<string> buttons = new List<string>{
        "%",
        "CE",
        "C",
        "\u2190",
        "1/x",
        "X<sup>2",
        "\u221Ax",
        "/",
        "7",
        "8",
        "9",
        "*",
        "4",
        "5",
        "6",
        "-",
        "1",
        "2",
        "3",
        "+",
        "+/-",
        "0",
        ".",
        "=",
    };
    #endregion

    private void OnEnable()
    {
        MyCalculator.OnCalculatorStarted += MyCalculator_OnCalculatorStarted;
        MyCalculator.OnUpdateExpressionDisplay += MyCalculator_OnUpdateExpressionDisplay;
        MyCalculator.OnUpdateExpressionResult += MyCalculator_OnUpdateExpressionResult;        
    }

    private void OnDisable()
    {
        MyCalculator.OnCalculatorStarted -= MyCalculator_OnCalculatorStarted;
        MyCalculator.OnUpdateExpressionDisplay -= MyCalculator_OnUpdateExpressionDisplay;
        MyCalculator.OnUpdateExpressionResult -= MyCalculator_OnUpdateExpressionResult;
    }

    private void MyCalculator_OnCalculatorStarted()
    {
        ClearCalculatorDisplays();
    }

    private void MyCalculator_OnUpdateExpressionDisplay(string value)
    {
        tmpCalculatorExpressionDisplay.text = value;
    }

    private void MyCalculator_OnUpdateExpressionResult(string value)
    {
        tmpCalculatorResultDisplay.text = value;
    }


    void ClearCalculatorDisplays()
    {
        tmpCalculatorExpressionDisplay.text = string.Empty;
        tmpCalculatorResultDisplay.text = "0";
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < buttonDictionary.Count; i++)
        {
            var butGO = Instantiate(CalculatorButtonPrefab, GridLayoutRoot) as GameObject;
            butGO.name = $"butGen_{i}";

            // get the button type ...
            var button = buttonDictionary[i] as MyButtonElement;

            var myButtom = butGO.transform.GetComponent<MyCalculatorButtonController>();
            myButtom.tmpButtonCaption.text = $"{button.Caption}";
            myButtom.buttonType = button.Type;

            butGO.transform.GetComponent<PressableButton>().OnClicked.AddListener(() => {
                var s = butGO.transform.GetComponent<MyCalculatorButtonController>().tmpButtonCaption.text;

                //OnButtonPressed?.Invoke(s);
                OnMyButtonElementPressed?.Invoke(button);
            });
        }
    }
}
