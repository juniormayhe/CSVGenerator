using System;
using System.Collections.Generic;
using System.Text;
using static System.Console;
namespace CSVGenerator
{
    /// <summary>
    /// Concrete strategy for reading vouchers by id range 1-999
    /// </summary>
    public class ReadVoucherByIDRangeStrategy : IVouchersReadingStrategy
    {

        private static IExternalRepository _infotaxiRepository { get; set; }

        public ReadVoucherByIDRangeStrategy(IExternalRepository infotaxiRepository)
        {
            _infotaxiRepository = infotaxiRepository;
        }

        public IEnumerable<int> GetVouchers(string argumentoFechaCorteORangos)
        {
            IEnumerable<int> vouchers = new List<int>();

            try
            {
                vouchers = _infotaxiRepository.ReadVouchersByIDRange(argumentoFechaCorteORangos);
                WriteLine($" - vouchers for a date range were processed");
            }
            catch (Exception ex)
            {
                WriteLine($"- there was an erro rwhile reading vouchers for a date range {ex.Message} {ex.StackTrace}");
            }
            return vouchers;
        }
    }
}
