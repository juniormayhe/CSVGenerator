using System.Collections.Generic;

namespace CSVGenerator
{
    public interface IVoucherReader
    {
        IEnumerable<int> ReadVouchers(string dateRangeOrVoucherIDsRange);
    }
}