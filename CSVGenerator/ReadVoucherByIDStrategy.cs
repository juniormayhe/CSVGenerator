using System;
using System.Collections.Generic;
using static System.Console;
namespace CSVGenerator
{
    /// <summary>
    /// Concrete strategy for reading one or more vouchers
    /// </summary>
    internal class ReadVoucherByIDStrategy : IVouchersReadingStrategy
    {
        private IExternalRepository _externalRepository;

        public ReadVoucherByIDStrategy(IExternalRepository repository)
        {
            _externalRepository = repository;
        }

        public IEnumerable<int> GetVouchers(string rangeOfDatesOrIDs)
        {
            IEnumerable<int> relaciones = new List<int>();

            try
            {
                relaciones = _externalRepository.ReadVouchersBySpecificIDs(rangeOfDatesOrIDs);
                WriteLine($" - vouchers by ID were processed");
            }
            catch (Exception ex)
            {
                WriteLine($"- there was an error while reading vouchers by ID from external repository {ex.Message} {ex.StackTrace}");
            }
            return relaciones;
        }
    }
}