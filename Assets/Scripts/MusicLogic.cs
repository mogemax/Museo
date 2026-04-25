using UnityEngine;

public class MusicLogic : MonoBehaviour
{
    [SerializeField] private AudioClip levelCompletedClip;

    private Rigidbody rb;
    private AudioSource audioSource;
    private bool wasMoving = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        bool isMoving = rb.linearVelocity.magnitude > 0.1f;

        if (isMoving && !wasMoving)
        {
            audioSource.PlayOneShot(levelCompletedClip);
        }

        wasMoving = isMoving;
    }
}
