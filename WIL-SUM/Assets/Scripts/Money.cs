using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour
{
    public int moneyValue = 15;
    public int index2 = 15;
    public IEnumerator MoneyPassive()
    {
        int index = 3;
        index2 = 15;
        while (true)
        {
            index2++;
            yield return new WaitForSeconds(index);
            moneyValue = index2;
         
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MoneyPassive());
    }

}
