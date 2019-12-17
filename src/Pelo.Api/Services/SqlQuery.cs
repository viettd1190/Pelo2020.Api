using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;

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
                                                    IsActive = @IsActive,
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
                                                       IsActive ,
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

        #region AppConfig

        public const string APP_CONFIG_GET_BY_PAGING = @"SELECT Id,
                                                                Name,
                                                                Value,
                                                                Description
                                                         FROM dbo.AppConfig
                                                         WHERE ISNULL(Description, '') COLLATE Latin1_General_CI_AI LIKE @Description COLLATE Latin1_General_CI_AI
                                                               AND Name LIKE @Name
                                                               AND IsDeleted = 0
                                                         ORDER BY {0} {1} OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
 
                                                         SELECT COUNT(*)
                                                         FROM dbo.AppConfig
                                                         WHERE ISNULL(Description, '') COLLATE Latin1_General_CI_AI LIKE @Description COLLATE Latin1_General_CI_AI
                                                               AND Name LIKE @Name
                                                               AND IsDeleted = 0;";

        public const string APP_CONFIG_INSERT = @"INSERT dbo.AppConfig
                                                  (
                                                      Name,
                                                      Value,
                                                      Description,
                                                      UserCreated,
                                                      DateCreated,
                                                      UserUpdated,
                                                      DateUpdated,
                                                      IsDeleted
                                                  )
                                                  VALUES
                                                  (   @Name,       -- Name - nvarchar(100)
                                                      @Value,       -- Value - nvarchar(max)
                                                      @Description,       -- Description - nvarchar(max)
                                                      @UserCreated,         -- UserCreated - int
                                                      GETDATE(), -- DateCreated - datetime
                                                      @UserUpdated,         -- UserUpdated - int
                                                      GETDATE(), -- DateUpdated - datetime
                                                      0       -- IsDeleted - bit
                                                      )";

        public const string APP_CONFIG_GET_BY_ID = @"SELECT Id,
                                                            Name,
                                                            Value,
                                                            Description
                                                     FROM dbo.AppConfig
                                                     WHERE Id = @Id
                                                           AND IsDeleted = 0;";

        public const string APP_CONFIG_CHECK_NAME_INVALID = @"SELECT Id
                                                              FROM dbo.AppConfig
                                                              WHERE Name = @Name
                                                                    AND IsDeleted = 0;";

        public const string APP_CONFIG_UPDATE = @"UPDATE dbo.AppConfig
                                                  SET Value = @Value,
                                                      Description = @Description,
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = GETDATE()
                                                  WHERE Id = @Id
                                                        AND IsDeleted = 0;";

        public const string APP_CONFIG_CHECK_ID_INVALID = @"SELECT Id
                                                            FROM dbo.AppConfig
                                                            WHERE Id = @Id
                                                                  AND IsDeleted = 0;";

        public const string APP_CONFIG_DELETE = @"UPDATE dbo.AppConfig
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = GETDATE(),
                                                      IsDeleted = 1
                                                  WHERE Id = @Id
                                                        AND IsDeleted = 0;";

        public const string APP_CONFIG_GET_VALUE_BY_NAME = @"SELECT Value
                                                             FROM dbo.AppConfig
                                                             WHERE Name = @Name
                                                                   AND IsDeleted = 0;";

        #endregion

        #region Province

        public const string PROVINCE_GET_ALL = @"SELECT Id,
                                                        Type,
                                                        Name
                                                 FROM dbo.Province
                                                 WHERE IsDeleted = 0
                                                 ORDER BY SortOrder;";

        #endregion

        #region District

        public const string DISTRICT_GET_ALL = @"SELECT Id,
                                                        Type,
                                                        Name
                                                 FROM dbo.District
                                                 WHERE IsDeleted = 0
                                                       AND ProvinceId = @ProvinceId
                                                 ORDER BY SortOrder;";

        #endregion

        #region Ward

        public const string WARD_GET_ALL = @"SELECT Id,
                                                    Type,
                                                    Name
                                             FROM dbo.Ward
                                             WHERE IsDeleted = 0
                                                   AND DistrictId = @DistrictId
                                             ORDER BY SortOrder;";

        #endregion

        #region CustomerGroup

        public const string CUSTOMER_GROUP_GET_BY_PAGING = @"SELECT Id,
                                                                    Name
                                                             FROM dbo.CustomerGroup
                                                             WHERE Name LIKE @Name
                                                                   AND IsDeleted = 0
                                                             ORDER BY {0} {1} OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
 
                                                             SELECT COUNT(*)
                                                             FROM dbo.CustomerGroup
                                                             WHERE Name LIKE @Name
                                                                   AND IsDeleted = 0;";

        public const string CUSTOMER_GROUP_INSERT = @"INSERT dbo.CustomerGroup
                                                      (
                                                          Name,
                                                          UserCreated,
                                                          DateCreated,
                                                          UserUpdated,
                                                          DateUpdated,
                                                          IsDeleted
                                                      )
                                                      VALUES
                                                      (   @Name,       -- Name - nvarchar(200)
                                                          @UserCreated,         -- UserCreated - int
                                                          GETDATE(), -- DateCreated - datetime
                                                          @UserUpdated,         -- UserUpdated - int
                                                          GETDATE(), -- DateUpdated - datetime
                                                          0       -- IsDeleted - bit
                                                          )";

        public const string CUSTOMER_GROUP_GET_BY_ID = @"SELECT Id,
                                                                Name
                                                         FROM dbo.CustomerGroup
                                                         WHERE Id = @Id
                                                               AND IsDeleted = 0;";

        public const string CUSTOMER_GROUP_CHECK_NAME_INVALID = @"SELECT Id
                                                                  FROM dbo.CustomerGroup
                                                                  WHERE Name = @Name
                                                                        AND IsDeleted = 0;";

        public const string CUSTOMER_GROUP_CHECK_NAME_INVALID_2 = @"SELECT Id
                                                                    FROM dbo.CustomerGroup
                                                                    WHERE Name = @Name
                                                                          AND Id <> @Id
                                                                          AND IsDeleted = 0;";

        public const string CUSTOMER_GROUP_UPDATE = @"UPDATE dbo.CustomerGroup
                                                      SET Name = @Name,
                                                          UserUpdated = @UserUpdated,
                                                          DateUpdated = GETDATE()
                                                      WHERE Id = @Id
                                                            AND IsDeleted = 0;";

        public const string CUSTOMER_GROUP_CHECK_ID_INVALID = @"SELECT Id
                                                                FROM dbo.CustomerGroup
                                                                WHERE Id = @Id
                                                                      AND IsDeleted = 0;";

        public const string CUSTOMER_GROUP_DELETE = @"UPDATE dbo.CustomerGroup
                                                      SET UserUpdated = @UserUpdated,
                                                          DateUpdated = GETDATE(),
                                                          IsDeleted = 1
                                                      WHERE Id = @Id
                                                            AND IsDeleted = 0;";

        #endregion

        #region CustomerVip

        public const string CUSTOMER_VIP_GET_BY_PAGING = @"SELECT Id,
                                                                  Name,
                                                                  Color,
                                                                  Profit
                                                           FROM dbo.CustomerVip
                                                           WHERE IsDeleted = 0
                                                           ORDER BY {0} {1} OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;

                                                           SELECT COUNT(*)
                                                           FROM dbo.CustomerVip
                                                           WHERE IsDeleted = 0;";

        public const string CUSTOMER_VIP_GET_BY_ID = @"SELECT Id,
                                                              Name,
                                                              Color,
                                                              Profit
                                                       FROM dbo.CustomerVip
                                                       WHERE Id = @Id
                                                             AND IsDeleted = 0;";

        public const string CUSTOMER_VIP_CHECK_PROFIT_INVALID = @"SELECT Id
                                                                  FROM dbo.CustomerVip
                                                                  WHERE Profit = @Profit
                                                                        AND IsDeleted = 0;";

        public const string CUSTOMER_VIP_CHJECK_NAME_INVALID = @"SELECT Id
                                                                 FROM dbo.CustomerVip
                                                                 WHERE Name = @Name
                                                                       AND IsDeleted = 0;";

        public const string CUSTOMER_VIP_CHECK_PROFIT_INVALID_2 = @"SELECT Id
                                                                    FROM dbo.CustomerVip
                                                                    WHERE Profit = @Profit
                                                                          AND Id <> @Id
                                                                          AND IsDeleted = 0;";

        public const string CUSTOMER_VIP_CHJECK_NAME_INVALID_2 = @"SELECT Id
                                                                   FROM dbo.CustomerVip
                                                                   WHERE Name = @Name
                                                                         AND Id <> @Id
                                                                         AND IsDeleted = 0;";

        public const string CUSTOMER_VIP_INSERT = @"INSERT dbo.CustomerVip
                                                    (
                                                        Name,
                                                        Color,
                                                        Profit,
                                                        UserCreated,
                                                        DateCreated,
                                                        UserUpdated,
                                                        DateUpdated,
                                                        IsDeleted
                                                    )
                                                    VALUES
                                                    (   @Name,       -- Name - nvarchar(200)
                                                        @Color,       -- Color - nvarchar(10)
                                                        @Profit,         -- Profit - int
                                                        @UserCreated,         -- UserCreated - int
                                                        GETDATE(), -- DateCreated - datetime
                                                        @UserUpdated,         -- UserUpdated - int
                                                        GETDATE(), -- DateUpdated - datetime
                                                        0       -- IsDeleted - bit
                                                        )";

        public const string CUSTOMER_VIP_UPDATE = @"UPDATE dbo.CustomerVip
                                                    SET Name = @Name,
                                                        Color = @Color,
                                                        Profit = @Profit,
                                                        UserUpdated = @UserUpdated,
                                                        DateUpdated = GETDATE()
                                                    WHERE Id = @Id
                                                          AND IsDeleted = 0;";

        public const string CUSTOMER_VIP_DELETE = @"UPDATE dbo.CustomerVip
                                                    SET UserUpdated = @UserUpdated,
                                                        DateUpdated = GETDATE(),
                                                        IsDeleted = 1
                                                    WHERE Id = @Id
                                                          AND IsDeleted = 0;";

        public const string CUSTOMER_VIP_CHECK_ID_INVALID = @"SELECT Id
                                                              FROM dbo.CustomerVip
                                                              WHERE Id = @Id
                                                                    AND IsDeleted = 0;";

        #endregion
    }
}
