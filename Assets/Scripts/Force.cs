using UnityEngine;

public class Force : MonoBehaviour
{
    [SerializeField] private Rigidbody[] targetRigidbodies;
    [SerializeField] private float forceAmount = 50f;
    [SerializeField] private bool fuerzaHaciaDerecha = true;
    [SerializeField] private KeyCode forceKey = KeyCode.Space;

    private void Awake()
    {
        if (targetRigidbodies == null || targetRigidbodies.Length == 0)
        {
            Rigidbody ownRigidbody = GetComponent<Rigidbody>();
            if (ownRigidbody != null)
            {
                targetRigidbodies = new Rigidbody[] { ownRigidbody };
            }
        }
    }

    private void Update()
    {
        if (!isActiveAndEnabled)
        {
            return;
        }

        if (!Input.GetKeyDown(forceKey))
        {
            return;
        }

        ApplyForceToRigidbodies(targetRigidbodies);
    }

    public void ApplyForceToObjects(params GameObject[] objects)
    {
        if (!isActiveAndEnabled || objects == null)
        {
            return;
        }

        for (int i = 0; i < objects.Length; i++)
        {
            GameObject currentObject = objects[i];
            if (currentObject == null)
            {
                continue;
            }

            if (currentObject.TryGetComponent(out Rigidbody currentRigidbody))
            {
                ApplyForce(currentRigidbody);
            }
        }
    }

    public void ApplyForceToRigidbodies(params Rigidbody[] rigidbodies)
    {
        if (!isActiveAndEnabled || rigidbodies == null)
        {
            return;
        }

        for (int i = 0; i < rigidbodies.Length; i++)
        {
            Rigidbody currentRigidbody = rigidbodies[i];
            if (currentRigidbody == null)
            {
                continue;
            }

            ApplyForce(currentRigidbody);
        }
    }

    private void ApplyForce(Rigidbody currentRigidbody)
    {
        Vector3 forceDirection = fuerzaHaciaDerecha ? Vector3.right : Vector3.left;
        currentRigidbody.AddForce(forceDirection * forceAmount, ForceMode.Force);
        Debug.Log("La velocidad es " + currentRigidbody.linearVelocity);
    }
}