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

        #region User
        public const string USER_FIND_BY_USERNAME = @"SELECT Id FROM dbo.[User] WHERE Username = @Username AND IsDeleted = 0";

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

        public const string USER_GET_ALL = @"SELECT Id,
                                                    DisplayName
                                             FROM   dbo.[User]
                                             WHERE  IsDeleted = 0;";

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

        public const string ROLE_NAME_GET_BY_USER_ID = @"SELECT r.Name
                                                         FROM dbo.Role r
                                                             INNER JOIN dbo.[User] u
                                                                 ON r.Id = u.RoleId
                                                         WHERE u.Id = @UserId
                                                               AND r.IsDeleted = 0
                                                               AND u.IsDeleted = 0;";

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

        public const string CUSTOMER_GROUP_GET_ALL = @"SELECT Id,
                                                              Name
                                                       FROM dbo.CustomerGroup
                                                       WHERE IsDeleted = 0
                                                       ORDER BY Id;";

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

        /// <summary>
        ///     Lấy mức độ khách hàng thân thiết có lợi nhuận thấp nhất để tự động gán cho khách hàng khi thêm khách hàng mới
        /// </summary>
        public const string CUSTOMER_VIP_GET_DEFAULT = @"SELECT TOP 1
                                                                Id
                                                         FROM dbo.CustomerVip
                                                         ORDER BY Profit;";

        public const string CUSTOMER_VIP_GET_ALL = @"SELECT Id,
                                                            Name
                                                     FROM dbo.CustomerVip
                                                     WHERE IsDeleted = 0
                                                     ORDER BY Profit;";

        #endregion

        #region Customer

        public const string CUSTOMER_GET_BY_PAGING = @"SELECT c.Id,
                                                              c.Code,
                                                              c.Name,
                                                              c.Phone,
                                                              c.Phone2,
                                                              c.Phone3,
                                                              c.Email,
                                                              p.Name AS Province,
                                                              d.Name AS District,
                                                              w.Name AS Ward,
                                                              c.Address,
                                                              c.Description,
                                                              cg.Name AS CustomerGroup,
                                                              cv.Name AS CustomerVip,
                                                              c.DateUpdated
                                                       FROM dbo.Customer c
                                                           LEFT JOIN dbo.Province p
                                                               ON p.Id = c.ProvinceId
                                                           LEFT JOIN dbo.District d
                                                               ON d.Id = c.DistrictId
                                                           LEFT JOIN dbo.Ward w
                                                               ON w.Id = c.WardId
                                                           LEFT JOIN dbo.CustomerGroup cg
                                                               ON cg.Id = c.CustomerGroupId
                                                           LEFT JOIN dbo.CustomerVip cv
                                                               ON cv.Id = c.CustomerVipId
                                                       WHERE ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @Name COLLATE Latin1_General_CI_AI
                                                             AND ISNULL(c.Code, '') LIKE @Code
                                                             AND
                                                             (
                                                                 ISNULL(c.Phone, '') LIKE @Phone
                                                                 OR ISNULL(c.Phone2, '') LIKE @Phone
                                                                 OR ISNULL(c.Phone3, '') LIKE @Phone
                                                             )
                                                             -- AND ISNULL(c.Email, '') LIKE @Email
                                                             AND ISNULL(c.Address, '') COLLATE Latin1_General_CI_AI LIKE @Address COLLATE Latin1_General_CI_AI
                                                             AND
                                                             (
                                                                 @ProvinceId = 0
                                                                 OR ISNULL(c.ProvinceId, 0) = @ProvinceId
                                                             )
                                                             AND
                                                             (
                                                                 @DistrictId = 0
                                                                 OR ISNULL(c.DistrictId, 0) = @DistrictId
                                                             )
                                                             AND
                                                             (
                                                                 @WardId = 0
                                                                 OR ISNULL(c.WardId, 0) = @WardId
                                                             )
	                                                         AND
                                                             (
                                                                 @CustomerGroupId = 0
                                                                 OR ISNULL(c.CustomerGroupId, 0) = @CustomerGroupId
                                                             )
	                                                         AND
                                                             (
                                                                 @CustomerVipId = 0
                                                                 OR ISNULL(c.CustomerVipId, 0) = @CustomerVipId
                                                             )
                                                             AND c.IsDeleted = 0
                                                       ORDER BY {0} {1} OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
                 
                                                       SELECT COUNT(*)
                                                       FROM dbo.Customer c
                                                       WHERE ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @Name COLLATE Latin1_General_CI_AI
                                                             AND ISNULL(c.Code, '') LIKE @Code
                                                             AND
                                                             (
                                                                 ISNULL(c.Phone, '') LIKE @Phone
                                                                 OR ISNULL(c.Phone2, '') LIKE @Phone
                                                                 OR ISNULL(c.Phone3, '') LIKE @Phone
                                                             )
                                                             -- AND ISNULL(c.Email, '') LIKE @Email
                                                             AND ISNULL(c.Address, '') COLLATE Latin1_General_CI_AI LIKE @Address COLLATE Latin1_General_CI_AI
                                                             AND
                                                             (
                                                                 @ProvinceId = 0
                                                                 OR ISNULL(c.ProvinceId, 0) = @ProvinceId
                                                             )
                                                             AND
                                                             (
                                                                 @DistrictId = 0
                                                                 OR ISNULL(c.DistrictId, 0) = @DistrictId
                                                             )
                                                             AND
                                                             (
                                                                 @WardId = 0
                                                                 OR ISNULL(c.WardId, 0) = @WardId
                                                             )
	                                                         AND
                                                             (
                                                                 @CustomerGroupId = 0
                                                                 OR ISNULL(c.CustomerGroupId, 0) = @CustomerGroupId
                                                             )
	                                                         AND
                                                             (
                                                                 @CustomerVipId = 0
                                                                 OR ISNULL(c.CustomerVipId, 0) = @CustomerVipId
                                                             )
                                                             AND c.IsDeleted = 0;";

        public const string CUSTOMER_CHECK_PHONE_INVALID = @"SELECT Id
                                                             FROM dbo.Customer
                                                             WHERE (
                                                                       Phone = @Phone
                                                                       OR Phone2 = @Phone2
                                                                       OR Phone3 = @Phone3
                                                                   )
                                                                   AND IsDeleted = 0;";

        public const string CUSTOMER_CHECK_PHONE_INVALID_2 = @"SELECT Id
                                                             FROM dbo.Customer
                                                             WHERE (
                                                                       Phone = @Phone
                                                                       OR Phone2 = @Phone2
                                                                       OR Phone3 = @Phone3
                                                                   )
                                                                   AND Id <> @Id
                                                                   AND IsDeleted = 0;";

        public const string CUSTOMER_CHECK_ID_INVALID = @"SELECT Id
                                                          FROM dbo.Customer
                                                          WHERE Id = @Id
                                                                AND IsDeleted = 0;";

        public const string CUSTOMER_COUNT_BY_DATE_CREATED = @"SELECT COUNT(Id)
                                                               FROM dbo.Customer
                                                               WHERE CAST(DateCreated AS DATE) = @DateCreated;";

        public const string CUSTOMER_INSERT = @"INSERT dbo.Customer
                                                (
                                                    Code,
                                                    Name,
                                                    Phone,
                                                    Email,
                                                    WardId,
                                                    Address,
                                                    Description,
                                                    UserCreated,
                                                    DateCreated,
                                                    UserUpdated,
                                                    DateUpdated,
                                                    IsDeleted,
                                                    ProvinceId,
                                                    DistrictId,
                                                    CustomerGroupId,
                                                    Phone2,
                                                    Phone3,
                                                    CustomerVipId,
                                                    Profit,
                                                    ProfitUpdate
                                                )
                                                VALUES
                                                (   @Code,       -- Code - nvarchar(50)
                                                    @Name,       -- Name - nvarchar(200)
                                                    @Phone,       -- Phone - nvarchar(50)
                                                    @Email,       -- Email - nvarchar(200)
                                                    @WardId,         -- WardId - int
                                                    @Address,       -- Address - nvarchar(max)
                                                    @Description,       -- Description - nvarchar(max)
                                                    @UserCreated,         -- UserCreated - int
                                                    GETDATE(), -- DateCreated - datetime
                                                    @UserUpdated,         -- UserUpdated - int
                                                    GETDATE(), -- DateUpdated - datetime
                                                    0,      -- IsDeleted - bit
                                                    @ProvinceId,         -- ProvinceId - int
                                                    @DistrictId,         -- DistrictId - int
                                                    @CustomerGroupId,         -- CustomerGroupId - int
                                                    @Phone2,       -- Phone2 - nvarchar(20)
                                                    @Phone3,       -- Phone3 - nvarchar(20)
                                                    @CustomerVipId,         -- CustomerVipId - int
                                                    0,         -- Profit - int
                                                    0          -- ProfitUpdate - int
                                                    )";

        public const string CUSTOMER_UPDATE = @"UPDATE dbo.Customer
                                                SET Name = @Name,
                                                    Phone = @Phone,
                                                    Phone2 = @Phone2,
                                                    Phone3 = @Phone3,
                                                    Email = @Email,
                                                    ProvinceId = @ProvinceId,
                                                    DistrictId = @DistrictId,
                                                    WardId = @WardId,
                                                    Address = @Address,
                                                    CustomerGroupId = @CustomerGroupId,
                                                    Description = @Description,
                                                    UserUpdated = @UserUpdated,
                                                    DateUpdated = GETDATE()
                                                WHERE Id = @Id
                                                      AND IsDeleted = 0;";

        public const string CUSTOMER_GET_BY_ID = @"SELECT Id,
                                                          Code,
                                                          Name,
                                                          Phone,
                                                          Phone2,
                                                          Phone3,
                                                          Email,
                                                          ProvinceId,
                                                          DistrictId,
                                                          WardId,
                                                          Address,
                                                          CustomerGroupId,
                                                          CustomerVipId,
                                                          Profit,
                                                          ProfitUpdate,
                                                          Description
                                                   FROM dbo.Customer
                                                   WHERE Id = @Id
                                                         AND IsDeleted = 0;";

        public const string CUSTOMER_DELETE = @"UPDATE dbo.Customer
                                                SET UserUpdated = @UserUpdated,
                                                    DateUpdated = GETDATE(),
                                                    IsDeleted = 1
                                                WHERE Id = @Id
                                                      AND IsDeleted = 0;";

        public const string CUSTOMER_GET_BY_PHONE = @"SELECT c.Id,
                                                             c.Code,
                                                             c.Name,
                                                             c.Address,
                                                             p.Type AS ProvinceType,
                                                             p.Name AS Province,
                                                             d.Type AS DistrictType,
                                                             d.Name AS District,
                                                             w.Type AS WardType,
                                                             w.Name AS Ward,
                                                             c.Description,
                                                             cg.Name AS CustomerGroup,
                                                             cv.Name AS CustomerVip,
                                                             c.Phone,
                                                             c.Phone2,
                                                             c.Phone3,
                                                             u.DisplayName AS UserCreated
                                                      FROM dbo.Customer c
                                                          LEFT JOIN dbo.Province p
                                                              ON p.Id = c.ProvinceId
                                                          LEFT JOIN dbo.District d
                                                              ON d.Id = c.DistrictId
                                                          LEFT JOIN dbo.Ward w
                                                              ON w.Id = c.WardId
                                                          LEFT JOIN dbo.CustomerGroup cg
                                                              ON cg.Id = c.CustomerGroupId
                                                          LEFT JOIN dbo.CustomerVip cv
                                                              ON cv.Id = c.CustomerVipId
                                                          LEFT JOIN dbo.User u
                                                              ON u.Id = c.UserCreated
                                                      WHERE (
                                                                c.Phone = @Phone
                                                                OR c.Phone2 = @Phone
                                                                OR c.Phone3 = @Phone
                                                            )
                                                            AND c.IsDeleted = 0;";

        public const string CUSTOMER_GET_DETAIL = @"SELECT c.Id,
                                                           c.Code,
                                                           c.Name,
                                                           c.Phone,
                                                           c.Phone2,
                                                           c.Phone3,
                                                           c.Email,
                                                           c.Address,
                                                           p.Type AS ProvinceType,
                                                           p.Name AS Province,
                                                           d.Type AS DistrictType,
                                                           d.Name AS District,
                                                           w.Type AS WardType,
                                                           w.Name AS Ward,
                                                           cg.Name AS CustomerGroup,
                                                           cv.Name AS CustomerVip,
                                                           c.Profit,
                                                           c.ProfitUpdate,
                                                           u1.DisplayName AS UserCare,
                                                           u1.PhoneNumber AS UserCarePhone,
                                                           u2.DisplayName AS UserFirst,
                                                           u2.PhoneNumber AS UserFirstPhone,
                                                           c.Description,
                                                           u3.DisplayName AS UserCreated,
                                                           u3.PhoneNumber AS UserCreatedPhone,
                                                           c.DateCreated,
                                                           c.DateUpdated
                                                    FROM dbo.Customer c
                                                        LEFT JOIN dbo.Province p
                                                            ON p.Id = c.ProvinceId
                                                        LEFT JOIN dbo.District d
                                                            ON d.Id = c.DistrictId
                                                        LEFT JOIN dbo.Ward w
                                                            ON w.Id = c.WardId
                                                        LEFT JOIN dbo.CustomerGroup cg
                                                            ON cg.Id = c.CustomerGroupId
                                                        LEFT JOIN dbo.CustomerVip cv
                                                            ON cv.Id = c.CustomerVipId
                                                        LEFT JOIN dbo.CustomerUser cu1
                                                            ON cu1.CustomerId = c.Id
                                                               AND cu1.Type = 1 --ban dang phu trach
                                                        LEFT JOIN dbo.CustomerUser cu2
                                                            ON cu2.CustomerId = c.Id
                                                               AND cu2.Type = 2 --nguon giup quen biet khach lan dau
                                                        LEFT JOIN dbo.[User] u1
                                                            ON u1.Id = cu1.UserId
                                                        LEFT JOIN dbo.[User] u2
                                                            ON u2.Id = cu2.UserId
                                                        LEFT JOIN dbo.[User] u3
                                                            ON u3.Id = c.UserCreated
                                                    WHERE c.Id = @Id
                                                          AND c.IsDeleted = 0;";

        #endregion

        #region CrmType

        public const string CRM_TYPE_GET_ALL = @"SELECT Id,
                                                        Name
                                                 FROM dbo.CrmType
                                                 WHERE IsDeleted = 0;";

        #endregion

        #region CrmStatus

        public const string CRM_STATUS_GET_ALL = @"SELECT Id,
                                                          Name
                                                   FROM dbo.CrmStatus
                                                   WHERE IsDeleted = 0
                                                   ORDER BY Id;";

        #endregion

        #region CustomerSource

        public const string CUSTOMER_SOURCE_GET_ALL = @"SELECT Id,
                                                               Name
                                                        FROM dbo.CustomerSource
                                                        WHERE IsDeleted = 0
                                                        ORDER BY Id;";

        #endregion

        #region ProductGroup

        public const string PRODUCT_GROUP_GET_ALL = @"SELECT Id,
                                                                Name
                                                         FROM dbo.ProductGroup
                                                         WHERE IsDeleted = 0
                                                         ORDER BY Id;";

        #endregion

        #region ProductUnit

        public const string PRODUCT_UNIT_GET_ALL = @"SELECT Id,
                                                            Name
                                                     FROM dbo.ProductUnit
                                                     WHERE IsDeleted = 0
                                                     ORDER BY Id;";

        #endregion

        #region CrmPriority

        public const string CRM_PRIORITY_GET_ALL = @"SELECT Id,
                                                            Name
                                                     FROM dbo.CrmPriority
                                                     WHERE IsDeleted = 0
                                                     ORDER BY Id;";

        #endregion

        #region Crm
        public const string CRM_USER_INSERT = @"INSERT dbo.CrmUser
                                                        (
                                                            CrmId,
                                                            UserId,
                                                            Type,
                                                            UserCreated,
                                                            DateCreated,
                                                            UserUpdated,
                                                            DateUpdated,
                                                            IsDeleted
                                                        )
                                                        VALUES
                                                        (   @CrmId,
                                                            @UserId,
                                                            @Type,
                                                            @UserCreated,
                                                            @DateCreated,
                                                            @UserUpdated,
                                                            @DateUpdated,
                                                            0 );
                                                        SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string CRM_COUNT_BY_DATE = @"SELECT COUNT(*) FROM dbo.Crm WHERE Code LIKE @Code";

        /// <summary>
        /// Lấy danh sách CRM ko có điều kiện UserCareId
        /// </summary>
        public const string CRM_GET_BY_PAGING = @"DROP TABLE IF EXISTS #tmpCrm;

                                                  SELECT c.Id
                                                  INTO #tmpCrm
                                                  FROM dbo.Crm c
                                                      LEFT JOIN dbo.Customer cu
                                                          ON cu.Id = c.CustomerId
                                                  WHERE c.Code LIKE @Code
                                                        AND ISNULL(cu.Name, '') COLLATE Latin1_General_CI_AI LIKE @CustomerName COLLATE Latin1_General_CI_AI
                                                        AND ISNULL(cu.Address, '') COLLATE Latin1_General_CI_AI LIKE @CustomerAddress COLLATE Latin1_General_CI_AI
                                                        AND ISNULL(c.Need, '') COLLATE Latin1_General_CI_AI LIKE @Need COLLATE Latin1_General_CI_AI
                                                        AND
                                                        (
                                                            cu.Phone LIKE @CustomerPhone
                                                            OR cu.Phone2 LIKE @CustomerPhone
                                                            OR cu.Phone3 LIKE @CustomerPhone
                                                        )
                                                        AND cu.Code LIKE @CustomerCode
                                                        AND
                                                        (
                                                            @ProvinceId = 0
                                                            OR ISNULL(cu.ProvinceId, 0) = @ProvinceId
                                                        )
                                                        AND
                                                        (
                                                            @DistrictId = 0
                                                            OR ISNULL(cu.DistrictId, 0) = @DistrictId
                                                        )
                                                        AND
                                                        (
                                                            @WardId = 0
                                                            OR ISNULL(cu.WardId, 0) = @WardId
                                                        )
                                                        AND
                                                        (
                                                            @CrmPriorityId = 0
                                                            OR ISNULL(c.CrmPriorityId, 0) = @CrmPriorityId
                                                        )
                                                        AND
                                                        (
                                                            @CrmStatusId = 0
                                                            OR ISNULL(c.CrmStatusId, 0) = @CrmStatusId
                                                        )
                                                        AND
                                                        (
                                                            @CustomerSourceId = 0
                                                            OR ISNULL(c.CustomerSourceId, 0) = @CustomerSourceId
                                                        )
                                                        AND
                                                        (
                                                            @CustomerGroupId = 0
                                                            OR ISNULL(cu.CustomerGroupId, 0) = @CustomerGroupId
                                                        )
                                                        AND
                                                        (
                                                            @ProductGroupId = 0
                                                            OR ISNULL(c.ProductGroupId, 0) = @ProductGroupId
                                                        )
                                                        AND
                                                        (
                                                            @CrmTypeId = 0
                                                            OR ISNULL(c.CrmTypeId, 0) = @CrmTypeId
                                                        )
                                                        AND
                                                        (
                                                            @CustomerVipId = 0
                                                            OR ISNULL(cu.CustomerVipId, 0) = @CustomerVipId
                                                        )
                                                        AND
                                                        (
                                                            @Visit = -1
                                                            OR ISNULL(c.Visit, 0) = @Visit
                                                        )
                                                        AND
                                                        (
                                                            @FromDate IS NULL
                                                            OR c.ContactDate >= @FromDate
                                                        )
                                                        AND
                                                        (
                                                            @ToDate IS NULL
                                                            OR c.ContactDate <= @ToDate
                                                        )
                                                        AND
                                                        (
                                                            @DateCreated IS NULL
                                                            OR @DateCreated = '2000-01-01 00:00:00'
                                                            OR CONVERT(DATE, c.DateCreated) = @DateCreated
                                                        )
                                                        AND
                                                        (
                                                            @UserCreatedId = 0
                                                            OR ISNULL(c.UserCreated, 0) = @UserCreatedId
                                                        )
                                                        AND c.IsDeleted = 0;
    
    
                                                  SELECT c.Id,
                                                         c.Code,
                                                         cs.Name AS CrmStatus,
                                                         cs.Color AS CrmStatusColor,
                                                         cu.Name AS CustomerName,
                                                         cu.Phone AS CustomerPhone,
                                                         cu.Phone2 AS CustomerPhone2,
                                                         cu.Phone3 AS CustomerPhone3,
                                                         cu.Address AS CustomerAddress,
                                                         p.Name AS Province,
                                                         d.Name AS District,
                                                         w.Name AS Ward,
                                                         cg.Name AS CustomerGroup,
                                                         cv.Name AS CustomerVip,
                                                         c.Need,
                                                         c.Description,
                                                         pg.Name AS ProductGroup,
                                                         cp.Name AS CrmPriority,
                                                         cus.Name AS CustomerSource,
                                                         ct.Name AS CrmType,
                                                         c.Visit,
                                                         u.DisplayName AS UserCreated,
                                                         u.PhoneNumber AS UserCreatedPhone,
                                                         c.ContactDate,
                                                         c.DateCreated
                                                  FROM #tmpCrm t
                                                      INNER JOIN dbo.Crm c
                                                          ON c.Id = t.Id
                                                      LEFT JOIN dbo.CrmStatus cs
                                                          ON cs.Id = c.CrmStatusId
                                                      LEFT JOIN dbo.CrmPriority cp
                                                          ON cp.Id = c.CrmPriorityId
                                                      LEFT JOIN dbo.CrmType ct
                                                          ON ct.Id = c.CrmTypeId
                                                      LEFT JOIN dbo.Customer cu
                                                          ON cu.Id = c.CustomerId
                                                      LEFT JOIN dbo.Province p
                                                          ON p.Id = cu.ProvinceId
                                                      LEFT JOIN dbo.District d
                                                          ON d.Id = cu.DistrictId
                                                      LEFT JOIN dbo.Ward w
                                                          ON w.Id = cu.WardId
                                                      LEFT JOIN dbo.CustomerGroup cg
                                                          ON cg.Id = cu.CustomerGroupId
                                                      LEFT JOIN dbo.CustomerSource cus
                                                          ON cus.Id = c.CustomerSourceId
                                                      LEFT JOIN dbo.CustomerVip cv
                                                          ON cv.Id = cu.CustomerVipId
                                                      LEFT JOIN dbo.ProductGroup pg
                                                          ON pg.Id = c.ProductGroupId
                                                      LEFT JOIN dbo.[User] u
                                                          ON u.Id = c.UserCreated
                                                  ORDER BY c.DateCreated DESC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;

                                                  SELECT COUNT(*)
                                                  FROM #tmpCrm;
    
                                                  DROP TABLE #tmpCrm;";

        /// <summary>
        /// Lấy danh sách CRM có điều kiện UserCareId
        /// </summary>
        public const string CRM_GET_BY_PAGING_2 = @"DROP TABLE IF EXISTS #tmpCrm;

                                                    SELECT c.Id
                                                    INTO #tmpCrm
                                                    FROM dbo.Crm c
                                                        LEFT JOIN dbo.Customer cu
                                                            ON cu.Id = c.CustomerId
                                                        INNER JOIN dbo.CrmUser cru
                                                            ON cru.CrmId = c.Id
                                                    WHERE c.Code LIKE @Code
                                                          AND ISNULL(cu.Name, '') COLLATE Latin1_General_CI_AI LIKE @CustomerName COLLATE Latin1_General_CI_AI
                                                          AND ISNULL(cu.Address, '') COLLATE Latin1_General_CI_AI LIKE @CustomerAddress COLLATE Latin1_General_CI_AI
                                                          AND ISNULL(c.Need, '') COLLATE Latin1_General_CI_AI LIKE @Need COLLATE Latin1_General_CI_AI
                                                          AND
                                                          (
                                                              cu.Phone LIKE @CustomerPhone
                                                              OR cu.Phone2 LIKE @CustomerPhone
                                                              OR cu.Phone3 LIKE @CustomerPhone
                                                          )
                                                          AND cu.Code LIKE @CustomerCode
                                                          AND
                                                          (
                                                              @ProvinceId = 0
                                                              OR ISNULL(cu.ProvinceId, 0) = @ProvinceId
                                                          )
                                                          AND
                                                          (
                                                              @DistrictId = 0
                                                              OR ISNULL(cu.DistrictId, 0) = @DistrictId
                                                          )
                                                          AND
                                                          (
                                                              @WardId = 0
                                                              OR ISNULL(cu.WardId, 0) = @WardId
                                                          )
                                                          AND
                                                          (
                                                              @CrmPriorityId = 0
                                                              OR ISNULL(c.CrmPriorityId, 0) = @CrmPriorityId
                                                          )
                                                          AND
                                                          (
                                                              @CrmStatusId = 0
                                                              OR ISNULL(c.CrmStatusId, 0) = @CrmStatusId
                                                          )
                                                          AND
                                                          (
                                                              @CustomerSourceId = 0
                                                              OR ISNULL(c.CustomerSourceId, 0) = @CustomerSourceId
                                                          )
                                                          AND
                                                          (
                                                              @CustomerGroupId = 0
                                                              OR ISNULL(cu.CustomerGroupId, 0) = @CustomerGroupId
                                                          )
                                                          AND
                                                          (
                                                              @ProductGroupId = 0
                                                              OR ISNULL(c.ProductGroupId, 0) = @ProductGroupId
                                                          )
                                                          AND
                                                          (
                                                              @CrmTypeId = 0
                                                              OR ISNULL(c.CrmTypeId, 0) = @CrmTypeId
                                                          )
                                                          AND
                                                          (
                                                              @CustomerVipId = 0
                                                              OR ISNULL(cu.CustomerVipId, 0) = @CustomerVipId
                                                          )
                                                          AND
                                                          (
                                                              @Visit = -1
                                                              OR ISNULL(c.Visit, 0) = @Visit
                                                          )
                                                          AND
                                                          (
                                                              @FromDate IS NULL
                                                              OR c.ContactDate >= @FromDate
                                                          )
                                                          AND
                                                          (
                                                              @ToDate IS NULL
                                                              OR c.ContactDate <= @ToDate
                                                          )
                                                          AND
                                                          (
                                                              @DateCreated IS NULL
                                                              OR @DateCreated = '2000-01-01 00:00:00'
                                                              OR CONVERT(DATE, c.DateCreated) = @DateCreated
                                                          --)
                                                          AND
                                                          (
                                                              @UserCreatedId = 0
                                                              OR ISNULL(c.UserCreated, 0) = @UserCreatedId
                                                          )
                                                          AND cru.UserId = @UserCareId
                                                          AND cru.IsDeleted = 0
                                                          AND c.IsDeleted = 0;
      
                                                    SELECT c.Id,
                                                           c.Code,
                                                           cs.Name AS CrmStatus,
                                                           cs.Color AS CrmStatusColor,
                                                           cu.Name AS CustomerName,
                                                           cu.Phone AS CustomerPhone,
                                                           cu.Phone2 AS CustomerPhone2,
                                                           cu.Phone3 AS CustomerPhone3,
                                                           cu.Address AS CustomerAddress,
                                                           p.Name AS Province,
                                                           d.Name AS District,
                                                           w.Name AS Ward,
                                                           cg.Name AS CustomerGroup,
                                                           cv.Name AS CustomerVip,
                                                           c.Need,
                                                           c.Description,
                                                           pg.Name AS ProductGroup,
                                                           cp.Name AS CrmPriority,
                                                           cus.Name AS CustomerSource,
                                                           ct.Name AS CrmType,
                                                           c.Visit,
                                                           u.DisplayName AS UserCreated,
                                                           u.PhoneNumber AS UserCreatedPhone,
                                                           c.ContactDate,
                                                           c.DateCreated
                                                    FROM #tmpCrm t
                                                        INNER JOIN dbo.Crm c
                                                            ON c.Id = t.Id
                                                        LEFT JOIN dbo.CrmStatus cs
                                                            ON cs.Id = c.CrmStatusId
                                                        LEFT JOIN dbo.CrmPriority cp
                                                            ON cp.Id = c.CrmPriorityId
                                                        LEFT JOIN dbo.CrmType ct
                                                            ON ct.Id = c.CrmTypeId
                                                        LEFT JOIN dbo.Customer cu
                                                            ON cu.Id = c.CustomerId
                                                        LEFT JOIN dbo.Province p
                                                            ON p.Id = cu.ProvinceId
                                                        LEFT JOIN dbo.District d
                                                            ON d.Id = cu.DistrictId
                                                        LEFT JOIN dbo.Ward w
                                                            ON w.Id = cu.WardId
                                                        LEFT JOIN dbo.CustomerGroup cg
                                                            ON cg.Id = cu.CustomerGroupId
                                                        LEFT JOIN dbo.CustomerSource cus
                                                            ON cus.Id = c.CustomerSourceId
                                                        LEFT JOIN dbo.CustomerVip cv
                                                            ON cv.Id = cu.CustomerVipId
                                                        LEFT JOIN dbo.ProductGroup pg
                                                            ON pg.Id = c.ProductGroupId
                                                        LEFT JOIN dbo.[User] u
                                                            ON u.Id = c.UserCreated
                                                    ORDER BY c.DateCreated DESC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
      
                                                    SELECT COUNT(*)
                                                    FROM #tmpCrm;
      
                                                    DROP TABLE #tmpCrm;";

        public const string CRM_USER_CARE_GET_BY_CRM_ID = @"SELECT u.DisplayName,
                                                                   u.PhoneNumber
                                                            FROM dbo.CrmUser cu
                                                                INNER JOIN dbo.[User] u
                                                                    ON cu.UserId = u.Id
                                                            WHERE cu.CrmId = @CrmId
                                                                  AND cu.Type = 0
                                                                  AND cu.IsDeleted = 0
                                                                  AND u.IsDeleted = 0;";

        public const string CRM_INSERT = @"INSERT dbo.Crm
                                                    (
                                                        CustomerId,
                                                        CrmStatusId,
                                                        ContactDate,
                                                        ProductGroupId,
                                                        CrmPriorityId,
                                                        CrmTypeId,
                                                        Need,
                                                        Description,
                                                        CustomerSourceId,
                                                        Code,
                                                        Visit,
                                                        UserCreated,
                                                        DateCreated,
                                                        UserUpdated,
                                                        DateUpdated,
                                                        IsDeleted
                                                    )
                                                    VALUES
                                                    (   @CustomerId,         -- CustomerId - int
                                                        @CrmStatusId,         -- CrmStatusId - int
                                                        @ContactDate, -- ContactDate - datetime
                                                        @ProductGroupId,         -- ProductGroupId - int
                                                        @CrmPriorityId,         -- CrmPriorityId - int
                                                        @CrmTypeId,         -- CrmPriorityId - int
                                                        @Need,        -- Need - text
                                                        @Description,       -- Description - nvarchar(300)
                                                        @CustomerSourceId,         -- CustomerSourceId - int
                                                        @Code,       -- Code - nvarchar(20)
                                                        @Visit,         -- Visit - int
                                                        @UserCreated,         -- UserCreated - int
                                                        @DateCreated, -- DateCreated - datetime
                                                        @UserUpdated,         -- UserUpdated - int
                                                        @DateUpdated, -- DateUpdated - datetime
                                                        0       -- IsDeleted - bit
                                                        );
                                            SELECT CAST(SCOPE_IDENTITY() as int);";

        /// <summary>
        /// Lấy danh sách CRM của khách hàng đối với những user được quyền xem tất cả CRM
        /// </summary>
        public const string CRM_GET_BY_CUSTOMER_ID = @"DROP TABLE IF EXISTS #tmpCrm;

                                                       SELECT c.Id
                                                       INTO #tmpCrm
                                                       FROM dbo.Crm c
                                                           LEFT JOIN dbo.Customer cu
                                                               ON cu.Id = c.CustomerId
                                                       WHERE c.CustomerId = @CustomerId
                                                             AND cu.IsDeleted = 0
                                                             AND c.IsDeleted = 0;
                          
                          
                                                       SELECT c.Id,
                                                              c.Code,
                                                              cs.Name AS CrmStatus,
                                                              cs.Color AS CrmStatusColor,
                                                              cu.Name AS CustomerName,
                                                              cu.Phone AS CustomerPhone,
                                                              cu.Phone2 AS CustomerPhone2,
                                                              cu.Phone3 AS CustomerPhone3,
                                                              cu.Address AS CustomerAddress,
                                                              p.Name AS Province,
                                                              d.Name AS District,
                                                              w.Name AS Ward,
                                                              cg.Name AS CustomerGroup,
                                                              cv.Name AS CustomerVip,
                                                              c.Need,
                                                              c.Description,
                                                              pg.Name AS ProductGroup,
                                                              cp.Name AS CrmPriority,
                                                              cus.Name AS CustomerSource,
                                                              ct.Name AS CrmType,
                                                              c.Visit,
                                                              u.DisplayName AS UserCreated,
                                                              u.PhoneNumber AS UserCreatedPhone,
                                                              c.ContactDate,
                                                              c.DateCreated
                                                       FROM #tmpCrm t
                                                           INNER JOIN dbo.Crm c
                                                               ON c.Id = t.Id
                                                           LEFT JOIN dbo.CrmStatus cs
                                                               ON cs.Id = c.CrmStatusId
                                                           LEFT JOIN dbo.CrmPriority cp
                                                               ON cp.Id = c.CrmPriorityId
                                                           LEFT JOIN dbo.CrmType ct
                                                               ON ct.Id = c.CrmTypeId
                                                           LEFT JOIN dbo.Customer cu
                                                               ON cu.Id = c.CustomerId
                                                           LEFT JOIN dbo.Province p
                                                               ON p.Id = cu.ProvinceId
                                                           LEFT JOIN dbo.District d
                                                               ON d.Id = cu.DistrictId
                                                           LEFT JOIN dbo.Ward w
                                                               ON w.Id = cu.WardId
                                                           LEFT JOIN dbo.CustomerGroup cg
                                                               ON cg.Id = cu.CustomerGroupId
                                                           LEFT JOIN dbo.CustomerSource cus
                                                               ON cus.Id = c.CustomerSourceId
                                                           LEFT JOIN dbo.CustomerVip cv
                                                               ON cv.Id = cu.CustomerVipId
                                                           LEFT JOIN dbo.ProductGroup pg
                                                               ON pg.Id = c.ProductGroupId
                                                           LEFT JOIN dbo.[User] u
                                                               ON u.Id = c.UserCreated
                                                       ORDER BY c.DateCreated DESC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
                          
                                                       SELECT COUNT(*)
                                                       FROM #tmpCrm;
                          
                                                       DROP TABLE #tmpCrm;";

        /// <summary>
        /// Lấy danh sách CRM của khách hàng đối với những user không có quyền xem hết tất cả CRM
        /// </summary>
        public const string CRM_GET_BY_CUSTOMER_ID_2 = @"DROP TABLE IF EXISTS #tmpCrm;

                                                         SELECT c.Id
                                                         INTO #tmpCrm
                                                         FROM dbo.Crm c
                                                             LEFT JOIN dbo.Customer cu
                                                                 ON cu.Id = c.CustomerId
                                                             INNER JOIN dbo.CrmUser cru
                                                                 ON cru.CrmId = c.Id
                                                         WHERE c.CustomerId = @CustomerId
                                                               AND cru.UserId = @UserCareId
                                                               AND cu.IsDeleted = 0
                                                               AND cru.IsDeleted = 0
                                                               AND c.IsDeleted = 0;
                
                                                         SELECT c.Id,
                                                                c.Code,
                                                                cs.Name AS CrmStatus,
                                                                cs.Color AS CrmStatusColor,
                                                                cu.Name AS CustomerName,
                                                                cu.Phone AS CustomerPhone,
                                                                cu.Phone2 AS CustomerPhone2,
                                                                cu.Phone3 AS CustomerPhone3,
                                                                cu.Address AS CustomerAddress,
                                                                p.Name AS Province,
                                                                d.Name AS District,
                                                                w.Name AS Ward,
                                                                cg.Name AS CustomerGroup,
                                                                cv.Name AS CustomerVip,
                                                                c.Need,
                                                                c.Description,
                                                                pg.Name AS ProductGroup,
                                                                cp.Name AS CrmPriority,
                                                                cus.Name AS CustomerSource,
                                                                ct.Name AS CrmType,
                                                                c.Visit,
                                                                u.DisplayName AS UserCreated,
                                                                u.PhoneNumber AS UserCreatedPhone,
                                                                c.ContactDate,
                                                                c.DateCreated
                                                         FROM #tmpCrm t
                                                             INNER JOIN dbo.Crm c
                                                                 ON c.Id = t.Id
                                                             LEFT JOIN dbo.CrmStatus cs
                                                                 ON cs.Id = c.CrmStatusId
                                                             LEFT JOIN dbo.CrmPriority cp
                                                                 ON cp.Id = c.CrmPriorityId
                                                             LEFT JOIN dbo.CrmType ct
                                                                 ON ct.Id = c.CrmTypeId
                                                             LEFT JOIN dbo.Customer cu
                                                                 ON cu.Id = c.CustomerId
                                                             LEFT JOIN dbo.Province p
                                                                 ON p.Id = cu.ProvinceId
                                                             LEFT JOIN dbo.District d
                                                                 ON d.Id = cu.DistrictId
                                                             LEFT JOIN dbo.Ward w
                                                                 ON w.Id = cu.WardId
                                                             LEFT JOIN dbo.CustomerGroup cg
                                                                 ON cg.Id = cu.CustomerGroupId
                                                             LEFT JOIN dbo.CustomerSource cus
                                                                 ON cus.Id = c.CustomerSourceId
                                                             LEFT JOIN dbo.CustomerVip cv
                                                                 ON cv.Id = cu.CustomerVipId
                                                             LEFT JOIN dbo.ProductGroup pg
                                                                 ON pg.Id = c.ProductGroupId
                                                             LEFT JOIN dbo.[User] u
                                                                 ON u.Id = c.UserCreated
                                                         ORDER BY c.DateCreated DESC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
                
                                                         SELECT COUNT(*)
                                                         FROM #tmpCrm;
                
                                                         DROP TABLE #tmpCrm;";

        #endregion

        #region PayMethod

        public const string PAY_METHOD_GET_BY_PAGING = @"  SELECT *
                                                         FROM dbo.PayMethod 
                                                         WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                             AND IsDeleted = 0
                                                        ORDER BY {0} {1}
                                                        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;

                                                        SELECT COUNT(*)
                                                        FROM dbo.PayMethod 
                                                        WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                            AND IsDeleted = 0;";

        public const string PAY_METHOD_GET_BY_ID = @"SELECT Id, Name FROM dbo.PayMethod WHERE Id = @Id AND IsDeleted = 0";

        public const string PAY_METHOD_GET_ALL = "SELECT Id, Name FROM dbo.PayMethod WHERE IsDeleted = 0";

        public const string PAY_METHOD_INSERT = @"INSERT dbo.PayMethod
                                                        (Name,
                                                         UserCreated,
                                                         DateCreated,
                                                         UserUpdated,
                                                         DateUpdated,
                                                         IsDeleted)
                                                 VALUES (@Name,
                                                         @UserCreated,
                                                         @DateCreated,
                                                         @UserUpdated,
                                                         @DateUpdated,
                                                         0);

                                                 SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string PAY_METHOD_UPDATE = @"  UPDATE dbo.PayMethod
                                                  SET Name = @Name,
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string PAY_METHOD_DELETE = @"  UPDATE dbo.PayMethod
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        public const string PAY_METHOD_FIND_BY_NAME = @"SELECT * FROM dbo.PayMethod WHERE Name = @Name AND IsDeleted = 0";

        public const string PAY_METHOD_FIND_BY_NAME_AND_ID = @"SELECT * FROM dbo.PayMethod WHERE Name = @Name AND Id <> @Id AND IsDeleted = 0";

        #endregion

        #region InvoiceStatus

        public const string INVOICE_STATUS_GET_BY_PAGING = @"  SELECT *
                                                         FROM dbo.InvoiceStatus 
                                                         WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                             AND IsDeleted = 0
                                                        ORDER BY {0} {1}
                                                        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;

                                                        SELECT COUNT(*)
                                                        FROM dbo.InvoiceStatus 
                                                        WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                            AND IsDeleted = 0;";

        public const string INVOICE_STATUS_GET_BY_ID = @"SELECT * FROM dbo.InvoiceStatus WHERE Id = @Id AND IsDeleted = 0";

        public const string INVOICE_STATUS_GET_ALL = "SELECT Id, Name FROM dbo.InvoiceStatus WHERE IsDeleted = 0";

        public const string INVOICE_STATUS_INSERT = @"INSERT dbo.InvoiceStatus
                                                        (Name,
                                                         Color,
                                                         IsSendSms,
                                                         SmsContent,
                                                         UserCreated,
                                                         DateCreated,
                                                         UserUpdated,
                                                         DateUpdated,
                                                         IsDeleted)
                                                 VALUES (@Name,
                                                         @Color,
                                                         @IsSendSms,
                                                         @SmsContent,
                                                         @UserCreated,
                                                         @DateCreated,
                                                         @UserUpdated,
                                                         @DateUpdated,
                                                         0);

                                                 SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string INVOICE_STATUS_UPDATE = @"  UPDATE dbo.InvoiceStatus
                                                  SET Name = @Name,
                                                      Color = @Color,
                                                      IsSendSms = @IsSendSms,
                                                      SmsContent = @SmsContent,
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string INVOICE_STATUS_DELETE = @"  UPDATE dbo.InvoiceStatus
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        public const string INVOICE_STATUS_FIND_BY_NAME = @"SELECT * FROM dbo.InvoiceStatus WHERE Name = @Name AND IsDeleted = 0";

        public const string INVOICE_STATUS_FIND_BY_NAME_AND_ID = @"SELECT * FROM dbo.InvoiceStatus WHERE Name = @Name AND Id <> @Id AND IsDeleted = 0";

        #endregion
    }
}
