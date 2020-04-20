using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Pelo.Api.Services.BaseServices;
using Pelo.Api.Services.MasterServices;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Dtos.Candidate;
using Pelo.Common.Dtos.CandidateStatus;
using Pelo.Common.Kafka;
using Pelo.Common.Models;
using Pelo.Common.Repositories;
using Pelo.Common.Events.Candidate;
using Pelo.Common.Enums;
using Pelo.Common.Extensions;
using Pelo.Common.Dtos;

namespace Pelo.Api.Services.CandidateServices
{
    public interface ICandidateService
    {
        Task<TResponse<PageResult<GetCandidatePagingResponse>>> GetPaging(int v, GetCandidatePagingRequest request);

        Task<TResponse<GetCandidateResponse>> GetById(int userId, int id);

        Task<TResponse<bool>> Insert(int userId, InsertCandidate request);

        Task<TResponse<bool>> Update(int userId, UpdateCandidate request);

        Task<TResponse<bool>> Delete(int userId, int id);
        Task<TResponse<bool>> Comment(int userId, CandidateComment request);
        Task<TResponse<IEnumerable<CandidateLogResponse>>> GetLogs(int userId, int id);
    }

    public class CandidateService : BaseService,
                                    ICandidateService
    {
        readonly IRoleService _roleService;

        private readonly IAppConfigService _appConfigService;

        private readonly IBusPublisher _busPublisher;

        private readonly IConfiguration _configuration;

        public CandidateService(IDapperReadOnlyRepository readOnlyRepository,
                                IDapperWriteRepository writeRepository,
                                IHttpContextAccessor context,
                                IRoleService roleService, IAppConfigService appConfigService,
                          IConfiguration configuration,
                          IBusPublisher busPublisher) : base(readOnlyRepository,
                                                                 writeRepository,
                                                                 context)
        {
            _roleService = roleService;
            _appConfigService = appConfigService;
            _busPublisher = busPublisher;
            _configuration = configuration;
        }

        public async Task<TResponse<bool>> Comment(int userId, CandidateComment request)
        {
            try
            {
                var candidate = await ReadOnlyRepository.QueryFirstOrDefaultAsync<GetCandidateResponse>(SqlQuery.CANDIDATE_GET_BY_ID,
                                                                                                new
                                                                                                {
                                                                                                    request.Id
                                                                                                });
                if (candidate.IsSuccess)
                {
                    if (candidate.Data != null)
                    {
                        var rs = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.CANDIDATE_INSERT_COMMENT,
                                                                               new
                                                                               {
                                                                                   CandidateId = request.Id,
                                                                                   Comment = string.IsNullOrEmpty(request.Comment)
                                                                                                         ? "đã đính kèm file"
                                                                                                         : request.Comment,
                                                                                   UserId = userId,
                                                                                   candidate.Data.CandidateStatusId
                                                                               });
                        if (rs.IsSuccess)
                        {
                            if (request.Files != null
                               && request.Files.Any())
                            {
                                var candidateLogId = rs.Data;

                                var path = _configuration.GetValue<string>(WebHostDefaults.ContentRootKey) + "\\wwwroot\\Attachments";

                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }

                                foreach (var file in request.Files)
                                {
                                    var newFileName = RenameFile(file.FileName);

                                    using (FileStream fileStream = File.Create(Path.Combine(path,
                                                                                            newFileName) + Path.GetExtension(file.FileName)))
                                    {
                                        file.CopyTo(fileStream);
                                        fileStream.Flush();

                                        var result = await WriteRepository.ExecuteAsync(SqlQuery.CANDIDATE_LOG_ATTACHMENT_INSERT,
                                                                                        new
                                                                                        {
                                                                                            CandidateLogId = candidateLogId,
                                                                                            Attachment = $"{newFileName}{Path.GetExtension(file.FileName)}",
                                                                                            AttachmentName = file.FileName,
                                                                                            UserCreated = userId,
                                                                                            UserUpdated = userId
                                                                                        });
                                    }
                                }

                                await _busPublisher.SendEventAsync(new CandidateCommentSuccessEvent
                                {
                                    Id = request.Id,
                                    Code = candidate.Data.Code,
                                    Comment = request.Comment,
                                    HasAttachmentFile = request.Files != null && request.Files.Any(),
                                    UserId = userId
                                });

                                return await Ok(true);
                            }

                            return await Ok(true);
                        }
                    }
                }

                return await Fail<bool>(ErrorEnum.CRM_HAS_NOT_EXIST.GetStringValue());
            }
            catch (Exception)
            {
                return await Fail<bool>(ErrorEnum.SQL_QUERY_CAN_NOT_EXECUTE.GetStringValue());
            }
        }

        public async Task<TResponse<bool>> Delete(int userId, int id)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var data = await GetById(userId, id);
                    if (data.IsSuccess)
                    {
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.CANDIDATE_DELETE,
                                                                                                              new
                                                                                                              {
                                                                                                                  Id = id,
                                                                                                                  UserUpdated = userId,
                                                                                                                  DateUpdated = DateTime.Now
                                                                                                              });
                        if (result.IsSuccess)
                        {
                            return await Ok(true);
                        }
                        return await Fail<bool>(result.Message);
                    }
                    return await Fail<bool>(data.Message);
                }
                return await Fail<bool>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<bool>(exception);
            }
        }

        #region ICandidateService Members

        public async Task<TResponse<GetCandidateResponse>> GetById(int userId, int id)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var result = await ReadOnlyRepository.QueryFirstOrDefaultAsync<GetCandidateResponse>(SqlQuery.CANDIDATE_GET_BY_ID,
                                                                                                              new
                                                                                                              {
                                                                                                                  Id = id,
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(result.Data);
                    }

                    return await Fail<GetCandidateResponse>(result.Message);
                }

                return await Fail<GetCandidateResponse>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<GetCandidateResponse>(exception);
            }
        }

        public async Task<TResponse<IEnumerable<CandidateLogResponse>>> GetLogs(int userId, int id)
        {
            try
            {
                var result = await ReadOnlyRepository.QueryAsync<CandidateLogResponse>(SqlQuery.CANDIDATE_GET_LOGS,
                                                                                 new
                                                                                 {
                                                                                     CandidateId = id
                                                                                 });
                if (result.IsSuccess)
                {
                    foreach (var log in result.Data)
                    {
                        var user = await ReadOnlyRepository.QueryFirstOrDefaultAsync<UserInLog>(SqlQuery.GET_USER_IN_LOG,
                                                                                                new
                                                                                                {
                                                                                                    Id = log.UserId
                                                                                                });
                        if (user.IsSuccess
                           && user.Data != null)
                        {
                            log.User = user.Data;
                        }

                        var oldCandidateStatus = await ReadOnlyRepository.QueryFirstOrDefaultAsync<StatusInLog>(SqlQuery.GET_CANDIDATE_STATUS_IN_LOG,
                                                                                                             new
                                                                                                             {
                                                                                                                 Id = log.OldCandidateStatusId
                                                                                                             });
                        if (oldCandidateStatus.IsSuccess)
                        {
                            log.OldCandidateStatus = oldCandidateStatus.Data;
                        }

                        var crmStatus = await ReadOnlyRepository.QueryFirstOrDefaultAsync<StatusInLog>(SqlQuery.GET_CANDIDATE_STATUS_IN_LOG,
                                                                                                          new
                                                                                                          {
                                                                                                              Id = log.CandidateStatusId
                                                                                                          });
                        if (crmStatus.IsSuccess)
                        {
                            log.CandidateStatus = crmStatus.Data;
                        }

                        var attachments = await ReadOnlyRepository.QueryAsync<LogAttachment>(SqlQuery.GET_CANDIDATE_ATTACHMENT_IN_LOG,
                                                                                                new
                                                                                                {
                                                                                                    CandidateLogId = log.Id
                                                                                                });

                        if (attachments.IsSuccess)
                        {
                            log.Attachments = attachments.Data.ToList();
                        }
                    }

                    return await Ok(result.Data);
                }

                return await Fail<IEnumerable<CandidateLogResponse>>(result.Message);
            }
            catch (Exception exception)
            {
                return await Fail<IEnumerable<CandidateLogResponse>>(string.Format(ErrorEnum.SQL_QUERY_CAN_NOT_EXECUTE.GetStringValue(),
                                                                             "GetCandidateLog"));
            }
        }

        public async Task<TResponse<PageResult<GetCandidatePagingResponse>>> GetPaging(int userId, GetCandidatePagingRequest request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    string fromDate = string.Empty; string toDate = string.Empty;
                    if (!string.IsNullOrEmpty(request.FromDate))
                    {
                        fromDate = string.Format("{0:yyyy-MM-dd} 00:00:00", DateTime.Parse(request.FromDate));
                    }
                    if (!string.IsNullOrEmpty(request.ToDate))
                    {
                        toDate = string.Format("{0:yyyy-MM-dd} 23:59:00", DateTime.Parse(request.ToDate));
                    }
                    var result = await ReadOnlyRepository.QueryMultipleLFAsync<GetCandidatePagingResponse, int>(string.Format(SqlQuery.CANDIDATE_GET_BY_PAGING, request.ColumnOrder, request.SortDir.ToUpper()),
                                                                                                              new
                                                                                                              {
                                                                                                                  Name = $"%{request.Name}%",
                                                                                                                  Phone = $"%{request.Phone}%",
                                                                                                                  Code = $"%{request.Code}%",
                                                                                                                  CandidateStatusId = request.CandidateStatusId,
                                                                                                                  FromDate = fromDate,
                                                                                                                  ToDate = toDate,
                                                                                                                  Skip = (request.Page - 1) * request.PageSize,
                                                                                                                  Take = request.PageSize
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(new PageResult<GetCandidatePagingResponse>(request.Page,
                                                                                  request.PageSize,
                                                                                  result.Data.Item2,
                                                                                  result.Data.Item1));
                    }

                    return await Fail<PageResult<GetCandidatePagingResponse>>(result.Message);
                }

                return await Fail<PageResult<GetCandidatePagingResponse>>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<PageResult<GetCandidatePagingResponse>>(exception);
            }
        }

        public async Task<TResponse<bool>> Insert(int userId, InsertCandidate request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    string code = $"TD{DateTime.Today.ToString("yyMMdd")}001";
                    var rs = await ReadOnlyRepository.QueryFirstOrDefaultAsync<int>(SqlQuery.CANDIDATE_CURRENT_COUNT, new { CurrentDate = $"{DateTime.Today.ToString("yyyy-MM-dd")}" });
                    if (rs.IsSuccess)
                    {
                        code = $"TD{DateTime.Today.ToString("yyMMdd")}{(rs.Data + 1).ToString("D3")}";
                    }
                    int candidateStatusId = 0;
                    var candidateStatus = await ReadOnlyRepository.QueryFirstOrDefaultAsync<GetCandidateStatusResponse>(SqlQuery.CANDIDATE_STATUS_FIRST_SORT_ORDER, null);
                    if (candidateStatus.IsSuccess)
                    {
                        candidateStatusId = candidateStatus.Data.Id;
                    }
                    var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.CANDIDATE_INSERT,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Name,
                                                                                                                  Code = code,
                                                                                                                  request.Phone,
                                                                                                                  request.Address,
                                                                                                                  request.Email,
                                                                                                                  request.Description,
                                                                                                                  CandidateStatusId = candidateStatusId,
                                                                                                                  UserCreated = userId,
                                                                                                                  DateCreated = DateTime.Now,
                                                                                                                  UserUpdated = userId,
                                                                                                                  DateUpdated = DateTime.Now
                                                                                                              });
                    if (result.IsSuccess)
                    {
                        return await Ok(true);
                    }
                    return await Fail<bool>(result.Message);
                }
                return await Fail<bool>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<bool>(exception);
            }
        }

        public async Task<TResponse<bool>> Update(int userId, UpdateCandidate request)
        {
            try
            {
                var canGetAll = await CanGetAll(userId);
                if (canGetAll.IsSuccess)
                {
                    var data = await GetById(userId, request.Id);
                    if (data.IsSuccess)
                    {
                        var result = await WriteRepository.ExecuteScalarAsync<int>(SqlQuery.CANDIDATE_UPDATE,
                                                                                                              new
                                                                                                              {
                                                                                                                  request.Id,
                                                                                                                  request.Name,
                                                                                                                  request.Phone,
                                                                                                                  request.Address,
                                                                                                                  request.Email,
                                                                                                                  request.CandidateStatusId,
                                                                                                                  request.Description,
                                                                                                                  UserUpdated = userId,
                                                                                                                  DateUpdated = DateTime.Now
                                                                                                              });
                        if (result.IsSuccess)
                        {
                            return await Ok(true);
                        }
                        return await Fail<bool>(result.Message);
                    }
                    return await Fail<bool>(data.Message);
                }
                return await Fail<bool>(canGetAll.Message);
            }
            catch (Exception exception)
            {
                return await Fail<bool>(exception);
            }
        }

        #endregion

        private async Task<TResponse<bool>> CanGetAll(int userId)
        {
            try
            {
                var checkPermission = await _roleService.CheckPermission(userId);
                if (checkPermission.IsSuccess)
                {
                    return await Ok(true);
                }

                return await Fail<bool>(checkPermission.Message);
            }
            catch (Exception exception)
            {
                return await Fail<bool>(exception);
            }
        }

        private string RenameFile(string fileName)
        {
            var newName = Guid.NewGuid()
                              .ToString()
                              .Replace("-",
                                       string.Empty);
            return newName;
        }
    }
}
