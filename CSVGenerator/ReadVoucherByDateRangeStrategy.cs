using System;
using System.Collections.Generic;
using System.Text;
using static System.Console;
namespace CSVGenerator
{
    /// <summary>
    /// Concrete strategy for reading vouchers by date range
    /// </summary>
    public class ReadVoucherByDateRangeStrategy : IVouchersReadingStrategy
    {

        private static IExternalRepository _externalRepository { get; set; }

        public ReadVoucherByDateRangeStrategy(IExternalRepository infotaxiRepository)
        {
            _externalRepository = infotaxiRepository;
        }

        
        /// <summary>
        /// Get vouchers for the argument
        /// </summary>
        /// <param name="dateRangeOrIdsArgument">date range with start and end date dd/MM/yyyy-dd/MM/yyyy</param>
        /// <returns></returns>
        public IEnumerable<int> GetVouchers(string dateRangeOrIdsArgument)
        {
            IEnumerable<int> vouchers = new List<int>();

            string[] arguments = dateRangeOrIdsArgument.Split('-');
            if (arguments.Length > 2)
            {
                WriteLine($" - date range should have 2 arguments: start and end date dd/mm/yyyy-dd/mm/yyyy");
                return vouchers;
            }

            var dates = new List<DateTime>();
            foreach (var argument in arguments) { 
                DateTime date = DateTime.MinValue;
                bool fechaValida = DateTime.TryParseExact(argument, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out date);
                if (!fechaValida) {
                    WriteLine($" - date {argument} is invalid. The date format must be dd/mm/yyyy");
                    return vouchers;
                }
                dates.Add(date);
            }

            if (dates[0].Date > dates[1].Date) {
                WriteLine($" - final date must be greater or equals to start date.");
                return vouchers;
            }

            try
            {
                vouchers = _externalRepository.ReadVouchersByDateRange(dates[0].ToString("yyyy-MM-dd"), dates[1].ToString("yyyy-MM-dd"));
                WriteLine($" - vouchers for date range were processed");
            }
            catch (Exception ex)
            {
                WriteLine($"- could not read vouchers for date range {ex.Message} {ex.StackTrace}");
            }
            
            return vouchers;
        }
    }
}
