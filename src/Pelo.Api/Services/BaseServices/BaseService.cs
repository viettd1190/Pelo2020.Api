using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NLog;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.BaseServices
{
    public class BaseService
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected readonly IHttpContextAccessor Context;

        protected readonly IDapperReadOnlyRepository ReadOnlyRepository;

        protected readonly IDapperWriteRepository WriteRepository;

        public BaseService(IDapperReadOnlyRepository readOnlyRepository,
            IDapperWriteRepository writeRepository,
            IHttpContextAccessor context)
        {
            ReadOnlyRepository = readOnlyRepository;
            WriteRepository = writeRepository;
            Context = context;
        }

        /// <summary>
        ///     Ok
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected Task<TResponse<T>> Ok<T>(T data,
            string message = "")
        {
            return Task.FromResult(new TResponse<T>
            {
                Data = data,
                IsSuccess = true,
                Message = message
            });
        }

        /// <summary>
        ///     Fail
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ex"></param>
        /// <returns></returns>
        protected Task<TResponse<T>> Fail<T>(Exception ex)
        {
            _logger.Info(ex);
            return Task.FromResult(new TResponse<T>
            {
                Data = default(T),
                IsSuccess = false,
                Message = ex.ToString()
            });
        }

        /// <summary>
        ///     Fail
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        protected Task<TResponse<T>> Fail<T>(string message)
        {
            _logger.Info(message);
            return Task.FromResult(new TResponse<T>
            {
                Data = default(T),
                IsSuccess = false,
                Message = message
            });
        }

        protected void Log(string message)
        {
            _logger.Info(message);
        }

        protected void Log(Exception exception)
        {
            _logger.Info(exception);
        }
    }
}