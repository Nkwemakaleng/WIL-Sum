using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIStuff : MonoBehaviour
{
    public Money money;
    [SerializeField] TMP_Text text;


    void Update()
    {
        text.text = ("Money: "  + money.moneyValue);
    }
}
