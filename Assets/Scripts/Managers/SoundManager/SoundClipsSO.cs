using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create SoundsData/AllSoundClips")]
public class SoundClipsSO : ScriptableObject
{
    public SoundManager.SoundAudioClip[] soundAudioClips;
}
