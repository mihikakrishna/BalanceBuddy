using System;

namespace BalanceBuddyDesktop.Parsers
{
    public class BankStatementParserFactory
    {
        public static IBankStatementParser GetParser(string bankId)
        {
            switch (bankId)
            {
                case "Wells Fargo":
                    return new WellsFargoParser();
                default:
                    throw new ArgumentException("Unsupported bank");
            }
        }
    }
}
