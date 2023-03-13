using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ChatGPTAPI.Models;
using ChatGPTAPI.Config;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ChatGPTAPI
{
    public class ChatGPTConnection
    {
        private readonly string _apiKey;
        private readonly List<ChatGPTMessageModel> _messageList = new();
        private const string ApiUrl = "https://api.openai.com/v1/chat/completions";
        
        public ChatGPTConnection(string apiKey)
        {
            _apiKey = apiKey;
        }
        
        public void AddSystemProfile(SystemProfile profile)
        {
            var model = new ChatGPTMessageModel()
            {
                role = Role.system.ToString(),
                content = profile.systemContent
            };
            AddMessage(model);
        }
        
        public void AddUserProfile(UserProfile profile)
        {
            var model = new ChatGPTMessageModel()
            {
                role = Role.user.ToString(),
                content = $"自己紹介をします！私の名前は{profile.nickName}です。" +
                          $"年齢は{profile.age}歳です。" +
                          $"性別は{profile.sex}です。" +
                          $"生まれた場所は{profile.birthPlace}です。" +
                          $"仕事は{profile.job}です。" + 
                          $"今やっているタスクは{profile.currentTask}です。" +
                          $"趣味は{profile.hobby}です。" +
                          $"好きな食べ物は{profile.favoriteFood}です。" + 
                          $"好きな色は{profile.favoriteColor}です。" +
                          $"好きな動物は{profile.favoriteAnimal}です。" +
                          $"好きな季節は{profile.favoriteSeason}です。" +
                          $"好きなゲームは{profile.favoriteGame}です。" +
                          $"好きな映画は{profile.favoriteMovie}です。" +
                          $"好きな音楽は{profile.favoriteMusic}です。" +
                          $"好きな本は{profile.favoriteBook}です。" +
                          $"好きな場所は{profile.favoritePlace}です。"
            };
            AddMessage(model);
        }

        private void AddMessage(ChatGPTMessageModel model)
        {
            _messageList.Add(model);
        }

        public async UniTask<ChatGPTResponseModel> RequestAsync(string userMessage, CancellationToken token)
        {
            Debug.Log("自分: " + userMessage);
            
            _messageList.Add(new ChatGPTMessageModel()
            {
                role = "user",
                content = userMessage
            });

            var headers = new Dictionary<string, string>
            {
                { "Authorization", "Bearer " + _apiKey },
                { "Content-Type", "application/json" }
            };

            var options = new ChatGPTCompleteRequestModel()
            {
                model = "gpt-3.5-turbo",
                messages = _messageList
            };
            var jsonOptions = JsonUtility.ToJson(options);

            UnityWebRequest request = new UnityWebRequest(ApiUrl, "POST")
            {
                uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonOptions)),
                downloadHandler = new DownloadHandlerBuffer()
            };

            foreach (var header in headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }

            await request.SendWebRequest().ToUniTask(cancellationToken: token);
            
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                request.Dispose();
                throw new Exception();
            }
            else
            {
                var responseString = request.downloadHandler.text;
                var responseObject = JsonUtility.FromJson<ChatGPTResponseModel>(responseString);
                Debug.Log("ChatGPT: " + responseObject.choices[0].message.role);
                Debug.Log("ChatGPT: " + responseObject.choices[0].message.content);
                _messageList.Add(responseObject.choices[0].message);
                request.Dispose();
                return responseObject;
            }
        }
    }
}
