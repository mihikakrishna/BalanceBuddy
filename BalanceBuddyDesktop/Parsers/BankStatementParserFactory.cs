using System;

namespace BalanceBuddyDesktop.Parsers
{
    public class BankStatementParserFactory
    {
        public static IBankStatementParser GetParser(string bankId)
        {
            switch (bankId)
            {
                case "American Express":
                    return new AmericanExpressParser();
                case "Bank of America":
                    return new BankOfAmericaParser();
                case "Capital One Credit Account":
                    return new CapitalOneCreditParser();
                case "Capital One Savings Account":
                    return new CapitalOneSavingsParser();
                case "Chase":
                    return new ChaseParser();
                case "Wells Fargo":
                    return new WellsFargoParser();
                default:
                    throw new ArgumentException("Unsupported bank");
            }
        }
    }
}
