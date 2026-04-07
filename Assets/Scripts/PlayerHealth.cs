using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float hitPoints = 100f;

    AudioClip[] footstepSounds;
    AudioClip bgmSound;
    AudioSource footstepSource;
    AudioSource bgmSource;
    
    float walkStepInterval = 0.5f;
    float sprintStepInterval = 0.3f;
    float stepTimer;
    Rigidbody rb;
    CharacterController cc;

    void Start()
    {
        SpawnExtraLoot();
        
        // Load sounds from Resources
        footstepSounds = new AudioClip[] {
            Resources.Load<AudioClip>("footstep_1"),
            Resources.Load<AudioClip>("footstep_2")
        };
        bgmSound = Resources.Load<AudioClip>("bgm");

        // Setup BGM AudioSource
        bgmSource = gameObject.AddComponent<AudioSource>();
        if (bgmSound != null)
        {
            bgmSource.clip = bgmSound;
            bgmSource.loop = true;
            bgmSource.spatialBlend = 0f; // 2D sound for BGM
            bgmSource.volume = 0.4f;
            bgmSource.Play();
        }

        // Setup Footstep AudioSource
        footstepSource = gameObject.AddComponent<AudioSource>();
        footstepSource.spatialBlend = 0f;

        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CharacterController>();
    }

    void SpawnExtraLoot()
    {
        // Find existing pickups to use as a cloning base
        BatteryPickup existingBattery = FindObjectOfType<BatteryPickup>();
        if (existingBattery != null)
        {
            for (int i = 0; i < 8; i++)
            {
                // Cast a ray from high above the player downwards to find the actual ground
                Vector3 randomPosHigh = transform.position + new Vector3(Random.Range(-30f, 30f), 15f, Random.Range(-30f, 30f));
                if (Physics.Raycast(randomPosHigh, Vector3.down, out RaycastHit hit, 40f))
                {
                    // Spawn neatly on top of the ground
                    Instantiate(existingBattery.gameObject, hit.point + new Vector3(0, 0.5f, 0), existingBattery.transform.rotation);
                }
            }
        }
        
        AmmoPickup existingAmmo = FindObjectOfType<AmmoPickup>();
        if (existingAmmo != null)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector3 randomPosHigh = transform.position + new Vector3(Random.Range(-30f, 30f), 15f, Random.Range(-30f, 30f));
                if (Physics.Raycast(randomPosHigh, Vector3.down, out RaycastHit hit, 40f))
                {
                    Instantiate(existingAmmo.gameObject, hit.point + new Vector3(0, 0.5f, 0), existingAmmo.transform.rotation);
                }
            }
        }
    }

    void Update()
    {
        ProcessFootsteps();
    }

    void ProcessFootsteps()
    {
        bool isMoving = false;
        
        if (cc != null)
        {
            isMoving = cc.isGrounded && cc.velocity.magnitude > 0.1f;
        }
        else if (rb != null)
        {
            bool hasInput = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f;
            isMoving = rb.linearVelocity.magnitude > 0.1f && hasInput;
        }
        else
        {
            // Fallback if neither exists
            isMoving = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f;
        }

        if (isMoving)
        {
            float currentInterval = Input.GetKey(KeyCode.LeftShift) ? sprintStepInterval : walkStepInterval;
            
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = currentInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    void PlayFootstep()
    {
        if (footstepSounds != null && footstepSounds.Length > 0 && footstepSource != null)
        {
            AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
            if (clip != null)
            {
                footstepSource.pitch = Random.Range(0.9f, 1.1f);
                footstepSource.PlayOneShot(clip);
                footstepSource.pitch = 1f;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        hitPoints -= damage;
        if (hitPoints <= 0)
        {
            GetComponent<DeathHandler>().HandleDeath();
        }
    }
}
