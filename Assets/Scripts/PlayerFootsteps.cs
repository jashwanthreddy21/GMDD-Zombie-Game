using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    AudioClip[] footstepSounds;
    [SerializeField] float walkStepInterval = 0.5f;
    [SerializeField] float sprintStepInterval = 0.3f;

    AudioSource audioSource;
    Rigidbody rb;
    CharacterController cc;
    float stepTimer;

    void Start()
    {
        footstepSounds = new AudioClip[] {
            Resources.Load<AudioClip>("footstep_1"),
            Resources.Load<AudioClip>("footstep_2")
        };
        
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 0f; // 2D sound for the player
        }
    }

    void Update()
    {
        bool isMoving = false;
        
        if (cc != null)
        {
            isMoving = cc.isGrounded && cc.velocity.magnitude > 0.1f;
        }
        else if (rb != null)
        {
            // Simple check to ensure we only play sound when intentionally moving
            bool hasInput = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f;
            isMoving = rb.linearVelocity.magnitude > 0.1f && hasInput;
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
            stepTimer = 0f; // Reset timer when stopped
        }
    }

    void PlayFootstep()
    {
        if (footstepSounds.Length > 0 && audioSource != null)
        {
            AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
            // Vary the pitch slightly for realism
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(clip);
            audioSource.pitch = 1f; // Reset pitch
        }
    }
}
