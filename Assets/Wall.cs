using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public GameObject newwall;
    public GameObject oldwall;

    // Start is called before the first frame update
    void Start()
    {
        newwall.SetActive(false);
        oldwall.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon") || other.CompareTag("Item"))
        {
            newwall.SetActive(true);
            oldwall.SetActive(false);
            Destroy(newwall, 6);
        }
    }
}
