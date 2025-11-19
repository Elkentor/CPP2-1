using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoisonHazard : MonoBehaviour
{
    public float totalDamage = 15f;
    public float duration = 5f;

    private Dictionary<PlayerHealth, Coroutine> activePoisons = new Dictionary<PlayerHealth, Coroutine>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TryApplyPoison(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TryApplyPoison(other);
        }
    }

    private void TryApplyPoison(Collider other)
    {
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null && !activePoisons.ContainsKey(health))
        {
            Coroutine poison = StartCoroutine(ApplyPoison(health));
            activePoisons.Add(health, poison);
        }
    }

    private IEnumerator ApplyPoison(PlayerHealth health)
    {
        float elapsed = 0f;
        float damagePerSecond = totalDamage / duration;

        while (elapsed < duration)
        {
            health.TakeDamage(damagePerSecond * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        activePoisons.Remove(health);
    }
}

