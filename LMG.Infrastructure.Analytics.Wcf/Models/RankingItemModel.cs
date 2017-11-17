/*
    Project VITAL.Dashboard
    Copyright (C) 2017 - Universiteit Hasselt
    This project has been funded with support from the European Commission (Project number: 2015-BE02-KA203-012317). 

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMG.Infrastructure.Analytics.Wcf.Models
{
    public class RankingItemModel
    {
        private string m_uri= null;
        public string Uri
        {
            get { return m_uri; }
            set { m_uri = value; }
        }

        private string m_name = null;
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        private long? m_count = null;
        public long? Count
        {
            get { return m_count; }
            set { m_count = value; }
        }

        private decimal? m_ScoreAverage = null;
        public decimal? ScoreAverage
        {
            get { return m_ScoreAverage; }
            set { m_ScoreAverage = value; }
        }
        private int? m_RetakeAverage = null;
        public int? RetakeAverage
        {
            get { return m_RetakeAverage; }
            set { m_RetakeAverage = value; }
        }
        private int? m_RetakeTotal = null;
        public int? RetakeTotal
        {
            get { return m_RetakeTotal; }
            set { m_RetakeTotal = value; }
        }
        private int? m_TimeSpentAverage = null;
        public int? TimeSpentAverage
        {
            get { return m_TimeSpentAverage; }
            set { m_TimeSpentAverage = value; }
        }
        private int? m_TimeSpentTotal = null;
        public int? TimeSpentTotal
        {
            get { return m_TimeSpentTotal; }
            set { m_TimeSpentTotal = value; }
        }
        private DateTime? m_LastLogin = null;
        public DateTime? LastLogin
        {
            get { return m_LastLogin; }
            set { m_LastLogin = value; }
        }
        private decimal? m_PdfPrintAverage = null;
        public decimal? PdfPrintAverage
        {
            get { return m_PdfPrintAverage; }
            set { m_PdfPrintAverage = value; }
        }
        private int? m_PdfPrintTotal = null;
        public int? PdfPrintTotal
        {
            get { return m_PdfPrintTotal; }
            set { m_PdfPrintTotal = value; }
        }


        private decimal m_OrderFactor = 0;
        public decimal OrderFactor
        {
            get { return m_OrderFactor; }
            set { m_OrderFactor = value; }
        }

        public RankingItemModel()
        {

        }

        public decimal GetCalculatedOrderFactorForExercises(int maxRetakeAverage, int maxRetakeTotal, int maxTimeSpentAverage, int maxTimeSpentTotal)
        {
            //if (!RetakeAverage.HasValue && !RetakeTotal.HasValue && !ScoreAverage.HasValue)
            //    return 0;

            decimal perc_RetakeAverage = 0;
            if (RetakeAverage.HasValue)
                perc_RetakeAverage = (decimal)RetakeAverage.Value / (decimal)maxRetakeAverage;

            decimal perc_RetakeTotal = 0;
            if (RetakeTotal.HasValue)
                perc_RetakeTotal = (decimal)RetakeTotal.Value / (decimal)maxRetakeTotal;

            decimal perc_ScoreAverage = 0;
            if (ScoreAverage.HasValue)
                perc_ScoreAverage = (1 - ScoreAverage.GetValueOrDefault(0));

            decimal perc_TimeSpentAverage = 0;
            if (TimeSpentAverage.HasValue)
                perc_TimeSpentAverage = (decimal)TimeSpentAverage.Value / (decimal)maxTimeSpentAverage;

            decimal perc_TimeSpentTotal = 0;
            if (TimeSpentTotal.HasValue)
                perc_TimeSpentTotal = (decimal)TimeSpentTotal.Value / (decimal)maxTimeSpentTotal;

            //Order factor: Most difficult content gets a higher factor. 
            //      Therefore, invert ScoreAverage

            List<ValueAndWeight> valuesAndWeights = new List<ValueAndWeight>();
            valuesAndWeights.Add(new ValueAndWeight(perc_RetakeAverage, 1));
            valuesAndWeights.Add(new ValueAndWeight(perc_RetakeTotal, 0.5m));
            valuesAndWeights.Add(new ValueAndWeight(perc_ScoreAverage, 1));
            valuesAndWeights.Add(new ValueAndWeight(perc_TimeSpentAverage, 1));
            //valuesAndWeights.Add(new ValueAndWeight(perc_TimeSpentTotal, 0.25m));
            
            decimal factor = valuesAndWeights.Sum(x => x.Value * x.Weight);
            decimal totalWeight = valuesAndWeights.Sum(x => x.Weight);

            m_OrderFactor = factor / totalWeight;

            return m_OrderFactor;
        }
        public decimal GetCalculatedOrderFactorForTheoryPages(int maxRetakeAverage, int maxRetakeTotal, decimal maxPdfPrintAverage, int maxPdfPrintTotal, int maxTimeSpentAverage, int maxTimeSpentTotal)
        {
            //if (!RetakeAverage.HasValue && !RetakeTotal.HasValue && !ScoreAverage.HasValue)
            //    return 0;

            decimal perc_RetakeAverage = 0;
            if (RetakeAverage.HasValue)
                perc_RetakeAverage = (decimal)RetakeAverage.Value / (decimal)maxRetakeAverage;

            decimal perc_RetakeTotal = 0;
            if (RetakeTotal.HasValue)
                perc_RetakeTotal = (decimal)RetakeTotal.Value / (decimal)maxRetakeTotal;

            decimal perc_PdfPrintAverage = 0;
            if (PdfPrintAverage.HasValue && maxPdfPrintAverage != 0)
                perc_PdfPrintAverage = (decimal)PdfPrintAverage.Value / (decimal)maxPdfPrintAverage;

            decimal perc_PdfPrintTotal = 0;
            if (PdfPrintTotal.HasValue && maxPdfPrintTotal != 0)
                perc_PdfPrintTotal = (decimal)PdfPrintTotal.Value / (decimal)maxPdfPrintTotal;

            decimal perc_TimeSpentAverage = 0;
            if (TimeSpentAverage.HasValue)
                perc_TimeSpentAverage = (decimal)TimeSpentAverage.Value / (decimal)maxTimeSpentAverage;

            //decimal perc_TimeSpentTotal = 0;
            //if (TimeSpentTotal.HasValue)
            //    perc_TimeSpentTotal = (decimal)TimeSpentTotal.Value / (decimal)maxTimeSpentTotal;

            //Order factor: Most difficult content gets a higher factor. 

            List<ValueAndWeight> valuesAndWeights = new List<ValueAndWeight>();
            valuesAndWeights.Add(new ValueAndWeight(perc_RetakeAverage, 1));
            valuesAndWeights.Add(new ValueAndWeight(perc_RetakeTotal, 0.5m));
            valuesAndWeights.Add(new ValueAndWeight(perc_PdfPrintAverage, 1));
            valuesAndWeights.Add(new ValueAndWeight(perc_PdfPrintTotal, 0.5m));
            valuesAndWeights.Add(new ValueAndWeight(perc_TimeSpentAverage, 1));
            //valuesAndWeights.Add(new ValueAndWeight(perc_TimeSpentTotal, 0.25m));

            decimal factor = valuesAndWeights.Sum(x => x.Value * x.Weight);
            decimal totalWeight = valuesAndWeights.Sum(x => x.Weight);

            m_OrderFactor = factor / totalWeight;

            return m_OrderFactor;
        }

        private class ValueAndWeight
        {
            public decimal Value {get;set;}
            public decimal Weight {get;set;}
            public ValueAndWeight(decimal value, decimal weight)
            {
                Value = value;
                Weight = weight;
            }
        }
    }
}