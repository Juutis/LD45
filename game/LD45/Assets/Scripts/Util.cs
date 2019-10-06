using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static AudioClip getRandom(AudioClip[] clips)
    {
        return clips[Random.Range(0, clips.Length - 1)];
    }

}
