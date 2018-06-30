using Microsoft.Extensions.Configuration;
using Ninject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Console;
namespace CSVGenerator
{
    public class Program
    {

        private static IConfiguration _configuration { get; set; }
        private static IVoucherExporter _exporter { get; set; }
        private static IVoucherReader _voucherReader { get; set; }
        private static IExternalRepository _externalRepository { get; set; }
        static void Main(string[] args)
        {
            WriteLine("CSV generator for vouchers v1.0\n");

            bootstrapDI();
            loadConfiguration();
            processVouchers(args);

            finishProcessing();
            
        }
        
        private static void bootstrapDI()
        {
            //setup DI
            var kernel = new StandardKernel();
            kernel.Bind<IVoucherExporter>().To<VoucherExporter>().InSingletonScope();
            kernel.Bind<IExternalRepository>().To<ExternalRepository>().InSingletonScope();
            kernel.Bind<IVoucherReader>().To<VoucherReader>().InTransientScope();
            _externalRepository = kernel.Get<IExternalRepository>();
            _exporter = kernel.Get<IVoucherExporter>();
            _voucherReader = kernel.Get<IVoucherReader>();

        }

        private static void loadConfiguration()
        {
            try
            {
                //nuget Microsoft.Extensions.Configuration.Json
                var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("configuracion.json", optional: false, reloadOnChange: true);

                _configuration = builder.Build();

                if (!Directory.Exists(_configuration["csvPath"]))
                    Directory.CreateDirectory(_configuration["csvPath"]);
                
                WriteLine($" - Using path for vouchers: {_configuration["csvPath"]}");
            }
            catch (Exception ex)
            {
                WriteLine($" - Cannot load configuration.json: {ex.Message} {ex.StackTrace}");
                
            }
        }

        private static void finishProcessing()
        {
            WriteLine("End of processing.");

        }

        private static void processVouchers(string[] args)
        {
            //any argument passed by CLI?
            bool hasArgument = args.Length > 0;

            //are there CSV file generation requests fired from legacy system?
            bool hasRequestsForGeneratingCSV = false;
            HashSet<string> arguments = getArgumentsFromRequests(ref hasRequestsForGeneratingCSV);
            showArguments(arguments);
            
            //if there is no arguments, stop execution
            if (!hasArgument && !hasRequestsForGeneratingCSV)
            {
                showHelp();
                return;
            }

            //if there is an argument, process it
            if (hasArgument)
                arguments.Add(args[0]);

            foreach (var argument in arguments) {
                try
                {
                    processArgument(argument);
                }
                catch (Exception ex) {
                    WriteLine($" - the argument {argument} cannot be processed. {ex.Message} {ex.StackTrace}");
                }
            }

        }

        /// <summary>
        /// Get arguments for files containing generation request made by legacy system
        /// </summary>
        /// <param name="hasFileGenerationRequests"></param>
        /// <returns></returns>
        private static HashSet<string> getArgumentsFromRequests(ref bool hasFileGenerationRequests)
        {
            HashSet<string> arguments = new HashSet<string>();
            try
            {
                arguments = RequestReader.ReadFileGenerationRequests(_configuration["csvPath"]);
                hasFileGenerationRequests = arguments.Count() > 0;

            }
            catch (Exception ex)
            {
                WriteLine($" - could not read requests for generating vouchers in {_configuration["csvPath"]}. {ex.Message} {ex.StackTrace}");
            }

            //delete requests
            try {
                RequestReader.DeleteFileGenerationRequests(_configuration["csvPath"]);
            }
            catch (Exception ex)
            {
                WriteLine($" - there was an error while reading requests for generating vouchers in {_configuration["csvPath"]}. {ex.Message} {ex.StackTrace}");
            }

            return arguments;
        }

        private static void showArguments(HashSet<string> arguments)
        {
            WriteLine($" - there were file generation requests with {arguments.Count()} arguments, loaded from {_configuration["csvPath"]}");
            foreach (var argument in arguments)
            {
                WriteLine($" - argument set for generating CSV with a list of vouchers: {argument}");
            }
        }

        /// <summary>
        /// Processes argument provided by user or by a scheduled task
        /// </summary>
        /// <param name="dateRangeOrIDsArgument">date range or voucher ids</param>
        private static void processArgument(string dateRangeOrIDsArgument)
        {
            WriteLine($" - Processing argument: {dateRangeOrIDsArgument}...");
            
            //find vouchers for the provided argument
            IEnumerable<int> vouchers = _voucherReader.ReadVouchers(dateRangeOrIDsArgument);

            foreach (var voucher in vouchers)
            {
                string path = Path.Combine(_configuration["csvPath"], dateRangeOrIDsArgument.Replace("/", "-"));
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string filename = Path.Combine(path, $"VoucherList-{voucher}.csv");

                //try to generate csv file for each voucher
                try
                {
                    var voucherDetails = _externalRepository.ReadVoucherDetails(voucher);
                    _exporter.ExportVouchers(filename, voucherDetails);
                    WriteLine($" - {voucher} list was saved in {filename}");
                }
                catch (Exception ex)
                {
                    WriteLine($" - ERROR: could not save file for voucher {voucher} in {filename}. Message: {ex.Message}\n{ex.StackTrace}");
                }

            }
        }

        private static void showHelp()
        {
            WriteLine(@"USE:
CSVGenerator <start date>-<end date> 
CSVGenerator <id>,<id2>,...<idN>
CSVGenerator <id>-<idN>

Example:

    Generate csv file for data range:
        CSVGenerator 30/10/2018-12-11-2018
    ");
        }

      

    }


}
