using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartTurnOffDanger()
    {
        StartCoroutine(TurnOffDanger());
    }

    private IEnumerator TurnOffDanger()
    {
        yield return new WaitForSeconds(3);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
