using System;
using System.Collections.Generic;
using System.Text;

namespace CSVGenerator
{
    public interface IExternalRepository
    {
        IEnumerable<string> ReadVoucherDetails(int voucherID);

        IEnumerable<int> ReadVouchersByDateRange(string startDate, string endDate);
        IEnumerable<int> ReadVouchersByIDRange(string voucherIDsRange);
        IEnumerable<int> ReadVouchersBySpecificIDs(string commaSeparatedVoucherIDs);
        
    }
}
