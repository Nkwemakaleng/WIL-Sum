using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool enemyAlive = true;
    public Units units;
    public Money money;
    public int enemyHealth = 300;
    public int bossHealth = 1000;

    public IEnumerator MinionHealth()
    {
        while (enemyHealth < 0)
        {
            enemyHealth -= (units.numSmall * units.dmgSmall);
            enemyHealth -= (units.numMedium * units.dmgMedium);
            enemyHealth -= (units.numLarge * units.dmgLarge);
            yield return new WaitForSeconds(1);
        }
        money.moneyValue += 20;
        enemyAlive = false;
    }

    public IEnumerator BossHealth()
    {
        while (enemyHealth < 0)
        {
            enemyHealth -= (units.numSmall * units.dmgSmall-1);
            enemyHealth -= (units.numMedium * units.dmgMedium-2);
            enemyHealth -= (units.numLarge * units.dmgLarge-3);
            yield return new WaitForSeconds(1);
        }
        money.moneyValue += 50;
        enemyAlive = false;
    }

    private void Awake()
    {
        if (gameObject.CompareTag("Boss"))
        {
            StartCoroutine(BossHealth());
        }
        else
        {
            StartCoroutine(MinionHealth());
        }
    }
    private void Update()
    {
        if (!enemyAlive)
        {
            Destroy(this.gameObject);
        }
    }
}
