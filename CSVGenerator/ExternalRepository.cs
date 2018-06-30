using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
namespace CSVGenerator
{
    internal class ExternalRepository : IExternalRepository
    {
        private const string CONNECTION_STRING = "Data Source=SERVERNAME;Initial Catalog=DatabaseName;User Id=sa;Password=password";
        private const bool ADD_HEADER = true;

        public IEnumerable<string> ReadVoucherDetails(int voucherID)
        {
            IEnumerable<string> details = new List<string>();
            using (var cnn = new SqlConnection(CONNECTION_STRING))
            {
                cnn.Open();

                var command = new SqlCommand("a_stored_procedure_name_here", cnn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("voucherID",voucherID));
                
                using (var dr = command.ExecuteReader())
                {
                    if (dr.HasRows)
                        details = dr.ToCSV(ADD_HEADER, ",");
                    
                }
            }
            return details;
        }

        /// <summary>
        /// search vouchers for a date range
        /// </summary>
        /// <param name="startDate">yyyy-MM-dd</param>
        /// <param name="endDate">yyyy-MM-dd</param>
        /// <returns></returns>
        public IEnumerable<int> ReadVouchersByDateRange(string startDate, string endDate)
        {
            List<int> vouchers = new List<int>();
            using (var cnn = new SqlConnection(CONNECTION_STRING))
            {
                cnn.Open();
                var command = new SqlCommand($@"SELECT voucherID from a_table_name 
	                                            WHERE recordDate >= '{startDate} 00:00:00' 
                                                AND recordDate <= '{endDate} 23:59:59'", cnn);

                using (var dr = command.ExecuteReader())
                {
                    while (dr.Read()) { 
                        vouchers.Add(Convert.ToInt32(dr["voucherID"]));
                    }
                }
            }
            return vouchers;
        }


        /// <summary>
        /// search vouchers for ID range: 1-999 or comma separated Ids
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public IEnumerable<int> ReadVouchersByIDRange(string range)
        {
            List<int> vouchers = new List<int>();

            List<int> ranges = new List<int>();

            if (!range.Contains("-"))
                throw new ArgumentException("this argument should be a range separeted by dash: 1-9999");
            
            var partes = range.Split('-');
            int first = Convert.ToInt32(partes[0]);
            int last = Convert.ToInt32(partes[partes.Length]);

            int total = last - first;

            ranges.Add(first);
            for (int i = first; i < total; i++)
                ranges.Add(i);
            ranges.Add(last);
            
            using (var cnn = new SqlConnection(CONNECTION_STRING))
            {
                cnn.Open();

                range = string.Join(",", ranges.ToArray());
                var command = new SqlCommand($@"SELECT voucherID from a_table_name_here 
	                                            where voucherID IN ({range})", cnn);

                using (var dr = command.ExecuteReader())
                {
                    vouchers.Add(Convert.ToInt32(dr["voucherID"]));
                }
            }
            return vouchers;
        }

        /// <summary>
        /// search vouchers by specific IDs: 1,2,3
        /// </summary>
        /// <param name="commaSeparatedIDs"></param>
        /// <returns></returns>
        public IEnumerable<int> ReadVouchersBySpecificIDs(string commaSeparatedIDs)
        {
            List<int> vouchers = new List<int>();
            
            using (var cnn = new SqlConnection(CONNECTION_STRING))
            {
                cnn.Open();
                
                var command = new SqlCommand($@"SELECT voucherID from a_table_name_here
	                                            WHERE voucherID IN ({commaSeparatedIDs})", cnn);

                using (var dr = command.ExecuteReader())
                {
                    vouchers.Add(Convert.ToInt32(dr["idRelacion"]));
                }
            }
            return vouchers;
        }
        
    }
}