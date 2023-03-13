using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using Cysharp.Threading.Tasks;
using Audio;
using WhisperAPI;
using WhisperAPI.Models;
using ChatGPTAPI;
using ChatGPTAPI.Models;
using ChatGPTAPI.Config;

public class AIAssistantManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private Text recordingText;
    [SerializeField] private Text outputText;
    
    [Header("AIの人格設定")]
    [SerializeField] private SystemProfile systemProfile;
    [SerializeField] private UserProfile userProfile;
    
    private AudioClip _recordedClip;
    private const string MicName = "マイク (4- USB-LCS Audio)"; //マイクデバイスの名前
    private const int SamplingFrequency = 44100; //サンプリング周波数
    private const int MaxTimeSeconds = 10; //最大録音時間[s]
    
    private bool _isRecording = false;

    private CancellationTokenSource _cts = new();
    private CancellationToken _token;

    private WhisperAPIConnection _whisperConnection;
    private ChatGPTConnection _chatConnection;
    
    void Start()
    {
        // ShowAllMicDevices();
        
        _token = _cts.Token;
        _whisperConnection = new(Constants.API_KEY);
        _chatConnection = new(Constants.API_KEY);
        _chatConnection.AddSystemProfile(systemProfile);
        _chatConnection.AddUserProfile(userProfile);
        
        startButton.onClick.AddListener(StartRecording);
        stopButton.onClick.AddListener(StopRecording);
        
        SetUIByIsRecording();
    }

    private void StartRecording()
    {
        Debug.Log("recording start");
        _isRecording = true;
        SetUIByIsRecording();
        _recordedClip = Microphone.Start(deviceName: MicName, loop: false, lengthSec: MaxTimeSeconds, frequency: SamplingFrequency);
    }

    private async void StopRecording()
    {
        if (Microphone.IsRecording(deviceName: MicName))
        {
            Debug.Log("recording stopped");
            Microphone.End(deviceName: MicName);
        }
        else
        {
            Debug.Log("not recording");
            return;
        }
        
        _isRecording = false;
        SetUIByIsRecording();

        byte[] recordWavData = WavConverter.ToWav(_recordedClip);
        
        // WhisperAPI
        string responseText = await DisplayWhisperResponse(recordWavData);
        
        // ChatGPTAPI
        DisplayChatGPTResponse(responseText);
    }

    private async UniTask<string> DisplayWhisperResponse(byte[] recordData)
    {
        WhisperAPIResponseModel responseModel = await _whisperConnection.RequestWithVoiceAsync(recordData, _token);
        outputText.text += "自分: " + responseModel.text + Environment.NewLine;
        return responseModel.text;
    }
    
    private async UniTaskVoid DisplayChatGPTResponse(string text)
    {
        ChatGPTResponseModel responseModel = await _chatConnection.RequestAsync(text, _token);
        outputText.text += "先輩: " + responseModel.choices[0].message.content + Environment.NewLine;
        
        // 次の会話の前に1行開ける
        outputText.text += Environment.NewLine;
    }
    
    private void SetUIByIsRecording()
    {
        startButton.gameObject.SetActive(!_isRecording);
        stopButton.gameObject.SetActive(_isRecording);
        recordingText.gameObject.SetActive(_isRecording);
    }

    /// <summary>
    /// 自分が使用中のマイクを特定する際に利用
    /// </summary>
    private void ShowAllMicDevices()
    {
        foreach (string device in Microphone.devices)
        {
            Debug.Log("Name: " + device);
        }
    }
}