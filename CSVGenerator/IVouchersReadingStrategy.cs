using System;
using System.Collections.Generic;
using System.Text;

namespace CSVGenerator
{
    public interface IVouchersReadingStrategy
    {
        IEnumerable<int> GetVouchers(string dateRangeOrVoucherIDsRange);
    }
}
