using System.Linq;
using NUnit.Framework;
using UnityEngine;
using KyndeBlade;

namespace KyndeBlade.Tests
{
    /// <summary>Data contract for the vertical slice: tour → map → fayre_felde encounter.</summary>
    public class PlayableSliceEditModeTests
    {
        const string FayreFeldeResourcePath = "Data/Vision1/Loc_fayre_felde";
        const string TourResourcePath = "Data/Vision1/Loc_tour";

        [Test]
        public void PlayableSliceConstants_FirstCombatLocation_MatchesFayreFeldeAsset()
        {
            var loc = Resources.Load<LocationNode>(FayreFeldeResourcePath);
            Assert.IsNotNull(loc, $"Missing Resources asset at {FayreFeldeResourcePath}");
            Assert.AreEqual(GameWorldConstants.PlayableSliceFirstCombatLocationId, loc.LocationId);
        }

        [Test]
        public void PlayableSlice_FayreFelde_HasEncounter_WithFalseEnemy()
        {
            var loc = Resources.Load<LocationNode>(FayreFeldeResourcePath);
            Assert.IsNotNull(loc);
            Assert.IsNotNull(loc.Encounter, "Fair Field must reference an EncounterConfig for the slice combat loop.");
            var enemies = loc.Encounter.Enemies;
            Assert.IsNotNull(enemies);
            Assert.Greater(enemies.Count, 0, "Slice encounter should spawn at least one enemy.");
            bool hasFalse = enemies.Any(e =>
                e != null && !string.IsNullOrEmpty(e.CharacterTypeName) &&
                e.CharacterTypeName.ToLowerInvariant().Contains("false"));
            Assert.IsTrue(hasFalse, "Fair Field slice encounter should include a False-type spawn (CharacterTypeName).");
        }

        [Test]
        public void PlayableSlice_Tour_ListsFayreFeldeAsNext()
        {
            var tour = Resources.Load<LocationNode>(TourResourcePath);
            Assert.IsNotNull(tour);
            Assert.IsNotNull(tour.NextLocationIds);
            CollectionAssert.Contains(tour.NextLocationIds, GameWorldConstants.PlayableSliceFirstCombatLocationId);
        }
    }
}
