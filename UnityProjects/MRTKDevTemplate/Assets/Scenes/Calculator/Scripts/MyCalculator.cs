using MixedReality.Toolkit.Data; 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Schema;
using UnityEditor;
using UnityEngine;

// Using static to simplify access to MyCalculatorUIManager
using static MyCalculatorUIManager;

public class MyCalculator : MonoBehaviour
{
    #region CUSTOM EVENTS

    public delegate void CalculatorStarted();
    public static event CalculatorStarted OnCalculatorStarted;

    public delegate void UpdateExpressionDisplay(string value);
    public static event UpdateExpressionDisplay OnUpdateExpressionDisplay;

    public delegate void UpdateExpressionResult(string value);
    public static event UpdateExpressionResult OnUpdateExpressionResult;

    public delegate void UpdateHistory(string value);
    public static event UpdateHistory OnUpdateHistory;
    #endregion

    [SerializeField] private string CalculatorExpression = string.Empty;
    [SerializeField] private float CalculatorRunningTotal = 0f;      
    [SerializeField] private float PreviousValue = 0f;         
    [SerializeField] private float CurrentValue = 0f;                 

    private void OnEnable()
    {
        MyCalculatorUIManager.OnMyButtonElementPressed += MyCalculatorUIManager_OnMyButtonElementPressed;        
    }

    private void OnDisable()
    {
        MyCalculatorUIManager.OnMyButtonElementPressed -= MyCalculatorUIManager_OnMyButtonElementPressed;
    }

    private void MyCalculatorUIManager_OnMyButtonElementPressed(MyCalculatorUIManager.MyButtonElement value)
    {
        Debug.Log($"You click the button with value of {value.Caption} and of type {value.Type}");

        // on button pressed
        switch (value.Type)
        {
            case ButtonType.Numeric:
                {
                    // do the numeric stuff
                    CalculatorExpression += value.Caption;
                    break;
                }
            case ButtonType.Operation:
                {
                    // do operation stuff
                    switch (value.Caption)
                    {
                        case "+":
                        case "-":
                        case "/":
                        case "*":
                            {
                                CalculatorExpression += value.Caption;
                                break;
                            }
                        case "=":
                            {
                                // compute
                                bool ok = ExpressionEvaluator.Evaluate(CalculatorExpression, out float r);
                                Debug.Log($"Expression: {CalculatorExpression} -> Result: {r}");
                                
                                // update result and reset expression display
                                OnUpdateExpressionResult?.Invoke(r.ToString());
                                OnUpdateHistory?.Invoke($"{CalculatorExpression} = {r}");
                                CalculatorExpression = r.ToString();
                                OnUpdateExpressionDisplay?.Invoke("");
                                break;
                            }
                        case "CE":
                            {
                                // clear the expression display only
                                CalculatorExpression = string.Empty;
                                OnUpdateExpressionDisplay?.Invoke("");
                                break;
                            }
                        case "C":
                            {
                                // clear both the expression and the result display
                                CalculatorExpression = string.Empty;
                                OnUpdateExpressionResult?.Invoke("0");
                                OnUpdateHistory?.Invoke("");
                                break;
                            }
                        case "←":
                            {
                                // Remove the last character from the expression
                                if (CalculatorExpression.Length > 0)
                                {
                                    CalculatorExpression = CalculatorExpression.Substring(0, CalculatorExpression.Length - 1);
                                }
                                break;
                            }
                        case "%":
                        case "1/x":
                        case "X<sup>2":
                        case "√x":
                        case "+/-":
                            {
                                // Function to apply the operation to the last number in the expression
                                ApplyOperationToLastNumber(value.Caption, GetOperationFunc(value.Caption));
                                break;
                            }
                        case ".":
                            {
                                // Add a decimal point to the expression if it doesn't already have one
                                if (!CalculatorExpression.Contains("."))
                                {
                                    CalculatorExpression += ".";
                                }
                                break;
                            }
                    }
                    break;
                }
            default:
                {
                    Debug.Log("Operation not supported ...");
                    break;
                }
        }

        // update the expression display with the current expression after each button press
        OnUpdateExpressionDisplay?.Invoke(CalculatorExpression);
    }

    // Function to apply the operation to the last number in the expression
    private void ApplyOperationToLastNumber(string operation, Func<float, float> operationFunc)
    {
        // find the last operator in the expression
        int lastOperatorIndex = CalculatorExpression.LastIndexOfAny(new char[] { '+', '-', '*', '/' });
        // get the last number from the expression
        string lastNumberStr = lastOperatorIndex == -1 ? CalculatorExpression : CalculatorExpression.Substring(lastOperatorIndex + 1);

        // parse the last number and apply the operation
        if (float.TryParse(lastNumberStr, out float lastNumber))
        {
            float result = operationFunc(lastNumber);
            // replace the last number with the result
            CalculatorExpression = lastOperatorIndex == -1 ? result.ToString() : CalculatorExpression.Substring(0, lastOperatorIndex + 1) + result.ToString();
            
            // update the result display if the last operator is not found (for single number operations)
            if (lastOperatorIndex == -1)
            {
                // update here
                OnUpdateExpressionResult?.Invoke(result.ToString());
                OnUpdateHistory?.Invoke($"{operation.Replace("x", "")}({lastNumber}) = {result}");
            }
        }
    }

    // Function for mapping buttons/operations
    private Func<float, float> GetOperationFunc(string operation)
    {
        switch (operation)
        {
            case "%":
                return parsedValue => parsedValue / 100;  // Percent operation
            case "1/x":
                return parsedValue => 1 / parsedValue;    // Reciprocal
            case "X<sup>2":
                return parsedValue => parsedValue * parsedValue;  // Square the number
            case "√x":
                return parsedValue => Mathf.Sqrt(parsedValue);  // Square root
            case "+/-":
                return parsedValue => -parsedValue;  // Negate the number
            default:
                throw new InvalidOperationException("Unsupported operation");
        }
    }

    private void Start()
    {
        OnCalculatorStarted?.Invoke();
    }
}
