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
                case "American Express":
                    return new AmericanExpressParser();
                case "Capital One":
                    return new CapitalOneParser();
                default:
                    throw new ArgumentException("Unsupported bank");
            }
        }
    }
}
