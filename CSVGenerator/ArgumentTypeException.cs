using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CSVGenerator
{
    public static class ArgumentTypeException
    {
        /// <summary>
        /// discovers on the fly what type of string argument 
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public static SearchType GetSearchType(this string argument) {

            // date ranges in dd/mm/yyyy-dd/mm/yyyy
            const string DATE_RANGE = @"^(([0-2][0-9]|(3)[0-1])(\/)(((0)[0-9])|((1)[0-2]))(\/)\d{4})-(([0-2][0-9]|(3)[0-1])(\/)(((0)[0-9])|((1)[0-2]))(\/)\d{4})$";
            
            // specific dates separated by comma: dd/mm/yyyy,dd/mm/yyyy 
            //const string SPECIFIC_DATES = @"^(([0-2][0-9]|(3)[0-1])(\/)(((0)[0-9])|((1)[0-2]))(\/)\d{4})(,(([0-2][0-9]|(3)[0-1])(\/)(((0)[0-9])|((1)[0-2]))(\/)\d{4}))*$";

            // ids range separated by dash: 123-999
            const string ID_RANGE = @"^(\d*)-(\d*)$";

            // specific ID or IDs separated by coma: 123 or 123,456
            const string SPECIFIC_IDS = @"^(\d*)(,\d*)*$"; 

            Regex regex = new Regex(DATE_RANGE);
            if (regex.IsMatch(argument))
                return SearchType.ByDateRange;

            regex = new Regex(ID_RANGE);
            if (regex.IsMatch(argument))
                return SearchType.ByIDRange;

            regex = new Regex(SPECIFIC_IDS);
            if (regex.IsMatch(argument))
                return SearchType.BySpecificIDs;
            
            throw new ApplicationException($"There's no type that matches the string argument: {argument}");
        }
    }
}
