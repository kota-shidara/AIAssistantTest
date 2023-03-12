using System;

namespace ChatGPTAPI.Models
{
    // ここだけ本当は[Serializable]が必要な模様
    [Serializable]
    public class ChatGPTMessageModel
    {
        public string role;
        public string content;
    }
}