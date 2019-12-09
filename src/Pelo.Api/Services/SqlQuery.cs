namespace Pelo.Api.Services
{
    public class SqlQuery
    {
        #region Account

        public const string USER_LOGON = @"SELECT Id,
                                                  Username,
                                                  DisplayName,
                                                  Avatar
                                           FROM dbo.[User]
                                           WHERE Username = @Username
                                                 AND Password = @Password
                                                 AND IsDeleted = 0;";

        public const string USER_GET_LOGON_DETAIL = @"SELECT Id,
                                                             Username,
                                                             DisplayName,
                                                             Avatar
                                                      FROM dbo.[User]
                                                      WHERE Username = @Username
                                                            AND IsDeleted = 0;";

        public const string LOG_ACCOUNT_INSERT = @"INSERT LOG.Account
                                                   (
                                                       UserId,
                                                       UserAgent,
                                                       IpAddress,
                                                       DateCreated
                                                   )
                                                   VALUES
                                                   (   @UserId,        -- UserId - int
                                                       @UserAgent,      -- UserAgent - nvarchar(500)
                                                       @IpAddress,       -- IpAddress - varchar(50)
                                                       GETDATE() -- DateCreated - datetime
                                                       )";

        #endregion
    }
}