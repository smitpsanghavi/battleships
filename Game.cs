using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships
{
    // Imagine a game of battleships.
    //   The player has to guess the location of the opponent's 'ships' on a 10x10 grid
    //   Ships are one unit wide and 2-4 units long, they may be placed vertically or horizontally
    //   The player asks if a given co-ordinate is a hit or a miss
    //   Once all cells representing a ship are hit - that ship is sunk.
    public class Game
    {
        // ships: each string represents a ship in the form first co-ordinate, last co-ordinate
        //   e.g. "3:2,3:5" is a 4 cell ship horizontally across the 4th row from the 3rd to the 6th column
        // guesses: each string represents the co-ordinate of a guess
        //   e.g. "7:0" - misses the ship above, "3:3" hits it.
        // returns: the number of ships sunk by the set of guesses
        public const int MAX_BOARD_LENGTH = 10;
        public const int MAX_BOARD_BREADTH = 10;

        public static int Play(string[] ships, string[] guesses)
        {
            var shipObjects = ValidateShipCoordinates(ships);
            ValidateGuesses(guesses);
            var sunkShips = GetSunkenShips(shipObjects, guesses);
            if (sunkShips.Any())
            {
                Console.WriteLine($"You sunk {sunkShips.Count} ships.");
                Console.WriteLine($"Sunk ships are:");
                foreach (var item in sunkShips)
                {
                    Console.WriteLine(item.ToString());
                }
            }
            return sunkShips.Count;
        }

        private static Ship[] ValidateShipCoordinates(string[] ships)
        {
            var shipObjects = new Ship[ships.Length];
            for (var s = 0; s < ships.Length; s++)
            {
                // Validate co-ordinate format
                var shipStr = ships[s];
                var shipCoordinates = shipStr.Split(',');
                if (shipCoordinates.Length != 2)
                {
                    throw new Exception($"Ship co-ordinates {shipStr} are invalid");
                }

                // Validate start co-ordinate format
                var shipStart = shipCoordinates[0];
                var shipEnd = shipCoordinates[1];

                var shipStartCoords = shipStart.Split(':');
                if (shipStartCoords.Length != 2)
                {
                    throw new Exception($"Ship start co-ordinates are invalid for ship {shipStr}");
                }
                var shipStartX = Convert.ToInt32(shipStartCoords[0]);
                var shipStartY = Convert.ToInt32(shipStartCoords[1]);

                // Validate end co-ordinate format
                var shipEndCoords = shipEnd.Split(':');
                if (shipEndCoords.Length != 2)
                {
                    throw new Exception($"Ship end co-ordinates are invalid for ship {shipStr}");
                }
                var shipEndX = Convert.ToInt32(shipEndCoords[0]);
                var shipEndY = Convert.ToInt32(shipEndCoords[1]);

                // Validate ship bounds
                if (shipStartX >= MAX_BOARD_LENGTH || shipStartY >= MAX_BOARD_LENGTH || shipEndX >= MAX_BOARD_LENGTH || shipEndY >= MAX_BOARD_LENGTH)
                {
                    throw new Exception($"Ship {shipStr} is out of bounds");
                }

                // Validate direction and width of ship
                if (shipStartX != shipEndX && shipStartY != shipEndY)
                {
                    throw new Exception($"Ship co-ordinates {shipStr} are invalid as it is neither horizontal nor vertical");
                }

                // Validate ship length
                var shipObject = new Ship(shipStartCoords[0], shipStartCoords[1], shipEndCoords[0], shipEndCoords[1]);
                shipObject.AllCordinates.Add(shipStart);
                if (shipObject.IsHorizontal)
                {
                    for (var i = shipStartY + 1; i < shipEndY; i++)
                    {
                        shipObject.AllCordinates.Add($"{shipStartX}:{i}");
                        if (shipObject.AllCordinates.Count > 3)
                        {
                            throw new Exception($"Ship co-ordinates {shipStr} are too long. The maximum length is 4 units.");
                        }
                    }
                }
                else
                {
                    for (var i = shipStartX + 1; i < shipEndX; i++)
                    {
                        shipObject.AllCordinates.Add($"{i}:{shipStartY}");
                        if (shipObject.AllCordinates.Count > 3)
                        {
                            throw new Exception($"Ship co-ordinates {shipStr} are too long. The maximum length is 4 units.");
                        }
                    }
                }
                shipObject.AllCordinates.Add(shipEnd);

                // Validate overlapping ships
                for (var i = 0; i < s; i++)
                {
                    if (shipObject.AllCordinates.Intersect(shipObjects[i].AllCordinates).Any())
                    {
                        throw new Exception($"Invalid data. Ships {shipStr} and {shipObjects[i]} are overlapping.");
                    }
                }

                shipObjects[s] = shipObject;
            }
            return shipObjects;
        }

        private static void ValidateGuesses(string[] guesses)
        {
            for (var t = 0; t < guesses.Length; t++)
            {
                var guess = guesses[t];

                // Validate duplicate guesses
                if (guesses.Count(c => c == guess) > 1)
                {
                    throw new Exception($"Duplicate guess found: {guess}");
                }

                var targetCoordinates = guesses[t].Split(':');
                var targetX = Convert.ToInt32(targetCoordinates[0]);
                var targetY = Convert.ToInt32(targetCoordinates[1]);

                // Validate bounds of guess
                if (targetX >= MAX_BOARD_LENGTH || targetY >= MAX_BOARD_LENGTH)
                {
                    throw new Exception($"Guess {guess} is out of bounds");
                }
            }
        }

        private static List<Ship> GetSunkenShips(Ship[] ships, string[] guesses)
        {
            var sunkenShips = new List<Ship>();
            foreach (var item in ships)
            {
                // If all the co-ordinates of ship are in guesses then the ship has sunk
                if (!item.AllCordinates.Except(guesses).Any())
                {
                    sunkenShips.Add(item);
                }
            }
            return sunkenShips;
        }
    }

    public class Ship
    {
        public Ship(string startX, string startY, string endX, string endY)
        {
            StartX = startX;
            StartY = startY;
            EndX = endX;
            EndY = endY;
            AllCordinates = new List<string>();
        }

        public string StartX { get; set; }

        public string StartY { get; set; }

        public string EndX { get; set; }

        public string EndY { get; set; }

        public bool IsHorizontal => StartX == EndX;

        public List<string> AllCordinates { get; set; }

        public override string ToString()
        {
            return $"{StartX},{StartY}:{EndX},{EndY}";
        }
    }
}
