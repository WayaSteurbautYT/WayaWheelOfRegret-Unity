using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Settings")]
    [Range(0f, 1f)] public float masterVolume = 0.3f;
    public bool musicEnabled = true;
    public bool sfxEnabled = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }

        UpdateVolumes();
    }

    #region Settings Management

    public void SetMusicEnabled(bool enabled)
    {
        musicEnabled = enabled;
        UpdateVolumes();
    }

    public void SetSfxEnabled(bool enabled)
    {
        sfxEnabled = enabled;
        UpdateVolumes();
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    private void UpdateVolumes()
    {
        if (musicSource != null)
        {
            musicSource.volume = musicEnabled ? masterVolume : 0f;
        }

        if (sfxSource != null)
        {
            sfxSource.volume = sfxEnabled ? masterVolume : 0f;
        }
    }

    #endregion

    #region Sound Effect Methods

    public void PlayTick()
    {
        PlayTone(800 + Random.Range(0, 401), 0.05f, WaveType.Square);
    }

    public void PlaySpinStart()
    {
        PlaySweepTone(200f, 300f, 0.3f, WaveType.Sawtooth);
    }

    public void PlaySpinStop()
    {
        PlaySweepTone(150f, 100f, 0.5f, WaveType.Triangle);
    }

    public void PlayReveal()
    {
        StartCoroutine(PlayArpeggio(new float[] { 523f, 659f, 784f, 1047f }, 0.1f, WaveType.Sine));
    }

    public void PlayClick()
    {
        PlayTone(600f, 0.08f, WaveType.Square, 0.2f);
    }

    public void PlayHover()
    {
        PlayTone(400f, 0.05f, WaveType.Sine, 0.1f);
    }

    public void PlayDoom()
    {
        PlaySweepTone(100f, 80f, 0.5f, WaveType.Sawtooth);
    }

    public void PlaySuccess()
    {
        StartCoroutine(PlayArpeggio(new float[] { 392f, 523f, 659f, 784f }, 0.1f, WaveType.Sine));
    }

    public void PlayStar()
    {
        PlayTone(880f, 0.1f, WaveType.Sine);
    }

    public void PlayLightning()
    {
        PlayTone(50 + Random.Range(0, 51), 0.1f, WaveType.Sawtooth);
    }

    public void PlayWhoosh()
    {
        PlaySweepTone(200f, 800f, 0.2f, WaveType.Sine);
    }

    // Special ending sounds
    public void PlayHammerSlam()
    {
        StartCoroutine(PlayHammerSlamSequence());
    }

    public void PlayPokeballCatch()
    {
        StartCoroutine(PlayArpeggio(new float[] { 523f, 659f, 784f, 523f }, 0.15f, WaveType.Sine));
    }

    public void PlayScpBreach()
    {
        StartCoroutine(PlayScpAlarmSequence());
    }

    public void PlayGlitch()
    {
        StartCoroutine(PlayGlitchSequence());
    }

    #endregion

    #region Audio Generation

    private enum WaveType { Sine, Square, Triangle, Sawtooth }

    private void PlayTone(float frequency, float duration, WaveType waveType, float volumeMultiplier = 1f)
    {
        if (!sfxEnabled) return;

        AudioClip clip = CreateToneClip(frequency, duration, waveType);
        sfxSource.PlayOneShot(clip, volumeMultiplier);
    }

    private void PlaySweepTone(float startFreq, float endFreq, float duration, WaveType waveType)
    {
        if (!sfxEnabled) return;

        AudioClip clip = CreateSweepToneClip(startFreq, endFreq, duration, waveType);
        sfxSource.PlayOneShot(clip);
    }

    private IEnumerator PlayArpeggio(float[] frequencies, float noteDuration, WaveType waveType)
    {
        if (!sfxEnabled) yield break;

        foreach (float freq in frequencies)
        {
            PlayTone(freq, noteDuration, waveType);
            yield return new WaitForSeconds(noteDuration * 0.8f);
        }
    }

    private IEnumerator PlayHammerSlamSequence()
    {
        if (!sfxEnabled) yield break;

        // First hit (60Hz)
        PlayTone(60f, 0.4f, WaveType.Square, 0.8f);
        yield return new WaitForSeconds(0.2f);
        
        // Second hit (40Hz)
        PlayTone(40f, 0.6f, WaveType.Sawtooth, 0.8f);
    }

    private IEnumerator PlayScpAlarmSequence()
    {
        if (!sfxEnabled) yield break;

        for (int i = 0; i < 3; i++)
        {
            PlayTone(400f, 0.2f, WaveType.Square, 0.5f);
            yield return new WaitForSeconds(0.2f);
            PlayTone(300f, 0.2f, WaveType.Square, 0.5f);
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator PlayGlitchSequence()
    {
        if (!sfxEnabled) yield break;

        for (int i = 0; i < 5; i++)
        {
            float freq = Random.Range(200f, 1200f);
            PlayTone(freq, 0.03f, WaveType.Square, 0.3f);
            yield return new WaitForSeconds(Random.Range(0.01f, 0.05f));
        }
    }

    private AudioClip CreateToneClip(float frequency, float duration, WaveType waveType)
    {
        int sampleRate = AudioSettings.outputSampleRate;
        int samples = (int)(sampleRate * duration);
        AudioClip clip = AudioClip.Create("Tone", samples, 1, sampleRate, false);

        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float phase = 2f * Mathf.PI * frequency * t;
            
            data[i] = waveType switch
            {
                WaveType.Sine => Mathf.Sin(phase),
                WaveType.Square => Mathf.Sign(Mathf.Sin(phase)),
                WaveType.Triangle => 2f * Mathf.Abs(2f * (t * frequency % 1f) - 1f) - 1f,
                WaveType.Sawtooth => 2f * (t * frequency % 1f) - 1f,
                _ => 0f
            };
        }

        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip CreateSweepToneClip(float startFreq, float endFreq, float duration, WaveType waveType)
    {
        int sampleRate = AudioSettings.outputSampleRate;
        int samples = (int)(sampleRate * duration);
        AudioClip clip = AudioClip.Create("Sweep", samples, 1, sampleRate, false);

        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            float currentFreq = Mathf.Lerp(startFreq, endFreq, t);
            float phase = 2f * Mathf.PI * currentFreq * t * duration;
            
            data[i] = waveType switch
            {
                WaveType.Sine => Mathf.Sin(phase),
                WaveType.Square => Mathf.Sign(Mathf.Sin(phase)),
                WaveType.Triangle => 2f * Mathf.Abs(2f * (t * currentFreq % 1f) - 1f) - 1f,
                WaveType.Sawtooth => 2f * (t * currentFreq % 1f) - 1f,
                _ => 0f
            };
        }

        clip.SetData(data, 0);
        return clip;
    }

    #endregion
}
