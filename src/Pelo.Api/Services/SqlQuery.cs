namespace Pelo.Api.Services
{
    public class SqlQuery
    {
        #region Branch

        public const string BRANCH_GET_ALL = @"SELECT Id,
                                                      Name
                                               FROM dbo.Branch
                                               WHERE IsDeleted = 0;";

        public const string BRANCH_GET_BY_ID = @"SELECT * FROM dbo.Branch WHERE Id = @Id AND IsDeleted = 0";

        public const string BRANCH_INSERT = @"INSERT dbo.Branch
                                                        (Name,
                                                         Hotline,
                                                         ProvinceId,
                                                         DistrictId,                                                         
                                                         WardId,
                                                         Address,
                                                         UserCreated,
                                                         DateCreated,
                                                         UserUpdated,
                                                         DateUpdated,
                                                         IsDeleted)
                                                 VALUES (@Name,
                                                         @Hotline,
                                                         @ProvinceId,
                                                         @DistrictId,
                                                         @WardId,
                                                         @Address,
                                                         @UserCreated,
                                                         @DateCreated,
                                                         @UserUpdated,
                                                         @DateUpdated,
                                                         0);
                                                 SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string BRANCH_UPDATE = @"  UPDATE dbo.Branch
                                                  SET Name = @Name,
                                                      ProvinceId = @ProvinceId,
                                                      DistrictId = @DistrictId,
                                                      Hotline = @Hotline,
                                                      WardId = @WardId,
                                                      Address = @Address,
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string BRANCH_DELETE = @"  UPDATE dbo.Branch
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        public const string BRANCH_PAGING= @"SELECT c.Id,
                                                    c.Name,
                                                    c.Hotline,
                                                    p.Type+' '+p.Name AS Province,
                                                    d.Type+' '+d.Name AS District,
                                                    w.Type+' '+w.Name AS Ward,
                                                    c.Address,
                                                    c.DateUpdated
                                             FROM dbo.Branch c
                                                 LEFT JOIN dbo.Province p
                                                     ON p.Id = c.ProvinceId
                                                 LEFT JOIN dbo.District d
                                                     ON d.Id = c.DistrictId
                                                 LEFT JOIN dbo.Ward w
                                                     ON w.Id = c.WardId
                                             WHERE ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @Name COLLATE Latin1_General_CI_AI
                                                   AND ISNULL(c.Hotline, '') LIKE @Hotline
                                                   AND
                                                   (
                                                       ISNULL(@ProvinceId, 0) = 0
                                                       OR ISNULL(c.ProvinceId, 0) = @ProvinceId
                                                   )
                                                   AND
                                                   (
                                                       ISNULL(@DistrictId, 0) = 0
                                                       OR ISNULL(c.DistrictId, 0) = @DistrictId
                                                   )
                                                   AND
                                                   (
                                                       ISNULL(@WardId, 0) = 0
                                                       OR ISNULL(c.WardId, 0) = @WardId
                                                   )
                                                   AND c.IsDeleted = 0
                                             ORDER BY {0} {1} OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
                     
                                             SELECT COUNT(*)
                                             FROM dbo.Branch c
                                                 LEFT JOIN dbo.Province p
                                                     ON p.Id = c.ProvinceId
                                                 LEFT JOIN dbo.District d
                                                     ON d.Id = c.DistrictId
                                                 LEFT JOIN dbo.Ward w
                                                     ON w.Id = c.WardId
                                             WHERE ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @Name COLLATE Latin1_General_CI_AI
                                                   AND ISNULL(c.Hotline, '') LIKE @Hotline
                                                   AND
                                                   (
                                                       ISNULL(@ProvinceId, 0) = 0
                                                       OR ISNULL(c.ProvinceId, 0) = @ProvinceId
                                                   )
                                                   AND
                                                   (
                                                       ISNULL(@DistrictId, 0) = 0
                                                       OR ISNULL(c.DistrictId, 0) = @DistrictId
                                                   )
                                                   AND
                                                   (
                                                       ISNULL(@WardId, 0) = 0
                                                       OR ISNULL(c.WardId, 0) = @WardId
                                                   )
                                                   AND c.IsDeleted = 0;";


        #endregion

        #region Department

        public const string DEPARTMENT_GET_ALL = @"SELECT Id,
                                                          Name
                                                   FROM dbo.Department
                                                   WHERE IsDeleted = 0;";

        public const string DEPARTMENT_GET_BY_ID = @"SELECT * FROM dbo.Department WHERE Id = @Id AND IsDeleted = 0";

        public const string DEPARTMENT_INSERT = @"INSERT dbo.Department
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

        public const string DEPARTMENT_UPDATE = @"  UPDATE dbo.Department
                                                  SET Name = @Name,
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string DEPARTMENT_DELETE = @"  UPDATE dbo.Department
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        public const string DEPARTMENT_PAGING = @"SELECT c.Id,
                                                         c.Name
                                                 FROM dbo.Department c
                                                 WHERE ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @Name COLLATE Latin1_General_CI_AI
                                                       AND c.IsDeleted = 0
                                                 ORDER BY {0} {1} OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
                                                 SELECT COUNT(*)
                                                 FROM dbo.Department c
                                                 WHERE ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @Name COLLATE Latin1_General_CI_AI
                                                       AND c.IsDeleted = 0;";

        #endregion
        #region Manufacturer

        public const string MANUFACTURER_GET_ALL = @"SELECT Id,
                                                          Name
                                                   FROM dbo.Manufacturer
                                                   WHERE IsDeleted = 0;";

        public const string MANUFACTURER_GET_BY_ID = @"SELECT * FROM dbo.Manufacturer WHERE Id = @Id AND IsDeleted = 0";

        public const string MANUFACTURER_INSERT = @"INSERT dbo.Manufacturer
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

        public const string MANUFACTURER_UPDATE = @"  UPDATE dbo.Manufacturer
                                                  SET Name = @Name,
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string MANUFACTURER_DELETE = @"  UPDATE dbo.Manufacturer
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        public const string MANUFACTURER_PAGING = @"SELECT c.Id,
                                                              c.Name,
                                                              c.DateUpdated
                                                       FROM dbo.Manufacturer c
                                                       WHERE ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @Name COLLATE Latin1_General_CI_AI
                                                             AND c.IsDeleted = 0
                                                       ORDER BY {0} {1} OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
                                                       SELECT COUNT(*)
                                                       FROM dbo.Manufacturer c
                                                       WHERE ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @Name COLLATE Latin1_General_CI_AI
                                                             AND c.IsDeleted = 0;";

        #endregion
        #region Province

        public const string PROVINCE_GET_ALL = @"SELECT Id,
                                                        Type,
                                                        Name
                                                 FROM dbo.Province
                                                 WHERE IsDeleted = 0
                                                 ORDER BY SortOrder;";

        public const string PROVINCE_GET_BY_ID = @"SELECT * FROM dbo.Province WHERE Id = @Id AND IsDeleted = 0";

        public const string PROVINCE_INSERT = @"INSERT dbo.Province
                                                        (Type,
                                                         Name,
                                                         SortOrder,                                                         
                                                         UserCreated,
                                                         DateCreated,
                                                         UserUpdated,
                                                         DateUpdated,
                                                         IsDeleted)
                                                 VALUES (@Type,
                                                         @Name,
                                                         @SortOrder,                                                         
                                                         @UserCreated,
                                                         @DateCreated,
                                                         @UserUpdated,
                                                         @DateUpdated,
                                                         0);
                                                 SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string PROVINCE_UPDATE = @"  UPDATE dbo.Province
                                                  SET Name = @Name,
                                                      Type = @Type,
                                                      SortOrder = @SortOrder,                                                         
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string PROVINCE_DELETE = @"  UPDATE dbo.Province
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        public const string PROVINCE_PAGING = @"SELECT c.Id,
                                                              c.Name,
                                                              c.Type,
                                                              c.SortOrder,
                                                              c.DateUpdated
                                                       FROM dbo.Province c
                                                       WHERE ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @Name COLLATE Latin1_General_CI_AI
                                                             AND c.IsDeleted = 0
                                                       ORDER BY {0} {1} OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
                                                    SELECT COUNT(*) 
                                                    FROM dbo.Province c
                                                       WHERE ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @Name COLLATE Latin1_General_CI_AI
                                                             AND c.IsDeleted = 0;";

        #endregion

        #region District

        public const string DISTRICT_GET_ALL = @"SELECT Id,
                                                        Type,
                                                        Name
                                                 FROM dbo.District
                                                 WHERE IsDeleted = 0
                                                       AND ProvinceId = @ProvinceId
                                                 ORDER BY SortOrder;";

        public const string DISTRICT_GET_BY_ID = @"SELECT * FROM dbo.District WHERE Id = @Id AND IsDeleted = 0";

        public const string DISTRICT_INSERT = @"INSERT dbo.District
                                                        (Type,
                                                         Name,
                                                         ProvinceId,                                                         
                                                         SortOrder,                                                         
                                                         UserCreated,
                                                         DateCreated,
                                                         UserUpdated,
                                                         DateUpdated,
                                                         IsDeleted)
                                                 VALUES (@Type,
                                                         @Name,
                                                         @ProvinceId,
                                                         @SortOrder,                                                         
                                                         @UserCreated,
                                                         @DateCreated,
                                                         @UserUpdated,
                                                         @DateUpdated,
                                                         0);
                                                 SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string DISTRICT_UPDATE = @"  UPDATE dbo.District
                                                  SET Name = @Name,
                                                      Type = @Type,
                                                      ProvinceId = @ProvinceId,
                                                      SortOrder = @SortOrder,                                                         
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string DISTRICT_DELETE = @"  UPDATE dbo.District
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        public const string DISTRICT_PAGING = @"SELECT c.Id,
                                                       c.Name,
                                                       c.Type,
                                                       c.SortOrder,
                                                       p.Type+' '+p.Name AS Province,
                                                       c.DateUpdated
                                                FROM dbo.District c
                                                    LEFT JOIN dbo.Province p
                                                        ON p.Id = c.ProvinceId
                                                WHERE ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @Name COLLATE Latin1_General_CI_AI
                                                      AND
                                                      (
                                                          @ProvinceId = 0
                                                          OR ISNULL(c.ProvinceId, 0) = @ProvinceId
                                                      )
                                                      AND c.IsDeleted = 0
                                                ORDER BY p.SortOrder, c.SortOrder OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;

                                                SELECT COUNT(*)
                                                FROM dbo.District c
                                                    LEFT JOIN dbo.Province p
                                                        ON p.Id = c.ProvinceId
                                                WHERE ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @Name COLLATE Latin1_General_CI_AI
                                                      AND
                                                      (
                                                          @ProvinceId = 0
                                                          OR ISNULL(c.ProvinceId, 0) = @ProvinceId
                                                      )
                                                      AND c.IsDeleted = 0;";

        #endregion

        #region Ward

        public const string WARD_GET_ALL = @"SELECT Id,
                                                    Type,
                                                    Name
                                             FROM dbo.Ward
                                             WHERE IsDeleted = 0
                                                   AND DistrictId = @DistrictId
                                             ORDER BY SortOrder;";

        public const string WARD_GET_BY_ID = @"SELECT * FROM dbo.Ward WHERE Id = @Id AND IsDeleted = 0";

        public const string WARD_INSERT = @"INSERT dbo.Ward
                                                        (Type,
                                                         Name,
                                                         DistrictId,                                                         
                                                         SortOrder,                                                         
                                                         UserCreated,
                                                         DateCreated,
                                                         UserUpdated,
                                                         DateUpdated,
                                                         IsDeleted)
                                                 VALUES (@Type,
                                                         @Name,
                                                         @DistrictId,
                                                         @SortOrder,                                                         
                                                         @UserCreated,
                                                         @DateCreated,
                                                         @UserUpdated,
                                                         @DateUpdated,
                                                         0);
                                                 SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string WARD_UPDATE = @"  UPDATE dbo.Ward
                                                  SET Name = @Name,
                                                      Type = @Type,
                                                      DistrictId = @DistrictId,
                                                      SortOrder = @SortOrder,                                                         
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string WARD_DELETE = @"  UPDATE dbo.Ward
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        public const string WARD_PAGING = @"SELECT c.Id,
                                                   c.Name,
                                                   c.Type,
                                                   c.SortOrder,
                                                   d.Type + ' ' + d.Name AS District,
                                                   p.Type + ' ' + p.Name AS Province,
                                                   c.DateUpdated
                                            FROM dbo.Ward c
                                                LEFT JOIN dbo.District d
                                                    ON d.Id = c.DistrictId
                                                LEFT JOIN dbo.Province p
                                                    ON p.Id = c.ProvinceId
                                            WHERE ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @Name COLLATE Latin1_General_CI_AI
                                                  AND
                                                  (
                                                      @ProvinceId = 0
                                                      OR
                                                      (
                                                          @ProvinceId > 0
                                                          AND @DistrictId = 0
                                                          AND c.ProvinceId = @ProvinceId
                                                      )
                                                      OR
                                                      (
                                                          @ProvinceId > 0
                                                          AND @DistrictId > 0
                                                          AND c.DistrictId = @DistrictId
                                                      )
                                                  )
                                                  AND c.IsDeleted = 0
                                            ORDER BY p.SortOrder,
                                                     d.SortOrder,
                                                     c.SortOrder OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
                                            SELECT COUNT(*)
                                            FROM dbo.Ward c
                                                LEFT JOIN dbo.District d
                                                    ON d.Id = c.DistrictId
                                                LEFT JOIN dbo.Province p
                                                    ON p.Id = c.ProvinceId
                                            WHERE ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @Name COLLATE Latin1_General_CI_AI
                                                  AND
                                                  (
                                                      @ProvinceId = 0
                                                      OR
                                                      (
                                                          @ProvinceId > 0
                                                          AND @DistrictId = 0
                                                          AND c.ProvinceId = @ProvinceId
                                                      )
                                                      OR
                                                      (
                                                          @ProvinceId > 0
                                                          AND @DistrictId > 0
                                                          AND c.DistrictId = @DistrictId
                                                      )
                                                  )
                                                  AND c.IsDeleted = 0;";

        #endregion

        #region CrmType

        public const string CRM_TYPE_GET_ALL = @"SELECT Id,
                                                        Name
                                                 FROM dbo.CrmType
                                                 WHERE IsDeleted = 0;";

        public const string CRM_TYPE_GET_BY_PAGING = @"  SELECT *
                                                         FROM dbo.CrmType 
                                                         WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                             AND IsDeleted = 0
                                                        ORDER BY {0} {1}
                                                        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;

                                                        SELECT COUNT(*)
                                                        FROM dbo.CrmType 
                                                        WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                            AND IsDeleted = 0;";

        public const string CRM_TYPE_GET_BY_ID = @"SELECT * FROM dbo.CrmType WHERE Id = @Id AND IsDeleted = 0";


        public const string CRM_TYPE_INSERT = @"INSERT dbo.CrmType
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

        public const string CRM_TYPE_UPDATE = @"  UPDATE dbo.CrmType
                                                  SET Name = @Name,
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string CRM_TYPE_DELETE = @"  UPDATE dbo.CrmType
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        #endregion

        #region CrmStatus

        public const string CRM_STATUS_GET_ALL = @"SELECT Id,
                                                          Name
                                                   FROM dbo.CrmStatus
                                                   WHERE IsDeleted = 0
                                                   ORDER BY Id;";

        public const string CRM_STATUS_GET_BY_PAGING = @"  SELECT *
                                                         FROM dbo.CrmStatus 
                                                         WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                             AND IsDeleted = 0
                                                        ORDER BY {0} {1}
                                                        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;

                                                        SELECT COUNT(*)
                                                        FROM dbo.CrmStatus 
                                                        WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                            AND IsDeleted = 0;";

        public const string CRM_STATUS_GET_BY_ID = @"SELECT * FROM dbo.CrmStatus WHERE Id = @Id AND IsDeleted = 0";


        public const string CRM_STATUS_INSERT = @"INSERT dbo.CrmStatus
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

        public const string CRM_STATUS_UPDATE = @"  UPDATE dbo.CrmStatus
                                                  SET Name = @Name,
                                                      Color = @Color,
                                                      IsSendSms = @IsSendSms,
                                                      SmsContent = @SmsContent,
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string CRM_STATUS_DELETE = @"  UPDATE dbo.CrmStatus
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        #endregion

        #region CustomerSource

        public const string CUSTOMER_SOURCE_GET_ALL = @"SELECT Id,
                                                               Name
                                                        FROM dbo.CustomerSource
                                                        WHERE IsDeleted = 0
                                                        ORDER BY Id;";

        public const string CUSTOMER_SOURCE_GET_BY_PAGING = @"  SELECT *
                                                         FROM dbo.CustomerSource 
                                                         WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                             AND IsDeleted = 0
                                                        ORDER BY {0} {1}
                                                        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;

                                                        SELECT COUNT(*)
                                                        FROM dbo.CustomerSource 
                                                        WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                            AND IsDeleted = 0;";

        public const string CUSTOMER_SOURCE_GET_BY_ID = @"SELECT * FROM dbo.CustomerSource WHERE Id = @Id AND IsDeleted = 0";


        public const string CUSTOMER_SOURCE_INSERT = @"INSERT dbo.CustomerSource
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

        public const string CUSTOMER_SOURCE_UPDATE = @"  UPDATE dbo.CustomerSource
                                                  SET Name = @Name,
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string CUSTOMER_SOURCE_DELETE = @"  UPDATE dbo.CustomerSource
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        #endregion

        #region ProductGroup

        public const string PRODUCT_GROUP_GET_ALL = @"SELECT Id,
                                                                Name
                                                         FROM dbo.ProductGroup
                                                         WHERE IsDeleted = 0
                                                         ORDER BY Id;";

        public const string PRODUCT_GROUP_GET_BY_ID = @"SELECT * FROM dbo.ProductGroup WHERE Id = @Id AND IsDeleted = 0";

        public const string PRODUCT_GROUP_INSERT = @"INSERT dbo.ProductGroup
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

        public const string PRODUCT_GROUP_UPDATE = @"  UPDATE dbo.ProductGroup
                                                  SET Name = @Name,
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string PRODUCT_GROUP_DELETE = @"  UPDATE dbo.ProductGroup
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        public const string PRODUCT_GROUP_PAGING = @"SELECT c.Id,
                                                              c.Name,
                                                              c.DateUpdated
                                                       FROM dbo.ProductGroup c
                                                       WHERE ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @Name COLLATE Latin1_General_CI_AI
                                                             AND c.IsDeleted = 0
                                                       ORDER BY {0} {1} OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
SELECT COUNT(*) 
FROM dbo.ProductGroup c
                                                       WHERE ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @Name COLLATE Latin1_General_CI_AI
                                                             AND c.IsDeleted = 0";

        #endregion

        #region ProductUnit

        public const string PRODUCT_UNIT_GET_ALL = @"SELECT Id,
                                                            Name
                                                     FROM dbo.ProductUnit
                                                     WHERE IsDeleted = 0
                                                     ORDER BY Id;";

        public const string PRODUCT_UNIT_GET_BY_ID = @"SELECT * FROM dbo.ProductUnit WHERE Id = @Id AND IsDeleted = 0";

        public const string PRODUCT_UNIT_INSERT = @"INSERT dbo.ProductUnit
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

        public const string PRODUCT_UNIT_UPDATE = @"  UPDATE dbo.ProductUnit
                                                  SET Name = @Name,
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string PRODUCT_UNIT_DELETE = @"  UPDATE dbo.ProductUnit
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        public const string PRODUCT_UNIT_PAGING = @"SELECT c.Id,
                                                              c.Name,
                                                              c.DateUpdated
                                                       FROM dbo.ProductUnit c
                                                       WHERE ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @Name COLLATE Latin1_General_CI_AI
                                                             AND c.IsDeleted = 0
                                                       ORDER BY {0} {1} OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
SELECT COUNT(*) FROM dbo.ProductUnit c
                                                       WHERE ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @Name COLLATE Latin1_General_CI_AI
                                                             AND c.IsDeleted = 0";

        #endregion

        #region ProductStatus

        public const string PRODUCT_STATUS_GET_ALL = @"SELECT Id,
                                                            Name
                                                     FROM dbo.ProductStatus
                                                     WHERE IsDeleted = 0
                                                     ORDER BY Id;";

        public const string PRODUCT_STATUS_GET_BY_ID = @"SELECT * FROM dbo.ProductStatus WHERE Id = @Id AND IsDeleted = 0";

        public const string PRODUCT_STATUS_INSERT = @"INSERT dbo.ProductStatus
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

        public const string PRODUCT_STATUS_UPDATE = @"  UPDATE dbo.ProductStatus
                                                  SET Name = @Name,
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string PRODUCT_STATUS_DELETE = @"  UPDATE dbo.ProductStatus
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        public const string PRODUCT_STATUS_PAGING = @"SELECT c.Id,
                                                              c.Name,
                                                              c.DateUpdated
                                                       FROM dbo.ProductStatus c
                                                       WHERE ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @Name COLLATE Latin1_General_CI_AI
                                                             AND c.IsDeleted = 0
                                                       ORDER BY {0} {1} OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
SELECT COUNT(*) FROM dbo.ProductStatus c
                                                       WHERE ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @Name COLLATE Latin1_General_CI_AI
                                                             AND c.IsDeleted = 0";

        #endregion
        
        #region CrmPriority

        public const string CRM_PRIORITY_GET_ALL = @"SELECT Id,
                                                            Name
                                                     FROM dbo.CrmPriority
                                                     WHERE IsDeleted = 0
                                                     ORDER BY Id;";

        public const string CRM_PRIORITY_INSERT = @"INSERT dbo.CrmPriority
                                                        (Name,
                                                         Color,
                                                         UserCreated,
                                                         DateCreated,
                                                         UserUpdated,
                                                         DateUpdated,
                                                         IsDeleted)
                                                 VALUES (@Name,
                                                         @Color,
                                                         @UserCreated,
                                                         @DateCreated,
                                                         @UserUpdated,
                                                         @DateUpdated,
                                                         0);

                                                 SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string CRM_PRIORITY_UPDATE = @"  UPDATE dbo.CrmPriority
                                                  SET Name = @Name,
                                                      Color = @Color,
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string CRM_PRIORITY_DELETE = @"  UPDATE dbo.CrmPriority
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        public const string CRM_PRIORITY_GET_BY_PAGING = @"  SELECT *
                                                         FROM dbo.CrmPriority 
                                                         WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                             AND IsDeleted = 0
                                                        ORDER BY {0} {1}
                                                        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;

                                                        SELECT COUNT(*)
                                                        FROM dbo.CrmPriority 
                                                        WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                            AND IsDeleted = 0;";

        public const string CRM_PRIORITY_GET_BY_ID = @"SELECT * FROM dbo.InvoiceStatus WHERE Id = @Id AND IsDeleted = 0";

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
                                                    DisplayName,
                                                    PhoneNumber
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

        public const string ROLE_GET_BY_ID = @"SELECT * FROM dbo.Role WHERE Id = @Id AND IsDeleted = 0";

        public const string ROLE_INSERT = @"INSERT dbo.Role
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

        public const string ROLE_UPDATE = @"  UPDATE dbo.Role
                                                  SET Name = @Name,
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string ROLE_DELETE = @"  UPDATE dbo.Role
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        public const string ROLE_PAGING = @"SELECT c.Id,
                                                              c.Name,
                                                              c.DateUpdated
                                                       FROM dbo.Role c
                                                       WHERE ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @Name COLLATE Latin1_General_CI_AI
                                                             AND c.IsDeleted = 0
                                                       ORDER BY {0} {1} OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
SELECT COUNT(*) FROM dbo.Role c
                                                       WHERE ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @Name COLLATE Latin1_General_CI_AI
                                                             AND c.IsDeleted = 0";

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
                                                              p.Type+' '+p.Name AS Province,
                                                              d.Type+' '+d.Name AS District,
                                                              w.Type+' '+w.Name AS Ward,
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
                                                             u.DisplayName AS UserCreated,
                                                             c.DateCreated
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
                                                          LEFT JOIN dbo.[User] u
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

        public const string CRM_USER_UPDATE= @"UPDATE dbo.CrmUser SET UserId = @UserId, UserUpdated=@UserUpdated, DateUpdated=@DateUpdated WHERE Id = @Id AND IsDeleted=0";

        public const string CRM_USER_DELETE = @"UPDATE dbo.CrmUser SET IsDeleted = 1 WHERE CrmId = @CrmId AND UserId=@UserId";

        public const string GET_CRM_USER_BY_CRMID= @"SELECT * from dbo.CrmUser where CrmId = @CrmId AND IsDeleted=0";

        public const string CRM_COUNT_BY_DATE = @"SELECT COUNT(*) FROM dbo.Crm WHERE Code LIKE @Code";

        /// <summary>
        ///     Lấy danh sách CRM ko có điều kiện UserCareId
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
                                                         c.CustomerId,
                                                         cs.Name AS CrmStatus,
                                                         cs.Color AS CrmStatusColor,
                                                         cu.Name AS CustomerName,                                                         
                                                         cu.Phone AS CustomerPhone,
                                                         cu.Phone2 AS CustomerPhone2,
                                                         cu.Phone3 AS CustomerPhone3,
                                                         cu.Address AS CustomerAddress,
                                                         p.Type+' '+p.Name AS Province,
                                                         d.Type+' '+d.Name AS District,
                                                         w.Type+' '+w.Name AS Ward,
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
        ///     Lấy danh sách CRM có điều kiện UserCareId
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
                                                           c.CustomerId,
                                                           cs.Name AS CrmStatus,
                                                           cs.Color AS CrmStatusColor,
                                                           cu.Name AS CustomerName,
                                                           cu.Phone AS CustomerPhone,
                                                           cu.Phone2 AS CustomerPhone2,
                                                           cu.Phone3 AS CustomerPhone3,
                                                           cu.Address AS CustomerAddress,
                                                           p.Type+' '+p.Name AS Province,
                                                           d.Type+' '+d.Name AS District,
                                                           w.Type+' '+w.Name AS Ward,
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

        public const string CRM_KHACH_CHUA_XU_LY = @"DROP TABLE IF EXISTS #tmpCrm;

                                                     SELECT c.Id
                                                     INTO #tmpCrm
                                                     FROM dbo.Crm c
                                                         LEFT JOIN dbo.Customer cu
                                                             ON cu.Id = c.CustomerId
                                                         INNER JOIN dbo.CrmUser cru
                                                             ON cru.CrmId = c.Id
                                                     WHERE c.CrmStatusId = @CrmStatusId
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
                                                           AND cru.UserId = @UserCareId
                                                           AND cru.IsDeleted = 0
                                                           AND c.IsDeleted = 0;
  
                                                     SELECT c.Id,
                                                            c.Code,
                                                            c.CustomerId,
                                                            cs.Name AS CrmStatus,
                                                            cs.Color AS CrmStatusColor,
                                                            cu.Name AS CustomerName,
                                                            cu.Phone AS CustomerPhone,
                                                            cu.Phone2 AS CustomerPhone2,
                                                            cu.Phone3 AS CustomerPhone3,
                                                            cu.Address AS CustomerAddress,
                                                            p.Type + ' ' + p.Name AS Province,
                                                            d.Type + ' ' + d.Name AS District,
                                                            w.Type + ' ' + w.Name AS Ward,
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

        public const string CRM_KHACH_TOI_HEN_CAN_CHAM_SOC = @"DROP TABLE IF EXISTS #tmpCrm;

                                                               SELECT c.Id
                                                               INTO #tmpCrm
                                                               FROM dbo.Crm c
                                                                   LEFT JOIN dbo.Customer cu
                                                                       ON cu.Id = c.CustomerId
                                                                   INNER JOIN dbo.CrmUser cru
                                                                       ON cru.CrmId = c.Id
                                                               WHERE c.CrmStatusId <> 1
                                                                     AND c.CrmStatusId <> 2
                                                                     AND c.CrmStatusId <> 5
                                                                     AND c.CrmStatusId <> 7
                                                                     AND c.CrmStatusId <> 8
                                                                     AND c.CrmStatusId <> 9
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
                                                                     AND cru.UserId = @UserCareId
                                                                     AND cru.IsDeleted = 0
                                                                     AND c.IsDeleted = 0;

                                                               SELECT c.Id,
                                                                      c.Code,
                                                                      c.CustomerId,
                                                                      cs.Name AS CrmStatus,
                                                                      cs.Color AS CrmStatusColor,
                                                                      cu.Name AS CustomerName,
                                                                      cu.Phone AS CustomerPhone,
                                                                      cu.Phone2 AS CustomerPhone2,
                                                                      cu.Phone3 AS CustomerPhone3,
                                                                      cu.Address AS CustomerAddress,
                                                                      p.Type + ' ' + p.Name AS Province,
                                                                      d.Type + ' ' + d.Name AS District,
                                                                      w.Type + ' ' + w.Name AS Ward,
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

        public const string CRM_KHACH_QUA_HEN_CHAM_SOC = @"DROP TABLE IF EXISTS #tmpCrm;

                                                           SELECT c.Id
                                                           INTO #tmpCrm
                                                           FROM dbo.Crm c
                                                               LEFT JOIN dbo.Customer cu
                                                                   ON cu.Id = c.CustomerId
                                                               INNER JOIN dbo.CrmUser cru
                                                                   ON cru.CrmId = c.Id
                                                           WHERE c.CrmStatusId <> 5
                                                                 AND c.CrmStatusId <> 7
                                                                 AND c.CrmStatusId <> 8
                                                                 AND c.CrmStatusId <> 9
                                                                 AND
                                                                 (
                                                                     @FromDate IS NULL
                                                                     OR c.ContactDate >= @ToDate
                                                                 )
                                                                 AND
                                                                 (
                                                                     @ToDate IS NULL
                                                                     OR c.ContactDate <= @ToDate
                                                                 )
                                                                 AND cru.UserId = @UserCareId
                                                                 AND cru.IsDeleted = 0
                                                                 AND c.IsDeleted = 0;
                               
                                                           SELECT c.Id,
                                                                  c.Code,
                                                                  c.CustomerId,
                                                                  cs.Name AS CrmStatus,
                                                                  cs.Color AS CrmStatusColor,
                                                                  cu.Name AS CustomerName,
                                                                  cu.Phone AS CustomerPhone,
                                                                  cu.Phone2 AS CustomerPhone2,
                                                                  cu.Phone3 AS CustomerPhone3,
                                                                  cu.Address AS CustomerAddress,
                                                                  p.Type + ' ' + p.Name AS Province,
                                                                  d.Type + ' ' + d.Name AS District,
                                                                  w.Type + ' ' + w.Name AS Ward,
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
                                                                   u.PhoneNumber,
                                                                   u.Id
                                                            FROM dbo.CrmUser cu
                                                                INNER JOIN dbo.[User] u
                                                                    ON cu.UserId = u.Id
                                                            WHERE cu.CrmId = @CrmId
                                                                  AND cu.Type = 0
                                                                  AND cu.IsDeleted = 0
                                                                  AND u.IsDeleted = 0";

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

        public const string CRM_UPDATE = @"UPDATE dbo.Crm
                                            SET ContactDate = @ContactDate,
                                                ProductGroupId = @ProductGroupId,
                                                CrmPriorityId = @CrmPriorityId,
                                                CrmTypeId = @CrmTypeId,
                                                Need = @Need,
                                                Description = @Description,
                                                CustomerSourceId = @CustomerSourceId,
                                                Visit = @Visit,
                                                UserUpdated = @UserUpdated,
                                                DateUpdated = @DateUpdated
                                               WHERE Id = @Id";
        public const string STATUS_CRM_UPDATE = @"UPDATE dbo.Crm
                                            SET ContactDate = @ContactDate,
                                                ProductGroupId = @ProductGroupId,
                                                CrmPriorityId = @CrmPriorityId,
                                                CrmTypeId = @CrmTypeId,
                                                Need = @Need,
                                                Description = @Description,
                                                CustomerSourceId = @CustomerSourceId,
                                                Visit = @Visit,
                                                UserUpdated = @UserUpdated,
                                                DateUpdated = @DateUpdated
                                               WHERE Id = @Id";
        public const string CRM_COMMENT_INSERT= @"INSERT dbo.CrmComment
                                                        (
                                                            CrmId,
                                                            Comment,
                                                            Type,
                                                            UserIds,
                                                            OldStatusId,
                                                            NewStatusId,
                                                            UserCreated,
                                                            DateCreated,
                                                            UserUpdated
                                                        )
                                                        VALUES
                                                        (   @CrmId,
                                                            @Comment,
                                                            @Type,
                                                            @OldStatusId,
                                                            @NewStatusId,
                                                            @UserIds,
                                                            @UserCreated,
                                                            @DateCreated,
                                                            @UserUpdated;
                                                        SELECT CAST(SCOPE_IDENTITY() as int);";
        public const string GET_CRM_BY_ID = @"SELECT c.Id,
		                                        c.Code,
		                                        c.CustomerId,
		                                        c.CrmStatusId,
		                                        cs.Color AS CrmStatusColor,
		                                        cu.Name AS CustomerName,
		                                        cu.Phone AS CustomerPhone,
		                                        cu.Address AS CustomerAddress,                                                           
		                                        cg.Name AS CustomerGroup,
		                                        cv.Name AS CustomerVip,
		                                        c.Need,
		                                        c.Description,
		                                        pg.Id AS ProductGroupId,
		                                        cp.Id AS CrmPriorityId,
		                                        cus.Id AS CustomerSourceId,
		                                        ct.Id AS CrmTypeId,
		                                        c.Visit,
		                                        u.DisplayName AS UserCreated,
		                                        u.PhoneNumber AS UserCreatedPhone,
		                                        c.ContactDate,
		                                        c.DateCreated,
		                                        c.DateUpdated
		                                        from dbo.Crm c
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
                                                    WHERE c.Id = @Id AND c.IsDeleted = 0";
        /// <summary>
        ///     Lấy danh sách CRM của khách hàng đối với những user được quyền xem tất cả CRM
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
                                                              c.CustomerId,
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
        ///     Lấy danh sách CRM của khách hàng đối với những user không có quyền xem hết tất cả CRM
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
                                                                c.CustomerId,
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

        #region Invoice

        public const string INVOICE_GET_BY_CUSTOMER_ID = @"SELECT i.Id,
                                                                  ins.Name AS InvoiceStatus,
                                                                  ins.Color AS InvoiceStatusColor,
                                                                  i.Code,
                                                                  c.Name AS CustomerName,
                                                                  c.Phone AS CustomerPhone,
                                                                  c.Phone2 AS CustomerPhone2,
                                                                  c.Phone3 AS CustomerPhone3,
                                                                  p.Type + ' ' + p.Name AS Province,
                                                                  d.Type + ' ' + d.Name AS District,
                                                                  w.Type + ' ' + w.Name AS Ward,
                                                                  c.Address AS CustomerAddress,
                                                                  c.Code AS CustomerCode,
                                                                  b.Name AS Branch,
                                                                  u2.DisplayName AS UserCreated,
                                                                  u2.PhoneNumber AS UserCreatedPhone,
                                                                  u.DisplayName AS UserSell,
                                                                  u.PhoneNumber AS UserSellPhone,
                                                                  i.DeliveryDate,
                                                                  i.DateCreated
                                                           FROM dbo.Invoice i
                                                               INNER JOIN dbo.Customer c
                                                                   ON c.Id = i.CustomerId
                                                               INNER JOIN dbo.Branch b
                                                                   ON b.Id = i.BranchId
                                                               INNER JOIN dbo.InvoiceStatus ins
                                                                   ON ins.Id = i.InvoiceStatusId
                                                               INNER JOIN dbo.[User] u
                                                                   ON u.Id = i.UserSellId
                                                               INNER JOIN dbo.[User] u2
                                                                   ON u2.Id = i.UserCreated
                                                               LEFT JOIN dbo.Province p
                                                                   ON c.ProvinceId = p.Id
                                                               LEFT JOIN dbo.District d
                                                                   ON c.DistrictId = d.Id
                                                               LEFT JOIN dbo.Ward w
                                                                   ON c.WardId = c.Id
                                                           WHERE i.CustomerId = @CustomerId
                                                                 AND i.IsDeleted = 0
                                                                 AND c.IsDeleted = 0
                                                           ORDER BY i.Id DESC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
                    
                                                           SELECT COUNT(*)
                                                           FROM dbo.Invoice i
                                                               INNER JOIN dbo.Customer c
                                                                   ON c.Id = i.CustomerId
                                                           WHERE i.CustomerId = @CustomerId
                                                                 AND i.IsDeleted = 0
                                                                 AND c.IsDeleted = 0;";

        public const string INVOICE_GET_BY_CUSTOMER_ID_2 = @"SELECT i.Id,
                                                                    ins.Name AS InvoiceStatus,
                                                                    ins.Color AS InvoiceStatusColor,
                                                                    i.Code,
                                                                    c.Name AS CustomerName,
                                                                    c.Phone AS CustomerPhone,
                                                                    c.Phone2 AS CustomerPhone2,
                                                                    c.Phone3 AS CustomerPhone3,
                                                                    p.Type + ' ' + p.Name AS Province,
                                                                    d.Type + ' ' + d.Name AS District,
                                                                    w.Type + ' ' + w.Name AS Ward,
                                                                    c.Address AS CustomerAddress,
                                                                    c.Code AS CustomerCode,
                                                                    b.Name AS Branch,
                                                                    u2.DisplayName AS UserCreated,
                                                                    u2.PhoneNumber AS UserCreatedPhone,
                                                                    u.DisplayName AS UserSell,
                                                                    u.PhoneNumber AS UserSellPhone,
                                                                    i.DeliveryDate,
                                                                    i.DateCreated
                                                             FROM dbo.Invoice i
                                                                 INNER JOIN dbo.Customer c
                                                                     ON c.Id = i.CustomerId
                                                                 INNER JOIN dbo.Branch b
                                                                     ON b.Id = i.BranchId
                                                                 INNER JOIN dbo.InvoiceStatus ins
                                                                     ON ins.Id = i.InvoiceStatusId
                                                                 INNER JOIN dbo.[User] u
                                                                     ON u.Id = i.UserSellId
                                                                 INNER JOIN dbo.[User] u2
                                                                     ON u2.Id = i.UserCreated
                                                                 LEFT JOIN dbo.Province p
                                                                     ON c.ProvinceId = p.Id
                                                                 LEFT JOIN dbo.District d
                                                                     ON c.DistrictId = d.Id
                                                                 LEFT JOIN dbo.Ward w
                                                                     ON c.WardId = c.Id
                                                                 LEFT JOIN dbo.UserInInvoice uii
                                                                     ON c.Id = uii.InvoiceId
                                                             WHERE i.CustomerId = @CustomerId
                                                                   AND
                                                                   (
                                                                       i.UserSellId = @UserId
                                                                       OR i.UserCreated = @UserId
                                                                       OR uii.UserId = @UserId
                                                                   )
                                                                   AND i.IsDeleted = 0
                                                                   AND c.IsDeleted = 0
                                                             ORDER BY i.Id DESC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
                        
                                                             SELECT COUNT(*)
                                                             FROM dbo.Invoice i
                                                                 INNER JOIN dbo.Customer c
                                                                     ON c.Id = i.CustomerId
                                                                 LEFT JOIN dbo.UserInInvoice uii
                                                                     ON c.Id = uii.InvoiceId
                                                             WHERE i.CustomerId = @CustomerId
                                                                   AND
                                                                   (
                                                                       i.UserSellId = @UserId
                                                                       OR i.UserCreated = @UserId
                                                                       OR uii.UserId = @UserId
                                                                   )
                                                                   AND i.IsDeleted = 0
                                                                   AND c.IsDeleted = 0;";

        public const string INVOICE_USER_DELIVERY_GET_BY_INVOICE_ID = @"SELECT u.DisplayName,
                                                                   u.PhoneNumber
                                                            FROM dbo.UserInInvoice uii
                                                                INNER JOIN dbo.[User] u
                                                                    ON uii.UserId = u.Id
                                                            WHERE uii.InvoiceId = @InvoiceId
                                                                  AND uii.Type = 0
                                                                  AND uii.IsDeleted = 0
                                                                  AND u.IsDeleted = 0;";

        public const string PRODUCTS_IN_INVOICE_GET_BY_INVOICE_ID = @"SELECT Id,
                                                                             ProductName AS Name,
                                                                             Description
                                                                      FROM dbo.ProductInInvoice
                                                                      WHERE InvoiceId = @InvoiceId
                                                                            AND IsDeleted = 0;";

        /// <summary>
        ///     Lấy danh sách đơn hàng không có điều kiện người giao
        /// </summary>
        public const string INVOICE_GET_BY_PAGING = @"DROP TABLE IF EXISTS #tmpInvoice;

                                                      SELECT i.Id
                                                      INTO #tmpInvoice
                                                      FROM dbo.Invoice i
                                                          LEFT JOIN Customer c
                                                              ON c.Id = i.CustomerId
                                                      WHERE ISNULL(c.Code, '') LIKE @CustomerCode
                                                            AND ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @CustomerName COLLATE Latin1_General_CI_AI
                                                            AND
                                                            (
                                                                ISNULL(c.Phone, '') LIKE @CustomerPhone
                                                                OR ISNULL(c.Phone2, '') LIKE @CustomerPhone
                                                                OR ISNULL(c.Phone3, '') LIKE @CustomerPhone
                                                            )
                                                            AND i.Code LIKE @Code
                                                            AND
                                                            (
                                                                @BranchId = 0
                                                                OR ISNULL(i.BranchId, 0) = @BranchId
                                                            )
                                                            AND
                                                            (
                                                                @InvoiceStatusId = 0
                                                                OR ISNULL(i.InvoiceStatusId, 0) = @InvoiceStatusId
                                                            )
                                                            AND
                                                            (
                                                                @UserCreatedId = 0
                                                                OR i.UserCreated = @UserCreatedId
                                                            )
                                                            AND
                                                            (
                                                                @UserSellId = 0
                                                                OR i.UserSellId = @UserSellId
                                                            )
                                                            AND
                                                            (
                                                                @FromDate IS NULL
                                                                OR i.DateCreated >= @FromDate
                                                            )
                                                            AND
                                                            (
                                                                @ToDate IS NULL
                                                                OR i.DateCreated <= @ToDate
                                                            )
                                                            AND i.IsDeleted = 0
                                                            AND c.IsDeleted = 0;


                                                      SELECT i.Id,
                                                             i.Code,
                                                             ins.Name AS InvoiceStatus,
                                                             ins.Color AS InvoiceStatusColor,
                                                             c.Name AS Customer,
                                                             c.Phone AS CustomerPhone,
                                                             c.Phone2 AS CustomerPhone2,
                                                             c.Phone3 AS CustomerPhone3,
                                                             c.Address,
                                                             p.Type + ' ' + p.Name AS Province,
                                                             d.Type + ' ' + d.Name AS District,
                                                             w.Type + ' ' + w.Name AS Ward,
                                                             c.Code AS CustomerCode,
                                                             b.Name AS Branch,
                                                             u1.DisplayName AS UserCreated,
                                                             u1.PhoneNumber AS UserCreatedPhone,
                                                             u2.DisplayName AS UserSell,
                                                             u2.PhoneNumber AS UserSellPhone,
                                                             i.DeliveryDate,
                                                             i.DateCreated
                                                      FROM #tmpInvoice tmp
                                                          INNER JOIN dbo.Invoice i
                                                              ON tmp.Id = i.Id
                                                          LEFT JOIN dbo.Customer c
                                                              ON c.Id = i.CustomerId
                                                          LEFT JOIN dbo.Province p
                                                              ON p.Id = c.ProvinceId
                                                          LEFT JOIN dbo.District d
                                                              ON d.Id = c.DistrictId
                                                          LEFT JOIN dbo.Ward w
                                                              ON w.Id = c.WardId
                                                          LEFT JOIN dbo.Branch b
                                                              ON b.Id = i.BranchId
                                                          LEFT JOIN dbo.InvoiceStatus ins
                                                              ON ins.Id = i.InvoiceStatusId
                                                          LEFT JOIN dbo.[User] u1
                                                              ON u1.Id = i.UserCreated
                                                          LEFT JOIN dbo.[User] u2
                                                              ON u2.Id = i.UserSellId
                                                      ORDER BY i.DateCreated DESC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;

                                                      SELECT COUNT(*)
                                                      FROM #tmpInvoice;
                                                      DROP TABLE #tmpInvoice;";

        /// <summary>
        ///     Danh sách đơn hàng có thêm điều kiện người giao hàng
        /// </summary>
        public const string INVOICE_GET_BY_PAGING_2 = @"DROP TABLE IF EXISTS #tmpInvoice;

                                                        SELECT i.Id
                                                        INTO #tmpInvoice
                                                        FROM dbo.Invoice i
                                                            LEFT JOIN Customer c
                                                                ON c.Id = i.CustomerId
                                                        WHERE ISNULL(c.Code, '') LIKE @CustomerCode
                                                              AND ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @CustomerName COLLATE Latin1_General_CI_AI
                                                              AND
                                                              (
                                                                  ISNULL(c.Phone, '') LIKE @CustomerPhone
                                                                  OR ISNULL(c.Phone2, '') LIKE @CustomerPhone
                                                                  OR ISNULL(c.Phone3, '') LIKE @CustomerPhone
                                                              )
                                                              AND i.Code LIKE @Code
                                                              AND
                                                              (
                                                                  @BranchId = 0
                                                                  OR ISNULL(i.BranchId, 0) = @BranchId
                                                              )
                                                              AND
                                                              (
                                                                  @InvoiceStatusId = 0
                                                                  OR ISNULL(i.InvoiceStatusId, 0) = @InvoiceStatusId
                                                              )
                                                              AND
                                                              (
                                                                  @UserCreatedId = 0
                                                                  OR i.UserCreated = @UserCreatedId
                                                              )
                                                              AND
                                                              (
                                                                  @UserSellId = 0
                                                                  OR i.UserSellId = @UserSellId
                                                              )
                                                              AND
                                                              (
                                                                  @FromDate IS NULL
                                                                  OR i.DateCreated >= @FromDate
                                                              )
                                                              AND
                                                              (
                                                                  @ToDate IS NULL
                                                                  OR i.DateCreated <= @ToDate
                                                              )
                                                              AND i.IsDeleted = 0
                                                              AND c.IsDeleted = 0;
                         
                         
                                                        SELECT i.Id,
                                                               i.Code,
                                                               ins.Name AS InvoiceStatus,
                                                               ins.Color AS InvoiceStatusColor,
                                                               c.Name AS Customer,
                                                               c.Phone AS CustomerPhone,
                                                               c.Phone2 AS CustomerPhone2,
                                                               c.Phone3 AS CustomerPhone3,
                                                               c.Address,
                                                               p.Type + ' ' + p.Name AS Province,
                                                               d.Type + ' ' + d.Name AS District,
                                                               w.Type + ' ' + w.Name AS Ward,
                                                               c.Code AS CustomerCode,
                                                               b.Name AS Branch,
                                                               u1.DisplayName AS UserCreated,
                                                               u1.PhoneNumber AS UserCreatedPhone,
                                                               u2.DisplayName AS UserSell,
                                                               u2.PhoneNumber AS UserSellPhone,
                                                               i.DeliveryDate,
                                                               i.DateCreated
                                                        FROM #tmpInvoice tmp
                                                            INNER JOIN dbo.Invoice i
                                                                ON tmp.Id = i.Id
                                                            LEFT JOIN dbo.Customer c
                                                                ON c.Id = i.CustomerId
                                                            LEFT JOIN dbo.Province p
                                                                ON p.Id = c.ProvinceId
                                                            LEFT JOIN dbo.District d
                                                                ON d.Id = c.DistrictId
                                                            LEFT JOIN dbo.Ward w
                                                                ON w.Id = c.WardId
                                                            LEFT JOIN dbo.Branch b
                                                                ON b.Id = i.BranchId
                                                            LEFT JOIN dbo.InvoiceStatus ins
                                                                ON ins.Id = i.InvoiceStatusId
                                                            LEFT JOIN dbo.[User] u1
                                                                ON u1.Id = i.UserCreated
                                                            LEFT JOIN dbo.[User] u2
                                                                ON u2.Id = i.UserSellId
                                                        ORDER BY i.DateCreated DESC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
                         
                                                        SELECT COUNT(*)
                                                        FROM #tmpInvoice;
                                                        DROP TABLE #tmpInvoice;";

        public const string INVOICE_INSERT = @"INSERT dbo.Invoice
                                               (
                                                   Code,
                                                   InvoiceStatusId,
                                                   BranchId,
                                                   CustomerId,
                                                   PayMethodId,
                                                   Total,
                                                   Deposit,
                                                   DeliveryCost,
                                                   Discount,
                                                   UserSellId,
                                                   DeliveryDate,
                                                   Description,
                                                   UserCreated,
                                                   DateCreated,
                                                   UserUpdated,
                                                   DateUpdated,
                                                   IsDeleted
                                               )
                                               VALUES
                                               (   @Code,       -- Code - nvarchar(50)
                                                   @InvoiceStatusId,         -- InvoiceStatusId - int
                                                   @BranchId,         -- BranchId - int
                                                   @CustomerId,         -- CustomerId - int
                                                   @PayMethodId,         -- PayMethodId - int
                                                   @Total,         -- Total - bigint
                                                   @Deposit,         -- Deposit - bigint
                                                   @DeliveryCost,         -- DeliveryCost - int
                                                   @Discount,         -- Discount - int
                                                   @UserSellId,         -- UserSellId - int
                                                   @DeliveryDate, -- DeliveryDate - datetime
                                                   @Description,       -- Description - nvarchar(max)
                                                   @UserCreated,         -- UserCreated - int
                                                   GETDATE(), -- DateCreated - datetime
                                                   @UserUpdated,         -- UserUpdated - int
                                                   GETDATE(), -- DateUpdated - datetime
                                                   0       -- IsDeleted - bit
                                                   );

                                               SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string INVOICE_COUNT_BY_DATE = @"SELECT COUNT(*) FROM dbo.Invoice WHERE Code LIKE @Code";

        public const string PRODUCT_IN_INVOICE_INSERT = @"INSERT dbo.ProductInInvoice
                                                          (
                                                              ProductId,
                                                              InvoiceId,
                                                              Price,
                                                              Quantity,
                                                              ImportPrice,
                                                              ProductName,
                                                              Description,
                                                              UserCreated,
                                                              DateCreated,
                                                              UserUpdated,
                                                              DateUpdated,
                                                              IsDeleted
                                                          )
                                                          VALUES
                                                          (   @ProductId,         -- ProductId - int
                                                              @InvoiceId,         -- InvoiceId - int
                                                              @Price,         -- Price - int
                                                              @Quantity,         -- Quantity - int
                                                              @ImportPrice,         -- ImportPrice - int
                                                              @ProductName,       -- ProductName - nvarchar(500)
                                                              @Description,       -- Description - nvarchar(2000)
                                                              @UserCreated,         -- UserCreated - int
                                                              GETDATE(), -- DateCreated - datetime
                                                              @UserUpdated,         -- UserUpdated - int
                                                              GETDATE(), -- DateUpdated - datetime
                                                              0       -- IsDeleted - bit
                                                              )";


        #endregion

        #region Product

        public const string PRODUCT_GET_ALL = @"SELECT Id,
                                                       Name,
                                                       ImportPrice,
                                                       SellPrice
                                                FROM dbo.Product
                                                WHERE IsDeleted = 0;";

        public const string PRODUCT_GET_SIMPLE_BY_ID = @"SELECT Id,
                                                                Name,
                                                                ImportPrice,
                                                                SellPrice
                                                         FROM dbo.Product
                                                         WHERE Id = @Id
                                                               AND IsDeleted = 0;";

        public const string PRODUCT_GET_BY_ID = @"SELECT * FROM dbo.Product WHERE Id = @Id AND IsDeleted = 0";

        public const string PRODUCT_INSERT = @"INSERT dbo.Product
                                                        (Name,
                                                         ImportPrice,
                                                         SellPrice,
                                                         MinCount,
                                                         MaxCount,WarrantyMonth,ProductStatusId,ProductGroupId,ProductUnitId,ManufacturerId,CountryId,Description,
                                                         UserCreated,
                                                         DateCreated,
                                                         UserUpdated,
                                                         DateUpdated,
                                                         IsDeleted)
                                                 VALUES (@Name,
                                                         @ImportPrice,
                                                         @SellPrice,
                                                         @MinCount,
                                                         @MaxCount,@WarrantyMonth,@ProductStatusId,@ProductGroupId,@ProductUnitId,@ManufacturerId,@CountryId,@Description,
                                                         @UserCreated,
                                                         @DateCreated,
                                                         @UserUpdated,
                                                         @DateUpdated,
                                                         0);

                                                 SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string PRODUCT_UPDATE = @"  UPDATE dbo.Product
                                                  SET Name = @Name,
                                                      ImportPrice = @ImportPrice,
                                                         SellPrice=@SellPrice,
                                                         MinCount=@MinCount,
                                                         MaxCount=@MaxCount,WarrantyMonth=@WarrantyMonth,ProductStatusId=@ProductStatusId,ProductGroupId=@ProductGroupId,ProductUnitId=@ProductUnitId,ManufacturerId=@ManufacturerId,CountryId=@CountryId,Description=@Description,
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string PRODUCT_DELETE = @"  UPDATE dbo.Product
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        public const string PRODUCT_GET_BY_PAGING = @"  SELECT *
                                                         FROM dbo.Product 
                                                         WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                             AND IsDeleted = 0
                                                        ORDER BY {0} {1}
                                                        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;

                                                        SELECT COUNT(*)
                                                        FROM dbo.Product 
                                                        WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                            AND IsDeleted = 0;";

        #endregion

        #region WarrantStatus
        public const string WARRANTY_STATUS_GET_BY_PAGING = @"  SELECT *
                                                         FROM dbo.WarrantyStatus 
                                                         WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                             AND IsDeleted = 0
                                                        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;

                                                        SELECT COUNT(*)
                                                        FROM dbo.WarrantyStatus 
                                                        WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                            AND IsDeleted = 0;";

        public const string WARRANTY_STATUS_GET_BY_ID = @"SELECT * FROM dbo.WarrantyStatus WHERE Id = @Id AND IsDeleted = 0";

        public const string WARRANTY_STATUS_GET_ALL = "SELECT Id, Name FROM dbo.WarrantyStatus WHERE IsDeleted = 0";

        public const string WARRANTY_STATUS_INSERT = @"INSERT dbo.WarrantyStatus
                                                        (Name,
                                                         Color,
                                                         SortOrder,
                                                         IsSendSms,
                                                         SmsContent,
                                                         UserCreated,
                                                         DateCreated,
                                                         UserUpdated,
                                                         DateUpdated,
                                                         IsDeleted)
                                                 VALUES (@Name,
                                                         @Color,
                                                         @SortOrder,
                                                         @IsSendSms,
                                                         @SmsContent,
                                                         @UserCreated,
                                                         @DateCreated,
                                                         @UserUpdated,
                                                         @DateUpdated,
                                                         0);

                                                 SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string WARRANTY_STATUS_UPDATE = @"  UPDATE dbo.WarrantyStatus
                                                  SET Name = @Name,
                                                      Color = @Color,
                                                      IsSendSms = @IsSendSms,
                                                      SortOrder = @SortOrder,
                                                      SmsContent = @SmsContent,
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string WARRANTY_STATUS_DELETE = @"  UPDATE dbo.WarrantyStatus
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        #endregion

        #region CandidateStatus

        public const string CANDIDATE_STATUS_GET_ALL = @"SELECT Id,
                                                          Name
                                                   FROM dbo.CandidateStatus
                                                   WHERE IsDeleted = 0
                                                   ORDER BY Id;";

        public const string CANDIDATE_STATUS_GET_BY_PAGING = @"  SELECT *
                                                         FROM dbo.CandidateStatus 
                                                         WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                             AND IsDeleted = 0
                                                        ORDER BY {0} {1}
                                                        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;

                                                        SELECT COUNT(*)
                                                        FROM dbo.CandidateStatus 
                                                        WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                            AND IsDeleted = 0;";

        public const string CANDIDATE_STATUS_GET_BY_ID = @"SELECT * FROM dbo.CandidateStatus WHERE Id = @Id AND IsDeleted = 0";


        public const string CANDIDATE_STATUS_INSERT = @"INSERT dbo.CandidateStatus
                                                        (Name,
                                                         Color,
                                                         IsSendSms,
                                                         SmsContent,
                                                         SortOrder,
                                                         UserCreated,
                                                         DateCreated,
                                                         UserUpdated,
                                                         DateUpdated,
                                                         IsDeleted)
                                                 VALUES (@Name,
                                                         @Color,
                                                         @IsSendSms,
                                                         @SmsContent,
                                                         @SortOrder,                                                                                                                  
                                                         @UserCreated,
                                                         @DateCreated,
                                                         @UserUpdated,
                                                         @DateUpdated,
                                                         0);

                                                 SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string CANDIDATE_STATUS_UPDATE = @"  UPDATE dbo.CandidateStatus
                                                  SET Name = @Name,
                                                      Color = @Color,
                                                      IsSendSms = @IsSendSms,
                                                      SmsContent = @SmsContent,
                                                      SortOrder=@SortOrder, 
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string CANDIDATE_STATUS_DELETE = @"  UPDATE dbo.CandidateStatus
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        #endregion

        #region ReceiptStatus

        public const string RECEIPT_STATUS_GET_ALL = @"SELECT Id,
                                                          Name
                                                   FROM dbo.ReceiptStatus
                                                   WHERE IsDeleted = 0
                                                   ORDER BY Id;";

        public const string RECEIPT_STATUS_GET_BY_PAGING = @"  SELECT *
                                                         FROM dbo.ReceiptStatus 
                                                         WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                             AND IsDeleted = 0
                                                        ORDER BY {0} {1}
                                                        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;

                                                        SELECT COUNT(*)
                                                        FROM dbo.ReceiptStatus 
                                                        WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                            AND IsDeleted = 0;";

        public const string RECEIPT_STATUS_GET_BY_ID = @"SELECT * FROM dbo.CrmStatus WHERE Id = @Id AND IsDeleted = 0";


        public const string RECEIPT_STATUS_INSERT = @"INSERT dbo.ReceiptStatus
                                                        (Name,
                                                         Color,
                                                         IsSendSms,
                                                         SmsContent,
                                                         SortOrder,
                                                         UserCreated,
                                                         DateCreated,
                                                         UserUpdated,
                                                         DateUpdated,
                                                         IsDeleted)
                                                 VALUES (@Name,
                                                         @Color,
                                                         @IsSendSms,
                                                         @SmsContent,
                                                         @SortOrder,                                                                                                                  
                                                         @UserCreated,
                                                         @DateCreated,
                                                         @UserUpdated,
                                                         @DateUpdated,
                                                         0);

                                                 SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string RECEIPT_STATUS_UPDATE = @"  UPDATE dbo.ReceiptStatus
                                                  SET Name = @Name,
                                                      Color = @Color,
                                                      IsSendSms = @IsSendSms,
                                                      SmsContent = @SmsContent,
                                                      SortOrder=@SortOrder, 
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string RECEIPT_STATUS_DELETE = @"  UPDATE dbo.ReceiptStatus
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        #endregion

        #region ReceiptDescription

        public const string RECEIPT_DESCRIPTION_GET_ALL = @"SELECT Id,
                                                          Name
                                                   FROM dbo.ReceiptDescription
                                                   WHERE IsDeleted = 0;";

        public const string RECEIPT_DESCRIPTION_GET_BY_ID = @"SELECT * FROM dbo.ReceiptDescription WHERE Id = @Id AND IsDeleted = 0";

        public const string RECEIPT_DESCRIPTION_INSERT = @"INSERT dbo.ReceiptDescription
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

        public const string RECEIPT_DESCRIPTION_UPDATE = @"  UPDATE dbo.ReceiptDescription
                                                  SET Name = @Name,
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string RECEIPT_DESCRIPTION_DELETE = @"  UPDATE dbo.ReceiptDescription
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        public const string RECEIPT_DESCRIPTION_PAGING = @"SELECT c.Id,
                                                              c.Name,
                                                              c.DateUpdated
                                                       FROM dbo.ReceiptDescription c
                                                       WHERE ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @Name COLLATE Latin1_General_CI_AI
                                                             AND c.IsDeleted = 0
                                                       OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
                                                       SELECT COUNT(*)
                                                       FROM dbo.ReceiptDescription c
                                                       WHERE ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @Name COLLATE Latin1_General_CI_AI
                                                             AND c.IsDeleted = 0;";

        #endregion

        #region WarrantyDescription

        public const string WARRANTY_DESCRIPTION_GET_ALL = @"SELECT Id,
                                                          Name
                                                   FROM dbo.WarrantyDescription
                                                   WHERE IsDeleted = 0;";

        public const string WARRANTY_DESCRIPTION_GET_BY_ID = @"SELECT * FROM dbo.WarrantyDescription WHERE Id = @Id AND IsDeleted = 0";

        public const string WARRANTY_DESCRIPTION_INSERT = @"INSERT dbo.WarrantyDescription
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

        public const string WARRANTY_DESCRIPTION_UPDATE = @"  UPDATE dbo.WarrantyDescription
                                                  SET Name = @Name,
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string WARRANTY_DESCRIPTION_DELETE = @"  UPDATE dbo.WarrantyDescription
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        public const string WARRANTY_DESCRIPTION_PAGING = @"SELECT c.Id,
                                                              c.Name,
                                                              c.DateUpdated
                                                       FROM dbo.WarrantyDescription c
                                                       WHERE ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @Name COLLATE Latin1_General_CI_AI
                                                             AND c.IsDeleted = 0
                                                       ORDER BY {0} {1} OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
                                                       SELECT COUNT(*)
                                                       FROM dbo.WarrantyDescription c
                                                       WHERE ISNULL(c.Name, '') COLLATE Latin1_General_CI_AI LIKE @Name COLLATE Latin1_General_CI_AI
                                                             AND c.IsDeleted = 0;";

        #endregion

        #region TaskPriority

        public const string TASK_PRIORITY_GET_ALL = @"SELECT Id,
                                                            Name
                                                     FROM dbo.TaskPriority
                                                     WHERE IsDeleted = 0
                                                     ORDER BY Id;";

        public const string TASK_PRIORITY_INSERT = @"INSERT dbo.TaskPriority
                                                        (Name,
                                                         Color,
                                                         SortOrder,
                                                         UserCreated,
                                                         DateCreated,
                                                         UserUpdated,
                                                         DateUpdated,
                                                         IsDeleted)
                                                 VALUES (@Name,
                                                         @Color,
                                                         @SortOrder,
                                                         @UserCreated,
                                                         @DateCreated,
                                                         @UserUpdated,
                                                         @DateUpdated,
                                                         0);

                                                 SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string TASK_PRIORITY_UPDATE = @"  UPDATE dbo.TaskPriority
                                                  SET Name = @Name,
                                                      Color = @Color,
                                                      SortOrder = @SortOrder,
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string TASK_PRIORITY_DELETE = @"  UPDATE dbo.TaskPriority
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        public const string TASK_PRIORITY_GET_BY_PAGING = @"  SELECT *
                                                         FROM dbo.TaskPriority 
                                                         WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                             AND IsDeleted = 0
                                                        ORDER BY {0} {1}
                                                        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;

                                                        SELECT COUNT(*)
                                                        FROM dbo.TaskPriority 
                                                        WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                            AND IsDeleted = 0;";

        public const string TASK_PRIORITY_GET_BY_ID = @"SELECT * FROM dbo.TaskPriority WHERE Id = @Id AND IsDeleted = 0";

        #endregion

        #region TaskStatus

        public const string TASK_STATUS_GET_ALL = @"SELECT Id,
                                                          Name
                                                   FROM dbo.TaskStatus
                                                   WHERE IsDeleted = 0
                                                   ORDER BY Id;";

        public const string TASK_STATUS_GET_BY_PAGING = @"  SELECT *
                                                         FROM dbo.TaskStatus 
                                                         WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                             AND IsDeleted = 0
                                                        ORDER BY {0} {1}
                                                        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;

                                                        SELECT COUNT(*)
                                                        FROM dbo.TaskStatus 
                                                        WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                            AND IsDeleted = 0;";

        public const string TASK_STATUS_GET_BY_ID = @"SELECT * FROM dbo.TaskStatus WHERE Id = @Id AND IsDeleted = 0";


        public const string TASK_STATUS_INSERT = @"INSERT dbo.TaskStatus
                                                        (Name,
                                                         Color,
                                                         SortOrder,
                                                         IsSendSms,
                                                         SmsContent,
                                                         UserCreated,
                                                         DateCreated,
                                                         UserUpdated,
                                                         DateUpdated,
                                                         IsDeleted)
                                                 VALUES (@Name,
                                                         @Color,
                                                         @SortOrder,
                                                         @IsSendSms,
                                                         @SmsContent,
                                                         @UserCreated,
                                                         @DateCreated,
                                                         @UserUpdated,
                                                         @DateUpdated,
                                                         0);

                                                 SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string TASK_STATUS_UPDATE = @"  UPDATE dbo.TaskStatus
                                                  SET Name = @Name,
                                                      Color = @Color,
                                                      SortOrder = @SortOrder,
                                                      IsSendSms = @IsSendSms,
                                                      SmsContent = @SmsContent,
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string TASK_STATUS_DELETE = @"  UPDATE dbo.TaskStatus
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        #endregion

        #region Task Type

        public const string TASK_TYPE_GET_ALL = @"SELECT Id,
                                                          Name
                                                   FROM dbo.TaskType
                                                   WHERE IsDeleted = 0
                                                   ORDER BY Id;";

        public const string TASK_TYPE_GET_BY_PAGING = @"  SELECT *
                                                         FROM dbo.TaskType 
                                                         WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                             AND IsDeleted = 0
                                                        ORDER BY {0} {1}
                                                        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;

                                                        SELECT COUNT(*)
                                                        FROM dbo.TaskType 
                                                        WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                            AND IsDeleted = 0;";

        public const string TASK_TYPE_GET_BY_ID = @"SELECT * FROM dbo.TaskType WHERE Id = @Id AND IsDeleted = 0";


        public const string TASK_TYPE_INSERT = @"INSERT dbo.TaskType
                                                        (Name,
                                                         SortOrder,
                                                         UserCreated,
                                                         DateCreated,
                                                         UserUpdated,
                                                         DateUpdated,
                                                         IsDeleted)
                                                 VALUES (@Name,
                                                         @SortOrder,
                                                         @UserCreated,
                                                         @DateCreated,
                                                         @UserUpdated,
                                                         @DateUpdated,
                                                         0);

                                                 SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string TASK_TYPE_UPDATE = @"  UPDATE dbo.TaskType
                                                  SET Name = @Name,
                                                      SortOrder = @SortOrder,
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string TASK_TYPE_DELETE = @"  UPDATE dbo.TaskType
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        #endregion

        #region Task Loop

        public const string TASK_LOOP_GET_ALL = @"SELECT Id,
                                                          Name
                                                   FROM dbo.TaskLoop
                                                   WHERE IsDeleted = 0
                                                   ORDER BY Id;";

        public const string TASK_LOOP_GET_BY_PAGING = @"  SELECT *
                                                         FROM dbo.TaskLoop 
                                                         WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                             AND IsDeleted = 0
                                                        ORDER BY {0} {1}
                                                        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;

                                                        SELECT COUNT(*)
                                                        FROM dbo.TaskLoop 
                                                        WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                            AND IsDeleted = 0;";

        public const string TASK_LOOP_GET_BY_ID = @"SELECT * FROM dbo.TaskLoop WHERE Id = @Id AND IsDeleted = 0";


        public const string TASK_LOOP_INSERT = @"INSERT dbo.TaskLoop
                                                        (Name,
                                                         DayCount,
                                                         SortOrder,
                                                         UserCreated,
                                                         DateCreated,
                                                         UserUpdated,
                                                         DateUpdated,
                                                         IsDeleted)
                                                 VALUES (@Name,
                                                         @DayCount,
                                                         @SortOrder,
                                                         @UserCreated,
                                                         @DateCreated,
                                                         @UserUpdated,
                                                         @DateUpdated,
                                                         0);

                                                 SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string TASK_LOOP_UPDATE = @"  UPDATE dbo.TaskLoop
                                                  SET Name = @Name,
                                                      SortOrder = @SortOrder,
                                                      DayCount = @DayCount,
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string TASK_LOOP_DELETE = @"  UPDATE dbo.TaskLoop
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        #endregion
        #region Task

        public const string TASK_GET_ALL_BY_CUSTOMERID = @"SELECT Id,
                                                          Name
                                                   FROM dbo.Task
                                                   WHERE IsDeleted = 0 AND CustomerId = @CustomerId
                                                   ORDER BY Id;";

        public const string TASK_GET_BY_PAGING = @"DROP TABLE IF EXISTS #tmpTask;
                                                        SELECT t.Id,
                                                         t.TaskStatusId,
                                                         t.TaskPriorityId,
                                                         t.TaskLoopId,
                                                         t.TaskTypeId,
                                                         t.UserCreated, 
                                                         t.Code,
                                                         t.Name,
                                                         t.Content,
                                                         t.Description,
                                                         t.FromDateTime,
                                                         t.ToDateTime,                                                       
                                                         tl.Name AS TaskLoopName,
                                                         tp.Name AS TaskPriorityName,
                                                         tp.Color AS TaskPriorityColor,
                                                         ts.Color AS TaskStatusColor,
                                                         ts.Name AS TaskStatusName,
                                                         u.FullName AS UserNameCreated,
                                                         cs.Phone AS CustomerPhone,
                                                         cs.Name AS CustomerName,
                                                         cs.Address AS CustomerAddress,
                                                       INTO #tmpTask
                                                         FROM dbo.Task as t
                                                         LEFT JOIN dbo.[User] AS u
                                                               ON u.Id = t.UserCreated                                                         
                                                         LEFT JOIN dbo.TaskLoop AS tl
                                                               ON tl.Id = t.TaskLoopId
                                                         LEFT JOIN dbo.TaskPriority AS tp
                                                               ON tp.Id = t.TaskPriorityId
                                                         LEFT JOIN dbo.TaskStatus AS ts
                                                               ON ts.Id = t.TaskStatusId
                                                         LEFT JOIN dbo.Customer AS cs
                                                               ON cs.Id = t.CustomerId
                                                         WHERE ISNULL(t.Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                             AND ISNULL(t.Code,'') COLLATE Latin1_general_CI_AI LIKE @Code COLLATE Latin1_general_CI_AI
                                                             AND ISNULL(CustomerPhone,'') COLLATE Latin1_general_CI_AI LIKE @Phone COLLATE Latin1_general_CI_AI
                                                             AND ISNULL(CustomerName,'') COLLATE Latin1_general_CI_AI LIKE @CustomerName COLLATE Latin1_general_CI_AI
                                                             AND
                                                             (
                                                                 @TaskStatusId = 0
                                                                 OR ISNULL(t.TaskStatusId, 0) = @TaskStatusId
                                                             )
                                                             AND
                                                             (
                                                                 @TaskPriorityId = 0
                                                                 OR ISNULL(t.TaskPriorityId, 0) = @TaskPriorityId
                                                             )
                                                             AND
                                                             (
                                                                 @TaskLoopId = 0
                                                                 OR ISNULL(t.TaskLoopId, 0) = @TaskLoopId
                                                             )
                                                             AND
                                                             (
                                                                 @TaskTypeId = 0
                                                                 OR ISNULL(t.TaskTypeId, 0) = @TaskTypeId
                                                             )
                                                             AND
                                                             (
                                                                 @UserCreatedId = 0
                                                                 OR ISNULL(t.UserCreated, 0) = @UserCreatedId
                                                             )
                                                             AND
                                                             (
                                                                 @UserCaredId = 0
                                                                 OR ISNULL(ut.UserCaredId, 0) = @UserCaredId
                                                             )                                                             
                                                             AND
                                                             (
                                                                (@FromTime ='' OR DATEDIFF(DAY,t.FromDateTime,convert(datetime, @FromDate, 0)) <= 0 OR DATEDIFF(DAY,t.ToDateTime,convert(datetime, @FromDate, 0)) <= 0) AND (@ToTime ='' OR DATEDIFF(DAY,t.ToDateTime,convert(datetime, @ToDate, 0)) >= 0)
                                                             )           
                                                             AND IsDeleted = 0
                                                        ORDER BY {0} {1}
                                                        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;

                                                        SELECT COUNT(*)
                                                        FROM #tmpTask";

        public const string TASK_GET_BY_ID = @"SELECT * FROM dbo.TaskLoop WHERE Id = @Id AND IsDeleted = 0";


        public const string TASK_INSERT = @"INSERT dbo.Task
                                                        (Name,
                                                         Code,
                                                         CustomerId,
                                                         Content,
                                                         Description,
                                                         TaskStatusId,
                                                         TaskPriorityId,
                                                         TaskLoopId,
                                                         TaskTypeId,
                                                         FromDateTime,
                                                         ToDateTime,
                                                         UserCreated,
                                                         DateCreated,
                                                         UserUpdated,
                                                         DateUpdated,
                                                         IsDeleted)
                                                 VALUES (@Name,
                                                         @Code,
                                                         @CustomerId,
                                                         @Content,
                                                         @Description,
                                                         @TaskStatusId,
                                                         @TaskPriorityId,
                                                         @TaskLoopId,
                                                         @TaskTypeId,
                                                         @FromDateTime,
                                                         @ToDateTime,
                                                         @UserCreated,
                                                         @DateCreated,
                                                         @UserUpdated,
                                                         @DateUpdated,
                                                         0);

                                                 SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string TASK_UPDATE = @"UPDATE dbo.Task
                                                  SET Name = @Name,
                                                         Code=@Code,
                                                         Content=@Content,
                                                         Description=@Description,
                                                         TaskStatusId=@TaskStatusId,
                                                         TaskPriorityId=@TaskPriorityId,
                                                         TaskLoopId=@TaskLoopId,
                                                         TaskTypeId=@TaskTypeId,
                                                         FromDateTime=@FromDateTime,
                                                         ToDateTime=@ToDateTime,
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string TASK_DELETE = @"  UPDATE dbo.Task
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        public const string TASK_CURRENT_COUNT = @"SELECT COUNT(*) FROM dbo.Task
                                                         WHERE @CurrentDate ='' OR DATEDIFF(DAY,DateCreated,convert(datetime, @CurrentDate, 0)) = 0                                                                                                                       
                                                                AND IsDeleted = 0;";

        #endregion
        #region RecruitmentStatus

        public const string RECRUITMENT_STATUS_GET_ALL = @"SELECT Id,
                                                          Name
                                                   FROM dbo.RecruitmentStatus
                                                   WHERE IsDeleted = 0
                                                   ORDER BY Id;";

        public const string RECRUITMENT_STATUS_GET_BY_PAGING = @"  SELECT *
                                                         FROM dbo.RecruitmentStatus 
                                                         WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                             AND IsDeleted = 0
                                                        ORDER BY {0} {1}
                                                        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;

                                                        SELECT COUNT(*)
                                                        FROM dbo.RecruitmentStatus 
                                                        WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                            AND IsDeleted = 0;";

        public const string RECRUITMENT_STATUS_GET_BY_ID = @"SELECT * FROM dbo.RecruitmentStatus WHERE Id = @Id AND IsDeleted = 0";


        public const string RECRUITMENT_STATUS_INSERT = @"INSERT dbo.RecruitmentStatus
                                                        (Name,
                                                         Color,
                                                         SortOrder,
                                                         UserCreated,
                                                         DateCreated,
                                                         UserUpdated,
                                                         DateUpdated,
                                                         IsDeleted)
                                                 VALUES (@Name,
                                                         @Color,
                                                         @SortOrder,
                                                         @UserCreated,
                                                         @DateCreated,
                                                         @UserUpdated,
                                                         @DateUpdated,
                                                         0);

                                                 SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string RECRUITMENT_STATUS_UPDATE = @"  UPDATE dbo.RecruitmentStatus
                                                  SET Name = @Name,
                                                      Color = @Color,
                                                      SortOrder = @SortOrder,
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string RECRUITMENT_STATUS_DELETE = @"  UPDATE dbo.RecruitmentStatus
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        #endregion

        #region Candidate

        public const string CANDIDATE_GET_BY_ID = @"SELECT * FROM dbo.Candidate WHERE Id = @Id AND IsDeleted = 0";

        public const string CANDIDATE_INSERT = @"INSERT dbo.Candidate
                                                        (Name,
                                                         Code,
                                                         Address,
                                                         Email,Description,
                                                         UserCreated,
                                                         DateCreated,
                                                         UserUpdated,
                                                         DateUpdated,
                                                         IsDeleted)
                                                 VALUES (@Name,
                                                         @Code,
                                                         @Address,
                                                         @Email,@Description,
                                                         @UserCreated,
                                                         @DateCreated,
                                                         @UserUpdated,
                                                         @DateUpdated,
                                                         0);

                                                 SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string CANDIDATE_UPDATE = @"  UPDATE dbo.Candidate
                                                  SET Name = @Name,
                                                         Code=@Code,
                                                         Address=@Address,
                                                         Email=@Email,Description=@Description,
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string CANDIDATE_DELETE = @"  UPDATE dbo.Candidate
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        public const string CANDIDATE_GET_BY_PAGING = @"SELECT c.Id,
                                                             c.Code,
                                                             c.Name,
                                                             c.Phone,
                                                             c.Address,
                                                             c.Email,
                                                             c.Description
                                                             cs.Color
                                                             cs.Name AS CandidateStatusName
                                                             u.FullName AS UserNameCreated,
                                                             u.Phone AS UserPhoneCreated
                                                         FROM dbo.Candidate AS c
                                                            LEFT JOIN dbo.CandidateStatus AS cs
                                                               ON cs.Id = c.CandidateStatusId
                                                         LEFT JOIN dbo.[User] As u
                                                               ON u.Id = c.UserCreated
                                                         WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                             AND ISNULL(c.Phone, '') COLLATE Latin1_General_CI_AI LIKE @Phone COLLATE Latin1_General_CI_AI
                                                             AND ISNULL(c.Code, '') COLLATE Latin1_General_CI_AI LIKE @Code COLLATE Latin1_General_CI_AI
                                                             AND
                                                             (
                                                                 @CandidateStatusId = 0
                                                                 OR ISNULL(c.CandidateStatusId, 0) = @CandidateStatusId
                                                             )
                                                             AND
                                                             (
                                                                (@FromDate = '' OR c.DateCreated >= @FromDate) AND (@ToDate ='' OR c.DateCreated <= @ToDate)
                                                             )
                                                             
                                                                AND IsDeleted = 0
                                                        ORDER BY {0} {1}
                                                        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;

                                                        SELECT COUNT(*) FROM dbo.Candidate
                                                         WHERE ISNULL(Name,'') COLLATE Latin1_general_CI_AI LIKE @Name COLLATE Latin1_general_CI_AI
                                                             AND ISNULL(c.Phone, '') COLLATE Latin1_General_CI_AI LIKE @Phone COLLATE Latin1_General_CI_AI
                                                             AND ISNULL(c.Code, '') COLLATE Latin1_General_CI_AI LIKE @Code COLLATE Latin1_General_CI_AI
                                                             AND
                                                             (
                                                                 @CandidateStatusId = 0
                                                                 OR ISNULL(c.CandidateStatusId, 0) = @CandidateStatusId
                                                             )
                                                             AND
                                                             (
                                                                (@FromTime ='' OR DATEDIFF(DAY,DateCreated,convert(datetime, @FromTime, 0)) <= 0) AND (@ToTime ='' OR DATEDIFF(DAY,DateCreated,convert(datetime, @ToTime, 0)) >= 0)
                                                             )
                                                                AND IsDeleted = 0;";

        public const string CANDIDATE_CURRENT_COUNT = @"SELECT COUNT(*) FROM dbo.Candidate
                                                         WHERE @CurrentDate ='' OR DATEDIFF(DAY,DateCreated,convert(datetime, @CurrentDate, 0)) = 0                                                                                                                       
                                                                AND IsDeleted = 0;";

        #endregion

        #region Recruitment

        public const string RECRUITMENT_GET_BY_ID = @"SELECT * FROM dbo.Recruitment WHERE Id = @Id AND IsDeleted = 0";

        public const string RECRUITMENT_INSERT = @"INSERT dbo.Recruitment
                                                        (Name,
                                                         Code,
                                                         Description,
                                                         Content,
                                                         RecruitmentStatusId,
                                                         UserCreated,
                                                         DateCreated,
                                                         UserUpdated,
                                                         DateUpdated,
                                                         IsDeleted)
                                                 VALUES (@Name,
                                                         @Color,
                                                         @Code,
                                                         @Description,
                                                         @Content,
                                                         @RecruitmentStatusId,
                                                         @UserCreated,
                                                         @DateCreated,
                                                         @UserUpdated,
                                                         @DateUpdated,
                                                         0);

                                                 SELECT CAST(SCOPE_IDENTITY() as int);";

        public const string RECRUITMENT_UPDATE = @"  UPDATE dbo.Recruitment
                                                  SET RecruitmentStatusId = @RecruitmentStatusId,
                                                      UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated
                                                  WHERE Id = @Id";

        public const string RECRUITMENT_DELETE = @"  UPDATE dbo.Recruitment
                                                  SET UserUpdated = @UserUpdated,
                                                      DateUpdated = @DateUpdated,
                                                      IsDeleted = 1
                                                  WHERE Id = @Id";

        public const string RECRUITMENT_GET_BY_PAGING = @"SELECT c.Id,
                                                             c.Code,
                                                             c.Name,
                                                             c.Description
                                                             cs.Color
                                                             cs.Name AS RecruitmentStatusName
                                                             u.FullName AS UserNameCreated,
                                                             u.Phone AS UserPhoneCreated
                                                         FROM dbo.Recruitment AS c
                                                            LEFT JOIN dbo.RecruitmentStatus AS cs
                                                               ON cs.Id = c.RecruitmentStatusId
                                                         LEFT JOIN dbo.[User] As u
                                                               ON u.Id = c.UserCreated
                                                         WHERE ISNULL(c.Code, '') COLLATE Latin1_General_CI_AI LIKE @Code COLLATE Latin1_General_CI_AI
                                                             AND
                                                             (
                                                                 @RecruitmentStatusId = 0
                                                                 OR ISNULL(c.RecruitmentStatusId, 0) = @RecruitmentStatusId
                                                             )
                                                             AND
                                                             (
                                                                (@FromDate = '' OR c.DateCreated >= @FromDate) AND (@ToDate ='' OR c.DateCreated <= @ToDate)
                                                             )                                                             
                                                                AND IsDeleted = 0
                                                        ORDER BY {0} {1}
                                                        OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;

                                                        SELECT COUNT(*) FROM dbo.Recruitment
                                                         WHERE ISNULL(c.Code, '') COLLATE Latin1_General_CI_AI LIKE @Code COLLATE Latin1_General_CI_AI
                                                             AND
                                                             (
                                                                 @RecruitmentStatusId = 0
                                                                 OR ISNULL(c.RecruitmentStatusId, 0) = @RecruitmentStatusId
                                                             )
                                                             AND
                                                             (
                                                                (@FromTime ='' OR DATEDIFF(DAY,DateCreated,convert(datetime, @FromTime, 0)) <= 0) AND (@ToTime ='' OR DATEDIFF(DAY,DateCreated,convert(datetime, @ToTime, 0)) >= 0)
                                                             )                                                             
                                                                AND IsDeleted = 0;";

        public const string RECRUITMENT_CURRENT_COUNT = @"SELECT COUNT(*) FROM dbo.Recruitment 
                                                         WHERE @CurrentDate ='' OR DATEDIFF(DAY,DateCreated,convert(datetime, @CurrentDate, 0)) = 0                                                                                                                       
                                                                AND IsDeleted = 0;";

        #endregion
    }
}
