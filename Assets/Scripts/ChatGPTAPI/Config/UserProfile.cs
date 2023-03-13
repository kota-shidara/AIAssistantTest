using UnityEngine;

namespace ChatGPTAPI.Config
{
    [CreateAssetMenu(fileName = "UserProfile", menuName = "ChatGPTAPI/UserProfile", order = 2)]
    public class UserProfile : ScriptableObject
    {
        public string nickName;
        public string age;
        public string sex;
        public string birthPlace;
        public string job;
        public string currentTask;
        public string hobby;
        public string favoriteFood;
        public string favoriteColor;
        public string favoriteAnimal;
        public string favoriteSeason;
        public string favoriteGame;
        public string favoriteMovie;
        public string favoriteMusic;
        public string favoriteBook;
        public string favoritePlace;
    }
}