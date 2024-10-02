using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vocabulary : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;
    public GameObject panel_Recording;

    [Header("Obj Vocabulary")]
    public Text txt_Vocabulary;
    public Text txt_Status;
    public GameObject panel_vocabulary;

    [Header("Audio")]
    public AudioSource audioSource_Recording;
    public AudioSource audioSource_Speech;
    private AudioClip recordedClip;
    private AudioClip speechClip;

    private bool isRecording = false;
    private string microphoneName;

    public void On_Load()
    {
        this.panel_vocabulary.SetActive(false);
        this.panel_Recording.SetActive(false);
        if (Microphone.devices.Length > 0)
        {
            microphoneName = Microphone.devices[0];
        }
        else
        {
            this.txt_Status.text="No microphone detected!";
        }
    }

    public void On_Show(string s_Vocabulary)
    {
        this.txt_Vocabulary.text = s_Vocabulary;
        this.panel_vocabulary.SetActive(true);

        this.speechClip = Resources.Load<AudioClip>("voice/" + s_Vocabulary);
    }

    public void Close()
    {
        this.panel_vocabulary.SetActive(false);
    }

    private void compare_voice(AudioClip recordedClip, AudioClip originalClip)
    {
        float[] recordedSpectrum = new float[256];
        float[] originalSpectrum = new float[256];

        audioSource_Recording.clip = recordedClip;
        audioSource_Recording.Play();
        audioSource_Recording.GetSpectrumData(recordedSpectrum, 1, FFTWindow.Hanning);


        audioSource_Speech.clip = originalClip;
        audioSource_Speech.Play();
        audioSource_Speech.GetSpectrumData(originalSpectrum, 1, FFTWindow.Hanning);

        float similarity = 0f;
        for (int i = 0; i < recordedSpectrum.Length; i++)
        {
            Debug.Log("r"+recordedSpectrum[i]);
            Debug.Log("o"+originalSpectrum[i]);
            similarity += Mathf.Abs(recordedSpectrum[i] - originalSpectrum[i]);
        }

        this.txt_Status.text = "Return:" + similarity;
    }

    private void StartRecording(int duration = 10)
    {
        if (!isRecording && Microphone.devices.Length > 0)
        {
            recordedClip = Microphone.Start(microphoneName, false, duration, 44100);
            isRecording = true;
            this.txt_Status.text = "Recording started...";
        }
        else
        {
            this.txt_Status.text = "Recording already in progress or no microphone found.";
        }
    }

    private void StopRecording()
    {
        if (isRecording)
        {
            Microphone.End(microphoneName);
            isRecording = false;
            this.txt_Status.text = "Recording stopped.";
        }
    }

    private void PlayRecording()
    {
        if (recordedClip != null)
        {
            this.audioSource_Recording.clip = recordedClip;
            this.audioSource_Recording.Play();
            this.txt_Status.text = "Playing recorded audio...";
        }
        else
        {
            this.txt_Status.text = "No recorded audio found!";
        }
    }


    private bool CheckInternetConnection()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return false;
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            return true;
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            return true;
        }
        else
        {
            return true;
        }
    }

    public void On_start_check_voice()
    {
        this.panel_Recording.SetActive(true);
        this.StartRecording(5);
    }

    public void On_stop_check_voice()
    {
        this.app.play_sound();
        this.panel_Recording.SetActive(false);
        this.StopRecording();
        this.compare_voice(this.RemoveNose(this.recordedClip), this.speechClip);
    }

    public void btn_On_play_voice()
    {
        this.audioSource_Speech.clip = this.speechClip;
        this.audioSource_Speech.Play();
    }

    private AudioClip RemoveNose(AudioClip clip)
    {
        float silenceThreshold = 0.01f;
        float[] trimmedSamples;

        trimmedSamples = RemoveSilence(clip, silenceThreshold);
        AudioClip trimmedClip = AudioClip.Create("TrimmedClip", trimmedSamples.Length, audioSource_Recording.clip.channels, audioSource_Recording.clip.frequency, false);
        trimmedClip.SetData(trimmedSamples, 0);
        return trimmedClip;
    }

    float[] RemoveSilence(AudioClip clip, float threshold)
    {
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        int startSample = 0;
        int endSample = samples.Length - 1;

        for (int i = 0; i < samples.Length; i++)
        {
            if (Mathf.Abs(samples[i]) > threshold)
            {
                startSample = i;
                break;
            }
        }

        for (int i = samples.Length - 1; i >= 0; i--)
        {
            if (Mathf.Abs(samples[i]) > threshold)
            {
                endSample = i;
                break;
            }
        }

        int trimmedLength = endSample - startSample + 1;
        float[] trimmedSamples = new float[trimmedLength];
        System.Array.Copy(samples, startSample, trimmedSamples, 0, trimmedLength);

        return trimmedSamples;
    }
}
