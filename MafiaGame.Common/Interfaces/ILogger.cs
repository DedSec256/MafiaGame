using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MafiaGame.Common.Interfaces
{
    public interface ILogger
    {
        void Log(Exception ex);
        void Log(string message);
    }
}
