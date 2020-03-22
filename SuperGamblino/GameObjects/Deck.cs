using System;
using System.Collections.Generic;
using System.Text;

namespace SuperGamblino.GameObjects
{
    public class Deck
    {
        public enum CardType { Club, Diamond, Heart, Spade }
        public struct Card
        {
            public string name;
            public CardType type;
            public int value;
        }

        public List<Card> cards = new List<Card>();

        public void GenerateDeck()
        {
            for (int i = 0; i < 13; i++)
            {
                cards.Add(CreateCard(i + 1, CardType.Club));
            }
            for (int i = 0; i < 13; i++)
            {
                cards.Add(CreateCard(i + 1, CardType.Diamond));
            }
            for (int i = 0; i < 13; i++)
            {
                cards.Add(CreateCard(i + 1, CardType.Heart));
            }
            for (int i = 0; i < 13; i++)
            {
                cards.Add(CreateCard(i + 1, CardType.Spade));
            }
            Console.WriteLine(cards);
        }

        Card CreateCard(int value, CardType type)
        {
            Card newCard = new Card();
            newCard.type = type;
            
            switch (value)
            {
                case 1:
                    newCard.name = "Ace of " + type.ToString() + "s";
                    newCard.value = value;
                    break;
                case 2:
                    newCard.name = "Two of " + type.ToString() + "s";
                    newCard.value = value;
                    break;
                case 3:
                    newCard.name = "Three of " + type.ToString() + "s";
                    newCard.value = value;
                    break;
                case 4:
                    newCard.name = "Four of " + type.ToString() + "s";
                    newCard.value = value;
                    break;
                case 5:
                    newCard.name = "Five of " + type.ToString() + "s";
                    newCard.value = value;
                    break;
                case 6:
                    newCard.name = "Six of " + type.ToString() + "s";
                    newCard.value = value;
                    break;
                case 7:
                    newCard.name = "Seven of " + type.ToString() + "s";
                    newCard.value = value;
                    break;
                case 8:
                    newCard.name = "Eight of " + type.ToString() + "s";
                    newCard.value = value;
                    break;
                case 9:
                    newCard.name = "Nine of " + type.ToString() + "s";
                    newCard.value = value;
                    break;
                case 10:
                    newCard.name = "Ten of " + type.ToString() + "s";
                    newCard.value = value;
                    break;
                case 11:
                    newCard.name = "Jack of " + type.ToString() + "s";
                    newCard.value = 10;
                    break;
                case 12:
                    newCard.name = "Queen of " + type.ToString() + "s";
                    newCard.value = 10;
                    break;
                case 13:
                    newCard.name = "King of " + type.ToString() + "s";
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
    }
}
