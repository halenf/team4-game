using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLaserWarning : MonoBehaviour
{
    public float displayTime;
    
    private void Start()
    {
        // when the object is enabled, start a timer to turn it off
        StartCoroutine(TurnOffDanger());
    }

    private IEnumerator TurnOffDanger()
    {
        yield return new WaitForSeconds(displayTime);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
