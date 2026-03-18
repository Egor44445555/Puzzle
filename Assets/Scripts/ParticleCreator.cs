using System.Collections;
using UnityEngine;

public class ParticleCreator : MonoBehaviour
{
    [SerializeField] GameObject particlePrefab;
    [SerializeField] float delay = 0f;
    [SerializeField] bool createOnStart = false;
    
    void Start()
    {
        if (createOnStart)
        {
            StartCoroutine(Timer());
        }
    }
    
    public void StartCreate()
    {
        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {        
        yield return new WaitForSeconds(delay);
        Instantiate(particlePrefab, transform.position, Quaternion.identity);
    }
}
