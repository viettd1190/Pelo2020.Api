namespace Pelo.Api.Services
{
    public class SqlQuery
    {
        #region Branch

        public const string BRANCH_GET_ALL = @"SELECT Id,
                                                      Name
                                               FROM dbo.Branch
                                               WHERE IsDeleted = 0;";

        #endregion

        #region Department

        public const string DEPARTMENT_GET_ALL = @"SELECT Id,
                                                          Name
                                                   FROM dbo.Department
                                                   WHERE IsDeleted = 0;";

        #endregion

        #region User

        public const string USER_GET_BY_PAGING = @"SELECT u.Id,
                                                          u.Code,
                                                          u.Username,
                                                          u.Email,
                                                          u.DisplayName,
                                                          u.FullName,
                                                          u.PhoneNumber,
                                                          b.Name AS Branch,
                                                          r.Name AS Role,
                                                          d.Name AS Department,
                                                          u.Avatar,
                                                          u.IsActive,
                                                          u.DateCreated
                                                   FROM dbo.[User] u
                                                       LEFT JOIN dbo.Branch b
                                                           ON b.Id = u.BranchId
                                                       LEFT JOIN dbo.Role r
                                                           ON r.Id = u.RoleId
                                                       LEFT JOIN dbo.Department d
                                                           ON d.Id = u.DepartmentId
                                                   WHERE (ISNULL(u.FullName, '') COLLATE Latin1_General_CI_AI LIKE @FullNAme COLLATE Latin1_General_CI_AI
                                                         OR u.Username LIKE @FullNAme
                                                         OR ISNULL(u.DisplayName, '') COLLATE Latin1_General_CI_AI LIKE @FullNAme COLLATE Latin1_General_CI_AI)
                                                         AND ISNULL(u.Code, '') LIKE @Code
                                                         AND ISNULL(u.PhoneNumber, '') LIKE @PhoneNumber
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
                                                         AND
                                                         (
                                                             ISNULL(@DepartmentId, 0) = 0
                                                             OR ISNULL(u.DepartmentId, 0) = @DepartmentId
                                                         )
                                                         AND
                                                         (
                                                             ISNULL(@Status, -1) = -1
                                                             OR ISNULL(u.IsActive, 1) = @Status
                                                         )
                                                         AND u.IsDeleted = 0
                                                   ORDER BY {0} {1} OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;

                                                   SELECT COUNT(*)
                                                   FROM dbo.[User] u
                                                       LEFT JOIN dbo.Branch b
                                                           ON b.Id = u.BranchId
                                                       LEFT JOIN dbo.Role r
                                                           ON r.Id = u.RoleId
                                                   WHERE (ISNULL(u.FullName, '') COLLATE Latin1_General_CI_AI LIKE @FullNAme COLLATE Latin1_General_CI_AI
                                                         OR u.Username LIKE @FullNAme
                                                         OR ISNULL(u.DisplayName, '') COLLATE Latin1_General_CI_AI LIKE @FullNAme COLLATE Latin1_General_CI_AI)
                                                         AND ISNULL(u.Code, '') LIKE @Code
                                                         AND ISNULL(u.PhoneNumber, '') LIKE @PhoneNumber
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
                                                         AND
                                                         (
                                                             ISNULL(@DepartmentId, 0) = 0
                                                             OR ISNULL(u.DepartmentId, 0) = @DepartmentId
                                                         )
                                                         AND
                                                         (
                                                             ISNULL(@Status, -1) = -1
                                                             OR ISNULL(u.IsActive, 1) = @Status
                                                         )
                                                          AND u.IsDeleted = 0;";

        public const string USER_DELETE = @"UPDATE dbo.[User]
                                            SET UserUpdated = @UserUpdated,
                                                DateUpdated = GETDATE(),
                                                IsDeleted = 1
                                            WHERE Id = @Id;";

        public const string USER_CHECK_INVALID_ID = @"SELECT Id
                                                      FROM dbo.[User]
                                                      WHERE Id = @Id
                                                            AND IsDeleted = 0;";

        public const string USER_INSERT = @"INSERT dbo.[User]
                                            (
                                                Username,
                                                Password,
                                                Code,
                                                DisplayName,
                                                FullName,
                                                PhoneNumber,
                                                Email,
                                                Avatar,
                                                BranchId,
                                                RoleId,
                                                DepartmentId,
                                                Description,
                                                IsActive,
                                                UserCreated,
                                                DateCreated,
                                                UserUpdated,
                                                DateUpdated,
                                                IsDeleted
                                            )
                                            VALUES
                                            (   @Username,        -- Username - varchar(128)
                                                @Password,        -- Password - varchar(256)
                                                '',        -- Code - varchar(30)
                                                @DisplayName,       -- DisplayName - nvarchar(200)
                                                @FullName,       -- FullName - nvarchar(300)
                                                @PhoneNumber,        -- PhoneNumber - varchar(20)
                                                @Email,        -- Email - varchar(128)
                                                'default.png',        -- Avatar - varchar(128)
                                                @BranchId,         -- BranchId - int
                                                @RoleId,         -- RoleId - int
                                                @DepartmentId,         -- DepartmentId - int
                                                @Description,       -- Description - nvarchar(max)
                                                1,      -- IsActive - bit
                                                @UserCreated,         -- UserCreated - int
                                                GETDATE(), -- DateCreated - datetime
                                                @UserUpdated,         -- UserUpdated - int
                                                GETDATE(), -- DateUpdated - datetime
                                                0       -- IsDeleted - bit
                                                );

                                             SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string USER_CHECK_USERNAME_INVALID = @"SELECT Id
                                                            FROM dbo.[User]
                                                            WHERE Username = @Username
                                                                  AND IsDeleted = 0;";

        public const string USER_CHECK_PHONE_INVALID = @"SELECT Id
                                                         FROM dbo.[User]
                                                         WHERE PhoneNumber = @PhoneNumber
                                                               AND IsDeleted = 0;";

        public const string USER_UPDATE_CODE = @"UPDATE dbo.[User]
                                                 SET Code = @Code
                                                 WHERE Id = @Id
                                                       AND IsDeleted = 0;";

        public const string USER_UPDATE = @"UPDATE  dbo.[User]
                                            SET     DisplayName = @DisplayName ,
                                                    FullName = @FullName ,
                                                    PhoneNumber = @PhoneNumber ,
                                                    Email = @Email ,
                                                    BranchId = @BranchId ,
                                                    DepartmentId = @DepartmentId ,
                                                    RoleId = @RoleId ,
                                                    Description = @Description,
                                                    UserUpdated = @UserUpdated,
                                                    DateUpdated = GETDATE()
                                            WHERE   Id = @Id
                                                    AND IsDeleted = 0";

        public const string USER_CHECK_PHONE_INVALID_2 = @"SELECT Id
                                                           FROM dbo.[User]
                                                           WHERE PhoneNumber = @PhoneNumber
                                                                 AND Id <> @Id
                                                                 AND IsDeleted = 0;";

        public const string USER_GET_BY_ID = @"SELECT  Id ,
                                                       Username ,
                                                       DisplayName ,
                                                       FullName ,
                                                       PhoneNumber ,
                                                       Email ,
                                                       BranchId ,
                                                       DepartmentId ,
                                                       RoleId ,
                                                       Description
                                               FROM    dbo.[User]
                                               WHERE   Id = @Id
                                                       AND IsDeleted = 0";

        #endregion

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

        public const string ROLE_GET_ALL = @"SELECT Id, Name
                                             FROM dbo.Role
                                             WHERE IsDeleted = 0;";

        #endregion
    }
}
