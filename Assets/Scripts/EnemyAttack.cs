using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    PlayerHealth target;
    [SerializeField] float damage = 40f;
    AudioClip attackSound;
    AudioSource audioSource;

    void Start()
    {
        attackSound = Resources.Load<AudioClip>("zombie_attack");
        target = FindObjectOfType<PlayerHealth>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
        }
    }

    public void AttackHitEvent()
    {
        if (target == null) return;
        target.TakeDamage(damage);
        target.GetComponent<DisplayDamage>().ShowDamageImpact();
        
        if (attackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

}
