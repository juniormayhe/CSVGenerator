using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CSVGenerator
{
    public interface IVoucherExporter
    {
        bool ExportVouchers(string filename, IEnumerable<string> vouchers);
    }

    public class VoucherExporter : IVoucherExporter
    {
        /// <summary>
        /// Saves all lines containing comma separated voucher information 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="vouchers"></param>
        /// <returns></returns>
        public bool ExportVouchers(string filename, IEnumerable<string> vouchers)
        {
            bool exito = false;
            try
            {
                File.WriteAllLines($"{filename}", vouchers, Encoding.UTF8);
                exito = true;

            }
            catch (Exception ex)
            {
                string message = $"Could not generate file {filename}: {ex.Message} {ex.StackTrace}";
                Console.WriteLine(message);

            }
            return exito;
        }
    }
}
