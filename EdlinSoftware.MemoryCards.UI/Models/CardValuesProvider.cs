using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Cryptography;
using JetBrains.Annotations;

namespace EdlinSoftware.MemoryCards.UI.Models
{
    internal class CardValuesProvider
    {
        public static Stack<int> CreateCardsValues([NotNull] GameStage stage)
        {
            Contract.Requires(stage != null);

            var totalNumberOfCards = stage.CardsRows.Sum();
            var numberOfDifferentCards = totalNumberOfCards / stage.CardsInGroup;

            var cardsValues = new List<int>();

            for (int i = 0; i < numberOfDifferentCards; i++)
            {
                cardsValues.AddRange(Enumerable.Repeat(i, stage.CardsInGroup));
            }

            using (var rndGen = new RNGCryptoServiceProvider())
            {
                byte[] bytes = new byte[cardsValues.Count];
                rndGen.GetBytes(bytes);

                int[] ints = bytes.Select(b => b - byte.MaxValue / 2).ToArray();

                var rnd = new Random((int)DateTime.UtcNow.Ticks);

                cardsValues.Sort((c1, c2) => ints[rnd.Next(0, ints.Length)]);
            }

            return new Stack<int>(cardsValues);
        }
    }
}