using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mafiagame.DataLayer;
using MafiaGame.DataLayer.Implementations;
using Ninject.Modules;
using MafiaGame.Common.Interfaces;

namespace MafiaGame.DataLayer.NinjectKernel
{
    class LogNinjectModule : NinjectModule
    { 
        public override void Load()
        {
            Bind<ILogger>().To<DataLog>();
        }
    }
}
