using System;
using System.Collections.Generic;

namespace SuperGamblino.GameObjects
{
    public class Deck
    {
        public enum CardType
        {
            Club,
            Diamond,
            Heart,
            Spade
        }

        public List<Card> cards = new List<Card>();

        public void GenerateDeck()
        {
            for (var i = 0; i < 13; i++) cards.Add(CreateCard(i + 1, CardType.Club));
            for (var i = 0; i < 13; i++) cards.Add(CreateCard(i + 1, CardType.Diamond));
            for (var i = 0; i < 13; i++) cards.Add(CreateCard(i + 1, CardType.Heart));
            for (var i = 0; i < 13; i++) cards.Add(CreateCard(i + 1, CardType.Spade));
            Console.WriteLine(cards);
        }

        private Card CreateCard(int value, CardType type)
        {
            var newCard = new Card();
            newCard.type = type;

            switch (value)
            {
                case 1:
                    newCard.name = "Ace of " + type + "s";
                    newCard.value = value;
                    break;
                case 2:
                    newCard.name = "Two of " + type + "s";
                    newCard.value = value;
                    break;
                case 3:
                    newCard.name = "Three of " + type + "s";
                    newCard.value = value;
                    break;
                case 4:
                    newCard.name = "Four of " + type + "s";
                    newCard.value = value;
                    break;
                case 5:
                    newCard.name = "Five of " + type + "s";
                    newCard.value = value;
                    break;
                case 6:
                    newCard.name = "Six of " + type + "s";
                    newCard.value = value;
                    break;
                case 7:
                    newCard.name = "Seven of " + type + "s";
                    newCard.value = value;
                    break;
                case 8:
                    newCard.name = "Eight of " + type + "s";
                    newCard.value = value;
                    break;
                case 9:
                    newCard.name = "Nine of " + type + "s";
                    newCard.value = value;
                    break;
                case 10:
                    newCard.name = "Ten of " + type + "s";
                    newCard.value = value;
                    break;
                case 11:
                    newCard.name = "Jack of " + type + "s";
                    newCard.value = 10;
                    break;
                case 12:
                    newCard.name = "Queen of " + type + "s";
                    newCard.value = 10;
                    break;
                case 13:
                    newCard.name = "King of " + type + "s";
                    newCard.value = 10;
                    break;

                default:
                    newCard.name = "Joker";
                    newCard.value = 0;
                    newCard.type = 0;
                    break;
            }

            return newCard;
        }

        public struct Card
        {
            public string name;
            public CardType type;
            public int value;
        }
    }
}