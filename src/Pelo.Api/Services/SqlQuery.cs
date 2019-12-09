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

        public const string LOG_ACCOUNT_INSERT = @"INSERT HIS.Account
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

        #region Role

        public const string ROLE_CHECK_PERMISSION = @"SELECT Id
                                                      FROM dbo.RolePermission
                                                      WHERE RoleId = @RoleId
                                                            AND Controller = @Controller
                                                            AND Action = @Action
                                                            AND IsAccessable = 1;";

        public const string ROLE_ID_GET_BY_USER_ID = @"SELECT RoleId
                                                       FROM dbo.[User]
                                                       WHERE Id = @Id
                                                             AND IsDeleted = 0;";

        #endregion

        #region User

        public const string USER_GET_BY_PAGING = @"SELECT u.Id,
                                                          u.Username,
                                                          u.Email,
                                                          u.DisplayName,
                                                          u.FullName,
                                                          u.PhoneNumber,
                                                          b.Name AS Branch,
                                                          r.Name AS Role,
                                                          u.Avatar
                                                   FROM dbo.[User] u
                                                       LEFT JOIN dbo.Branch b
                                                           ON b.Id = u.BranchId
                                                       LEFT JOIN dbo.Role r
                                                           ON r.Id = u.RoleId
                                                   WHERE u.DisplayName COLLATE Latin1_General_CI_AI LIKE @DisplayName COLLATE Latin1_General_CI_AI
                                                         AND u.FullName COLLATE Latin1_General_CI_AI LIKE @FullName COLLATE Latin1_General_CI_AI
                                                         AND u.Username LIKE @Username
                                                         AND u.PhoneNumber LIKE @PhoneNumber
                                                         AND
                                                         (
                                                             ISNULL(@BranchId, 0) = 0
                                                             OR ISNULL(u.BranchId, 0) = @BranchId
                                                         )
                                                         AND
                                                         (
                                                             ISNULL(@RoleId, 0) = 0
                                                             OR ISNULL(u.RoleId, 0) = @RoleId
                                                         )
                                                         AND u.IsDeleted = 0
                                                   ORDER BY {0} {1} OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;

                                                   SELECT COUNT(*)
                                                   FROM dbo.[User] u
                                                       LEFT JOIN dbo.Branch b
                                                           ON b.Id = u.BranchId
                                                       LEFT JOIN dbo.Role r
                                                           ON r.Id = u.RoleId
                                                   WHERE u.DisplayName COLLATE Latin1_General_CI_AI LIKE @DisplayName COLLATE Latin1_General_CI_AI
                                                         AND u.FullName COLLATE Latin1_General_CI_AI LIKE @FullName COLLATE Latin1_General_CI_AI
                                                         AND u.Username LIKE @Username
                                                         AND u.PhoneNumber LIKE @PhoneNumber
                                                         AND
                                                         (
                                                             ISNULL(@BranchId, 0) = 0
                                                              OR ISNULL(u.BranchId, 0) = @BranchId
                                                         )
                                                         AND
                                                         (
                                                             ISNULL(@RoleId, 0) = 0
                                                             OR ISNULL(u.RoleId, 0) = @RoleId
                                                         )
                                                         AND u.IsDeleted = 0;";

        #endregion
    }
}