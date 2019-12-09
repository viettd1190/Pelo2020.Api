using System;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.UserServices.Auth;
using Pelo.Common.Dtos.Account;
using Pelo.Common.Enums;
using Pelo.Common.Extensions;
using Pelo.Common.Models;
using Pelo.Common.Repositories;

namespace Pelo.Api.Services.UserServices
{
    public interface IAccountService
    {
        Task<TResponse<LogonResponse>> Logon(LogonRequest request, string userAgent, string ipAddress);

        Task<TResponse<LogonDetail>> CheckToken(string token);
    }

    public class AccountService : BaseService, IAccountService
    {
        private readonly string _secretKey;

        public AccountService(IDapperReadOnlyRepository readOnlyRepository, IDapperWriteRepository writeRepository,
            IHttpContextAccessor httpContextAccessor,
            string secretKey) : base(
            readOnlyRepository, writeRepository, httpContextAccessor)
        {
            _secretKey = secretKey;
        }

        public async Task<TResponse<LogonResponse>> Logon(LogonRequest request, string userAgent, string ipAddress)
        {
            try
            {
                var canLogon = await CanLogon(request);
                if (canLogon.IsSuccess)
                {
                    var password = Sha512(request.Password);
                    var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<LogonDetail>(SqlQuery.USER_LOGON, new
                    {
                        request.Username,
                        Password = password
                    });
                    if (result.IsSuccess)
                    {
                        if (result.Data != null)
                        {
                            var token = new AuthenticationModule().GenerateTokenForUser(result.Data,
                                _secretKey);
                            if (token != null)
                            {
                                #region Insert log account

                                var tmp = await WriteRepository.ExecuteAsync(SqlQuery.LOG_ACCOUNT_INSERT, new
                                {
                                    UserId = result.Data.Id,
                                    UserAgent = userAgent,
                                    IpAddress = ipAddress
                                });

                                #endregion

                                return await Ok(new LogonResponse(token));
                            }

                            return await Fail<LogonResponse>(ErrorEnum.CAN_NOT_CREATE_TOKEN.GetStringValue());
                        }

                        return await Fail<LogonResponse>(ErrorEnum.AUTHENTICATION_WRONG.GetStringValue());
                    }

                    return await Fail<LogonResponse>(result.Message);
                }

                return await Fail<LogonResponse>(canLogon.Message);
            }
            catch (Exception exception)
            {
                return await Fail<LogonResponse>(exception);
            }
        }

        public async Task<TResponse<LogonDetail>> CheckToken(string token)
        {
            try
            {
                var auth = new AuthenticationModule();
                var userPayloadToken = auth.GenerateUserClaimFromJwt(token,
                    _secretKey);

                if (userPayloadToken != null)
                {
                    var identity = auth.PopulateUserIdentity(userPayloadToken);
                    string[] roles =
                    {
                        "All"
                    };
                    var genericPrincipal = new GenericPrincipal(identity,
                        roles);
                    Thread.CurrentPrincipal = genericPrincipal;

                    if (Thread.CurrentPrincipal.Identity is JwtAuthenticationIdentity authenticationIdentity
                        && !string.IsNullOrEmpty(authenticationIdentity.Username))
                    {
                        var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<LogonDetail>(
                            SqlQuery.USER_GET_LOGON_DETAIL, new
                            {
                                authenticationIdentity.Username
                            });
                        if (result.IsSuccess)
                        {
                            if (result.Data != null) return await Ok(result.Data);

                            return await Fail<LogonDetail>(ErrorEnum.TOKEN_INVALID.GetStringValue());
                        }

                        return await Fail<LogonDetail>(result.Message);
                    }
                }

                return await Fail<LogonDetail>(ErrorEnum.TOKEN_INVALID.GetStringValue());
            }
            catch (Exception exception)
            {
                return await Fail<LogonDetail>(exception);
            }
        }

        private async Task<TResponse<bool>> CanLogon(LogonRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Username))
                    return await Fail<bool>(ErrorEnum.USERNAME_IS_NOT_EMPTY.GetStringValue());

                if (string.IsNullOrEmpty(request.Password))
                    return await Fail<bool>(ErrorEnum.PASSWORD_IS_NOT_EMPTY.GetStringValue());

                return await Ok(true);
            }
            catch (Exception exception)
            {
                return await Fail<bool>(exception);
            }
        }

        private string Sha512(string input)
        {
            input = $"123{input}xyz";

            var bytes = Encoding.UTF8.GetBytes(input);
            using (var hash = SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
                var hashedInputStringBuilder = new StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                return hashedInputStringBuilder.ToString();
            }
        }
    }
}