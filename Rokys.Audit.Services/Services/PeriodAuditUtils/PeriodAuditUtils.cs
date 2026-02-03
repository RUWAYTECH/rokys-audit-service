using Rokys.Audit.Common.Constant;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Services.Services.PeriodAuditUtils
{
    public class ScaleCalculationDetail
    {
        public string FieldName { get; set; } = string.Empty;
        public decimal FieldValue { get; set; }
    }

    public static class PeriodAuditCalculator
    {
        public static (decimal score, decimal roundedScore, string scaleDescription, string scaleColor, List<ScaleCalculationDetail> calculationDetails) CalculateScoreAndScale(
            decimal acumulatedScore,
            IEnumerable<ScaleCompany> scaleCompany,
            IEnumerable<SubScale> subScales,
            string scaleType
        )
        {
            var score = acumulatedScore;

            // Ordenar scaleCompany primero para mantener el orden en todos los cálculos
            var orderedScaleCompany = scaleCompany.OrderBy(sc => sc.LevelOrder).ToList();

            var calculatedScaleCompany = orderedScaleCompany.Select(sc => new
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

            if (scaleType == ScaleType.Weighted)
            {
                var firstCalValue = subScales.Max(s => s.Value);
                decimal? lastNormalizedScore = null;

                calculatedScaleCompany = orderedScaleCompany.Select((sc, index) =>
                {
                    Decimal? calculatedValue = null;
                    if (index == 0)
                    {
                        calculatedValue = acumulatedScore > sc.NormalizedScore ? ((acumulatedScore - sc.NormalizedScore) / (firstCalValue - sc.NormalizedScore)) * (sc.ExpectedDistribution / 100) : null;
                    }
                    else if (index != orderedScaleCompany.Count - 1)
                    {
                        calculatedValue = acumulatedScore > sc.NormalizedScore && lastNormalizedScore.HasValue ? ((acumulatedScore - sc.NormalizedScore) / (lastNormalizedScore.Value - sc.NormalizedScore)) * (sc.ExpectedDistribution / 100) : null;
                    }
                    lastNormalizedScore = sc.NormalizedScore;
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
                if (calculatedScaleCompany.Count > 1)
                {
                    var secondLastElement = calculatedScaleCompany[^2];
                    var lastElement = calculatedScaleCompany.Last();
                    var hasCalculatedValueDifferent = calculatedScaleCompany
                        .Take(calculatedScaleCompany.Count - 1)
                        .Any(c => c.CalculatedValue != c.NormalizedScore);
                    var calculatedValueLast = hasCalculatedValueDifferent ?
                        lastElement.ExpectedDistribution / 100 :
                        (acumulatedScore >= lastElement.NormalizedScore ? ((acumulatedScore - lastElement.NormalizedScore) / (secondLastElement.NormalizedScore - lastElement.NormalizedScore)) * (lastElement.ExpectedDistribution / 100) : null);
                    calculatedScaleCompany[^1] = new
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
                score = calculatedScaleCompany.Aggregate(0m, (total, sc) => total + (sc.CalculatedValue ?? 0)) * 100;
            }
            var roundedScore = score > 100 ? 100 : Math.Round(score, 2);

            bool scaleFound = false;
            string scaleDescription = string.Empty;
            string scaleColor = string.Empty;

            foreach (var scale in scaleCompany)
            {
                if (roundedScore >= scale.MinValue && roundedScore <= scale.MaxValue)
                {
                    scaleDescription = scale.Name ?? string.Empty;
                    scaleColor = scale.ColorCode ?? string.Empty;
                    scaleFound = true;
                    break;
                }
            }

            if (!scaleFound)
            {
                throw new InvalidOperationException("No se encontró una escala que coincida con el puntaje obtenido.");
            }

            var calculationDetails = calculatedScaleCompany.Select(c => new ScaleCalculationDetail
            {
                FieldName = c.Name ?? string.Empty,
                FieldValue = (c.CalculatedValue ?? 0m) * 100
            }).ToList();

            return (score, roundedScore, scaleDescription, scaleColor, calculationDetails);
        }
    }
}
