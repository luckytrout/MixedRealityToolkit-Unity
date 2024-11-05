using MixedReality.Toolkit.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Schema;
using UnityEditor;
//using UnityEditor.UIElements;
using UnityEngine;

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

        // determine what you want to do ...
        switch (value.Type)
        {
            case ButtonType.Numeric:
                {                    
                    // do the numeric stuff ...
                    CalculatorExpression += value.Caption;
                    break;
                }
            case ButtonType.Operation:
                {
                    // do the operation stuff ...
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
                                // compute the expression ...
                                bool ok = ExpressionEvaluator.Evaluate(CalculatorExpression, out float r);
                                Debug.Log($"Expression: {CalculatorExpression} -> Result: {r}");
                                OnUpdateExpressionResult?.Invoke(r.ToString());
                                CalculatorExpression = r.ToString();
                                OnUpdateExpressionDisplay?.Invoke("");
                                break;
                            }
                        case "CE":
                            {
                                CalculatorExpression = string.Empty;
                                OnUpdateExpressionDisplay?.Invoke("");
                                break;
                            }
                        case "C":
                            {
                                CalculatorExpression = string.Empty;
                                OnUpdateExpressionResult?.Invoke("0");
                                break;
                            }
                        case "←":
                            {
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
                                ApplyOperationToLastNumber(value.Caption, GetOperationFunc(value.Caption));
                                break;
                            }
                        case ".":
                            {
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

        OnUpdateExpressionDisplay?.Invoke(CalculatorExpression);
    }

    private void ApplyOperationToLastNumber(string operation, Func<float, float> operationFunc)
    {
        int lastOperatorIndex = CalculatorExpression.LastIndexOfAny(new char[] { '+', '-', '*', '/' });
        string lastNumberStr = lastOperatorIndex == -1 ? CalculatorExpression : CalculatorExpression.Substring(lastOperatorIndex + 1);

        if (float.TryParse(lastNumberStr, out float lastNumber))
        {
            float result = operationFunc(lastNumber);
            CalculatorExpression = lastOperatorIndex == -1 ? result.ToString() : CalculatorExpression.Substring(0, lastOperatorIndex + 1) + result.ToString();
            
            if (lastOperatorIndex == -1)
            {
                OnUpdateExpressionResult?.Invoke(result.ToString());
            }
        }
    }

    private Func<float, float> GetOperationFunc(string operation)
    {
        switch (operation)
        {
            case "%":
                return parsedValue => parsedValue / 100;
            case "1/x":
                return parsedValue => 1 / parsedValue;
            case "X<sup>2":
                return parsedValue => parsedValue * parsedValue;
            case "√x":
                return parsedValue => Mathf.Sqrt(parsedValue);
            case "+/-":
                return parsedValue => -parsedValue;
            default:
                throw new InvalidOperationException("Unsupported operation");
        }
    }

    private void Start()
    {
        OnCalculatorStarted?.Invoke();
    }
}
