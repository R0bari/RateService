namespace UserInterface.Infrastructure
{
    public static class DecimalExtensions
    {
        public static decimal ToFixed(this decimal value, int precision) => ToFixed(value, CalculatorModificator(precision));
        private static decimal CalculatorModificator(int precision) => (decimal)Math.Pow(10, precision);
        private static decimal ToFixed(decimal value, decimal modificator) => decimal.Round(modificator * value) / modificator;
    }
}
