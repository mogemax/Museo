using UnityEngine;
using System.Collections;

public class OnTriggerPause : MonoBehaviour
{
    private WaitForSeconds waitOneSecond = new WaitForSeconds(1f);
    private WaitForSeconds waitTwoSeconds = new WaitForSeconds(2f);

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Stop(other));
        }
    }
    IEnumerator Stop(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.attachedRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            other.attachedRigidbody.constraints = RigidbodyConstraints.None;
            other.attachedRigidbody.AddTorque(new Vector3(1f, 0f, 1f) * 500f, ForceMode.Force);
            yield return waitTwoSeconds;
            other.attachedRigidbody.useGravity = false;
            other.attachedRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            yield return waitOneSecond;
            other.attachedRigidbody.useGravity = true;
            other.attachedRigidbody.constraints = RigidbodyConstraints.None;
            other.attachedRigidbody.AddTorque(new Vector3(1f, 0f, 0f) * 500f, ForceMode.Force);
        }
    }
}
