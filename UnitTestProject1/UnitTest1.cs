using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        List<Card> myCards;
        List<Card> opponentCards;
        Player me;
        Player opponent;
        Decider decider = new Decider();
        string[] inputs;
        string input;

        void prepare(string[] inputStrings, int turn = 43)
        {
            input = inputStrings[0];
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
            input = inputStrings[1];
            inputs = input.Split(' ');
            opponent = new Player()
            {
                playerHealth = int.Parse(inputs[0]),
                playerMana = int.Parse(inputs[1]),
                playerDeck = int.Parse(inputs[2]),
                playerRune = int.Parse(inputs[3]),
                mutations = new LinkedList<Player>()
            };

            int opponentHand = int.Parse(inputStrings[2]);
            int cardCount = int.Parse(inputStrings[3]);
            for (int i = 0; i < cardCount; i++)
            {
                input = inputStrings[4 + i];
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
            decider.SetValues(myCards, opponentCards, me, opponent, turn, DateTime.Now, TimeSpan.FromHours(6));
        }
        [TestMethod]
        public void TestMethod1()
        {
            var inputStrings = @"32 3 23 25
30 2 23 25
6
7
26 1 0 0 2 3 2 ------ 0 -1 0 
139 5 0 1 4 0 0 ----LW 0 0 0 
118 13 0 1 0 0 3 ------ 0 0 0 
1 3 1 0 1 2 4 ------ 0 0 0 
1 7 1 0 1 2 1 ------ 1 0 0 
3 11 1 0 1 2 2 ------ 0 0 0 
27 4 -1 0 2 2 2 ------ 2 0 0".Split(new[] { Environment.NewLine },
    StringSplitOptions.None);
            prepare(inputStrings);

            var actions = decider.FindBestMoves();
            decider.DisplayBestMoves(actions);
        }
        [TestMethod]
        public void TestMethod2()
        {
            var turnStart = DateTime.Now;
            var inputStrings = @"32 6 20 25
15 5 19 10
6
9
27 19 0 0 2 2 2 ------ 2 0 0 
1 3 1 0 1 2 1 ----L- 0 0 0 
26 1 1 0 2 3 2 ------ 0 0 0 
1 7 1 0 1 2 1 ------ 0 0 0 
3 11 1 0 1 2 2 ------ 0 0 0 
41 15 1 0 3 2 2 -CD--- 0 0 0 
2 17 1 0 1 1 2 ------ 0 -1 0 
98 2 -1 0 3 2 4 ---G-- 0 0 0 
6 8 -1 0 2 3 2 ------ 0 0 0".Split(new[] { Environment.NewLine },
    StringSplitOptions.None);
            prepare(inputStrings);

            var thisTurnStart = DateTime.Now;
            var actions = decider.FindBestMoves();
            decider.DisplayBestMoves(actions);
            var thisTurnEnd = DateTime.Now;
            Console.Error.WriteLine("Turn took: {0} ms entered find comb: {1} pos estim: {2}", (DateTime.Now - turnStart).TotalMilliseconds, decider.enteredFindComb, decider.positionsEstimated);
            Assert.IsTrue(thisTurnEnd < thisTurnStart + TimeSpan.FromMilliseconds(50));
        }

        [TestMethod]
        public void TestMethod3()
        {
            var turnStart = DateTime.Now;
            var inputStrings = @"16 7 17 15
47 6 19 25
0
12
66 7 0 0 5 5 1 -----W 0 0 0 
140 13 0 1 2 0 0 -C---- 0 0 0 
83 15 0 0 0 1 1 -C---- 0 0 0 
83 21 0 0 0 1 1 -C---- 0 0 0 
139 23 0 1 4 0 0 ----LW 0 0 0 
55 25 0 0 2 0 5 ---G-- 0 0 0 
56 5 1 0 4 2 13 ------ 0 0 0 
9 19 1 0 3 3 2 ------ 0 0 0 
38 12 -1 0 1 2 6 --DG-- 0 0 0 
73 10 -1 0 4 4 4 B----- 0 0 0 
7 18 -1 0 2 2 2 ------ 0 0 0 
39 22 -1 0 1 2 1 --D--- 0 0 0".Split(new[] { Environment.NewLine },
    StringSplitOptions.None);
            prepare(inputStrings);

            var thisTurnStart = DateTime.Now;
            var actions = decider.FindBestMoves();
            decider.DisplayBestMoves(actions);
            var thisTurnEnd = DateTime.Now;
            Console.Error.WriteLine("Turn took: {0} ms entered find comb: {1} pos estim: {2}", (DateTime.Now - turnStart).TotalMilliseconds, decider.enteredFindComb, decider.positionsEstimated);
            Assert.IsTrue(thisTurnEnd < thisTurnStart + TimeSpan.FromMilliseconds(50));
        }

        [TestMethod]
        public void TestMethod4()
        {
            var turnStart = DateTime.Now;
            var inputStrings = @"10 11 11 5
21 10 14 20
3
12
91 3 0 0 0 1 2 ---G-- 0 1 0 
151 7 0 2 5 0 -99 BCDGLW 0 0 0 
91 31 0 0 0 1 2 ---G-- 0 1 0 
126 33 0 1 3 2 3 ------ 0 0 0 
84 35 0 0 2 1 1 -CD--W 0 0 0 
26 37 0 0 2 3 2 ------ 0 -1 0 
48 25 1 0 1 1 1 ----L- 0 0 0 
4 22 -1 0 2 1 3 ------ 0 0 0 
15 12 -1 0 4 4 3 ------ 0 0 0 
68 6 -1 0 6 9 8 ------ 0 0 0 
91 30 -1 0 0 1 2 ---G-- 0 0 0 
115 26 -1 0 8 5 5 ---G-W 0 0 0".Split(new[] { Environment.NewLine },
    StringSplitOptions.None);
            prepare(inputStrings);

            var thisTurnStart = DateTime.Now;
            var actions = decider.FindBestMoves();
            decider.DisplayBestMoves(actions);
            var thisTurnEnd = DateTime.Now;
            Console.Error.WriteLine("Turn took: {0} ms entered find comb: {1} pos estim: {2}", (DateTime.Now - turnStart).TotalMilliseconds, decider.enteredFindComb, decider.positionsEstimated);
            Assert.IsTrue(thisTurnEnd < thisTurnStart + TimeSpan.FromMilliseconds(50));
        }

        [TestMethod]
        public void TestMethod5()
        {
            var turnStart = DateTime.Now;
            var inputStrings = @"4 9 12 0
17 9 14 10
3
14
72 6 0 0 4 5 3 B----- 0 0 0 
91 14 0 0 0 1 2 ---G-- 0 1 0 
86 30 0 0 3 1 5 -C---- 0 0 0 
4 32 0 0 2 1 5 ------ 0 0 0 
26 34 0 0 2 3 2 ------ 0 -1 0 
136 36 0 1 0 1 1 ------ 0 0 0 
6 16 1 0 2 3 5 ------ 0 0 0 
26 22 1 0 2 3 2 ------ 0 0 0 
52 4 1 0 4 2 4 ----L- 0 0 0 
52 20 1 0 4 2 4 ----L- 0 0 0 
76 25 -1 0 6 5 5 B-D--- 0 0 0 
43 27 -1 0 6 5 5 --D--- 0 0 0 
31 17 -1 0 3 3 1 ------ 0 -1 0 
91 29 -1 0 0 1 2 ---G-- 0 1 0".Split(new[] { Environment.NewLine },
    StringSplitOptions.None);
            prepare(inputStrings);

            var thisTurnStart = DateTime.Now;
            var actions = decider.FindBestMoves();
            decider.DisplayBestMoves(actions);
            var thisTurnEnd = DateTime.Now;
            Console.Error.WriteLine("Turn took: {0} ms entered find comb: {1} pos estim: {2}", (DateTime.Now - turnStart).TotalMilliseconds, decider.enteredFindComb, decider.positionsEstimated);
            Assert.IsTrue(thisTurnEnd < thisTurnStart + TimeSpan.FromMilliseconds(50));
        }

        [TestMethod]
        public void TestMethod6()
        {
            var turnStart = DateTime.Now;
            var inputStrings = @"27 8 18 25
22 7 16 15
4
11
112 17 0 0 6 4 7 ---G-- 0 0 0 
22 19 0 0 6 7 5 ------ 0 0 0 
47 21 0 0 2 1 5 --D--- 0 0 0 
117 23 0 1 1 1 1 B----- 0 0 0 
47 13 1 0 2 0 1 --D-L- 0 0 0 
51 3 1 0 4 3 1 ----L- 0 0 0 
17 1 1 0 4 4 5 ------ 0 0 0 
111 11 1 0 6 6 6 ---G-- 0 0 0 
47 12 -1 0 2 2 3 B-D--- 0 0 0 
30 24 -1 0 3 4 2 ------ 0 -2 0 
11 28 -1 0 3 5 2 ------ 0 0 0".Split(new[] { Environment.NewLine },
    StringSplitOptions.None);
            prepare(inputStrings);

            var thisTurnStart = DateTime.Now;
            var actions = decider.FindBestMoves();
            decider.DisplayBestMoves(actions);
            var thisTurnEnd = DateTime.Now;
            Console.Error.WriteLine("Turn took: {0} ms entered find comb: {1} pos estim: {2}", (DateTime.Now - turnStart).TotalMilliseconds, decider.enteredFindComb, decider.positionsEstimated);
            Assert.IsTrue(thisTurnEnd < thisTurnStart + TimeSpan.FromMilliseconds(50));
        }

        [TestMethod]
        public void TestMethod7()
        {
            var turnStart = DateTime.Now;
            var inputStrings = @"13 7 16 10
25 6 18 20
6
16
36 3 0 0 6 4 4 ------ 0 0 2 
80 7 0 0 8 8 8 B--G-- 0 0 1 
70 17 0 0 4 6 3 B----- 0 0 0 
129 19 0 1 4 2 5 ------ 0 0 0 
38 21 0 0 1 1 3 --D--- 0 0 0 
93 23 0 0 1 2 1 ---G-- 0 0 0 
86 25 0 0 3 1 5 -C---- 0 0 0 
130 27 0 1 4 0 6 ------ 4 0 0 
86 1 1 0 3 1 5 -C---- 0 0 0 
4 13 1 0 2 1 5 ------ 0 0 0 
20 9 1 0 5 8 1 ------ 0 0 0 
22 5 1 0 6 7 5 ------ 0 0 0 
4 6 -1 0 2 1 5 ------ 0 0 0 
12 2 -1 0 3 2 2 ------ 0 0 0 
20 18 -1 0 5 8 2 ------ 0 0 0 
22 8 -1 0 6 7 5 ------ 0 0 0".Split(new[] { Environment.NewLine },
    StringSplitOptions.None);
            prepare(inputStrings);

            var thisTurnStart = DateTime.Now;
            var actions = decider.FindBestMoves();
            decider.DisplayBestMoves(actions);
            var thisTurnEnd = DateTime.Now;
            Console.Error.WriteLine("Turn took: {0} ms entered find comb: {1} pos estim: {2}", (DateTime.Now - turnStart).TotalMilliseconds, decider.enteredFindComb, decider.positionsEstimated);
            Assert.IsTrue(thisTurnEnd < thisTurnStart + TimeSpan.FromMilliseconds(50));
        }

        [TestMethod]
        public void TestMethod8()
        {
            var turnStart = DateTime.Now;
            var inputStrings = @"30 4 22 25
30 3 22 25
6
10
23 3 0 0 7 8 8 ------ 0 0 0 
29 5 0 0 2 2 1 ------ 0 0 1 
104 9 0 0 4 4 4 ---G-- 0 0 0 
69 11 0 0 3 4 4 B----- 0 0 0 
39 13 0 0 1 2 1 --D--- 0 0 0 
118 15 0 1 0 0 3 ------ 0 0 0 
94 1 1 0 2 1 3 ---G-- 0 0 0 
69 7 1 0 3 4 4 B----- 0 0 0 
94 6 -1 0 2 1 3 ---G-- 0 0 0 
94 4 -1 0 2 1 4 ---G-- 0 0 0".Split(new[] { Environment.NewLine },
    StringSplitOptions.None);
            prepare(inputStrings);

            var thisTurnStart = DateTime.Now;
            var actions = decider.FindBestMoves();
            decider.DisplayBestMoves(actions);
            var thisTurnEnd = DateTime.Now;
            Console.Error.WriteLine("Turn took: {0} ms entered find comb: {1} pos estim: {2}", (DateTime.Now - turnStart).TotalMilliseconds, decider.enteredFindComb, decider.positionsEstimated);
            Assert.IsTrue(thisTurnEnd < thisTurnStart + TimeSpan.FromMilliseconds(50));
        }

        [TestMethod]
        public void TestMethod9()
        {
            var turnStart = DateTime.Now;
            var inputStrings = @"30 2 23 25
30 2 24 25
5
8
73 2 0 0 4 4 4 B----- 4 0 0 
55 4 0 0 2 0 5 ---G-- 0 0 0 
79 8 0 0 8 8 8 B----- 0 0 0 
28 10 0 0 2 1 2 ------ 0 0 1 
80 12 0 0 8 8 8 B--G-- 0 0 1 
79 14 0 0 8 8 8 B----- 0 0 0 
38 6 1 0 1 1 3 --D--- 0 0 0 
64 5 -1 0 2 1 1 ---G-W 0 0 0".Split(new[] { Environment.NewLine },
    StringSplitOptions.None);
            prepare(inputStrings);

            var thisTurnStart = DateTime.Now;
            var actions = decider.FindBestMoves();
            decider.DisplayBestMoves(actions);
            var thisTurnEnd = DateTime.Now;
            Console.Error.WriteLine("Turn took: {0} ms entered find comb: {1} pos estim: {2}", (DateTime.Now - turnStart).TotalMilliseconds, decider.enteredFindComb, decider.positionsEstimated);
            Assert.IsTrue(thisTurnEnd < thisTurnStart + TimeSpan.FromMilliseconds(50));
        }

        [TestMethod]
        public void TestMethod10()
        {
            var turnStart = DateTime.Now;
            var inputStrings = @"14 10 15 10
16 10 12 10
4
13
79 14 0 0 8 8 8 B----- 0 0 0 
22 16 0 0 6 7 5 ------ 0 0 0 
79 18 0 0 8 8 8 B----- 0 0 0 
7 20 0 0 2 2 2 -----W 0 0 0 
8 24 0 0 2 2 3 ------ 0 0 0 
73 26 0 0 4 4 4 B----- 4 0 0 
55 28 0 0 2 0 5 ---G-- 0 0 0 
112 30 0 0 6 4 7 ---G-- 0 0 0 
7 12 1 0 2 2 2 ------ 0 0 0 
79 8 1 0 8 8 2 B----- 0 0 0 
61 4 1 0 9 10 10 ------ 0 0 0 
80 13 -1 0 8 8 1 B--G-- 0 0 0 
59 21 -1 0 7 7 7 ------ 1 -1 0".Split(new[] { Environment.NewLine },
    StringSplitOptions.None);
            prepare(inputStrings);

            var thisTurnStart = DateTime.Now;
            var actions = decider.FindBestMoves();
            decider.DisplayBestMoves(actions);
            var thisTurnEnd = DateTime.Now;
            Console.Error.WriteLine("Turn took: {0} ms entered find comb: {1} pos estim: {2}", (DateTime.Now - turnStart).TotalMilliseconds, decider.enteredFindComb, decider.positionsEstimated);
            Assert.IsTrue(thisTurnEnd < thisTurnStart + TimeSpan.FromMilliseconds(50));
        }

        [TestMethod]
        public void TestMethod11()
        {
            var turnStart = DateTime.Now;
            var inputStrings = @"32 3 23 25
30 2 23 25
6
8
7 1 0 0 2 2 2 -----W 0 0 0 
111 3 0 0 6 6 6 ---G-- 0 0 0 
36 7 0 0 6 4 4 ------ 0 0 2 
27 11 0 0 2 2 2 ------ 2 0 0 
7 13 0 0 2 2 2 -----W 0 0 0 
92 5 1 0 1 0 1 ---G-- 2 0 0 
7 9 1 0 2 2 2 -----W 0 0 0 
63 8 -1 0 2 0 4 ---G-W 0 0 0".Split(new[] { Environment.NewLine },
    StringSplitOptions.None);
            prepare(inputStrings);

            var thisTurnStart = DateTime.Now;
            var actions = decider.FindBestMoves();
            decider.DisplayBestMoves(actions);
            var thisTurnEnd = DateTime.Now;
            Console.Error.WriteLine("Turn took: {0} ms entered find comb: {1} pos estim: {2}", (DateTime.Now - turnStart).TotalMilliseconds, decider.enteredFindComb, decider.positionsEstimated);
            Assert.IsTrue(thisTurnEnd < thisTurnStart + TimeSpan.FromMilliseconds(50));
        }

        [TestMethod]
        public void TestMethod12()
        {
            var turnStart = DateTime.Now;
            var inputStrings = @"21 4 20 20
29 4 22 25
2
12
73 4 0 0 4 4 4 B----- 4 0 0 
87 6 0 0 4 2 5 -C-G-- 0 0 0 
62 10 0 0 12 12 12 B--G-- 0 0 0 
87 12 0 0 4 2 5 -C-G-- 0 0 0 
11 14 0 0 3 5 2 ------ 0 0 0 
95 18 0 0 2 2 3 ---G-- 0 0 0 
114 20 0 0 7 7 7 ---G-- 0 0 0 
12 16 1 0 3 2 5 ------ 0 0 0 
95 3 -1 0 2 2 3 ---G-- 0 0 0 
83 11 -1 0 0 1 1 -C---- 0 0 0 
95 5 -1 0 2 2 3 ---G-- 0 0 0 
33 15 -1 0 4 4 3 ------ 0 0 1".Split(new[] { Environment.NewLine },
    StringSplitOptions.None);
            prepare(inputStrings);

            var thisTurnStart = DateTime.Now;
            var actions = decider.FindBestMoves();
            decider.DisplayBestMoves(actions);
            var thisTurnEnd = DateTime.Now;
            Console.Error.WriteLine("Turn took: {0} ms entered find comb: {1} pos estim: {2}", (DateTime.Now - turnStart).TotalMilliseconds, decider.enteredFindComb, decider.positionsEstimated);
            Assert.IsTrue(thisTurnEnd < thisTurnStart + TimeSpan.FromMilliseconds(50));
        }

        [TestMethod]
        public void TestMethod13()
        {
            var turnStart = DateTime.Now;
            var inputStrings = @"28 8 15 25
28 7 16 25
5
10
102 9 0 0 4 3 3 ---G-- 0 -1 0 
140 19 0 1 2 0 0 -C---- 0 0 0 
15 21 0 0 4 4 5 ------ 0 0 0 
47 23 0 0 2 1 5 --D--- 0 0 0 
105 27 0 0 5 4 6 ---G-- 0 0 0 
160 29 0 3 2 0 0 ------ 2 -2 0 
160 29 0 3 2 0 0 ------ 2 -2 0 
68 25 1 0 6 7 5 ------ 0 0 0 
105 2 -1 0 5 4 2 ------ 0 0 0 
3 6 -1 0 1 2 2 ------ 0 0 0 
111 28 -1 0 6 6 6 ---G-- 0 0 0".Split(new[] { Environment.NewLine },
    StringSplitOptions.None);
            prepare(inputStrings);

            var thisTurnStart = DateTime.Now;
            var actions = decider.FindBestMoves();
            decider.DisplayBestMoves(actions);
            var thisTurnEnd = DateTime.Now;
            Console.Error.WriteLine("Turn took: {0} ms entered find comb: {1} pos estim: {2}", (DateTime.Now - turnStart).TotalMilliseconds, decider.enteredFindComb, decider.positionsEstimated);
            Assert.IsTrue(thisTurnEnd < thisTurnStart + TimeSpan.FromMilliseconds(50));
        }

        [TestMethod]
        public void ShouldTargetWithRedCardsUselessCreatures()
        {
            var turnStart = DateTime.Now;
            var inputStrings = @"28 6 19 25
30 5 19 25
6
10
108 9 0 0 5 2 6 ---G-- 0 0 0 
149 11 0 2 3 0 0 BCDGLW 0 0 1 
149 13 0 2 3 0 0 BCDGLW 0 0 1 
3 15 0 0 1 2 2 ------ 0 0 0 
140 19 0 1 2 0 0 -C---- 0 0 0 
15 21 0 0 4 4 5 ------ 0 0 0 
97 3 1 0 3 3 3 ---G-- 0 0 0 
65 17 1 0 2 2 2 -----W 0 0 0 
33 12 -1 0 4 4 3 ------ 0 0 1 
105 2 -1 0 5 4 6 ---G-- 0 0 0".Split(new[] { Environment.NewLine },
    StringSplitOptions.None);
            prepare(inputStrings);

            var thisTurnStart = DateTime.Now;
            var actions = decider.FindBestMoves();
            decider.DisplayBestMoves(actions);
            var thisTurnEnd = DateTime.Now;
            Console.Error.WriteLine("Turn took: {0} ms entered find comb: {1} pos estim: {2}", (DateTime.Now - turnStart).TotalMilliseconds, decider.enteredFindComb, decider.positionsEstimated);
            Assert.IsTrue(!(actions.Any(act => act.source?.instanceId == 11 && act.target?.instanceId == 2) && actions.Any(act => act.source?.instanceId == 13 && act.target?.instanceId == 2)));
            Assert.IsTrue(!actions.Any(act => act.source?.instanceId == 11 && act.target?.instanceId == 12));
            Assert.IsTrue(thisTurnEnd < thisTurnStart + TimeSpan.FromMilliseconds(50));
        }

        [TestMethod]
        public void ShouldPickDmgBuff()
        {
            var turnStart = DateTime.Now;
            var inputStrings = @"30 0 0 25
30 0 0 25
0
3
131 -1 0 1 4 4 1 ------ 0 0 0 
142 -1 0 2 0 0 0 BCDGLW 0 0 0 
140 -1 0 1 2 0 0 -C---- 0 0 0".Split(new[] { Environment.NewLine },
    StringSplitOptions.None);
            prepare(inputStrings, 3);

            var thisTurnStart = DateTime.Now;
            var chosenCard = decider.chooseCard();
            var thisTurnEnd = DateTime.Now;
            Assert.IsTrue(chosenCard.attack == 4);
        }

        [TestMethod]
        public void TestMethod14()
        {
            var turnStart = DateTime.Now;
            var inputStrings = @"19 12 7 15
22 12 2 10
8
20
76 29 0 0 6 5 5 B-D--- 0 0 0 
9 33 0 0 3 3 4 ------ 0 0 0 
15 35 0 0 4 4 5 ------ 0 0 0 
111 37 0 0 6 6 6 ---G-- 0 0 0 
45 39 0 0 6 6 5 B-D--- -3 0 0 
33 41 0 0 4 4 3 ------ 0 0 1 
101 43 0 0 4 3 4 ---G-- 0 0 0 
51 45 0 0 4 3 5 ----L- 0 0 0 
15 21 1 0 4 4 1 ------ 0 0 0 
47 23 1 0 2 1 5 --D--- 0 0 0 
68 25 1 0 6 7 5 ------ 0 0 0 
3 11 1 0 1 2 2 ------ 0 0 0 
105 27 1 0 5 4 6 ---G-- 0 0 0 
51 31 1 0 4 3 5 ----L- 0 0 0 
100 24 -1 0 3 1 2 ---G-- 0 0 0 
76 34 -1 0 6 5 3 B-D--- 0 0 0 
20 36 -1 0 5 8 2 ------ 0 0 0 
51 40 -1 0 4 3 5 ----L- 0 0 0 
7 42 -1 0 2 2 2 -----W 0 0 0 
1 18 -1 0 1 2 1 ------ 1 0 0".Split(new[] { Environment.NewLine },
    StringSplitOptions.None);
            prepare(inputStrings);

            var thisTurnStart = DateTime.Now;
            var actions = decider.FindBestMoves();
            decider.DisplayBestMoves(actions);
            var thisTurnEnd = DateTime.Now;
            Console.Error.WriteLine("Turn took: {0} ms entered find comb: {1} pos estim: {2}", (DateTime.Now - turnStart).TotalMilliseconds, decider.enteredFindComb, decider.positionsEstimated);
            Assert.IsTrue(thisTurnEnd < thisTurnStart + TimeSpan.FromMilliseconds(50));
        }

        [TestMethod]
        public void ShouldAttackAnything()
        {
            var turnStart = DateTime.Now;
            var inputStrings = @"17 6 18 15
30 6 20 25
3
15
116 2 0 0 12 8 8 BCDGLW 0 0 0 
4 4 0 0 2 1 5 ------ 0 0 0 
115 10 0 0 8 5 5 ---G-W 0 0 0 
18 14 0 0 4 7 4 ------ 0 0 0 
61 16 0 0 9 10 10 ------ 0 0 0 
30 20 0 0 3 4 2 ------ 0 -2 0 
102 22 0 0 4 3 3 ---G-- 0 -1 0 
95 24 0 0 2 2 3 ---G-- 0 0 0 
18 6 1 0 4 7 3 ------ 0 0 0 
19 18 1 0 5 5 6 ------ 0 0 0 
100 5 -1 0 3 1 3 ---G-- 0 0 0 
18 7 -1 0 4 7 3 ------ 0 0 0 
95 13 -1 0 2 2 3 ---G-- 0 0 0 
50 11 -1 0 3 3 2 ----L- 0 0 0 
30 17 -1 0 3 4 2 ------ 0 -2 0".Split(new[] { Environment.NewLine },
    StringSplitOptions.None);
            prepare(inputStrings);

            var thisTurnStart = DateTime.Now;
            var actions = decider.FindBestMoves();
            decider.DisplayBestMoves(actions);
            var thisTurnEnd = DateTime.Now;
            Console.Error.WriteLine("Turn took: {0} ms entered find comb: {1} pos estim: {2}", (DateTime.Now - turnStart).TotalMilliseconds, decider.enteredFindComb, decider.positionsEstimated);
            Assert.IsTrue(actions.Any(act => act.source.location == Location.myDesk && act.target.location == Location.opponentDesk));
            Assert.IsTrue(thisTurnEnd < thisTurnStart + TimeSpan.FromMilliseconds(50));
        }

        [TestMethod]
        public void ShouldPickRemoval()
        {
            var turnStart = DateTime.Now;
            var inputStrings = @"30 0 0 25
30 0 0 25
0
3
151 -1 0 2 5 0 -99 BCDGLW 0 0 0 
145 -1 0 2 3 -2 -2 ------ 0 0 0 
92 -1 0 0 1 0 1 ---G-- 2 0 0".Split(new[] { Environment.NewLine },
    StringSplitOptions.None);
            prepare(inputStrings, 3);

            var thisTurnStart = DateTime.Now;
            var chosenCard = decider.chooseCard();
            var thisTurnEnd = DateTime.Now;
            Assert.IsTrue(chosenCard.defense == -99);
        }

        [TestMethod]
        public void WhySoManyCombinations()
        {
            var turnStart = DateTime.Now;
            var inputStrings = @"30 6 19 25
16 5 22 15
8
10
151 3 0 2 5 0 -99 BCDGLW 0 0 0 
151 5 0 2 5 0 -99 BCDGLW 0 0 0 
141 15 0 2 0 -1 -1 ------ 0 0 0 
51 19 0 0 4 3 5 ----L- 0 0 0 
106 21 0 0 5 5 5 ---G-- 0 0 0 
48 9 1 0 1 1 4 ----L- 0 0 0 
48 11 1 0 1 1 1 ----L- 0 0 0 
96 13 1 0 2 3 2 ---G-- 0 0 0 
65 17 1 0 2 2 2 -----W 0 0 0 
21 1 1 0 5 6 5 ------ 0 0 0".Split(new[] { Environment.NewLine },
    StringSplitOptions.None);
            prepare(inputStrings);

            var thisTurnStart = DateTime.Now;
            var actions = decider.FindBestMoves();
            decider.DisplayBestMoves(actions);
            var thisTurnEnd = DateTime.Now;
            Console.Error.WriteLine("Turn took: {0} ms entered find comb: {1} pos estim: {2}", (DateTime.Now - turnStart).TotalMilliseconds, decider.enteredFindComb, decider.positionsEstimated);
            Assert.IsTrue(thisTurnEnd < thisTurnStart + TimeSpan.FromMilliseconds(50));
        }

        [TestMethod]
        public void TestMethod15()
        {
            var turnStart = DateTime.Now;
            var inputStrings = @"31 7 18 25
28 6 19 25
4
12
80 9 0 0 8 8 8 B--G-- 0 0 1 
62 19 0 0 12 12 12 B--G-- 0 0 0 
15 21 0 0 4 4 5 ------ 0 0 0 
64 23 0 0 2 1 1 ---G-W 0 0 0 
38 13 1 0 1 1 2 --D--- 0 0 0 
70 3 1 0 4 6 1 B----- 0 0 0 
29 7 1 0 2 2 1 ------ 0 0 1 
69 12 -1 0 3 4 2 B----- 0 0 0 
38 20 -1 0 1 1 3 --D--- 0 0 0 
7 4 -1 0 2 2 2 -----W 0 0 0 
64 2 -1 0 2 1 1 ---G-- 0 0 0 
76 6 -1 0 6 5 5 B-D--- 0 0 0".Split(new[] { Environment.NewLine },
    StringSplitOptions.None);
            prepare(inputStrings);

            var thisTurnStart = DateTime.Now;
            var actions = decider.FindBestMoves();
            decider.DisplayBestMoves(actions);
            var thisTurnEnd = DateTime.Now;
            Console.Error.WriteLine("Turn took: {0} ms entered find comb: {1} pos estim: {2}", (DateTime.Now - turnStart).TotalMilliseconds, decider.enteredFindComb, decider.positionsEstimated);
            Assert.IsTrue(thisTurnEnd < thisTurnStart + TimeSpan.FromMilliseconds(50));
        }

        [TestMethod]
        public void ShouldPickLethal()
        {
            var turnStart = DateTime.Now;
            var inputStrings = @"30 0 8 25
30 0 9 25
0
3
54 -1 0 0 3 2 2 ----L- 0 0 0 
23 -1 0 0 7 8 8 ------ 0 0 0 
22 -1 0 0 6 7 5 ------ 0 0 0".Split(new[] { Environment.NewLine },
    StringSplitOptions.None);
            prepare(inputStrings, 3);

            var thisTurnStart = DateTime.Now;
            var chosenCard = decider.chooseCard();
            var thisTurnEnd = DateTime.Now;
            Assert.IsTrue(chosenCard.lethal);
        }

        [TestMethod]
        public void TestMethod16()
        {
            var turnStart = DateTime.Now;
            var inputStrings = @"28 3 22 25
30 3 22 25
6
10
54 4 0 0 3 2 2 ----L- 0 0 0 
27 6 0 0 2 2 2 ------ 2 0 0 
44 8 0 0 6 3 7 --D-L- 0 0 0 
7 10 0 0 2 2 2 -----W 0 0 0 
51 12 0 0 4 3 5 ----L- 0 0 0 
33 14 0 0 4 4 3 ------ 0 0 1 
18 16 0 0 4 7 4 ------ 0 0 0 
65 2 1 0 2 2 2 -----W 0 0 0 
29 1 -1 0 2 2 1 ------ 0 0 0 
98 5 -1 0 3 2 4 ---G-- 0 0 0".Split(new[] { Environment.NewLine },
    StringSplitOptions.None);
            prepare(inputStrings);

            var thisTurnStart = DateTime.Now;
            var actions = decider.FindBestMoves();
            decider.DisplayBestMoves(actions);
            var thisTurnEnd = DateTime.Now;
            Console.Error.WriteLine("Turn took: {0} ms entered find comb: {1} pos estim: {2}", (DateTime.Now - turnStart).TotalMilliseconds, decider.enteredFindComb, decider.positionsEstimated);
            Assert.IsTrue(thisTurnEnd < thisTurnStart + TimeSpan.FromMilliseconds(50));
        }
    }
}
