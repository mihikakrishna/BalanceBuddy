using System.IO;

namespace BalanceBuddyDesktop.Parsers;

public interface IBankStatementParser
{
    void ParseStatement(Stream csvStream);
}
