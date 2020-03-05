using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace CodeAndMagicReferee
{
    public class Referee
    {
        List<Card> allCards;
        const int draftPhaseTurns = 30;
        const int suddenDeathTurns = 50;
        int fullCycleCount = 0;
        Random rng;
        int lastId = 0;
        public Referee()
        {
            allCards = AllCardsInput.getAllCards();
            rng = new Random();
        }

        public void draftCard(StreamWriter writer, StreamReader reader, Player player, Player opponent, List<Card> choises)
        {

            writer.WriteLine(player.ToInputString());
            writer.WriteLine(opponent.ToInputString());
            writer.WriteLine("0");
            writer.WriteLine("3");
            writer.WriteLine(choises[0].ToInputString(false));
            writer.WriteLine(choises[1].ToInputString(false));
            writer.WriteLine(choises[2].ToInputString(false));

            var input = reader.ReadLine();
            if (input.ToUpper().StartsWith("PASS"))
            {
                player.cards.Add(new Card(choises[0]));
            }
            else
            {
                var choiseI = int.Parse(input.Replace("PICK", "").Trim(' ', ';'));
                player.cards.Add(new Card(choises[choiseI]));
            }
        }

        public void checkForRunes(Player player)
        {
            if (player.playerHealth < player.playerRune)
            {
                var newRuneValue = Math.Floor(player.playerHealth / 5);
                player.draw += (player.playerRune - newRuneValue * 5) / 5;
                player.playerRune = newRuneValue * 5;
            }
        }

        public void drawCards(Player player)
        {
            while (player.draw > 0)
            {
                if (player.cards.Where(card => card.location == Location.Deck).Count() > 0)
                {
                    if (player.cards.Where(card => card.location == Location.Hand).Count() < 8)
                    {
                        var deckCards = player.cards.Where(card => card.location == Location.Deck).ToList();
                        var drawnCard = deckCards[rng.Next(deckCards.Count)];
                        drawnCard.location = Location.Hand;
                        drawnCard.instanceId = lastId++;
                    }
                }
                else
                {
                    player.playerHealth = player.playerRune;
                    player.playerRune -= 5;
                }
                player.draw--;
            }
            player.draw = 1;
        }

        public void drawInitCards(Player player)
        {
            for (int i = 0; i < 5; ++i)
            {
                var deckCards = player.cards.Where(card => card.location == Location.Deck).ToList();
                var drawnCard = deckCards[rng.Next(deckCards.Count)];
                drawnCard.location = Location.Hand;
                drawnCard.instanceId = lastId++;
            }
        }

        public void writeGameData(StreamWriter writer, StreamReader reader, Player player, Player opponent)
        {
            writer.WriteLine(player.ToInputString());
            writer.WriteLine(opponent.ToInputString());
            writer.WriteLine(opponent.cards.Where(card => card.location == Location.Hand).Count());
            var playerCardsInGame = player.cards.Where(card => card.location == Location.Desk || card.location == Location.Hand).ToList();
            var opponentCardsInGame = opponent.cards.Where(card => card.location == Location.Desk).ToList();
            writer.WriteLine(playerCardsInGame.Count + opponentCardsInGame.Count);
            foreach (Card card in playerCardsInGame)
            {
                writer.WriteLine(card.ToInputString(false));
            }
            foreach (Card card in opponentCardsInGame)
            {
                writer.WriteLine(card.ToInputString(true));
            }
        }

        public void processOutput(StreamWriter writer, StreamReader reader, Player player, Player opponent)
        {
            var readInput = reader.ReadLine();
            var inputs = readInput.ToUpper().Split(';').Select(inp => inp.Trim()).ToList();
            var playerOneGoesFirst = true;
            foreach (var input in inputs)
            {
                if (input.Contains("PASS"))
                {
                }
                else if (input.Contains("SUMMON"))
                {
                    var instanceId = int.Parse(input.Replace("SUMMON", "").Trim(' ').Split(" ".ToCharArray())[0]);
                    var summonCard = player.cards.Where(card => card.instanceId == instanceId).First();
                    summonCard.location = Location.Desk;
                    summonCard.canAttack = summonCard.charge;
                    player.playerHealth += summonCard.myHealthChange;
                    opponent.playerHealth += summonCard.opponentHealthChange;
                    player.draw += summonCard.cardDraw;
                    player.playerMana -= summonCard.cost;
                }
                else if (input.Contains("ATTACK"))
                {
                    var creaturesIds = input.Replace("ATTACK", "").Trim(' ').Split(" ".ToCharArray()).Select(str => int.Parse(str)).ToArray();
                    var attackerInstanceId = creaturesIds[0];
                    var defenderInstanceId = creaturesIds[1];
                    if (defenderInstanceId != -1)
                    {
                        processAttack(player, opponent, attackerInstanceId, defenderInstanceId);
                    }
                    else
                    {
                        processAttack(player, opponent, attackerInstanceId);
                    }
                }
                else if (input.Contains("USE"))
                {
                    var cardIds = input.Replace("USE", "").Trim(' ').Split(" ".ToCharArray()).Select(str => int.Parse(str)).ToList();
                    var useCardId = cardIds[0];
                    var affectCardId = cardIds.Count > 1 ? cardIds[1] : (int?)null;
                    processUse(player, opponent, useCardId, affectCardId);
                }
            }
        }

        public void processUse(Player player, Player opponent, int useInstanceId, int? affectInstanceId)
        {
            var useCard = player.cards.Where(card => card.instanceId == useInstanceId).First();
            var affectCard = affectInstanceId != null && affectInstanceId != -1 ? player.cards.Where(card => card.instanceId == affectInstanceId).FirstOrDefault() ?? opponent.cards.Where(card => card.instanceId == affectInstanceId).FirstOrDefault() : null;
            player.playerHealth += useCard.myHealthChange;
            opponent.playerHealth += useCard.opponentHealthChange;
            player.draw += useCard.cardDraw;
            player.playerMana -= useCard.cost;
            if (useCard.cardType == CardType.GreenItem)
            {
                if (useCard.charge)
                    affectCard.canAttack = !affectCard.hasAttacked; // No Swift Strike hack
                affectCard.breakthrough = affectCard.breakthrough || useCard.breakthrough;
                affectCard.drain = affectCard.drain || useCard.drain;
                affectCard.guard = affectCard.guard || useCard.guard;
                affectCard.lethal = affectCard.lethal || useCard.lethal;
                affectCard.ward = affectCard.ward || useCard.ward;
                affectCard.defense += useCard.defense;
                affectCard.attack += useCard.attack;
            }
            else if (useCard.cardType == CardType.RedItem || (useCard.cardType == CardType.BlueItem && affectCard != null))
            {
                affectCard.breakthrough = affectCard.breakthrough || !useCard.breakthrough;
                affectCard.drain = affectCard.drain || !useCard.drain;
                affectCard.guard = affectCard.guard || !useCard.guard;
                affectCard.lethal = affectCard.lethal || !useCard.lethal;
                affectCard.ward = affectCard.ward || !useCard.ward;
                affectCard.defense += useCard.defense;
                affectCard.attack += useCard.attack;
            }
            else
            {
                opponent.playerHealth += useCard.defense;
            }
        }

        public void processAttack(Player player, Player opponent, int attackerInstanceId)
        {
            var attackCard = player.cards.Where(card => card.instanceId == attackerInstanceId).First();
            var attackerAfter = new Card(attackCard);
            attackerAfter.canAttack = false;
            attackerAfter.hasAttacked = true;

            attackerAfter.canAttack = false;
            attackerAfter.hasAttacked = true;

            int healthGain = attackCard.drain ? attackCard.attack : 0;
            int healthTaken = -attackCard.attack;

            player.playerHealth += healthGain;
            player.cards.Remove(attackCard);
            if (attackerAfter != null && attackerAfter.defense > 0)
            {
                player.cards.Add(attackerAfter);
            }

            opponent.playerHealth -= healthTaken;
        }

        public void processAttack(Player player, Player opponent, int attackerInstanceId, int defenderInstanceId)
        {
            var attackCard = player.cards.Where(card => card.instanceId == attackerInstanceId).First();
            var defenderCard = defenderInstanceId != -1 ? opponent.cards.Where(card => card.instanceId == defenderInstanceId).First() : null;
            var attackerAfter = new Card(attackCard);
            var defenderAfter = new Card(defenderCard);
            attackerAfter.canAttack = false;
            attackerAfter.hasAttacked = true;
            if (defenderCard.ward) defenderAfter.ward = attackCard.attack == 0;
            if (attackCard.ward) attackerAfter.ward = defenderCard.attack == 0;
            var damageGiven = defenderCard.ward ? 0 : attackCard.attack;
            var damageTaken = attackCard.ward ? 0 : defenderCard.attack;
            int healthGain = 0;
            int healthTaken = 0;

            // attacking
            if (damageGiven >= defenderCard.defense) defenderAfter = null;
            if (attackerAfter.breakthrough && defenderAfter == null) healthTaken = defenderCard.defense - damageGiven;
            if (attackCard.lethal && damageGiven > 0) defenderAfter = null;
            if (attackCard.drain && damageGiven > 0) healthGain = attackCard.attack;
            if (defenderAfter != null) defenderAfter.defense -= damageGiven;

            // defending
            if (damageTaken >= attackCard.defense) attackerAfter = null;
            if (defenderCard.lethal && damageTaken > 0) attackerAfter = null;
            if (attackerAfter != null) attackerAfter.defense -= damageTaken;

            player.playerHealth += healthGain;
            player.cards.Remove(attackCard);
            if (attackerAfter != null && attackerAfter.defense > 0)
            {
                player.cards.Add(attackerAfter);
            }

            opponent.playerHealth -= healthTaken;
            opponent.cards.Remove(defenderCard);
            if (defenderAfter != null && defenderAfter.defense > 0)
            {
                opponent.cards.Add(defenderAfter);
            }
        }

        public int CompareWeights(List<double> weights1, string exePath1, List<double> weights2, string exePath2, int gamesCount = 200, int forfeitWeightLose = 5, double forfeitWeightLosePercent = 1.05)
        {
            var player1Wins = 0;
            var player2Wins = 0;
            fullCycleCount += 1;
            for (var gameNumber = 0; gameNumber < gamesCount; ++gameNumber)
            {
                var proc1 = StartProc(weights1, exePath1, "1");
                var proc2 = StartProc(weights2, exePath2, "2");
                var writer1 = proc1.StandardInput;
                var writer2 = proc2.StandardInput;
                var reader1 = proc1.StandardOutput;
                var reader2 = proc2.StandardOutput;
                var player1 = new Player();
                player1.writer = writer1;
                player1.reader = reader1;
                var player2 = new Player();
                player2.writer = writer2;
                player2.reader = reader2;
                var turn = 0;
                var player1GoesFirst = rng.NextDouble() > 0.5;
                while (player1.playerHealth > 0 && player2.playerHealth > 0)
                {
                    if (turn < draftPhaseTurns)
                    {
                        List<Card> choises = new List<Card>();

                        choises.Add(allCards[rng.Next(allCards.Count)]);
                        Card nextCard = null;
                        do
                        {
                            nextCard = allCards[rng.Next(allCards.Count)];
                        } while (nextCard == choises[0]);
                        choises.Add(nextCard);
                        do
                        {
                            nextCard = allCards[rng.Next(allCards.Count)];
                        } while (nextCard == choises[0] || nextCard == choises[1]);
                        choises.Add(nextCard);

                        draftCard(writer1, reader1, player1, player2, choises);
                        draftCard(writer2, reader2, player2, player1, choises);
                    }
                    else
                    {
                        if (turn == draftPhaseTurns + suddenDeathTurns)
                        {
                            player1.cards = new List<Card>();
                            player2.cards = new List<Card>();
                        }
                        Player playerGoes1;
                        Player playerGoes2;
                        if (player1GoesFirst)
                        {
                            playerGoes1 = player1;
                            playerGoes2 = player2;
                        }
                        else
                        {
                            playerGoes1 = player2;
                            playerGoes2 = player1;
                        }
                        if (turn == draftPhaseTurns)
                        {
                            drawInitCards(playerGoes1);
                            drawInitCards(playerGoes2);
                        }
                        else
                        {
                            checkForRunes(playerGoes1);
                            drawCards(playerGoes1);
                        }
                        playerGoes1.playerMaxMana = Math.Min(12, playerGoes1.playerMaxMana + 1);
                        playerGoes1.playerMana = playerGoes1.playerMaxMana;
                        
                        if (playerGoes1.playerHealth < 0)
                        {
                            continue;
                        }

                        writeGameData(playerGoes1.writer, playerGoes1.reader, playerGoes1, playerGoes2);
                        processOutput(playerGoes1.writer, playerGoes1.reader, playerGoes1, playerGoes2);
                        if (playerGoes1.playerHealth <= 0 || playerGoes2.playerHealth <= 0)
                            continue;

                        checkForRunes(playerGoes2);
                        drawCards(playerGoes2);

                        playerGoes2.playerMaxMana = Math.Min(12, playerGoes2.playerMaxMana + 1);
                        playerGoes2.playerMana = playerGoes2.playerMaxMana;
                        if (playerGoes2.playerHealth < 0)
                        {
                            continue;
                        }

                        writeGameData(playerGoes2.writer, playerGoes2.reader, playerGoes2, playerGoes1);
                        processOutput(playerGoes2.writer, playerGoes2.reader, playerGoes2, playerGoes1);
                        if (playerGoes2.playerHealth <= 0 || playerGoes1.playerHealth <= 0)
                            continue;

                    }
                    turn++;
                }
                if (player1.playerHealth <= 0)
                {
                    player2Wins += 1;
                }
                else
                {
                    player1Wins += 1;
                }

                proc1.Kill();
                proc2.Kill();

                if (player1Wins * forfeitWeightLosePercent > player2Wins + forfeitWeightLose)
                {
                    Console.WriteLine("FULL CYCLE {0} {1}", fullCycleCount, gameNumber);
                    Console.WriteLine("Player 1 total1Wins: {0}", player1Wins);
                    Console.WriteLine("Player 2 total2Wins: {0}", player2Wins);
                    return -1;
                }
            }
            Console.WriteLine("FULL CYCLE {0} {1}", fullCycleCount);
            Console.WriteLine("Player 1 total1Wins: {0}", player1Wins);
            Console.WriteLine("Player 2 total2Wins: {0}", player2Wins);
            return player2Wins - player1Wins;
        }
        Process StartProc(List<double> weights, string exePath, string mark)
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = string.Join(" ", weights),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            proc.ErrorDataReceived += (s, e) =>
            {
                //Console.WriteLine("Error{0}: {1}", mark, e.Data);
            };

            proc.Start();

            proc.PriorityClass = ProcessPriorityClass.High;

            proc.BeginErrorReadLine();

            return proc;
        }
    }
}
