using System;
using Avalonia.Controls;

namespace BalanceBuddyDesktop
{
    public interface INavigable
    {
        event Action<UserControl> RequestNavigate;
    }
}