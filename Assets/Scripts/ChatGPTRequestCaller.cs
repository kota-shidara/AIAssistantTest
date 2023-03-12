using System;
using System.Threading;
using UnityEngine;
using ChatGPTAPI;
using ChatGPTAPI.Models;
using UnityEngine.UI;

public class ChatGPTRequestCaller : MonoBehaviour
{
    [SerializeField] private InputField inputText;
    [SerializeField] private Button submitButton;
    [SerializeField] private Text outputText;
    
    private CancellationTokenSource _cts = new();
    private CancellationToken _token;
    
    private ChatGPTConnection _chatConnection;

    void Start()
    {
        _token = _cts.Token;
        _chatConnection = new(Constants.API_KEY);
        submitButton.onClick.AddListener(RequestAndChangeText);
    }

    private async void RequestAndChangeText()
    {
        outputText.text += "自分: " + inputText.text + Environment.NewLine;

        // ChatGPTにリクエストを送信
        ChatGPTResponseModel responseModel = await _chatConnection.RequestAsync(inputText.text, _token);
        outputText.text += "AI: " + responseModel.choices[0].message.content + Environment.NewLine;
        outputText.text += Environment.NewLine;

        // 入力欄を空にする
        inputText.text = "";
    }
}
