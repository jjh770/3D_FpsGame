using System.Collections;
using UnityEngine;

public class CoroutineExample_2 : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(1.6f);
        // Reload()
    }

}
