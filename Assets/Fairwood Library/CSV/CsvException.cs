using System;

namespace Edu.CSV
{
    public class CsvException:Exception
    {
        public CsvException()
        {

        }

        public CsvException(string message) : base(message)
        {

        }
    }
}
