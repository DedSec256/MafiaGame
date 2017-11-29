using System;
using Ninject;
using Ninject.Extensions.Interception;
using Ninject.Extensions.Interception.Attributes;
using Ninject.Extensions.Interception.Request;
using MafiaGame.Common.Interfaces;

namespace MafiaGame.DataLayer.NinjectKernel
{
    public class ExceptionInterceptor : IInterceptor
    {
        private readonly ILogger _logger;

        public ExceptionInterceptor(ILogger logger)
        {
            _logger = logger;
        }

        public void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
            }

        }
    }

    public class LogExceptionAttribute : InterceptAttribute
    {
        public override IInterceptor CreateInterceptor(IProxyRequest request)
        {
            return request.Context.Kernel.Get<ExceptionInterceptor>();
        }
    }
}
