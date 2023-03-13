using UnityEngine;

namespace ChatGPTAPI.Config
{
    [CreateAssetMenu(fileName = "SystemProfile", menuName = "ChatGPTAPI/SystemProfile", order = 1)]
    public class SystemProfile : ScriptableObject
    { 
        [TextArea] public string systemContent;
    }
}