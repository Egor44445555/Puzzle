using System.ComponentModel;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;
    
    Canvas canvas;

    void Awake()
    {
        if (main == null)
        {
            main = this;
        }
        else
        {
            Destroy(gameObject);
        }

        canvas = GetComponent<Canvas>();
    }

    public Canvas GetCanvas()
    {
        return canvas;
    }
}
