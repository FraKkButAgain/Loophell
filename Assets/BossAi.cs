using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    public int maxHealth = 20;
    public float actionTime = 1.5f;
    public List<int> actionSequence = new List<int> { 0 };

    public GameObject swordAttackPrefab;
    public GameObject wallBreakPrefab;
    public GameObject bombPrefab;
    

    private int currentHealth;
    private int currentActionIndex = 0;
    private bool isPerformingAction = false;

    public GameObject warningPrefab;
    public GameObject projectilePrefab;

    void Start()
    {
        currentHealth = maxHealth;
        StartCoroutine(BossLoop());
    }

    private IEnumerator BossLoop()
    {
        float nextActionTime = Time.time;

        while (currentHealth > 0)
        {
            if (!isPerformingAction && Time.time >= nextActionTime)
            {
                int action = actionSequence[currentActionIndex];
                currentActionIndex = (currentActionIndex + 1) % actionSequence.Count;

                StartCoroutine(PerformAction(action));

                nextActionTime = Time.time + actionTime;
            }

            yield return null;
        }

        Die();
    }

    private IEnumerator PerformAction(int action)
    {
        isPerformingAction = true;

        switch (action)
        {
            case 0:
                yield return new WaitForSeconds(actionTime);
                yield return new WaitForSeconds(actionTime);
                yield return new WaitForSeconds(actionTime);
                break;

            case 1:
                yield return StartCoroutine(SummonSwordAttack());
                break;

            case 2:
                yield return StartCoroutine(SummonTripleSwordAttack());
                break;
            case 3:
                yield return StartCoroutine(SummonWallLineAttack());
                break;

            case 4:
                yield return StartCoroutine(SummonBombPushAttack());
                break;
            case 5:
                yield return StartCoroutine(SummonBombDropAttack());
                break;
            case 6:
                yield return StartCoroutine(SummonSideArrowAttack());
                break;

            default:
                break;
        }

        isPerformingAction = false;
    }

    private IEnumerator SummonSwordAttack()
    {
        Vector3 bossPos = transform.position;

        SpawnSwordAtY(bossPos, -2);
        yield return new WaitForSeconds(actionTime);
        
    }

    private IEnumerator SummonTripleSwordAttack()
    {
        Vector3 bossPos = transform.position;

        SpawnSwordAtY(bossPos, -2);
        yield return new WaitForSeconds(actionTime);

        SpawnSwordAtY(bossPos, -5);
        yield return new WaitForSeconds(actionTime);

        SpawnSwordAtY(bossPos, -8);
        yield return new WaitForSeconds(actionTime);

        SpawnSwordAtY(bossPos, -2);
        yield return new WaitForSeconds(actionTime);

        yield return new WaitForSeconds(actionTime);
        
    }

    private IEnumerator SummonWallLineAttack()
    {
        Vector3 bossPos = transform.position;
        int totalBlocks = 20;
        float delayBetweenSpawns = actionTime / totalBlocks;

        // Linha horizontal de -9 a +10 blocos (centro do boss)
        for (int i = -10; i <= 10; i++)
        {
            Vector3 spawnPos = new Vector3(bossPos.x + i, bossPos.y - 3f, 0f);
            Instantiate(wallBreakPrefab, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(delayBetweenSpawns);
        }


        yield return new WaitForSeconds(actionTime);
    }

    private IEnumerator SummonBombPushAttack()
                {
            Vector3 bossPos = transform.position;
            float[] rightHeights = new float[] { -8f, -6f }; 
            float xLeft = -10f;
            float xRight = 10f;

            foreach (float yOffset in rightHeights)
            {

                Vector3 leftSpawn = new Vector3(xLeft, bossPos.y + yOffset - 1f, 0f);
                GameObject bombL = Instantiate(bombPrefab, leftSpawn, Quaternion.identity);
                bombL.transform.localScale *= 1.5f;

                Bomb bombScriptL = bombL.GetComponent<Bomb>();
                if (bombScriptL != null)
                    bombScriptL.PushInDirection(Vector2.right, 3f); 


                Vector3 rightSpawn = new Vector3(xRight, bossPos.y + yOffset, 0f);
                GameObject bombR = Instantiate(bombPrefab, rightSpawn, Quaternion.identity);
                bombR.transform.localScale *= 1.5f; 

                Bomb bombScriptR = bombR.GetComponent<Bomb>();
                if (bombScriptR != null)
                    bombScriptR.PushInDirection(Vector2.left, 3f); 

                
            }

            yield return new WaitForSeconds(actionTime);
        }

    private IEnumerator SummonBombDropAttack()
    {
        Vector3 bossPos = transform.position;

        Vector3 spawnPos = new Vector3(bossPos.x, bossPos.y - 4f, 0f);

        GameObject bomb = Instantiate(bombPrefab, spawnPos, Quaternion.identity);

        bomb.transform.localScale *= 1f;

        
        Bomb bombScript = bomb.GetComponent<Bomb>();
        if (bombScript != null)
            bombScript.PushInDirection(Vector2.down, 2f); 

        yield return new WaitForSeconds(actionTime); 
    }
    
   private IEnumerator SummonSideArrowAttack()
    {
        Vector3 bossPos = transform.position;


        Vector3 leftWarningPos = new Vector3(-9f, bossPos.y - 4f, 0f);
        Vector3 rightWarningPos = new Vector3(9f, bossPos.y - 4f, 0f);

        GameObject leftWarning = Instantiate(warningPrefab, leftWarningPos, Quaternion.identity);
        GameObject rightWarning = Instantiate(warningPrefab, rightWarningPos, Quaternion.identity);

        yield return new WaitForSeconds(1.5f);

        Destroy(leftWarning);
        Destroy(rightWarning);


        float[] yOffsets = new float[] { -5f, -6.5f, -8f, -9.5f };

        for (int i = 0; i < yOffsets.Length; i++)
        {
            float y = bossPos.y + yOffsets[i];

            if (i < yOffsets.Length / 2)
            {

                ShootArrowFromPosition(new Vector2(-10f, y), 4);
            }
            else
            {

                ShootArrowFromPosition(new Vector2(10f, y), 3);
            }
        }

        yield return new WaitForSeconds(1.5f);
    }

    private void SpawnSwordAtY(Vector3 bossPos, float relativeY)
    {
        Vector3 spawnPos = new Vector3(0f, bossPos.y + relativeY, 0f);
        Instantiate(swordAttackPrefab, spawnPos, Quaternion.identity);
    }

    

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            StopAllCoroutines();
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void ShootArrowFromPosition(Vector2 position, int direction)
    {
        GameObject arrow = Instantiate(projectilePrefab, position, Quaternion.identity);

        float angle = 0f;
        switch (direction)
        {
            case 1: angle = 0f; break;  
            case 2: angle = 180f; break;  
            case 3: angle = 90f; break;   
            case 4: angle = -90f; break;  
        }

        arrow.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}