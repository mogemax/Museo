using UnityEngine;
using System.Collections.Generic;

public class Duplicator : MonoBehaviour
{
    public GameObject clonePrefab; // Prefab of the object to clone
    public Vector3 spawnPoint; // Point where the clone will be spawned
    public Vector3 spawnRotationEuler = Vector3.zero; // Rotation (X,Y,Z) in degrees for spawned clones
    public float xSpacing = 5f;
    public float zSpacing = 2f;
    private readonly List<GameObject> clones = new List<GameObject>(); // References to all spawned clones

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            SpawnClonesInRows();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            DeactivateClones();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            DestroyScriptsInClones();
        }
    }

    private void SpawnClonesInRows()
    {
        Vector3 currentRowStart = spawnPoint;
        int totalRows = 3;
        int clonesPerRow = 2;
        int totalClones = totalRows * clonesPerRow;

        for (int i = 0; i < totalRows; i++)
        {
            for (int j = 0; j < clonesPerRow; j++)
            {
                int cloneIndex = (i * clonesPerRow) + j;
                Vector3 clonePosition = currentRowStart + new Vector3(j * xSpacing, 0, 0);
                Vector3 cloneEuler = spawnRotationEuler;
                if (cloneIndex >= totalClones - 3)
                {
                    cloneEuler.y += 180f;
                }

                Quaternion cloneRotation = Quaternion.Euler(cloneEuler);
                GameObject clone = Instantiate(clonePrefab, clonePosition, cloneRotation);
                clones.Add(clone);
            }
            currentRowStart += new Vector3(0, 0, zSpacing);
        }
    }

    private void DeactivateClones()
    {
        for (int i = clones.Count - 1; i >= 0; i--)
        {
            if (clones[i] == null)
            {
                clones.RemoveAt(i);
                continue;
            }

            clones[i].SetActive(false);
        }
    }

    private void DestroyScriptsInClones()
    {
        for (int i = clones.Count - 1; i >= 0; i--)
        {
            if (clones[i] == null)
            {
                clones.RemoveAt(i);
                continue;
            }

            MonoBehaviour[] scripts = clones[i].GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                if (script != null)
                {
                    Destroy(script);
                }
            }
        }
    }

    // Assign this method to the UI button with X shape via Inspector -> OnClick().
    public void DestroyAllClones()
    {
        for (int i = clones.Count - 1; i >= 0; i--)
        {
            if (clones[i] != null)
            {
                Destroy(clones[i]);
            }
        }

        clones.Clear();
    }
}
