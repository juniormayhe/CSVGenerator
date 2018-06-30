using System;
using System.Collections.Generic;
using System.Text;

namespace CSVGenerator
{
    public enum SearchType {
        ByDateRange,
        ByIDRange,
        BySpecificIDs
        
    }

    /// <summary>
    /// A strategy pattern to avoid a bunch of ifs
    /// </summary>
    public class VoucherReader : IVoucherReader
    {
        private readonly Dictionary<SearchType, IVouchersReadingStrategy> _strategies;

        public VoucherReader(IExternalRepository externalRepository)
        {
            _strategies = new Dictionary<SearchType, IVouchersReadingStrategy>();
            _strategies.Add(SearchType.ByDateRange, new ReadVoucherByDateRangeStrategy(externalRepository));
            _strategies.Add(SearchType.ByIDRange, new ReadVoucherByIDRangeStrategy(externalRepository));
            _strategies.Add(SearchType.BySpecificIDs, new ReadVoucherByIDStrategy(externalRepository));
        }

        public IEnumerable<int> ReadVouchers(string dateRangeOrVoucherIDsRange) {
            //setup strategy based on argument type
            SearchType searchType = dateRangeOrVoucherIDsRange.GetSearchType();
            
            //return the concrete strategy for the search type
            return _strategies[searchType].GetVouchers(dateRangeOrVoucherIDsRange);
        }


    }
}
