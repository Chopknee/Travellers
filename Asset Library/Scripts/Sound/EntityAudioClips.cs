using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAudioClips: MonoBehaviour {
    public AudioClip[] playerSoundClips;

    public AudioClip GetClip ( string name ) {
        foreach (AudioClip clip in playerSoundClips) {
            if (clip.name == name) {
                return clip;
            }
        }
        return null;
    }

}
