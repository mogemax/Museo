using UnityEngine;

public class ActivateLogic : MonoBehaviour
{
    [SerializeField] private GameObject triggerObject;
    [SerializeField] private GameObject objectToToggle;

    public bool IsObjectActive => objectToToggle != null && objectToToggle.activeSelf;

    void Start()
    {
        objectToToggle.SetActive(false);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        SetToggleState(other, true);
    }

    private void OnTriggerStay(Collider other)
    {
        SetToggleState(other, true);
    }

    private void OnTriggerExit(Collider other)
    {
        SetToggleState(other, false);
    }

    private void SetToggleState(Collider other, bool isInside)
    {
        if (triggerObject == null || objectToToggle == null || other == null)
        {
            return;
        }

        if (other.gameObject != triggerObject && !other.transform.IsChildOf(triggerObject.transform))
        {
            return;
        }

        objectToToggle.SetActive(isInside);
    }
}
