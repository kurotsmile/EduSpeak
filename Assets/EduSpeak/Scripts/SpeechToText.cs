using KKSpeech;
using UnityEngine;
using UnityEngine.Windows.Speech;
public class SpeechToText : MonoBehaviour
{
    [Header("Main Object")]
    public App app;

    private DictationRecognizer dictationRecognizer;
    public void On_Load()
    {
        if(this.app.carrot.os_app==Carrot.OS.Window){
            dictationRecognizer = new DictationRecognizer();
            dictationRecognizer.DictationResult += (text, confidence) => {
                Debug.LogFormat("Content: {0}, level: {1}", text, confidence);
                this.app.v.OnFinalResult(text);
            };

            dictationRecognizer.DictationComplete += (completionCause) => {};
        }else{
            if (SpeechRecognizer.ExistsOnDevice())
            {
                SpeechRecognizerListener listener = FindAnyObjectByType<SpeechRecognizerListener>();
                listener.onAuthorizationStatusFetched.AddListener(this.OnAuthorizationStatusFetched);
                listener.onFinalResults.AddListener(this.app.v.OnFinalResult);
                SpeechRecognizer.RequestAccess();
            }
            else
            {
                this.app.v.txt_Status.text = "Sorry, but this device doesn't support speech recognition";
            }
            SpeechRecognizer.SetDetectionLanguage("en-US");
        }
    }

    public void StartRecording()
    {
        if(this.app.carrot.os_app==Carrot.OS.Window)
            dictationRecognizer.Start();
        else
            SpeechRecognizer.StartRecording(true);
    }

    public void StopRecording()
    {
        if(this.app.carrot.os_app==Carrot.OS.Window){
            if (dictationRecognizer != null)
            {
                dictationRecognizer.Stop();
                //dictationRecognizer.Dispose();
            }
        }else{
            SpeechRecognizer.StopIfRecording();
        }
    }

    #region voice
    public void OnAuthorizationStatusFetched(AuthorizationStatus status)
    {
        switch (status)
        {
            case AuthorizationStatus.Authorized:
                break;
            default:
                this.app.v.txt_Status.text = "Cannot use Speech Recognition, authorization status is " + status;
                break;
        }
    }
    #endregion
}