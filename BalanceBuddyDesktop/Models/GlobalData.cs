using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceBuddyDesktop.Models;

public class GlobalData
{
    private static UserData _instance;
    private static readonly object _lock = new object();

    public static UserData Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new UserData();
                    // Initialize with default or saved data
                }
                return _instance;
            }
        }
    }
}

