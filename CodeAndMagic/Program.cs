using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
public static class Weights
{
    public static double draftAttackWeightMultiplier = 1;
    public static double draftAttackWeightConst = 0;
    public static double draftDefenseWeightMultiplier = 1;
    public static double draftDefenseWeightConst = 0;
    public static double draftMyHPWeightMultiplier = 0.5;
    public static double draftMyHPWeightConst = 0;
    public static double draftOpponentHPWeightMultiplier = -0.5;
    public static double draftOpponentHPWeightConst = 0;
    public static double draftDrawWeightMultiplier = 2.5;
    public static double draftDrawWeightCost = 0;
    public static double draftChooseCardCostMultiplier = -0.1;
    public static double draftHpXDmgMultiplier = 0.05;

    public static double draftBreakthroughDmgWeightMultiplier = 0.1;
    public static double draftBreakthroughWeightConst = 0;
    public static double draftChargeWeightMultiplier = 3;
    public static double draftChargeDmgWeightMultiplier = 0.5;
    public static double draftDrainWeightConst = 0;
    public static double draftDrainDmgWeightConst = 0.10;
    public static double draftLethalDmgWeightMultiplier = -0.75;
    public static double draftLethalWeightMultiplier = 6;
    public static double draftLethalWeightConst = 0;
    public static double draftWardDmgWeightMultiplier = 0.5;
    public static double draftWardWeightMultiplier = 2;
    public static double draftWardWeightConst = 0;
    public static double draftGuardHpAndDmgWeightMultiplier = 0.20;
    public static double draftGuardWeightMultiplier = 0.5;
    public static double draftCostGravityCenter1 = 3.5;
    public static double draftCostGravityCenter2 = 11;
    public static double draftCostGravityForce = 0.2;

    public static double roundMyHpWeight = 0.33;
    public static double roundOpponentHpWeight = -0.33;
    //public static double 
}
public enum Location
{
    myHand = 0,
    myDesk = 1,
    opponentDesk = -1
}
public enum CardType
{
    Creature = 0,
    GreenItem = 1,
    RedItem = 2,
    BlueItem = 3
}
public class Action
{
    public Card target;
    public Card source;
    public List<Card> mutations = new List<Card>();
    public List<Player> playerMutations = new List<Player>();
}
public class Card
{
    //mutation part
    public Action action;
    public Card rootCard;
    public Card lastMutation;
    //end of mutation part
    public Card()
    {
        this.rootCard = this;
        this.rootCard.lastMutation = this;
    }
    public Card(Card initCard, Action action)
    {
        var card = initCard.rootCard.lastMutation;
        this.action = action;
        this.action.mutations.Add(this);
        this.rootCard = card.rootCard;
        this.rootCard.mutations.AddLast(this);
        this.cardNumber = card.cardNumber;
        this.instanceId = card.instanceId;
        this.location = card.location;
        this.cardType = card.cardType;
        this.cost = card.cost;
        this.attack = card.attack;
        this.defense = card.defense;
        this.abilities = card.abilities;
        this.breakthrough = card.breakthrough;
        this.charge = card.charge;
        this.drain = card.drain;
        this.guard = card.guard;
        this.lethal = card.lethal;
        this.ward = card.ward;
        this.myHealthChange = card.myHealthChange;
        this.opponentHealthChange = card.opponentHealthChange;
        this.canAttack = card.canAttack;
        this.hasAttacked = card.hasAttacked;
        this.summoned = card.summoned;
        this.hadWard = card.hadWard;
        this.used = card.used;

        this.rootCard.lastMutation = this;
    }

    public int cardNumber;
    public int instanceId;
    public Location location;
    public CardType cardType;
    public double cost;
    public double attack;
    public double defense;
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
    public bool summoned;
    public bool hadWard;
    public bool used;
    public int defenseInstanceId
    {
        //first we take G+W, then G, then W, then everyone else
        get
        {
            return this.instanceId + (this.hadWard ? 0 : 250) + (this.guard ? 0 : 500);
        }
    }

    public int attackInstanceId
    {
        get
        {
            return this.instanceId + (this.breakthrough ? 250 : 0);
        }
    }

    public LinkedList<Card> mutations;

    public double value;
}
public class Player
{
    //mutation part
    public Action action;
    public Player rootPlayer;
    public Player lastMutation;

    //end of mutation part
    public Player()
    {
        this.rootPlayer = this;
        this.rootPlayer.lastMutation = this;
    }
    public Player(Player initPlayer, Action action)
    {
        var player = initPlayer.rootPlayer.lastMutation;
        this.rootPlayer = player.rootPlayer;
        this.rootPlayer.mutations.AddLast(this);
        this.action = action;
        this.action.playerMutations.Add(this);
        this.playerHealth = player.playerHealth;
        this.playerMana = player.playerMana;
        this.playerDeck = player.playerDeck;
        this.playerRune = player.playerRune;
        this.draw = 1;

        this.rootPlayer.lastMutation = this;
    }
    private double _playerHealth;
    public double playerHealth
    {

        get { return _playerHealth; }
        set
        {
            if (_playerHealth != 0)
            {
                // FIRST TIME HP SET
                _playerHealth = value;
            }
            else
            {
                //DMG DONE
                if (value < playerRune)
                {
                    var newRuneValue = Math.Floor(value / 5);
                    draw += (playerRune - newRuneValue * 5) / 5;
                    playerRune = newRuneValue * 5;
                    _playerHealth = value;
                }
                else
                {
                    _playerHealth = value;
                }
            }
        }
    }

    public double playerMana;
    public double playerDeck;
    public double playerRune;
    public double draw = 1;
    public LinkedList<Player> mutations;
}
public class Decider
{

    public bool debug = false;
    public static int turn = 0;
    public static List<Card> myCards;
    public static List<Card> opponentCards;
    public static List<Card> myCardsOpponenPhase;
    public static List<Card> opponentCardsOpponentPhase;
    public static List<Card> deck = new List<Card>();
    public static List<List<Card>> allCards = new List<List<Card>>();
    public static Player me;
    public static Player opponent;

    public double estimateDraftCardValue(Card card)
    {
        var value = 0.0;
        if (card.cardType == CardType.Creature)
        {
            value = (Weights.draftAttackWeightMultiplier * card.attack
                + Weights.draftDefenseWeightMultiplier * card.defense
                + Weights.draftMyHPWeightMultiplier * card.myHealthChange
                + Weights.draftOpponentHPWeightMultiplier * card.opponentHealthChange
                + (card.breakthrough ? Weights.draftBreakthroughDmgWeightMultiplier * card.attack : 0)
                + (card.drain ? Weights.draftDrainDmgWeightConst * card.attack : 0)
                + (card.lethal ? Weights.draftLethalDmgWeightMultiplier * card.attack + Weights.draftLethalWeightMultiplier : 0)
                + (card.ward ? Weights.draftWardDmgWeightMultiplier * card.attack + Weights.draftWardWeightMultiplier : 0)
                + (card.guard ? Weights.draftGuardHpAndDmgWeightMultiplier * (card.attack + card.defense) + Weights.draftGuardWeightMultiplier : 0)
                + Weights.draftDrawWeightMultiplier * card.cardDraw
                + Weights.draftHpXDmgMultiplier * card.attack * card.defense
                + Weights.draftChooseCardCostMultiplier
                ) / (Math.Max(card.cost, 0.5) + 0.25)

                + (card.breakthrough ? Weights.draftBreakthroughWeightConst : 0)
                + (card.drain ? Weights.draftDrainWeightConst : 0)
                + (card.lethal ? Weights.draftLethalWeightConst : 0)
                + (card.ward ? Weights.draftWardWeightConst : 0)
                + Weights.draftAttackWeightConst * card.attack
                + Weights.draftDefenseWeightConst * card.defense
                + Weights.draftMyHPWeightConst * card.myHealthChange
                + Weights.draftOpponentHPWeightConst * card.opponentHealthChange
                + Weights.draftDrawWeightCost * card.cardDraw;
        }
        else if (card.cardType == CardType.GreenItem)
        {
            value = 1.3 * (Weights.draftAttackWeightMultiplier * card.attack
                + Weights.draftDefenseWeightMultiplier * card.defense
                + Weights.draftMyHPWeightMultiplier * card.myHealthChange
                + Weights.draftOpponentHPWeightMultiplier * card.opponentHealthChange
                + 0.5 * (
                    (card.breakthrough ? Weights.draftBreakthroughDmgWeightMultiplier * card.attack : 0)
                    + (card.charge ? Weights.draftChargeDmgWeightMultiplier * card.attack + Weights.draftChargeWeightMultiplier : 0)
                    + (card.drain ? Weights.draftDrainDmgWeightConst * card.attack : 0)
                    + (card.lethal ? Weights.draftLethalDmgWeightMultiplier * card.attack + Weights.draftLethalWeightMultiplier : 0)
                    + (card.ward ? Weights.draftWardDmgWeightMultiplier * card.attack + Weights.draftWardWeightMultiplier : 0)
                    )
                + Weights.draftDrawWeightMultiplier * card.cardDraw
                ) / (Math.Max(card.cost, 0.5) + 0.5)

                + (card.breakthrough ? Weights.draftBreakthroughWeightConst : 0)
                + (card.drain ? Weights.draftDrainWeightConst : 0)
                + (card.lethal ? Weights.draftLethalWeightConst : 0)
                + (card.ward ? Weights.draftWardWeightConst : 0)
                + Weights.draftAttackWeightConst * card.attack
                + Weights.draftDefenseWeightConst * card.defense
                + Weights.draftMyHPWeightConst * card.myHealthChange
                + Weights.draftOpponentHPWeightConst * card.opponentHealthChange
                + Weights.draftDrawWeightCost * card.cardDraw;
        }
        else if (card.cardType == CardType.RedItem || card.cardType == CardType.BlueItem)
        {
            value = (-1.5 * Weights.draftAttackWeightMultiplier * card.attack
                - 1.5 * Weights.draftDefenseWeightMultiplier * card.defense
                + Weights.draftMyHPWeightMultiplier * card.myHealthChange
                + Weights.draftOpponentHPWeightMultiplier * card.opponentHealthChange
                + (card.guard ? 0.5 : 0)
                + (card.lethal ? 0.5 : 0)
                + (card.ward ? 0.5 : 0)
                ) / (Math.Max(card.cost, 0.5) + 0.5)

                - Weights.draftAttackWeightConst * card.attack
                - Weights.draftDefenseWeightConst * card.defense
                + Weights.draftMyHPWeightConst * card.myHealthChange
                + Weights.draftOpponentHPWeightConst * card.opponentHealthChange
                + Weights.draftDrawWeightCost * card.cardDraw;
        }
        var totalCardsOfThisCost = deck.Count(deckCard => deckCard.cost == card.cost) * 1.0;
        var frequency = totalCardsOfThisCost / Math.Max(1, deck.Count);
        value = value * (1 + Weights.draftCostGravityForce * ( 1.0 / (1 + Math.Pow(Math.Abs(card.cost - Weights.draftCostGravityCenter1), 2)) + 1.0 / (1 + Math.Pow(Math.Abs(card.cost - Weights.draftCostGravityCenter2), 2))));
        card.value = value;
        return value;
    }

    public Card chooseCard()
    {
        var maxValue = 0.0;
        for (int i = 0; i < myCards.Count; ++i)
        {
            var value = estimateDraftCardValue(myCards[i]);
            if (value > maxValue)
            {
                maxValue = value;
            }
        }
        for (int i = 0; i < myCards.Count; ++i)
        {
            if (myCards[i].value == maxValue)
            {
                Console.WriteLine("PICK " + i);
                return myCards[i];
            }
        }
        Console.WriteLine("PASS");
        return null;
    }

    void FillSummonMutations(Action action, Card source, Player meMutation, Player opponentMutation)
    {
        source.location = Location.myDesk;
        source.canAttack = source.charge;
        source.summoned = true;

        meMutation.playerHealth += source.myHealthChange;
        opponentMutation.playerHealth += source.opponentHealthChange;
        meMutation.draw += source.cardDraw;
        meMutation.playerMana -= source.cost;
    }
    void FillAttackMutations(Action action, Card source, Card target, Player meMutation, Player opponentMutation)
    {
        source.canAttack = false;
        source.hasAttacked = true;
        if (target != null && target.lethal)
        {
            if (source.ward)
            {
                source.ward = false;
            }
            else
            {
                source.defense = 0;
            }
        }
        else if (target != null && target.attack > 0)
        {
            if (source.ward)
            {
                source.ward = false;
            }
            else
            {
                source.defense = source.defense - target.attack;
            }
        }

        //drain check
        if (source.drain && (target == null || !target.ward))
        {
            meMutation.playerHealth = meMutation.playerHealth + source.attack;
        }

        if (source.lethal && target != null)
        {
            if (target.ward)
            {
                target.ward = false;
            }
            else
            {
                if (source.breakthrough && source.attack > target.defense)
                {
                    opponentMutation.playerHealth = opponentMutation.playerHealth - (source.attack - target.defense);
                }
                target.defense = 0;
            }
        }
        else if (source.attack > 0)
        {
            if (target == null)
            {
                opponentMutation.playerHealth = opponentMutation.playerHealth - source.attack;
            }
            else
            {
                if (target.ward)
                {
                    target.ward = false;
                }
                else
                {
                    if (source.breakthrough && source.attack > target.defense)
                    {
                        opponentMutation.playerHealth = opponentMutation.playerHealth - (source.attack - target.defense);
                    }
                    target.defense = target.defense - source.attack;
                }
            }
        }
    }

    void FillUseCardMutations(Action action, Card source, Card target, Player meMutation, Player opponentMutation)
    {
        //This is green item
        source.used = true;
        if (source.cardType == CardType.GreenItem)
        {
            target.charge = source.charge || target.charge;
            if (target.charge)
            {
                target.canAttack = !target.hasAttacked;
            }
            target.breakthrough = source.breakthrough || target.breakthrough;
            target.drain = source.drain || target.drain;
            target.guard = source.guard || target.guard;
            target.lethal = source.lethal || target.lethal;
            target.ward = source.ward || target.ward;
        }
        //blue or red item
        else
        {
            if (target != null)
            {
                target.charge = !source.charge && target.charge;
                target.breakthrough = !source.breakthrough && target.breakthrough;
                target.drain = !source.drain && target.drain;
                target.guard = !source.guard && target.guard;
                target.lethal = !source.lethal && target.lethal;
                target.ward = !source.ward && target.ward;
            }
        }

        if (target != null)
        {
            target.attack = Math.Max(0, target.attack + source.attack);

            if (target.ward && source.defense < 0)
            {
                target.ward = false;
            }
            else
            {
                target.defense = target.defense + source.defense;
            }
        }
        else
        {
            opponentMutation.playerHealth += source.defense;
        }

        meMutation.playerHealth += source.myHealthChange;
        opponentMutation.playerHealth += source.opponentHealthChange;
        meMutation.draw += source.cardDraw;
        meMutation.playerMana -= source.cost;
    }

    void EraseMutations(Action action)
    {
        action.mutations.ForEach(mutation =>
        {
            var root = mutation.rootCard;
            root.mutations.RemoveLast();
            root.lastMutation = root.mutations.Count > 0 ? root.mutations.Last.Value : root;
        });
        action.playerMutations.ForEach(mutation =>
        {
            var root = mutation.rootPlayer;
            root.mutations.RemoveLast();
            root.lastMutation = root.mutations.Count > 0 ? root.mutations.Last.Value : root;
        });
    }
    void CreateMutations(Action action)
    {
        Card source = new Card(action.source, action);
        Card target = action.target != null ? new Card(action.target, action) : null;
        Player meMutation = new Player(me, action);
        Player opponentMutation = new Player(opponent, action);
        // CREATURE
        if (source.cardType == CardType.Creature)
        {
            // IN MY HAND
            if (source.location == Location.myHand)
            {
                FillSummonMutations(action, source, meMutation, opponentMutation);
            }
            // ON MY PART OF THE DESK
            else if (source.location == Location.myDesk)
            {
                FillAttackMutations(action, source, target, meMutation, opponentMutation);
            }
            else if (source.location == Location.opponentDesk)
            {
                FillAttackMutations(action, source, target, opponentMutation, meMutation);
            }
        }
        else
        {
            FillUseCardMutations(action, source, target, meMutation, opponentMutation);
        }
    }
    public double estimateInHandCardValue(Card card)
    {
        var value = 0.0;
        if (card.cardType == CardType.Creature)
        {
            value = (Weights.draftAttackWeightMultiplier * card.attack
                + Weights.draftDefenseWeightMultiplier * card.defense
                + (card.breakthrough ? Weights.draftBreakthroughDmgWeightMultiplier * card.attack : 0)
                + (card.charge ? Weights.draftChargeDmgWeightMultiplier * card.attack + Weights.draftChargeWeightMultiplier : 0)
                + (card.drain ? Weights.draftDrainDmgWeightConst * card.attack : 0)
                + (card.lethal ? Weights.draftLethalDmgWeightMultiplier * card.attack + Weights.draftLethalWeightMultiplier : 0)
                + (card.ward ? Weights.draftWardDmgWeightMultiplier * card.attack + Weights.draftWardWeightMultiplier : 0)
                + Weights.draftHpXDmgMultiplier * card.attack * card.defense
                ) / Math.Max(card.cost, 0.5)

                + (card.breakthrough ? Weights.draftBreakthroughWeightConst : 0)
                + (card.drain ? Weights.draftDrainWeightConst : 0)
                + (card.lethal ? Weights.draftLethalWeightConst : 0)
                + (card.ward ? Weights.draftWardWeightConst : 0)
                + Weights.draftAttackWeightConst * card.attack
                + Weights.draftDefenseWeightConst * card.defense;
        }
        if (card.cardType == CardType.GreenItem)
        {
            value = 1.3 * (Weights.draftAttackWeightMultiplier * card.attack
                + Weights.draftDefenseWeightMultiplier * card.defense
                + 0.33 * (
                +(card.breakthrough ? Weights.draftBreakthroughDmgWeightMultiplier * card.attack : 0)
                + (card.charge ? Weights.draftChargeDmgWeightMultiplier * card.attack + Weights.draftChargeWeightMultiplier : 0)
                + (card.drain ? Weights.draftDrainDmgWeightConst * card.attack : 0)
                + (card.lethal ? Weights.draftLethalDmgWeightMultiplier * card.attack + Weights.draftLethalWeightMultiplier : 0)
                + (card.ward ? Weights.draftWardDmgWeightMultiplier * card.attack + Weights.draftWardWeightMultiplier : 0))
                ) / Math.Max(card.cost, 0.5)

                + (card.breakthrough ? Weights.draftBreakthroughWeightConst : 0)
                + (card.drain ? Weights.draftDrainWeightConst : 0)
                + (card.lethal ? Weights.draftLethalWeightConst : 0)
                + (card.ward ? Weights.draftWardWeightConst : 0)
                + Weights.draftAttackWeightConst * card.attack
                + Weights.draftDefenseWeightConst * card.defense;
        }
        if (card.cardType == CardType.RedItem || card.cardType == CardType.BlueItem)
        {
            value = 1.3 * (Weights.draftAttackWeightMultiplier * card.attack
                + Weights.draftDefenseWeightMultiplier * card.defense
                + 0.33 * (
                +(card.breakthrough ? Weights.draftBreakthroughDmgWeightMultiplier * card.attack : 0)
                + (card.drain ? Weights.draftDrainDmgWeightConst * card.attack : 0)
                + (card.lethal ? Weights.draftLethalDmgWeightMultiplier * card.attack + Weights.draftLethalWeightMultiplier : 0)
                + (card.ward ? Weights.draftWardDmgWeightMultiplier * card.attack + Weights.draftWardWeightMultiplier : 0))
                + Weights.draftDrawWeightMultiplier * card.cardDraw
                ) / Math.Max(card.cost, 0.5)

                + (card.breakthrough ? Weights.draftBreakthroughWeightConst : 0)
                + (card.drain ? Weights.draftDrainWeightConst : 0)
                + (card.lethal ? Weights.draftLethalWeightConst : 0)
                + (card.ward ? Weights.draftWardWeightConst : 0)
                + Weights.draftAttackWeightConst * card.attack
                + Weights.draftDefenseWeightConst * card.defense
                + Weights.draftDrawWeightCost * card.cardDraw;
        }
        return value;
    }

    double estimateOnDeskCardValue(Card card)
    {
        var value = 0.0;
        value = 1.0 +
            Weights.draftAttackWeightMultiplier * card.attack
            + Weights.draftDefenseWeightMultiplier * card.defense
            + (card.breakthrough ? Weights.draftBreakthroughDmgWeightMultiplier * card.attack : 0)
            + (card.drain ? Weights.draftDrainDmgWeightConst * card.attack : 0)
            + (card.lethal ? Weights.draftLethalDmgWeightMultiplier * card.attack + Weights.draftLethalWeightMultiplier : 0)
            + (card.ward ? Weights.draftWardDmgWeightMultiplier * card.attack + Weights.draftWardWeightMultiplier : 0)
            + Weights.draftHpXDmgMultiplier * card.attack * card.defense

            + (card.breakthrough ? Weights.draftBreakthroughWeightConst : 0)
            + (card.drain ? Weights.draftDrainWeightConst : 0)
            + (card.lethal ? Weights.draftLethalWeightConst : 0)
            + (card.ward ? Weights.draftWardWeightConst : 0)
            + Weights.draftAttackWeightConst * card.attack
            + Weights.draftDefenseWeightConst * card.defense;
        //Console.Error.WriteLine("estim: {0}_{1}={2}", card.attack, card.defense, value);
        return value;
    }
    public double InGameCardValue(Card card, IEnumerable<Card> myCards, IEnumerable<Card> opponentCards, Player meLast, Player opponentLast)
    {
        if (card.location == Location.myHand)
        {
            return estimateInHandCardValue(card);
        }
        else
        {
            return estimateOnDeskCardValue(card);
        }
    }
    double EstimatePosition(IEnumerable<Action> actions)
    {
        var debugAction = actions.ToList();
        positionsEstimated++;
        var value = 0.0;
        var posMyCards = myCards.Select(card => card.lastMutation).Where(card => (card.location == Location.myHand && !card.used) || card.defense > 0);
        var posOpponentCards = opponentCards.Select(card => card.lastMutation).Where(card => card.defense > 0);
        var meLast = me.lastMutation;
        var opponentLast = opponent.lastMutation;
        double myCardValue = 0.0;
        double oppCardValue = 0.0;
        foreach (var card in posMyCards)
        {
            myCardValue = myCardValue + InGameCardValue(card, posMyCards, posOpponentCards, meLast, opponentLast);
        }
        foreach (var card in posOpponentCards)
        {
            oppCardValue = oppCardValue + InGameCardValue(card, posMyCards, posOpponentCards, meLast, opponentLast);
        }
        value = myCardValue - oppCardValue;
        value += Weights.roundMyHpWeight * meLast.playerHealth + Math.Min(8 - posMyCards.Where(card => card.location == Location.myHand).Count(), meLast.draw) * 2;
        value += Weights.roundOpponentHpWeight * opponentLast.playerHealth;
        if (opponentLast.playerHealth <= 0)
        {
            value = lethalValue;
        }
        else if (meLast.playerHealth <= 0)
        {
            value = -lethalValue;
        }
        if (debug)
        {
            using (StreamWriter ws = File.AppendText("../../test.txt"))
                ws.WriteLine("EsPos,{1,-50},value={2,-10},myCardValue={3,-10},oppCardValue={4,-10}", actions.Count(), string.Join(" ", actions.Select(act => getActionName(act) + act.source.instanceId + "_" + (act.target?.instanceId ?? -1))), value.ToString("F3"), myCardValue.ToString("F3"), oppCardValue.ToString("F3"));
        }
        return value;
    }
    public string getActionName(Action action)
    {
        if (action.source.cardType == CardType.Creature)
        {
            if (action.source.location == Location.myHand)
                return "S";
            else if (action.source.location == Location.myDesk)
                return "A";
            else
                return "E";
        }
        else
            return "C";
    }
    public enum EnemyPhase
    {
        attack = 1,
        noMoreAction = 2
    }
    public class EnemyBoundsForAction
    {
        public EnemyBoundsForAction()
        {
        }
        public EnemyBoundsForAction(EnemyBoundsForAction bounds)
        {
            minMyInstanceIdToGetAttacked = bounds.minMyInstanceIdToGetAttacked;
            minOpponentMinInstanceIdToAttackSameTarget = bounds.minOpponentMinInstanceIdToAttackSameTarget;
            myLastAttackedCard = bounds.myLastAttackedCard;
        }
        public int minMyInstanceIdToGetAttacked = 0;
        public int minOpponentMinInstanceIdToAttackSameTarget = 0;
        public Card myLastAttackedCard;
    }
    void FindBestEnemyMovesValue(LinkedList<Action> actions, EnemyPhase phase, EnemyBoundsForAction bounds)
    {
        if (phase == EnemyPhase.noMoreAction)
        {
            var value = EstimatePosition(actions);
            if (value < bestCounterMoveValue)
            {
                bestCounterMoveValue = value;
            }
        }
        else if (bestPossibleMoveValue < lethalValue && DateTime.Now < turnStart + maximumTurnLength)
        {
            var meLast = me.lastMutation;
            var opponentLast = opponent.lastMutation;
            var myLastDeskCards = myCardsOpponenPhase.Select(card => card.rootCard.lastMutation).Where(card => card.location == Location.myDesk && card.defense > 0);
            var opponentLastCards = opponentCardsOpponentPhase.Select(card => card.rootCard.lastMutation).Where(card => card.defense > 0);
            FindBestEnemyMovesValue(actions, phase + 1, bounds);
            if (phase == EnemyPhase.attack)
            {
                var oppLastDeskWithAttackCards = opponentLastCards.Where(card => (card.attack > 0 || card.lethal) && card.canAttack).ToList();
                var myGuards = myLastDeskCards.Where(card => card.guard).ToList();

                foreach (var myCreature in myLastDeskCards.Concat(new Card[] { null })
                    .Where(card => (!myGuards.Any() || card?.guard == true) && ((card?.defenseInstanceId ?? 2000) == bounds.minMyInstanceIdToGetAttacked
                        || ((bounds.myLastAttackedCard == null || bounds.myLastAttackedCard.rootCard.lastMutation.defense <= 0 || bounds.myLastAttackedCard.rootCard.ward) && (card?.defenseInstanceId ?? 2000) > bounds.minMyInstanceIdToGetAttacked)
                    )))
                {
                    foreach (var oppCreature in oppLastDeskWithAttackCards.Where(card =>
                        (myCreature?.defenseInstanceId ?? 2000) == bounds.minMyInstanceIdToGetAttacked
                        ? card.instanceId >= bounds.minOpponentMinInstanceIdToAttackSameTarget : true))
                    {
                        Action actionVsCreature = new Action()
                        {
                            source = oppCreature,
                            target = myCreature
                        };
                        var myCreatureHasWard = myCreature?.ward ?? false;
                        CreateMutations(actionVsCreature);
                        actions.AddLast(actionVsCreature);
                        FindBestEnemyMovesValue(actions,
                            oppLastDeskWithAttackCards.Count == 1 ? phase + 1 : phase,
                            new EnemyBoundsForAction(bounds)
                            {
                                minMyInstanceIdToGetAttacked = (myCreature?.defenseInstanceId ?? 2000),
                                minOpponentMinInstanceIdToAttackSameTarget = !myCreatureHasWard ? oppCreature.instanceId : 0,
                                myLastAttackedCard = myCreature
                            });
                        actions.RemoveLast();
                        EraseMutations(actionVsCreature);
                    }
                }
            }
        }
    }
    public double lethalValue = 100000000;
    public enum Phase
    {
        initUseRedAndBlueTarget = 1,
        initBlueNoTarget = 2,
        initUseGreen = 3,
        attack = 4,
        redAndTargetBlueOnWardAfterAttack = 5,
        castWardOnAttacked = 6,
        summon = 7,
        greenOnSummonCast = 8,
        attackWithSummon = 9,
        noMoreAction = 10
    }
    public List<Action> bestPossibleActions;
    public double bestPossibleMoveValue;
    public double bestCounterMoveValue;
    public bool bestPossibleMoveFirstEval;
    public List<Action> FindBestMoves()
    {
        myCards = myCards.OrderBy(card => card.attackInstanceId).ToList();
        opponentCards = opponentCards.OrderBy(card => card.defenseInstanceId).ToList();
        CheckForPossibleMoves(new LinkedList<Action>(), Phase.initUseRedAndBlueTarget,
            new BoundForAction()
            {
                everMaxedBoard = false,
                greenMinInstanceId = 0,
                minBlueNoTargetInstanceId = 0,
                minInstanceIdForSummon = 0,
                minOpponentInstanceIdToAttack = 0,
                minRedAndBlueInstanceId = 0,
                opponentLastAttackedCard = null
            });
        return bestPossibleActions;
    }
    public List<Card> myLastCards;
    public class BoundForAction
    {
        public BoundForAction()
        {
        }
        public BoundForAction(BoundForAction bounds)
        {
            everMaxedBoard = bounds.everMaxedBoard;
            greenMinInstanceId = bounds.greenMinInstanceId;
            minOpponentInstanceIdToAttack = bounds.minOpponentInstanceIdToAttack;
            minInstanceIdForSummon = bounds.minInstanceIdForSummon;
            minRedAndBlueInstanceId = bounds.minRedAndBlueInstanceId;
            minBlueNoTargetInstanceId = bounds.minBlueNoTargetInstanceId;
            minRedAndTargetBlueInstanceIdOnWardAfterAttackSameTarget = bounds.minRedAndTargetBlueInstanceIdOnWardAfterAttackSameTarget;
            minGreenOnSameSummonCast = bounds.minGreenOnSameSummonCast;
            minMyMinInstanceIdToAttackSameTarget = bounds.minMyMinInstanceIdToAttackSameTarget;
            opponentLastAttackedCard = bounds.opponentLastAttackedCard;
        }
        public bool everMaxedBoard = false;
        public int greenMinInstanceId = 0;
        public int minOpponentInstanceIdToAttack = 0;
        public int minInstanceIdForSummon = 0;
        public int minRedAndBlueInstanceId = 0;
        public int minBlueNoTargetInstanceId = 0;
        public int minRedAndTargetBlueInstanceIdOnWardAfterAttackSameTarget = 0;
        public int minGreenOnSameSummonCast = 0;
        public int minMyMinInstanceIdToAttackSameTarget = 0;
        public Card opponentLastAttackedCard;
    }

    void CheckForPossibleMoves(LinkedList<Action> actions,
        Phase phase,
        BoundForAction bounds)
    {
        enteredFindComb++;
        if (phase == Phase.noMoreAction)
        {
            bestCounterMoveValue = double.MaxValue;
            myCardsOpponenPhase = myCards.Select(card => card.lastMutation)
                .Where(card => card.location == Location.myDesk && card.defense > 0).OrderBy(card => card.defenseInstanceId).ToList();
            opponentCardsOpponentPhase = opponentCards.Select(card => card.lastMutation)
                .Where(card => card.location == Location.opponentDesk && card.defense > 0).OrderBy(card => card.attackInstanceId).ToList();
            FindBestEnemyMovesValue(actions, EnemyPhase.attack, new EnemyBoundsForAction());
            if (debug)
            {
                using (StreamWriter ws = File.AppendText("../../test.txt"))
                    ws.WriteLine("BEST MOVE {0} {1}", bestCounterMoveValue, bestPossibleMoveValue);
            }
            Console.Error.WriteLine("BEST MOVE {0} {1}", bestCounterMoveValue, bestPossibleMoveValue);
            if (bestCounterMoveValue > bestPossibleMoveValue)
            {
                bestPossibleMoveValue = bestCounterMoveValue;
                bestPossibleActions = actions.ToList();
            }
        }
        else if (bestPossibleMoveValue < lethalValue && DateTime.Now < turnStart + maximumTurnLength)
        {
            var meLast = me.lastMutation;
            var opponentLast = opponent.lastMutation;
            var myLastCards = myCards.Select(card => card.lastMutation).Where(card => (card.location == Location.myHand && !card.used) || card.defense > 0);
            var myLastHandCards = myLastCards.Where(card => card.location == Location.myHand && card.cost <= meLast.playerMana && !card.used);
            var myLastDeskCards = myLastCards.Where(card => card.location == Location.myDesk);
            var opponentLastCards = opponentCards.Select(card => card.lastMutation).Where(card => card.defense > 0);
            var enemyGuards = opponentLastCards.Where(card => card.guard);

            // HARMFULL OPTIMIZATION GO FOR FACE IF CAN
            // UNLESS OPPONENT HAS FEW CARDS ON TABLE AND WE DON'T WANT GIVE HIM DRAW
            if (!(phase == Phase.attack && !enemyGuards.Any() && opponentLastCards.Count() > myLastDeskCards.Count() - 3
                && myLastDeskCards.Any(card => card.canAttack && card.attack > 0))
            // HARMFULL OPTIMIZATION Go FOR FACE WITH SUMMON IF CAN
                && !(phase == Phase.attackWithSummon && !enemyGuards.Any() && myLastDeskCards.Any(card => card.canAttack && card.instanceId >= bounds.minInstanceIdForSummon))
            // HARMFULL OPTIMIZATION SUMMON SOMEONE IF CAN
                && !(phase == Phase.summon && myLastDeskCards.Count() < 6 && myLastHandCards.Any(card => card.cardType == CardType.Creature && card.cost <= meLast.playerMana)))
            {
                CheckForPossibleMoves(actions, phase + 1, bounds);
            }
            if (phase == Phase.initUseRedAndBlueTarget)
            {
                var myRedAndBlueTargetCards = myLastHandCards.Where(card => card.instanceId >= bounds.minRedAndBlueInstanceId &&
                    (card.cardType == CardType.RedItem || (card.cardType == CardType.BlueItem && card.defense != 0))).ToList();
                foreach (var myRedAndBlueCard in myRedAndBlueTargetCards)
                {
                    foreach (var enemyCard in opponentLastCards)
                    {
                        if (myRedAndBlueCard.defense != 0
                            || myRedAndBlueCard.opponentHealthChange != 0
                            || myRedAndBlueCard.myHealthChange != 0
                            || (myRedAndBlueCard.attack != 0 && enemyCard.attack > 0)
                            || (myRedAndBlueCard.breakthrough && enemyCard.breakthrough)
                            || (myRedAndBlueCard.cardDraw > 0)
                            || (myRedAndBlueCard.drain && enemyCard.drain)
                            || (myRedAndBlueCard.guard && enemyCard.guard)
                            || (myRedAndBlueCard.lethal && enemyCard.lethal)
                            || (myRedAndBlueCard.ward && enemyCard.ward))
                        {
                            Action action = new Action()
                            {
                                source = myRedAndBlueCard,
                                target = enemyCard
                            };
                            var hadWard = enemyCard.ward;
                            CreateMutations(action);
                            actions.AddLast(action);
                            if (!enemyCard.rootCard.lastMutation.ward && hadWard)
                            {
                                CheckForPossibleMoves(actions, phase, bounds);
                            }
                            else
                            {
                                CheckForPossibleMoves(actions, phase, new BoundForAction(bounds) { minRedAndBlueInstanceId = myRedAndBlueCard.instanceId });
                            }
                            actions.RemoveLast();
                            EraseMutations(action);
                        }
                    }
                }

            }
            if (phase == Phase.initBlueNoTarget)
            {
                var myBlueCards = myLastHandCards.Where(card => card.cardType == CardType.BlueItem && card.defense == 0 && card.instanceId >= bounds.minBlueNoTargetInstanceId);
                foreach (var myBlueCard in myBlueCards)
                {
                    Action actionVsOpponent = new Action()
                    {
                        source = myBlueCard,
                        target = null
                    };
                    CreateMutations(actionVsOpponent);
                    actions.AddLast(actionVsOpponent);
                    CheckForPossibleMoves(actions, phase, new BoundForAction(bounds) { minBlueNoTargetInstanceId = myBlueCard.instanceId + 1 });
                    actions.RemoveLast();
                    EraseMutations(actionVsOpponent);
                }
            }

            if (phase == Phase.initUseGreen)
            {
                var myGreenCards = myLastHandCards.Where(card => card.cardType == CardType.GreenItem && card.instanceId >= bounds.greenMinInstanceId);
                foreach (var myGreenCard in myGreenCards)
                {
                    foreach (var myDeskCard in myLastDeskCards)
                    {
                        Action action = new Action()
                        {
                            source = myGreenCard,
                            target = myDeskCard
                        };
                        CreateMutations(action);
                        actions.AddLast(action);
                        CheckForPossibleMoves(actions, phase, new BoundForAction() { greenMinInstanceId = myGreenCard.instanceId + 1 });
                        actions.RemoveLast();
                        EraseMutations(action);
                    }
                }
            }
            if (phase == Phase.attack)
            {
                var myLastDeskWithAttackCards = myLastDeskCards.Where(card => (card.attack > 0 || card.lethal || myLastCards.Count() == 6) && card.canAttack).ToList();
                var opponentCreatures = opponentLastCards.Concat(new Card[] { null })
                    .Where(card => (!enemyGuards.Any() || card?.guard == true) && ((card?.defenseInstanceId ?? 2000) == bounds.minOpponentInstanceIdToAttack
                        || ((bounds.opponentLastAttackedCard == null || bounds.opponentLastAttackedCard.rootCard.lastMutation.defense <= 0 || bounds.opponentLastAttackedCard.rootCard.ward) && (card?.defenseInstanceId ?? 2000) > bounds.minOpponentInstanceIdToAttack)
                    )).ToList();// don't attack next if attacked previous and it's still alive or it's had ward
                foreach (var opponentCreature in opponentCreatures)
                {
                    var myCreatesCanAttack = myLastDeskWithAttackCards.Where(card =>
                        (opponentCreature?.defenseInstanceId ?? 2000) == bounds.minOpponentInstanceIdToAttack
                        ? card.instanceId >= bounds.minMyMinInstanceIdToAttackSameTarget : true);
                    foreach (var myCreature in myCreatesCanAttack)
                    {
                        Action actionVsCreature = new Action()
                        {
                            source = myCreature,
                            target = opponentCreature
                        };
                        var oppCreatureHasWard = opponentCreature?.ward ?? false;
                        CreateMutations(actionVsCreature);
                        actions.AddLast(actionVsCreature);
                        CheckForPossibleMoves(actions,
                            myLastDeskWithAttackCards.Count == 1 ? phase + 1 : phase,
                            new BoundForAction(bounds)
                            {
                                minOpponentInstanceIdToAttack = (opponentCreature?.defenseInstanceId ?? 2000),
                                opponentLastAttackedCard = opponentCreature,
                                minMyMinInstanceIdToAttackSameTarget = !oppCreatureHasWard ? myCreature.instanceId : 0,
                            });
                        actions.RemoveLast();
                        EraseMutations(actionVsCreature);
                    }
                }
            }
            if (phase == Phase.redAndTargetBlueOnWardAfterAttack)
            {
                var enemyCardWhichLostWard = opponentLastCards.FirstOrDefault(card => !card.ward && card.hadWard && card.defenseInstanceId == bounds.minOpponentInstanceIdToAttack);
                var myRedAndBlueCards = myLastHandCards.Where(card => card.instanceId >= bounds.minRedAndTargetBlueInstanceIdOnWardAfterAttackSameTarget &&
                    ((card.cardType == CardType.RedItem || card.cardType == CardType.BlueItem) && card.defense < 0 && !card.ward));

                if (enemyCardWhichLostWard != null)
                {
                    foreach (var myRedAndBlueCard in myRedAndBlueCards)
                    {
                        Action action = new Action()
                        {
                            source = myRedAndBlueCard,
                            target = enemyCardWhichLostWard
                        };
                        CreateMutations(action);
                        actions.AddLast(action);
                        //killed card, now we can attack next one, or decreased it's dmg
                        if (enemyCardWhichLostWard.rootCard.lastMutation.defense < 0 || myRedAndBlueCard.attack != 0)
                        {
                            CheckForPossibleMoves(actions, Phase.attack, new BoundForAction(bounds)
                            {
                                minRedAndTargetBlueInstanceIdOnWardAfterAttackSameTarget = myRedAndBlueCard.instanceId
                            });
                        }
                        else
                        {
                            CheckForPossibleMoves(actions, phase, new BoundForAction(bounds)
                            {
                                minRedAndTargetBlueInstanceIdOnWardAfterAttackSameTarget = myRedAndBlueCard.instanceId
                            });
                        }
                        actions.RemoveLast();
                        EraseMutations(action);
                    }
                }
            }
            if (phase == Phase.castWardOnAttacked)
            {
                var myWardableCards = myLastDeskCards.Where(card => card.hasAttacked && !card.ward);
                var myWardGreenCards = myLastHandCards.Where(card => card.cardType == CardType.GreenItem && card.ward);
                foreach (var myGreenWard in myWardGreenCards)
                {
                    foreach (var myWardableCard in myWardableCards)
                    {
                        Action action = new Action()
                        {
                            source = myGreenWard,
                            target = myWardableCard
                        };
                        CreateMutations(action);
                        actions.AddLast(action);
                        CheckForPossibleMoves(actions, phase, new BoundForAction(bounds));
                        actions.RemoveLast();
                        EraseMutations(action);
                    }
                }
            }
            if (phase == Phase.summon)
            {
                var lastDeskCardsAmount = myLastDeskCards.Count();
                if (lastDeskCardsAmount < 6)
                {
                    var mySummonableCards = myLastHandCards.Where(card => card.cardType == CardType.Creature && card.instanceId >= bounds.minInstanceIdForSummon);
                    foreach (var mySummon in mySummonableCards)
                    {
                        Action action = new Action()
                        {
                            source = mySummon,
                            target = null
                        };
                        CreateMutations(action);
                        actions.AddLast(action);
                        CheckForPossibleMoves(actions, phase,
                            new BoundForAction(bounds)
                            {
                                everMaxedBoard = lastDeskCardsAmount == 5 ? true : false,
                                minInstanceIdForSummon = mySummon.instanceId,
                                minGreenOnSameSummonCast = 0
                            });
                        actions.RemoveLast();
                        EraseMutations(action);
                    }
                }
            }
            if (phase == Phase.greenOnSummonCast)
            {
                // Select only summon which hasn't attacked
                var mySummonedCard = myLastDeskCards.Where(card => card.instanceId == bounds.minInstanceIdForSummon).FirstOrDefault();
                if (mySummonedCard != null)
                {
                    var myGreenCards = myLastDeskCards.Where(card => card.cardType == CardType.GreenItem && card.instanceId >= bounds.minGreenOnSameSummonCast);
                    foreach (var myGreenCard in myGreenCards)
                    {
                        Action action = new Action()
                        {
                            source = myGreenCard,
                            target = mySummonedCard
                        };
                        CreateMutations(action);
                        actions.AddLast(action);
                        CheckForPossibleMoves(actions, phase, new BoundForAction(bounds)
                        {
                            minGreenOnSameSummonCast = myGreenCard.instanceId
                        });
                        actions.RemoveLast();
                        EraseMutations(action);
                    }
                }
            }
            if (phase == Phase.attackWithSummon)
            {
                var opponentCardsCanBeAttacked = opponentLastCards.Concat(new Card[] { null })
                    .Where(card => (!enemyGuards.Any() || card?.guard == true) && ((card?.defenseInstanceId ?? 2000) == bounds.minOpponentInstanceIdToAttack
                    || ((bounds.opponentLastAttackedCard == null || bounds.opponentLastAttackedCard.rootCard.lastMutation.defense <= 0 || bounds.opponentLastAttackedCard.rootCard.ward) && (card?.defenseInstanceId ?? 2000) > bounds.minOpponentInstanceIdToAttack)
                    )).ToList();
                foreach (var opponentCard in opponentCardsCanBeAttacked)// don't attack next if attacked previous and it's still alive or it's had ward)
                {
                    var mySummonedCanAttackCards = myLastDeskCards.Where(card => card.summoned && (card.attack > 0 || card.lethal) && card.canAttack);
                    foreach (var mySummoned in mySummonedCanAttackCards)
                    {
                        Action action = new Action()
                        {
                            source = mySummoned,
                            target = opponentCard
                        };
                        var attackedIntoWard = opponentCard?.ward;
                        var attackedIntoGuard = opponentCard?.guard;
                        CreateMutations(action);
                        actions.AddLast(action);
                        // We killed with summon guard or dewarded ward and now can exchange profitably with other cards
                        if (attackedIntoWard == true)
                        {
                            CheckForPossibleMoves(actions, Phase.attack,
                                new BoundForAction(bounds)
                                {
                                    minOpponentInstanceIdToAttack = opponentCard.defenseInstanceId,
                                    opponentLastAttackedCard = opponentCard,
                                    minMyMinInstanceIdToAttackSameTarget = 0
                                });
                        }
                        // We killed guard
                        else if (attackedIntoGuard == true && opponentCard.rootCard.lastMutation.defense < 0)
                        {
                            CheckForPossibleMoves(actions, Phase.attack,
                                new BoundForAction(bounds)
                                {
                                    minOpponentInstanceIdToAttack = opponentCard.defenseInstanceId,
                                    opponentLastAttackedCard = opponentCard,
                                    minMyMinInstanceIdToAttackSameTarget = 0
                                });
                        }
                        else
                        {
                            // We killed our last summon after we had 6 card on desk and now can summon again
                            if (bounds.everMaxedBoard && mySummoned.rootCard.lastMutation.defense <= 0)
                            {
                                CheckForPossibleMoves(actions, Phase.summon, new BoundForAction(bounds)
                                {
                                    minInstanceIdForSummon = 0,
                                    minOpponentInstanceIdToAttack = opponentCard?.defenseInstanceId ?? 2000,
                                    opponentLastAttackedCard = opponentCard,
                                    minMyMinInstanceIdToAttackSameTarget = opponentCard.defenseInstanceId,
                                    everMaxedBoard = false
                                });
                            }
                            else
                            {
                                CheckForPossibleMoves(actions, phase, new BoundForAction(bounds)
                                {
                                    minOpponentInstanceIdToAttack = opponentCard?.defenseInstanceId ?? 2000,
                                    opponentLastAttackedCard = opponentCard,
                                    minMyMinInstanceIdToAttackSameTarget = 0
                                });
                            }
                        }
                        actions.RemoveLast();
                        EraseMutations(action);
                    }
                }
            }
        }
    }
    public void DisplayBestMoves(List<Action> actions)
    {
        var didAction = false;
        if (!actions.Any())
        {
            Console.WriteLine("PASS");
        }
        else
        {
            foreach (Action action in actions)
            {
                if (action.source.location == Location.myDesk)
                {
                    Console.Write("ATTACK {0} {1}", action.source.instanceId, action.target != null ? action.target.instanceId : -1);
                }
                else if (action.source.cardType == CardType.Creature)
                {
                    Console.Write("SUMMON {0}", action.source.instanceId);
                }
                else
                {
                    Console.Write("USE {0} {1}", action.source.instanceId, action.target != null ? action.target.instanceId : -1);
                }
                Console.Write(";");
            }
            Console.WriteLine();
        }
    }
    public int enteredFindComb = 0;
    public int positionsEstimated = 0;
    static DateTime turnStart;
    static TimeSpan maximumTurnLength;
    public void SetValues(List<Card> _myCards, List<Card> _opponentCards, Player _me, Player _opponent, int _turn, DateTime _turnStart, TimeSpan _maximumTurnLength)
    {
        if (debug)
        {
            File.Create("../../test.txt").Close();
        }
        me = _me;
        opponent = _opponent;
        turn = _turn;
        turnStart = _turnStart;
        maximumTurnLength = _maximumTurnLength;
        myCards = _myCards;
        opponentCards = _opponentCards;
        bestPossibleActions = new List<Action>();
        bestPossibleMoveValue = -1000;
        bestPossibleMoveFirstEval = true;
        enteredFindComb = 0;
        positionsEstimated = 0;
        foreach (var card in myCards)
        {
            card.hadWard = card.ward;
            card.canAttack = card.location == Location.myDesk || (card.charge && card.location == Location.myHand) || card.location == Location.opponentDesk;
        }
        foreach (var card in opponentCards)
        {
            card.hadWard = card.ward;
        }
    }
}
class Game
{
    public static List<Card> myCards;
    public static List<Card> opponentCards;
    public static List<Card> deck = new List<Card>();
    public static List<List<Card>> allCards = new List<List<Card>>();
    public static Player me;
    public static Player opponent;
    public static int turn = 0;
    static DateTime turnStart;
    static TimeSpan maximumTurnLength = TimeSpan.FromMilliseconds(100);
    static void Main(string[] args)
    {
        //bool IsPlayer0 = Environment.UserName.Last() == '1';
        var decider = new Decider();
        string[] inputs;
        string input;
        // game loop
        while (true)
        {
            input = Console.ReadLine();
            Console.Error.WriteLine("{0}", input);
            inputs = input.Split(' ');
            myCards = new List<Card>();
            opponentCards = new List<Card>();
            me = new Player()
            {
                playerHealth = int.Parse(inputs[0]),
                playerMana = int.Parse(inputs[1]),
                playerDeck = int.Parse(inputs[2]),
                playerRune = int.Parse(inputs[3]),
                mutations = new LinkedList<Player>()
            };
            input = Console.ReadLine();
            Console.Error.WriteLine("{0}", input);
            inputs = input.Split(' ');
            opponent = new Player()
            {
                playerHealth = int.Parse(inputs[0]),
                playerMana = int.Parse(inputs[1]),
                playerDeck = int.Parse(inputs[2]),
                playerRune = int.Parse(inputs[3]),
                mutations = new LinkedList<Player>()
            };

            input = Console.ReadLine();
            Console.Error.WriteLine("{0}", input);
            int opponentHand = int.Parse(input);
            input = Console.ReadLine();
            Console.Error.WriteLine("{0}", input);
            int cardCount = int.Parse(input);
            for (int i = 0; i < cardCount; i++)
            {
                input = Console.ReadLine();
                inputs = input.Split(' ');
                Console.Error.WriteLine("{0}", input);
                var card = new Card()
                {
                    cardNumber = int.Parse(inputs[0]),
                    instanceId = int.Parse(inputs[1]),
                    location = (Location)Enum.Parse(typeof(Location), inputs[2]),
                    cardType = (CardType)Enum.Parse(typeof(CardType), inputs[3]),
                    cost = int.Parse(inputs[4]),
                    attack = int.Parse(inputs[5]),
                    defense = int.Parse(inputs[6]),
                    abilities = inputs[7],
                    breakthrough = inputs[7].Contains('B'),
                    charge = inputs[7].Contains('C'),
                    drain = inputs[7].Contains('D'),
                    guard = inputs[7].Contains('G'),
                    lethal = inputs[7].Contains('L'),
                    ward = inputs[7].Contains('W'),
                    myHealthChange = int.Parse(inputs[8]),
                    opponentHealthChange = int.Parse(inputs[9]),
                    cardDraw = int.Parse(inputs[10]),
                    mutations = new LinkedList<Card>(),
                    hasAttacked = false,
                    canAttack = true,
                    summoned = false,
                    hadWard = false,
                    used = false
                };
                switch (card.location)
                {
                    case Location.myHand:
                        myCards.Add(card);
                        break;
                    case Location.myDesk:
                        myCards.Add(card);
                        break;
                    case Location.opponentDesk:
                        opponentCards.Add(card);
                        break;
                }
            }
            turnStart = DateTime.Now;
            decider.SetValues(myCards, opponentCards, me, opponent, turn, turnStart, maximumTurnLength);
            if (turn < 30)
            {
                allCards.Add(myCards);
                var chosenCard = decider.chooseCard();
                if (chosenCard != null)
                {
                    deck.Add(chosenCard);
                }
            }
            else
            {

                var actions = decider.FindBestMoves();
                decider.DisplayBestMoves(actions);
            }
            Console.Error.WriteLine("Turn took: {0} ms entered find comb: {1} pos estim: {2}", (DateTime.Now - turnStart).TotalMilliseconds, decider.enteredFindComb, decider.positionsEstimated);
            GC.Collect();
            GC.Collect();
            turn++;
        }
    }
}