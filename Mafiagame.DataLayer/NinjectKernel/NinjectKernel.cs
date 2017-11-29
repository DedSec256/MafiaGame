using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;

namespace MafiaGame.DataLayer.NinjectKernel
{
    public static class NinjectKernel
    {
        public static readonly StandardKernel Kernel = new StandardKernel(new LogNinjectModule());

    }
}
