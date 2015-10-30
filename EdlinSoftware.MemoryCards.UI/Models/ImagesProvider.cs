using System;
using System.IO;
using JetBrains.Annotations;

namespace EdlinSoftware.MemoryCards.UI.Models
{
    /// <summary>
    /// Represents provider of images for a game.
    /// </summary>
    internal class ImagesProvider
    {
        [NotNull]
        public Result<string[]> GetImagesPaths([NotNull] string gameFolder)
        {
            try
            {
                return new Success<string[]>(Directory.GetFiles(gameFolder, "*.jpg"));
            }
            catch (Exception ex)
            {
                return new Failure<string[]>(ex.Message);
            }
        }
    }
}