using System;
using System.Collections.Generic;

namespace ChatGPTAPI.Models
{
    [Serializable]
    public class ChatGPTCompleteRequestModel
    {
        public string model;
        public List<ChatGPTMessageModel> messages;
    }
}