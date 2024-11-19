using UnityEngine;
using System.Collections;

public class AndroidTTS : MonoBehaviour
{
    private AndroidJavaObject ttsObject;

    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Debug.Log("[TTS] Initializing TTS...");
            // Initialize TTS object
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            ttsObject = new AndroidJavaObject("android.speech.tts.TextToSpeech", activity, new TTSListener());

            // Speak the welcome message with a delay after initialization
            StartCoroutine(SpeakAfterDelay(3f)); // A 3-second delay to ensure TTS is ready
        }
        else
        {
            Debug.LogWarning("[TTS] TTS is only supported on Android devices.");
        }
    }

    private IEnumerator SpeakAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpeakWelcomeMessage();
    }

    private void SpeakWelcomeMessage()
    {
        string message = "Welcome to Stephanie's Animal Rescue - Secret Island! " +
                         "Embark on a journey, to save endangered animals, and learn about wildlife conservation, in immersive, real-life locations. " +
                         "Team up with Flash and Ketch, to respond to rescue alerts, uncover hidden creatures, and protect Australia's unique biodiversity!";
        Debug.Log("[TTS] Speaking message: " + message);
        Speak(message);
    }

    public void Speak(string message)
    {
        if (Application.platform == RuntimePlatform.Android && ttsObject != null)
        {
            try
            {
                // Use the "QUEUE_FLUSH" constant from Android's TextToSpeech class
                AndroidJavaClass ttsClass = new AndroidJavaClass("android.speech.tts.TextToSpeech");
                int queueFlush = ttsClass.GetStatic<int>("QUEUE_FLUSH");

                // Create a HashMap for parameters
                AndroidJavaObject paramsMap = new AndroidJavaObject("java.util.HashMap");

                // Call the speak method with the correct parameters
                ttsObject.Call<int>("speak", message, queueFlush, paramsMap);
                Debug.Log("[TTS] Speak method successfully called.");
            }
            catch (AndroidJavaException ex)
            {
                Debug.LogError("[TTS] Speak method failed: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("[TTS] TTS object is not initialized or platform is not Android.");
        }
    }

    private class TTSListener : AndroidJavaProxy
    {
        public TTSListener() : base("android.speech.tts.TextToSpeech$OnInitListener") { }

        public void onInit(int status)
        {
            if (status == 0) // SUCCESS
            {
                Debug.Log("[TTS] TTS Initialized successfully.");
            }
            else
            {
                Debug.LogError("[TTS] TTS Initialization failed.");
            }
        }
    }
}
