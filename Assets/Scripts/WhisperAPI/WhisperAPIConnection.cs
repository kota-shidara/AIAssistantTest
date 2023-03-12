using System;
using Cysharp.Threading.Tasks;
using WhisperAPI.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace WhisperAPI
{
    public class WhisperAPIConnection
    {
        private readonly string _apiKey;
        private const string ApiUrl = "https://api.openai.com/v1/audio/transcriptions";
        private const string FilePath = "path/to/sampleFile";
        private const string ModelName = "whisper-1";
        
        public WhisperAPIConnection(string apiKey)
        {
            _apiKey = apiKey;
        }

        /// <summary>
        /// ローカルに保存されているファイルを読み込むタイプ
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async UniTask<WhisperAPIResponseModel> RequestAsync(CancellationToken token)
        {
            var headers = new Dictionary<string, string>
            {
                { "Authorization", "Bearer " + _apiKey }
            };

            byte[] fileBytes = await File.ReadAllBytesAsync(FilePath, cancellationToken: token);

            // 下の★とどちらでも正しく処理される
            // WWWForm form = new();
            // form.AddField("model", ModelName);
            // form.AddBinaryData("file", fileBytes, Path.GetFileName(FilePath), "multipart/form-data");

            // 上の★とどちらでも正しく処理される
            List<IMultipartFormSection> form = new();
            form.Add(new MultipartFormDataSection("model", ModelName));
            form.Add(new MultipartFormFileSection("file", fileBytes, Path.GetFileName(FilePath), "multipart/form-data"));

            using UnityWebRequest request = UnityWebRequest.Post(ApiUrl, form);
            
            foreach (var header in headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }
            
            await request.SendWebRequest().ToUniTask(cancellationToken: token);
            
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                throw new Exception();
            }
            else
            {
                var responseString = request.downloadHandler.text;
                var responseObject = JsonUtility.FromJson<WhisperAPIResponseModel>(responseString);
                Debug.Log("WhisperAPI: " + responseObject.text);
                return responseObject;
            }
        }
        
        /// <summary>
        /// wavデータをメソッド自体に渡すタイプ。
        /// </summary>
        /// <param name="recordData"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async UniTask<WhisperAPIResponseModel> RequestWithVoiceAsync(byte[] recordData, CancellationToken token)
        {
            var headers = new Dictionary<string, string>
            {
                { "Authorization", "Bearer " + _apiKey }
            };

            // 上の★とどちらでも正しく処理される
            List<IMultipartFormSection> form = new();
            form.Add(new MultipartFormDataSection("model", ModelName));
            form.Add(new MultipartFormFileSection("file", recordData, "recordData.wav", "multipart/form-data"));

            using UnityWebRequest request = UnityWebRequest.Post(ApiUrl, form);
            
            foreach (var header in headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }
            
            await request.SendWebRequest().ToUniTask(cancellationToken: token);
            
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                throw new Exception();
            }
            else
            {
                var responseString = request.downloadHandler.text;
                var responseObject = JsonUtility.FromJson<WhisperAPIResponseModel>(responseString);
                Debug.Log("WhisperAPI: " + responseObject.text);
                return responseObject;
            }
        }
    }
}