using System.Collections;
using UnityEngine;

public class Units : MonoBehaviour
{
    private bool unitAlive =true;
    private string enemyType = "null";
    public int numSmall = 1;
    public int healthSmall = 100;
    public int dmgSmall = 2;
    public int numMedium = 0;
    public int healthMedium = 200;
    public int dmgMedium = 5;
    public int numLarge = 0;
    public int healthLarge = 400;
    public int dmgLarge = 10;



    public IEnumerator SmallUnitHealth()
    {
        while (healthSmall>0)
        {
            if (enemyType == "minion")
            {
                healthSmall -= 15;
            }
            else if (enemyType == "Boss")
            {
                healthSmall -= 25;
            }
            else
            {
                this.StopCoroutine(SmallUnitHealth());
            }
            yield return new WaitForSeconds(2);            
        }
        unitAlive = false;


    }
    public IEnumerator MediumUnitHealth()
    {
        while (healthMedium > 0)
        {
            if (enemyType == "minion")
            {
                healthMedium -= 15;
            }
            else if (enemyType == "Boss")
            {
                healthMedium -= 25;
            }
            else
            {
                healthMedium += 0;
            }
            yield return new WaitForSeconds(2);            
        }
        unitAlive = false;

    }
    public IEnumerator LargeUnitHealth()
    {
        while (healthLarge > 0)
        {
            if (enemyType == "Minion")
            {
                healthLarge -= 10;
            }
            else if (enemyType == "Boss")
            {
                healthLarge -= 25;
            }
            else
            {
                StopCoroutine(LargeUnitHealth());
            }
            yield return new WaitForSeconds(2);           
        }
        unitAlive = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if ( other.CompareTag("Boss"))
        {
            enemyType = "Boss";
        }
        else if (other.CompareTag("Minion"))
        {
            enemyType = "Minion";
        }
        if(this.gameObject.CompareTag("SmallUnit") && (other.CompareTag("Boss") || other.CompareTag("Minion")))
        {
            StartCoroutine(SmallUnitHealth());
        }
        else if (this.gameObject.CompareTag("MediumUnit") && (other.CompareTag("Boss") || other.CompareTag("Minion")))
        {
            StartCoroutine(MediumUnitHealth());
        }
        else if (this.gameObject.CompareTag("LargeUnit") && (other.CompareTag("Boss") || other.CompareTag("Minion")))
        {
            StartCoroutine(LargeUnitHealth());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        enemyType = "Null"; 
    }
    private void Update()
    {
        if (!this.unitAlive)
        {
            Destroy(this.gameObject);
        }
    }
}
