using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UText : MonoBehaviour
{
    public Rigidbody rb;
    public TextMeshProUGUI textVelocity;


    // Update is called once per frame
    void Update()
    {
        textVelocity.text = rb.linearVelocity.ToString();
    }
}
