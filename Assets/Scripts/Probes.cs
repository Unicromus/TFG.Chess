using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Probes : MonoBehaviour
{
    public ReflectionProbe reflectionProbe;

    public void UpdateReflection()
    {
        if (reflectionProbe != null)
        {
            reflectionProbe.RenderProbe();
        }
    }
}

