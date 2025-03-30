using System;

namespace BalanceBuddyDesktop.Models
{
    public static class GlobalData
    {
        private static UserData _instance;
        private static readonly object _lock = new object();

        public static UserData Instance
        {
            get
            {
                lock (_lock)
                {
                    return _instance ??= new UserData();
                }
            }
        }
    }
}
