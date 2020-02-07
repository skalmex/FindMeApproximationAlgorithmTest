using FindMe.ApproximationAlgorithmTest.Algorithms;
using FindMe.ApproximationAlgorithmTest.Data.Models;
using FindMe.ApproximationAlgorithmTest.Data.Reposaitories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindMe.ApproximationAlgorithmTest
{
    public class DataService
    {
        private readonly IRawDataRepository rawDataRepository;

        // Maksymanla różnica czasu między odczytami by zostały uznane jako pasujące (w sekundach)
        private double ParameterMaxTimeDifferenceOfReadsToMatch = 5;

        // Czy wyliczać pozycje z odczytów z dwóch anten
        private bool ParameterCalculateFromTwoAnchors = false;

        // Czy wyliczać pozycje z odczytów z jednej anteny
        private bool ParameterCalculateFromOneAnchor = false;

        // Czy wyliczać pozycje z odczytów z czterech anten
        private bool ParameterCalculateFromFourAnchors = true;

        // Parametr określający czy lokalizacje są aproksymowane)
        private bool ParameterEnableApproximation = true;

        // Parametr określający rozmiar paczki, na które dzielimy wyniki dla tagów przy apkrosymacji)
        private int ParameterApproximationChunkSize = 5;

        // Parametr określający maksymalną różnicę trójkątów dla uwzględnienia drugiego wyniku dla 2 przecięć [piksele]
        private int ParameterMaxTrilaterationTriangleDifference2 = 20;

        // Parametr określający maksymalną różnicę trójkątów dla uwzględnienia drugiego wyniku dla 3 przecięć [piksele]
        private int ParameterMaxTrilaterationTriangleDifference3 = 40;

        // Jak wysoko noszony jest tag od ziemi (w cm)
        private int ParameterDefaultTagHeight = 110;

        // Parametr czas w sekundach pomiedzy lokalizacjami wyliczonymi z 3 anten, dla ktorych nie bierzemy pod uwage lokalizacji wyliczonych z mniejszej ilosci anten
        private double ParameterTimeBetweenLocationsFrom3Aerials = 2;

        public DataService(IRawDataRepository rawDataRepository, int meansurementsNumber, bool approximationEnabled)
        {
            this.rawDataRepository = rawDataRepository;

            this.ParameterEnableApproximation = approximationEnabled;


            switch(meansurementsNumber)
            {
                case 1:
                    {
                        this.ParameterCalculateFromOneAnchor = true;
                    }
                    break;

                case 2:
                    {
                        this.ParameterCalculateFromTwoAnchors = true;
                    }
                    break;

                case 4:
                    {
                        this.ParameterCalculateFromFourAnchors = true;
                    }
                    break;
            }
        }

        public List<LocationModel> RecalculateLocalizationData()
        {
            IEnumerable<RawDataModel> rawData = this.rawDataRepository.GetData();
            List<AnchorModel> anchorList = new List<AnchorModel>();
            List<LocationModel> locations = null;

            anchorList.Add(new AnchorModel() { AnchorId = 15380, X = 1004.50000000000000M, Y = 440.00000000000000M, Scale = 1, Height = 295 });
            anchorList.Add(new AnchorModel() { AnchorId = 15381, X = 634.25000000000000M, Y = 348.00000000000000M, Scale = 1, Height = 295 });
            anchorList.Add(new AnchorModel() { AnchorId = 15382, X = 1011.53000000000000M, Y = 28.62000000000000M, Scale = 1, Height = 295 });
            anchorList.Add(new AnchorModel() { AnchorId = 15383, X = 327.25000000000000M, Y = 286.00000000000000M, Scale = 1, Height = 295 });

            locations = this.CalculatePositions(rawData.ToList(), anchorList);
            return locations;
        }

        public List<LocationModel> CalculatePositions(List<RawDataModel> rawData, List<AnchorModel> anchors)
        {
            List<LocationModel> results = new List<LocationModel>();

            try
            {
                var filteredData = rawData.Where(r => anchors.Any(a => a.AnchorId == r.AnchorId));

                int count = filteredData.Count();

                var tagsData = filteredData.ToLookup(d => d.TagId);

                foreach (var item in tagsData)
                {
                    List<LocationModel> tagResults = new List<LocationModel>();

                    tagResults.Add(new LocationModel() {TagId = 29625, Position_X = 609.3900000000M, Position_Y = 29.2800000000M, Position_Z = 37.7400000000M, NoMovement = true, Time = DateTime.Parse("2020-01-23 13:08:23.087") });
                    tagResults.Add(new LocationModel() { TagId = 29628, Position_X = 815.5800000000M, Position_Y = 100.9700000000M, Position_Z = 0.0000000000M, NoMovement = true, Time = DateTime.Parse("2020-01-23 13:08:23.087") });
                    tagResults.Add(new LocationModel() { TagId = 29807, Position_X = 995.3300000000M, Position_Y = 236.0400000000M, Position_Z = 0.0000000000M, NoMovement = true, Time = DateTime.Parse("2020-01-23 13:08:23.087") });
                    tagResults.Add(new LocationModel() { TagId = 30107, Position_X = 597.6300000000M, Position_Y = 312.7900000000M, Position_Z = 0.0000000000M, NoMovement = true, Time = DateTime.Parse("2020-01-23 13:08:23.087") });

                    var matchedData = this.GroupData(filteredData.OrderBy(r => r.Time).ToList());

                    var matchedArray = matchedData.ToArray();
                    var length = matchedArray.Length;


                    for (int i = 0; i < length; i++)
                    {
                        var time = matchedArray[i].Key;
                        var data = matchedArray[i].Value;

                        if (data.Any())
                        {
                            try
                            {
                                var firstAnchor = anchors.FirstOrDefault(a => a.AnchorId == data.First().AnchorId);
                                bool isSucceeded = this.TryToCalculate(anchors, item, tagResults, time, data, firstAnchor, 4);

                                if (!isSucceeded)
                                {
                                    isSucceeded = this.TryToCalculate(anchors, item, tagResults, time, data, firstAnchor, 4);

                                    for (int j = 2; j >= 1; j--)
                                    {
                                        if (!isSucceeded)
                                        {
                                            var next = i >= length - 1 ? matchedArray[i] : matchedArray[i + 1];
                                            var nextNext = i >= length - 2 ? matchedArray[i] : matchedArray[i + 2];
                                            var limit = time.AddSeconds(this.ParameterTimeBetweenLocationsFrom3Aerials);

                                            if ((next.Value.Count < 3 || next.Key > limit || next.Key == time) && (nextNext.Value.Count < 3 || nextNext.Key > limit || nextNext.Key == time))
                                            {
                                                isSucceeded = this.TryToCalculate(anchors, item, tagResults, time, data, firstAnchor, j);
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }

                    if (this.ParameterEnableApproximation)
                    {
                        tagResults = this.ApplyApproximation(tagResults);
                    }

                    results.AddRange(tagResults);
                }
            }
            catch 
            { 
            }

            return results;
        }

        private Sensor CalculateSensor(AnchorModel anchor, RawDataModel rawData, DateTime batchTime, bool disablePitagorsCalculation = false)
        {
            if (rawData.Time != batchTime && rawData.Time != rawData.NextTime && (rawData.NextTime - rawData.Time).TotalSeconds <= 3)
            {
                rawData.Distance += Math.Round((rawData.NextDistance - rawData.Distance) * Convert.ToDecimal((batchTime - rawData.Time).TotalSeconds / (rawData.NextTime - rawData.Time).TotalSeconds), 2);
            }

            var heightDiff = Math.Abs(anchor.Height - this.ParameterDefaultTagHeight);
            var distance = Convert.ToDouble(rawData.Distance) * 100.0;

            if (distance > heightDiff && !disablePitagorsCalculation)
            {
                distance = Math.Sqrt((Math.Pow(distance, 2) - Math.Pow(heightDiff, 2))) / 100.0;
            }
            else
            {
                distance /= 100.0;
            }

            if (distance > 2 && distance <= 4)
            {
                distance -= 0.1;
            }
            else if (distance > 4 && distance <= 5)
            {
                distance -= 0.2;
            }
            else if (distance > 5 && distance <= 6)
            {
                distance -= 0.3;
            }
            else if (distance > 6)
            {
                distance -= 0.4;
            }

            return new Sensor()
            {
                X = Convert.ToDouble(anchor.X),
                Y = Convert.ToDouble(anchor.Y),
                Z = (anchor.Height / 100) * anchor.Scale,
                Distance = distance * anchor.Scale
            };
        }

        private bool TryToCalculate(List<AnchorModel> anchors, IGrouping<int, RawDataModel> item, List<LocationModel> tagResults, DateTime time, List<RawDataModel> dataToCalculate, AnchorModel anchor, int aerialsCount)
        {
            bool isSuccedded = false;

            try
            {
                if (this.ParameterCalculateFromFourAnchors && dataToCalculate.Count() >= 4 && aerialsCount == 4)
                {
                    var reads = dataToCalculate.Take(4);
                    bool noMovement = reads.All(r => r.NoMovement);

                    if (noMovement == true && tagResults.Any())
                    {
                        var previous = tagResults.Last();
                        this.AddResult(tagResults, item.Key, time, previous.Position_X, previous.Position_Y, previous.Position_Z, anchor.Scale, false, true);
                        isSuccedded = true;
                    }
                    else
                    {
                        var sensors = reads.Select(d => this.CalculateSensor(anchors.Single(a => a.AnchorId == d.AnchorId), d, time, true));
                        var previous = tagResults.LastOrDefault();
                        var result = Algorithms.Multilateration.Algorithm.Calculate(sensors.ToList());

                        if (result != null)
                        {
                            this.AddResult(tagResults, item.Key, time, Convert.ToDecimal(result.X), Convert.ToDecimal(result.Y), Convert.ToDecimal(result.Z), anchor.Scale, false, false);
                            isSuccedded = true;
                        }
                    }
                }
                else if (dataToCalculate.Count() >= 3 && aerialsCount == 3)
                {
                    var reads = dataToCalculate.Take(3);
                    bool noMovement = reads.All(r => r.NoMovement);

                    if (noMovement == true && tagResults.Any())
                    {
                        var previous = tagResults.Last();
                        this.AddResult(tagResults, item.Key, time, previous.Position_X, previous.Position_Y, previous.Position_Z, anchor.Scale, false, true);
                        isSuccedded = true;
                    }
                    else
                    {
                        var sensors = reads.Select(d => this.CalculateSensor(anchors.Single(a => a.AnchorId == d.AnchorId), d, time));
                        var previous = tagResults.LastOrDefault();
                        var resultPair = Algorithms.Trilateration.Trilateration.Calculate(sensors.ToList(), this.ParameterMaxTrilaterationTriangleDifference2, this.ParameterMaxTrilaterationTriangleDifference3);

                        if (resultPair != null)
                        {
                            var result = resultPair.Item1;
                            if (resultPair.Item2 != null && previous != null)
                            {
                                var distanceItem1 = Math.Pow(Math.Pow(resultPair.Item1.X - Convert.ToDouble(previous.Position_X), 2) + Math.Pow(resultPair.Item1.Y - Convert.ToDouble(previous.Position_Y), 2), 0.5);
                                var distanceItem2 = Math.Pow(Math.Pow(resultPair.Item2.X - Convert.ToDouble(previous.Position_X), 2) + Math.Pow(resultPair.Item2.Y - Convert.ToDouble(previous.Position_Y), 2), 0.5);

                                result = distanceItem1 < distanceItem2 ? resultPair.Item1 : resultPair.Item2;
                            }

                            this.AddResult(tagResults, item.Key, time, Convert.ToDecimal(result.X), Convert.ToDecimal(result.Y), Convert.ToDecimal((this.ParameterDefaultTagHeight / 100) * anchor.Scale), anchor.Scale, false, false);
                            isSuccedded = true;
                        }
                    }
                }
                else if (this.ParameterCalculateFromTwoAnchors && dataToCalculate.Count() >= 2 && aerialsCount == 2)
                {
                    var reads = dataToCalculate.Take(2);
                    bool noMovement = reads.All(r => r.NoMovement);

                    if (noMovement == true && tagResults.Any())
                    {
                        var previous = tagResults.Last();
                        this.AddResult(tagResults, item.Key, time, previous.Position_X, previous.Position_Y, Convert.ToDecimal((this.ParameterDefaultTagHeight / 100) * anchor.Scale), anchor.Scale, false, true);
                        isSuccedded = true;
                    }
                }
                else if (this.ParameterCalculateFromOneAnchor && dataToCalculate.Count() >= 1 && aerialsCount == 1)
                {
                    var data = dataToCalculate.First();

                    if (tagResults.Any() && data.Time != data.PreviousTime && (data.Time - data.PreviousTime).TotalSeconds <= 3)
                    {
                        var previousLocation = tagResults.Last();
                        var scale = data.Distance / data.PreviousDistance;
                        var x = (anchor.X ?? 0m) + (previousLocation.Position_X - (anchor.X ?? 0m)) * scale;
                        var y = (anchor.Y ?? 0m) + (previousLocation.Position_Y - (anchor.Y ?? 0m)) * scale;

                        this.AddResult(tagResults, item.Key, time, Math.Round(x, 2), Math.Round(y, 2), Convert.ToDecimal((this.ParameterDefaultTagHeight / 100) * anchor.Scale), anchor.Scale, true, false);
                    }
                }
            }
            catch
            {
            }

            return isSuccedded;
        }

        private void AddResult(List<LocationModel> tagResults, int tagId, DateTime time, decimal x, decimal y, decimal z, double scale, bool validate, bool noMovement)
        {
            var tagResult = new LocationModel()
            {
                Position_X = x,
                Position_Y = y,
                Position_Z = z,
                TagId = tagId,
                Time = time,
                NoMovement = noMovement
            };

            tagResults.Add(tagResult);
        }

        private List<LocationModel> ApplyApproximation(List<LocationModel> locations)
        {
            List<LocationModel> result = new List<LocationModel>();

            try
            {
                var chunks = locations
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / this.ParameterApproximationChunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();

                chunks.ForEach(c =>
                {
                    var chunkResult = Algorithms.LinearRegression.Algorithm.Calculate(c.Select(i => new Point(i.Position_X, i.Position_Y)).ToArray());
                    for (int i = 0; i < chunkResult.Length; i++)
                    {
                        c[i].Position_X = Convert.ToDecimal(chunkResult[i].X);
                        if (!double.IsNaN(chunkResult[i].Y))
                        {
                            c[i].Position_Y = Convert.ToDecimal(chunkResult[i].Y);
                        }
                    }

                    result.AddRange(c);
                });
            }
            catch
            {
            }

            return result;
        }

        private Dictionary<DateTime, List<RawDataModel>> GroupData(List<RawDataModel> tagData)
        {
            Dictionary<DateTime, List<RawDataModel>> result = new Dictionary<DateTime, List<RawDataModel>>();

            try
            {
                var aerialsData = tagData.Select(r => r.AnchorId).Distinct();

                if ((!this.ParameterCalculateFromTwoAnchors && aerialsData.Count() < 3) || (this.ParameterCalculateFromTwoAnchors && aerialsData.Count() < 2))
                {
                    return result;
                }

                RawDataModel firstItem = null;
                RawDataModel secondItem = null;
                RawDataModel thirdItem = null;
                RawDataModel fourthItem = null;

                Dictionary<int, RawDataModel> previousData = new Dictionary<int, RawDataModel>();

                if (tagData.Count() >= 4)
                {
                    DateTime lastTime = DateTime.MinValue;
                    for (int i = 0; i <= tagData.Count() - 4; i++)
                    {
                        firstItem = tagData[i];
                        secondItem = tagData[i + 1];
                        thirdItem = tagData[i + 2];
                        fourthItem = tagData[i + 3];

                        if (!previousData.ContainsKey(firstItem.AnchorId))
                        {
                            firstItem.PreviousDistance = firstItem.Distance;
                            firstItem.PreviousTime = firstItem.Time;

                            previousData.Add(firstItem.AnchorId, firstItem);
                        }
                        else
                        {
                            var previousItem = previousData[firstItem.AnchorId];

                            firstItem.PreviousDistance = previousItem.Distance;
                            firstItem.PreviousTime = previousItem.Time;

                            previousData[firstItem.AnchorId] = firstItem;
                        }

                        if (i == tagData.Count() - 4)
                        {
                            secondItem.PreviousDistance = previousData.ContainsKey(secondItem.AnchorId) ? previousData[secondItem.AnchorId].Distance : secondItem.Distance;
                            secondItem.PreviousTime = previousData.ContainsKey(secondItem.AnchorId) ? previousData[secondItem.AnchorId].Time : secondItem.Time;
                            thirdItem.PreviousDistance = previousData.ContainsKey(thirdItem.AnchorId) ? previousData[thirdItem.AnchorId].Distance : thirdItem.Distance;
                            thirdItem.PreviousTime = previousData.ContainsKey(thirdItem.AnchorId) ? previousData[thirdItem.AnchorId].Time : thirdItem.Time;
                            fourthItem.PreviousDistance = previousData.ContainsKey(fourthItem.AnchorId) ? previousData[fourthItem.AnchorId].Distance : fourthItem.Distance;
                            fourthItem.PreviousTime = previousData.ContainsKey(fourthItem.AnchorId) ? previousData[fourthItem.AnchorId].Time : fourthItem.Time;
                        }

                        if (this.ParameterCalculateFromFourAnchors &&
                            firstItem.AnchorId != secondItem.AnchorId &&
                            firstItem.AnchorId != thirdItem.AnchorId &&
                            firstItem.AnchorId != fourthItem.AnchorId &&
                            secondItem.AnchorId != thirdItem.AnchorId &&
                            secondItem.AnchorId != fourthItem.AnchorId &&
                            (secondItem.Time - firstItem.Time).TotalSeconds <= this.ParameterMaxTimeDifferenceOfReadsToMatch &&
                            (thirdItem.Time - firstItem.Time).TotalSeconds <= this.ParameterMaxTimeDifferenceOfReadsToMatch &&
                            (fourthItem.Time - firstItem.Time).TotalSeconds <= this.ParameterMaxTimeDifferenceOfReadsToMatch)
                        {
                            if (!result.Keys.Contains(fourthItem.Time) && fourthItem.Time > lastTime)
                            {
                                result.Add(fourthItem.Time, new List<RawDataModel>() { firstItem, secondItem, thirdItem, fourthItem }.OrderBy(r => r.AnchorId).ToList());
                                lastTime = fourthItem.Time;
                            }
                        }
                        else if (firstItem.AnchorId != secondItem.AnchorId &&
                            firstItem.AnchorId != thirdItem.AnchorId &&
                            secondItem.AnchorId != thirdItem.AnchorId &&
                            (secondItem.Time - firstItem.Time).TotalSeconds <= this.ParameterMaxTimeDifferenceOfReadsToMatch &&
                            (thirdItem.Time - firstItem.Time).TotalSeconds <= this.ParameterMaxTimeDifferenceOfReadsToMatch)
                        {
                            if (!result.Keys.Contains(thirdItem.Time) && thirdItem.Time > lastTime)
                            {
                                result.Add(thirdItem.Time, new List<RawDataModel>() { firstItem, secondItem, thirdItem }.OrderBy(r => r.AnchorId).ToList());
                                lastTime = thirdItem.Time;
                            }
                        }
                        else if (this.ParameterCalculateFromTwoAnchors &&
                                 firstItem.AnchorId != secondItem.AnchorId &&
                                 (secondItem.Time - firstItem.Time).TotalSeconds <= this.ParameterMaxTimeDifferenceOfReadsToMatch &&
                                 (firstItem.AnchorId == thirdItem.AnchorId ||
                                 secondItem.AnchorId == thirdItem.AnchorId))
                        {
                            if (!result.Keys.Contains(secondItem.Time) && secondItem.Time > lastTime)
                            {
                                result.Add(secondItem.Time, new List<RawDataModel>() { firstItem, secondItem }.OrderBy(r => r.AnchorId).ToList());
                                lastTime = secondItem.Time;
                            }
                        }
                        else if (this.ParameterCalculateFromOneAnchor)
                        {
                            if (!result.Keys.Contains(firstItem.Time) && firstItem.Time > lastTime)
                            {
                                result.Add(firstItem.Time, new List<RawDataModel>() { firstItem }.OrderBy(r => r.AnchorId).ToList());
                                lastTime = firstItem.Time;
                            }
                        }
                    }
                }

                var list = result.ToList().OrderByDescending(l => l.Key).ToList();

                Dictionary<int, RawDataModel> nextData = new Dictionary<int, RawDataModel>();
                foreach (var elem in list)
                {
                    for (int j = elem.Value.Count - 1; j >= 0; j--)
                    {
                        if (!nextData.ContainsKey(elem.Value[j].AnchorId))
                        {
                            elem.Value[j].NextDistance = elem.Value[j].Distance;
                            elem.Value[j].NextTime = elem.Value[j].Time;

                            nextData.Add(elem.Value[j].AnchorId, elem.Value[j]);
                        }
                        else
                        {
                            elem.Value[j].NextDistance = nextData[elem.Value[j].AnchorId].Distance;
                            elem.Value[j].NextTime = nextData[elem.Value[j].AnchorId].Time;

                            nextData[elem.Value[j].AnchorId] = elem.Value[j];
                        }
                    }
                }
            }
            catch 
            { 
            }

            return result;
        }
    }
}
