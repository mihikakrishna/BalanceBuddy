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
                }
                return _instance;
            }
        }
    }
}

