using UnityEngine;

public static class SoundFX
{
    // A sound that plays the same everywhere (UI, pickups)
    public static void Play2D(AudioClip clip, float volume = 1f, float pitchVariation = 0f)
    {
        if (!clip) return;
        AudioSource s = MakeSource(clip, volume, pitchVariation);
        s.spatialBlend = 0f;
        s.Play();
    }

    // A sound that comes from a place in the world (quieter further away)
    public static void Play3D(AudioClip clip, Vector3 position, float volume = 1f, float pitchVariation = 0f)
    {
        if (!clip) return;
        AudioSource s = MakeSource(clip, volume, pitchVariation);
        s.transform.position = position;
        s.spatialBlend = 1f;
        s.rolloffMode = AudioRolloffMode.Linear;
        s.minDistance = 1f;
        s.maxDistance = 20f;
        s.Play();
    }

    static AudioSource MakeSource(AudioClip clip, float volume, float pitchVariation)
    {
        GameObject go = new GameObject("SFX_" + clip.name);
        AudioSource s = go.AddComponent<AudioSource>();
        s.clip = clip;
        s.volume = volume;
        s.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        Object.Destroy(go, clip.length / Mathf.Max(0.1f, s.pitch) + 0.1f);
        return s;
    }
}