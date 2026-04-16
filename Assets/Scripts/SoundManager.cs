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

    private void UpdateVolumes()
    {
        if (musicSource != null) musicSource.volume = musicEnabled ? masterVolume : 0f;
        if (sfxSource != null) sfxSource.volume = sfxEnabled ? masterVolume : 0f;
    }

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

    public void PlaySuccess()
    {
        StartCoroutine(PlayArpeggio(new float[] { 392f, 523f, 659f, 784f }, 0.1f, WaveType.Sine));
    }

    public void PlaySuccessSound()
    {
        PlaySuccess();
    }

    public void PlayRegretSound()
    {
        StartCoroutine(PlayArpeggio(new float[] { 784f, 659f, 523f, 392f }, 0.15f, WaveType.Sawtooth));
    }

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
}
