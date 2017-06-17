﻿using System;
using System.Collections.Generic;
using System.Linq;
using WasteTrader.Database;
using WasteTrader.MathUtils;
using WasteTrader.Measurements;

namespace WasteTrader.Matchmaking
{
    class SimpleMatchMaker : RoughMatchMaker
    {
        public override Waste[] Match(IMatchParameters parameters, IEnumerable<Waste> searchspace)
        {
            List<Waste> filteredResult = new List<Waste>();

            foreach (Waste waste in searchspace)
            {
                IMeasurement measurement = waste.Measurement;

                if (DateFilter(waste.EntryTime, parameters) &&
                    UnitTypeFilter(waste.Unit, parameters) &&
                    QuantityFilter(measurement.Quantity, parameters) &&
                    PricePerUnitFilter(measurement.Quantity, waste.Price, parameters) &&
                    DistanceFilter(waste.Location, parameters))
                {
                    filteredResult.Add(waste);
                }
            }

            return parameters.Sorter.Sort(filteredResult).Take(parameters.MaxMatches).ToArray();
        }

        protected bool DateFilter(DateTime time, IMatchParameters parameters)
        {
            if (time <= parameters.Youngest.ToUniversalTime())
                return false;
            else
                return (parameters.Oldest == null || time <= parameters.Oldest?.ToUniversalTime());
        }

        protected bool UnitTypeFilter(UnitType type, IMatchParameters parameters)
        {
            return (parameters.UnitType == 0 || type == parameters.UnitType);
        }

        protected bool QuantityFilter(long quantity, IMatchParameters parameters)
        {
            if (quantity < parameters.MinQuantity)
                return false;
            else
                return (parameters.MaxQuantity == 0 || quantity < parameters.MaxQuantity);
        }

        protected bool PricePerUnitFilter(long quantity, long price, IMatchParameters parameters)
        {
            return (parameters.PricePerUnitLimit == 0 || ((double)quantity) / price < parameters.PricePerUnitLimit);
        }

        protected bool DistanceFilter(ILocation location, IMatchParameters parameters)
        {
            if (parameters.SearchFrom == null || location == null)
                return true;

            double distance = GeographyMath.RoughEarthDistance(parameters.SearchFrom, location);

            if (distance > parameters.MinDistance)
                return false;
            else
                return (parameters.MaxDistance == 0 || distance < parameters.MaxDistance);

        }
    }
}
