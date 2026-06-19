using System;
using System.Collections.Generic;
using FishNet.Object.Synchronizing;

namespace Core.Save
{
    [Serializable]
    public class HeroModel
    {
        public string HeroId;
        
        public string Name;
        public int Health;
        public int Armor;
        public bool InBattle;

        public HeroStats Stats;
    
        public Dictionary<int, int> ResourcesStorage = new();

        public List<string> CardCollection; // все карты которые есть у игрока (id)
        public List<string> Deck;           // собранная колода (id), макс 30
        public List<SavedDeckDefinition> Decks;
        public string SelectedDeckId;
        
        public TimeSpan CreateTime;
        public TimeSpan GameTime;
        public TimeSpan ExitTime;
        
        
        public HeroModel()
        {
            Name = "name";
            Health = 100;

            ResourcesStorage = new Dictionary<int, int>()
            {
                { 1, 100 }, // gold
                { 2, 0 }, // starwberry
                { 3, 0 }, // cristal
            };

            GameTime = new TimeSpan();
            ExitTime = new TimeSpan();

            Stats = new HeroStats();

            CardCollection = new List<string>();
            Deck = new List<string>();
            Decks = new List<SavedDeckDefinition>();
            SelectedDeckId = string.Empty;
        }
    }
    
    [Serializable]
    public class HeroStats
    {
        public int Agility;    
        public int Strength;   
        public int Endurance;  
        public int Intellect;  
    }
}