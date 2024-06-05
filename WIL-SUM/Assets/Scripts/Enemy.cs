using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool enemyAlive = true;
    public Units units;
    public Money money;
    public int enemyHealth = 300;
    public int bossHealth = 20;

    public IEnumerator MinionHealth()
    {
        while (enemyHealth > 0)
        {
            enemyHealth -= (units.numSmall * units.dmgSmall);
            enemyHealth -= (units.numMedium * units.dmgMedium);
            enemyHealth -= (units.numLarge * units.dmgLarge);
            yield return new WaitForSeconds(1);
            Debug.Log(enemyHealth);
           
        }
        money.index2 += 10;
        enemyAlive = false;
    }

    public IEnumerator BossHealth()
    {
        while (bossHealth > 0)
        {
            bossHealth = bossHealth - (units.numSmall * units.dmgSmall);
            bossHealth = bossHealth -(units.numMedium * units.dmgMedium);
            bossHealth = bossHealth -(units.numLarge * units.dmgLarge);
            yield return new WaitForSeconds(1);
            Debug.Log(bossHealth);
            
        }
        money.index2 += 20;
        enemyAlive = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (gameObject.CompareTag("Boss"))
        {
            if (other.CompareTag("SmallUnit"))
            {
                StartCoroutine(BossHealth());
            }
            else if (other.CompareTag("MediumUnit"))
            {
                StartCoroutine(BossHealth());
            }
            else if (other.CompareTag("LargeUnit"))
            {
                StartCoroutine(BossHealth());
            }
        }
        else if (gameObject.CompareTag("Minion"))
        {
            if (other.CompareTag("SmallUnit"))
            {
                StartCoroutine(MinionHealth());
            }
            else if (other.CompareTag("MediumUnit"))
            {
                StartCoroutine(MinionHealth());
            }
            else if (other.CompareTag("LargeUnit"))
            {
                StartCoroutine(MinionHealth()); 
            }
        }
    }
    private void Update()
    {
        if (!this.enemyAlive)
        {
            Destroy(this.gameObject);
        }
    }
}
