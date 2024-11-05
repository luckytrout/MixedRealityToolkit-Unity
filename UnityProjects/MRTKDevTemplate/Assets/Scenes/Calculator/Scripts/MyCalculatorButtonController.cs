using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using TMPro;
using System.Globalization;


public enum ButtonType
{
    Numeric,
    Operation
}

public class MyCalculatorButtonController : MonoBehaviour
{
    public TMP_Text tmpButtonCaption;
    public ButtonType buttonType;
}
