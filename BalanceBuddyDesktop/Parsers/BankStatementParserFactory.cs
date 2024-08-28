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
                case "Bank of America":
                    return new BankOfAmericaParser();
                default:
                    throw new ArgumentException("Unsupported bank");
            }
        }
    }
}
