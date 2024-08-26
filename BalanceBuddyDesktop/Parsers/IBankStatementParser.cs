using System.IO;
using BalanceBuddyDesktop.Models;

namespace BalanceBuddyDesktop.Parsers;

public interface IBankStatementParser
{
    void ParseStatement(Stream csvStream);
}
