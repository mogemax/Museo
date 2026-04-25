using UnityEngine;

public class Force : MonoBehaviour
{
    private float forceAmount = 500f;
    Rigidbody rb;
    Vector3 fDireccion = new Vector3 (0f, 0f, 1f);
    void Start()
    {
        rb=GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //rb.AddForce(fDireccion * forceAmount * Time.time, ForceMode.Force);
            rb.AddForce(fDireccion * forceAmount, ForceMode.Force);
            Debug.Log("La velocidad es " + rb.linearVelocity);
        }
    }       
}