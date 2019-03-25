﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProgressiveColonizationSystem.ProductionChain;

namespace ProgressiveColonizationSystem.UnitTests
{
    [TestClass]
    public class CombinerTests
    {
        public const double SecondsPerKerbanDay = 6.0 * 60.0 * 60.0;

        public const double TestTolerance = .01 / SecondsPerKerbanDay;

        public const double PlentifulAmount = 1000.0; // Wildly larger than our tests use

        /// <summary>
        ///   Validates the basic case where we have some supplies on board from Kerban.
        /// </summary>
        [TestMethod]
        public void Combiner_SaturateConverter()
        {
            var colonizationResearchScenario = new StubColonizationResearchScenario(TechTier.Tier0);
            Dictionary<string, double> available = new Dictionary<string, double>();
            available.Add("Snacks-Tier4", PlentifulAmount);
            available.Add(StubRocketPartCombiner.ExpectedInputResource, PlentifulAmount);
            Dictionary<string, double> storage = new Dictionary<string, double>();
            storage.Add(StubRocketPartCombiner.ExpectedOutputResource, PlentifulAmount);
            var producers = new List<ITieredProducer>()
            {
                new StubDrill() { ProductionRate = 10.0 },
                new StubPartFactory() { ProductionRate = 2.0 },
                new StubPartFactory() { ProductionRate = 2.0 },
            };
            var combiners = new List<ITieredCombiner>()
            {
                new StubRocketPartCombiner() { ProductionRate = 5.0 }
            };

            TieredProduction.CalculateResourceUtilization(
                5 /* kerbals */, 1.0 /* seconds*/, producers, combiners,
                colonizationResearchScenario, available, storage,
                out double timePassedInSeconds, out List<TieredResource> breakthroughs,
                out Dictionary<string, double> consumptionPerSecond, out Dictionary<string, double> productionPerSecond);
            Assert.AreEqual(timePassedInSeconds, 1.0);
            Assert.AreEqual(false, breakthroughs.Any());
            Assert.IsNotNull(consumptionPerSecond);
            Assert.AreEqual(2, consumptionPerSecond.Count);
            Assert.AreEqual(5 / SecondsPerKerbanDay, consumptionPerSecond["Snacks-Tier4"], TestTolerance);
            Assert.AreEqual(5.0 * (1.0 - StubRocketPartCombiner.ExpectedTier0Ratio) / SecondsPerKerbanDay, consumptionPerSecond[StubRocketPartCombiner.ExpectedInputResource], TestTolerance);
            Assert.AreEqual(1, productionPerSecond.Count);
            Assert.AreEqual(5.0 / SecondsPerKerbanDay, productionPerSecond[StubRocketPartCombiner.ExpectedOutputResource], TestTolerance);
        }

        /// <summary>
        ///   Validates the basic case where we have some supplies on board from Kerban.
        /// </summary>
        [TestMethod]
        public void Combiner_SingleSaturateLocalPartMaker()
        {
            var colonizationResearchScenario = new StubColonizationResearchScenario(TechTier.Tier0);
            Dictionary<string, double> available = new Dictionary<string, double>();
            available.Add("Snacks-Tier4", PlentifulAmount);
            available.Add(StubRocketPartCombiner.ExpectedInputResource, PlentifulAmount);
            Dictionary<string, double> storage = new Dictionary<string, double>();
            storage.Add(StubRocketPartCombiner.ExpectedOutputResource, PlentifulAmount);
            var producers = new List<ITieredProducer>()
            {
                new StubDrill() { ProductionRate = 10.0 },
                new StubPartFactory() { ProductionRate = 2.0 },
                new StubPartFactory() { ProductionRate = 2.0 },
            };
            var combiners = new List<ITieredCombiner>()
            {
                new StubRocketPartCombiner() { ProductionRate = 5.0 },
                new StubRocketPartCombiner() { ProductionRate = 6.0 }
            };

            TieredProduction.CalculateResourceUtilization(
                5 /* kerbals */, 1.0 /* seconds*/, producers, combiners,
                colonizationResearchScenario, available, storage,
                out double timePassedInSeconds, out List<TieredResource> breakthroughs,
                out Dictionary<string, double> consumptionPerSecond, out Dictionary<string, double> productionPerSecond);
            var consumptionPerDay = ConvertToPerDay(consumptionPerSecond);
            var productionPerDay = ConvertToPerDay(productionPerSecond);
            Assert.AreEqual(timePassedInSeconds, 1.0);
            Assert.AreEqual(false, breakthroughs.Any());
            Assert.IsNotNull(consumptionPerDay);
            Assert.AreEqual(2, consumptionPerDay.Count);
            Assert.AreEqual(5, consumptionPerDay["Snacks-Tier4"], TestTolerance);
            Assert.AreEqual(10.0 * (1.0 - StubRocketPartCombiner.ExpectedTier0Ratio), consumptionPerDay[StubRocketPartCombiner.ExpectedInputResource], TestTolerance);
            Assert.AreEqual(1, productionPerSecond.Count);
            Assert.AreEqual(10.0, productionPerDay[StubRocketPartCombiner.ExpectedOutputResource], TestTolerance);
        }

        /// <summary>
        ///   Validates the basic case where we have some supplies on board from Kerban.
        /// </summary>
        [TestMethod]
        public void Combiner_SingleSaturateDrill()
        {
            var colonizationResearchScenario = new StubColonizationResearchScenario(TechTier.Tier0);
            Dictionary<string, double> available = new Dictionary<string, double>();
            available.Add("Snacks-Tier4", PlentifulAmount);
            available.Add(StubRocketPartCombiner.ExpectedInputResource, PlentifulAmount);
            Dictionary<string, double> storage = new Dictionary<string, double>();
            storage.Add(StubRocketPartCombiner.ExpectedOutputResource, PlentifulAmount);
            var producers = new List<ITieredProducer>()
            {
                new StubDrill() { ProductionRate = 4.0 },
                new StubPartFactory() { ProductionRate = 2.1 },
                new StubPartFactory() { ProductionRate = 2.0 },
            };
            var combiners = new List<ITieredCombiner>()
            {
                new StubRocketPartCombiner() { ProductionRate = 5.0 },
                new StubRocketPartCombiner() { ProductionRate = 6.0 }
            };

            TieredProduction.CalculateResourceUtilization(
                5 /* kerbals */, 1.0 /* seconds*/, producers, combiners,
                colonizationResearchScenario, available, storage,
                out double timePassedInSeconds, out List<TieredResource> breakthroughs,
                out Dictionary<string, double> consumptionPerSecond, out Dictionary<string, double> productionPerSecond);
            var consumptionPerDay = ConvertToPerDay(consumptionPerSecond);
            var productionPerDay = ConvertToPerDay(productionPerSecond);
            Assert.AreEqual(timePassedInSeconds, 1.0);
            Assert.AreEqual(false, breakthroughs.Any());
            Assert.IsNotNull(consumptionPerDay);
            Assert.AreEqual(2, consumptionPerDay.Count);
            Assert.AreEqual(5, consumptionPerDay["Snacks-Tier4"], TestTolerance);
            Assert.AreEqual(10.0 * (1.0 - StubRocketPartCombiner.ExpectedTier0Ratio), consumptionPerDay[StubRocketPartCombiner.ExpectedInputResource], TestTolerance);
            Assert.AreEqual(1, productionPerSecond.Count);
            Assert.AreEqual(10.0, productionPerDay[StubRocketPartCombiner.ExpectedOutputResource], TestTolerance);
        }

        /// <summary>
        ///   Validates the basic case where we have some supplies on board from Kerban.
        /// </summary>
        [TestMethod]
        public void Combiner_SingleSaturateRawMaterials()
        {
            var colonizationResearchScenario = new StubColonizationResearchScenario(TechTier.Tier0);
            Dictionary<string, double> available = new Dictionary<string, double>();
            available.Add("Snacks-Tier4", PlentifulAmount);
            available.Add(StubRocketPartCombiner.ExpectedInputResource, PlentifulAmount);
            Dictionary<string, double> storage = new Dictionary<string, double>();
            storage.Add(StubRocketPartCombiner.ExpectedOutputResource, PlentifulAmount);
            var producers = new List<ITieredProducer>()
            {
                new StubDrill() { ProductionRate = 1.0, Tier = TechTier.Tier1 },
                new StubPartFactory() { ProductionRate = 1.0, Tier = TechTier.Tier1 },
            };
            var combiners = new List<ITieredCombiner>()
            {
                new StubRocketPartCombiner() { ProductionRate = 1.0 }
            };

            // No snacks = no production
            available["Snacks-Tier4"] = 0;
            TieredProduction.CalculateResourceUtilization(
                5 /* kerbals */, 1.0 /* seconds*/, producers, combiners,
                colonizationResearchScenario, available, storage,
                out double timePassedInSeconds, out List<TieredResource> breakthroughs,
                out Dictionary<string, double> consumptionPerSecond, out Dictionary<string, double> productionPerSecond);
            Assert.AreEqual(timePassedInSeconds, 0.0);
            available["Snacks-Tier4"] = PlentifulAmount;

            // Limit by the input resource
            available[StubRocketPartCombiner.ExpectedInputResource] =  .25 * (1 - StubRocketPartCombiner.ExpectedTier1Ratio)/SecondsPerKerbanDay;
            TieredProduction.CalculateResourceUtilization(
                5 /* kerbals */, 1.0 /* seconds*/, producers, combiners,
                colonizationResearchScenario, available, storage,
                out timePassedInSeconds, out breakthroughs,
                out consumptionPerSecond, out productionPerSecond);
            Assert.AreEqual(timePassedInSeconds, .25, TestTolerance);
            available[StubRocketPartCombiner.ExpectedInputResource] = PlentifulAmount;

            // Limit by storage
            storage[StubRocketPartCombiner.ExpectedOutputResource] = .25 / SecondsPerKerbanDay;
            TieredProduction.CalculateResourceUtilization(
                5 /* kerbals */, 1.0 /* seconds*/, producers, combiners,
                colonizationResearchScenario, available, storage,
                out timePassedInSeconds, out breakthroughs,
                out consumptionPerSecond, out productionPerSecond);
            Assert.AreEqual(timePassedInSeconds, .25, TestTolerance);
        }

        private static Dictionary<string, double> ConvertToPerDay(Dictionary<string, double> unitsPerSecond)
            => unitsPerSecond.ToDictionary(pair => pair.Key, pair => pair.Value * SecondsPerKerbanDay);
    }
}