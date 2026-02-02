using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Services.Services.PeriodAuditUtils
{
    public static class PeriodoAuditUtils
    {
        private static void BuildPreEvaluationScale(ScaleCompany scaleCompany,  List<SubScale> subScales)
        {
             var firstCalValue = subScales.Max(s => s.Value);

                var calculatesScaleCompany = scaleCompany.Select(sc => new
                {
                    sc.Name,
                    sc.ColorCode,
                    sc.MinValue,
                    sc.MaxValue,
                    sc.NormalizedScore,
                    sc.ExpectedDistribution,
                    sc.LevelOrder,
                    CalculatedValue = null as Decimal?
                }).ToList();
                calculatesScaleCompany = [.. calculatesScaleCompany.OrderBy(sc => sc.LevelOrder)];

                if (enterpriseGrouping.ScaleType == ScaleType.Weighted)
                {
                    var lastScaleCompany = null as ScaleCompany;

                    calculatesScaleCompany = scaleCompany.Select((sc, index) =>
                    {
                        Decimal? calculatedValue = null;
                        if (index == 0)
                        {
                            calculatedValue = acumulatedScore > sc.NormalizedScore ? ((acumulatedScore - sc.NormalizedScore) / (firstCalValue - sc.NormalizedScore)) * sc.ExpectedDistribution : null;
                        }
                        else if (index != scaleCompany.Count() - 1)
                        {
                            calculatedValue = acumulatedScore > sc.NormalizedScore ? ((acumulatedScore - sc.NormalizedScore) / (lastScaleCompany.NormalizedScore - sc.NormalizedScore)) * sc.ExpectedDistribution : null;
                        }
                        lastScaleCompany = sc;
                        return new
                        {
                            sc.Name,
                            sc.ColorCode,
                            sc.MinValue,
                            sc.MaxValue,
                            sc.NormalizedScore,
                            sc.ExpectedDistribution,
                            sc.LevelOrder,
                            CalculatedValue = calculatedValue
                        };
                    }).ToList();

                    // modificar CalculatedValue solo para el ultimo elemento de la lista
                    // calculatedvalue = si al menos uno de los anteriores elementos calculatedValue != normalizedScore, entonces calculatedValue = expectedDistribution
                    // caso contrario calculatedValue = acumuladedScore >= lastElement.NormalizedScore ? ((acumulatedScore - lastElement.NormalizedScore) / (secondLastElement.NormalizedScore - lastElement.NormalizedScore)) * lastElement.ExpectedDistribution : null;
                    if (calculatesScaleCompany.Count > 1)
                    {
                        var secondLastElement = calculatesScaleCompany[calculatesScaleCompany.Count - 2];
                        var lastElement = calculatesScaleCompany.Last();
                        var hasCalculatedValueDifferent = calculatesScaleCompany
                            .Take(calculatesScaleCompany.Count - 1)
                            .Any(c => c.CalculatedValue != c.NormalizedScore);
                        var calculatedValueLast = hasCalculatedValueDifferent ?
                            lastElement.ExpectedDistribution :
                            (acumulatedScore >= lastElement.NormalizedScore ? ((acumulatedScore - lastElement.NormalizedScore) / (secondLastElement.NormalizedScore - lastElement.NormalizedScore)) * lastElement.ExpectedDistribution : null);
                        calculatesScaleCompany[calculatesScaleCompany.Count - 1] = new
                        {
                            lastElement.Name,
                            lastElement.ColorCode,
                            lastElement.MinValue,
                            lastElement.MaxValue,
                            lastElement.NormalizedScore,
                            lastElement.ExpectedDistribution,
                            lastElement.LevelOrder,
                            CalculatedValue = calculatedValueLast,
                        };
                    }
                }

                var score = calculatesScaleCompany.Aggregate(0m, (total, sc) => total + (sc.CalculatedValue ?? 0));
                score = score > 100 ? 100 : Math.Round(score, 2);
        } 
    }
}