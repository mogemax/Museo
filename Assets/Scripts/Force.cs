using UnityEngine;

public class Force : MonoBehaviour
{
    private float forceAmount = 50f;
    [SerializeField] private bool fuerzaHaciaDerecha = true;
    Rigidbody rb;

    void Start()
    {
        rb=GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 fDireccion = fuerzaHaciaDerecha ? Vector3.right : Vector3.left;
            //rb.AddForce(fDireccion * forceAmount * Time.time, ForceMode.Force);
            rb.AddForce(fDireccion * forceAmount, ForceMode.Force);
            Debug.Log("La velocidad es " + rb.linearVelocity);
        }
    }       
}