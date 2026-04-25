using UnityEngine;

public class TorqueForce : MonoBehaviour
{
    private float torqueAmount = 100f;
    Rigidbody rb;
    Vector3 torqueDireccion = new Vector3(1f, 0f, 0f);
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            rb.AddTorque(torqueDireccion * torqueAmount, ForceMode.Force);
            Debug.Log("La velocidad angular es " + rb.angularVelocity);
        }
    }
}
