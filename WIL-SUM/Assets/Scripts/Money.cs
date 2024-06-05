using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour
{
    public int moneyValue = 50;

    public IEnumerator MoneyPassive()
    {
        int index = 3;
        while (true)
        {
            moneyValue = moneyValue + 1;
            yield return new WaitForSeconds(index);
            Debug.Log(moneyValue);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MoneyPassive());
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
