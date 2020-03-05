using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAndMagicReferee
{
    public enum Location
    {
        Desk = 0,
        Hand = 1,
        Deck = 2
    }
    public enum CardType
    {
        Creature = 0,
        GreenItem = 1,
        RedItem = 2,
        BlueItem = 3
    }
    public class Card
    {
        public Card()
        {
        }

        public Card(Card card)
        {
            cardNumber = card.cardNumber;
            instanceId = card.instanceId;
            location = card.location;
            cardType = card.cardType;
            cost = card.cost;
            attack = card.attack;
            defense = card.defense;
            abilities = card.abilities;
            breakthrough = card.breakthrough;
            charge = card.charge;
            drain = card.drain;
            guard = card.guard;
            lethal = card.lethal;
            ward = card.ward;
            myHealthChange = card.myHealthChange;
            opponentHealthChange = card.opponentHealthChange;
            cardDraw = card.cardDraw;

            canAttack = card.canAttack;
            hasAttacked = card.hasAttacked;
        }

        public int cardNumber;
        public int instanceId;
        public Location location = Location.Deck;
        public CardType cardType;
        public int cost;
        public int attack;
        public int defense;
        public string abilities;
        public bool breakthrough;
        public bool charge;
        public bool drain;
        public bool guard;
        public bool lethal;
        public bool ward;
        public int myHealthChange;
        public int opponentHealthChange;
        public int cardDraw;

        public bool canAttack;
        public bool hasAttacked;

        public String ToInputString(bool opponent = false)
        {
            StringBuilder s = new StringBuilder();

            s.Append(cardNumber).Append(" ");
            s.Append(instanceId).Append(" ");
            s.Append(opponent ? -1 : (location == Location.Hand ? 0 : 1)).Append(" ");
            s.Append((int)cardType).Append(" ");
            s.Append(cost).Append(" ");
            s.Append(attack).Append(" ");
            s.Append(defense).Append(" ");
            s.Append(breakthrough ? "B" : "-").Append(charge ? "C" : "-").Append(ward ? "W" : "-")
                .Append(guard ? "G" : "-").Append(lethal ? "L" : "-").Append(drain ? "D" : "-").Append(" ");
            s.Append(myHealthChange).Append(" ");
            s.Append(opponentHealthChange).Append(" ");
            s.Append(cardDraw).Append(" ");
            return s.ToString();
        }
    }
}
