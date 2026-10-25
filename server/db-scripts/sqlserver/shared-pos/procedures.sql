USE [SBODemoAU];
GO

CREATE OR ALTER PROCEDURE dbo.APZ_UserFields_GetListField
    @TableId nvarchar(50) = N''
AS
BEGIN
    SET NOCOUNT ON;
    IF @TableId <> N''
    BEGIN
        IF @TableId LIKE N'APZ%'
        BEGIN
            SELECT TableID, FieldID, AliasID, Descr, TypeID, EditType, SizeID, EditSize, Dflt, NotNull, IndexID,
                   RTable, RField, Action, Sys, DfltDate, RelSO, RelUDO, ValidRule
            FROM dbo.APZ_CUFD WHERE TableID = @TableId;
            SELECT TableID, FieldID, IndexID, FldValue, Descr, FldDate
            FROM dbo.APZ_UFD1 WHERE TableID = @TableId ORDER BY FieldID, IndexID;
        END
        ELSE
        BEGIN
            SELECT TableID, FieldID, AliasID, Descr, TypeID, EditType, SizeID, EditSize, Dflt, NotNull, IndexID,
                   RTable, RField, Action, Sys, DfltDate, RelSO, RelUDO, ValidRule
            FROM dbo.CUFD
            WHERE TableID = @TableId
              AND ((N'U_' + AliasID NOT IN (N'U_ItemCode',N'U_ItemName',N'U_FrgnName',N'U_Quantity',N'U_IPercent',N'U_Price',N'U_PotentAmount',N'U_DeliveryDate',N'U_OpportId',N'U_line',N'U_AttFile') AND TableID=N'@OPR62') OR TableID<>N'@OPR62');
            SELECT TableID, FieldID, IndexID, FldValue, Descr, FldDate
            FROM dbo.UFD1 WHERE TableID = @TableId ORDER BY FieldID, IndexID;
        END
    END
    ELSE
        SELECT TableID, FieldID, AliasID, Descr, EditType FROM dbo.CUFD;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_FormatRule_Get
    @UserCode nvarchar(50), @FormId nvarchar(50), @ItemId nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM dbo.APZ_FCRE WHERE UserCode=@UserCode AND FormId=@FormId AND ItemId=@ItemId)
        SET @UserCode = N'manager';
    SELECT * FROM dbo.APZ_FCRE WHERE UserCode=@UserCode AND FormId=@FormId AND ItemId=@ItemId;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Apz_Formular_Get @ObjType nvarchar(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Code, Name, U_Description AS Description, U_ObjType AS ObjType, U_TableName AS TableName,
           U_FieldName AS FieldName, U_FieldType AS FieldType, U_FormularType AS FormularType,
           U_DependTable AS DependTable, U_DependField AS DependField, U_Formular AS FormularString
    FROM dbo.[@FORMULAR]
    WHERE U_ObjType=@ObjType AND U_FormularType NOT IN (N'CFL',N'CBX',N'MSCFL');
END;
GO

CREATE OR ALTER PROCEDURE dbo.Apz_Report_GetListUserReports
    @UserName nvarchar(20)=N'', @Branch nvarchar(20)=N''
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @result TABLE
    (TypeCode nvarchar(50), TypeName nvarchar(100), DocCode nvarchar(50), DocName nvarchar(254),
     FileName nvarchar(254), StoreName nvarchar(max), TableType nvarchar(10), FatherId int);

    INSERT @result(TypeCode,TypeName,DocCode,DocName,FileName,FatherId)
    SELECT a.TypeCode,a.TypeName,a.DocCode,a.DocName,a.FileName,ISNULL(b.FatherId,0)
    FROM dbo.APZ_PRINTLAYOUT a LEFT JOIN dbo.APZ_OMNI b ON N'frm_'+a.DocCode=b.Code
    WHERE a.TypeName=N'User Reports' AND NULLIF(a.FileName,N'') IS NOT NULL;

    UPDATE r SET StoreName=m.Code FROM @result r JOIN dbo.APZ_OMNI m ON r.FatherId=m.Id WHERE m.Code<>N'barSub_CrystalReport';

    INSERT @result(TypeCode,TypeName,DocCode,DocName,FileName,StoreName,TableType,FatherId)
    SELECT a.Code,N'Form Reports',a.Code,a.MenuCapt,a.ReportFile,a.StoreName,a.[TYPE],b.Id
    FROM dbo.APZ_OUDR a JOIN dbo.APZ_OMNI b ON b.Code=a.FatherMenu WHERE a.MenuItem=N'Y';

    INSERT @result(TypeCode,TypeName,DocCode,DocName,FileName,StoreName,TableType,FatherId)
    SELECT a.TypeCode,N'XtraReports',a.DocCode,a.DocName,a.Description,a.[Text],N'',b.Id
    FROM dbo.APZ_RDOC a JOIN dbo.APZ_OMNI b ON b.Code=a.Menu
    WHERE a.Status=N'A' AND ISNULL(a.DocCode,N'')=N'';

    SELECT * FROM @result;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_RetailStore_GetDoc @StoreCode nvarchar(100)=N''
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.APZ_STORE WHERE ISNULL(@StoreCode,N'')=N'' OR Code=@StoreCode;
END;
GO

CREATE OR ALTER PROCEDURE dbo.apz_retail_SaveCmdConfig
AS
BEGIN
    SET NOCOUNT ON;
    IF OBJECT_ID(N'tempdb..#APZ_RCMC') IS NULL THROW 50020, 'POS command configuration data is required.', 1;
    DELETE FROM dbo.APZ_RCMC;
    INSERT dbo.APZ_RCMC(CmdID,CmdName,[Text],TextColor,BackColor,Hotkey,MnGroup,VisOrder,Visible)
    SELECT CASE WHEN ISNULL(CmdID,0)=0 THEN ROW_NUMBER() OVER(ORDER BY VisOrder,CmdName) ELSE CmdID END,
           CmdName,[Text],TextColor,BackColor,Hotkey,MnGroup,VisOrder,Visible
    FROM #APZ_RCMC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.apz_retail_SavePosMaster
AS
BEGIN
    SET NOCOUNT ON;
    IF OBJECT_ID(N'tempdb..#APZ_RPOS') IS NULL THROW 50021, 'POS master data is required.', 1;
    MERGE dbo.APZ_RPOS AS t USING #APZ_RPOS AS s ON t.PosID=s.PosID AND ISNULL(s.PosID,0)<>0
    WHEN MATCHED THEN UPDATE SET PosCode=s.PosCode,Store=s.Store,Dscription=s.Dscription,UserSign=s.UserSign,UpdateDate=s.UpdateDate,CreateDate=s.CreateDate,UserSign2=s.UserSign2,Inactive=s.Inactive
    WHEN NOT MATCHED THEN INSERT(PosID,PosCode,Store,Dscription,UserSign,UpdateDate,CreateDate,UserSign2,Inactive)
      VALUES((SELECT ISNULL(MAX(x.PosID),0)+1 FROM dbo.APZ_RPOS x),s.PosCode,s.Store,s.Dscription,s.UserSign,s.UpdateDate,s.CreateDate,s.UserSign2,s.Inactive);
END;
GO

CREATE OR ALTER PROCEDURE dbo.apz_retail_SavePosAssignment @UserCode nvarchar(20)
AS
BEGIN
    SET NOCOUNT ON;
    IF OBJECT_ID(N'tempdb..#APZ_POS1') IS NULL THROW 50022, 'POS assignment data is required.', 1;
    DELETE FROM dbo.APZ_POS1 WHERE UserCode=@UserCode;
    INSERT dbo.APZ_POS1 SELECT * FROM #APZ_POS1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.APZ_Retail_GetUserPosAssignment @UserCode nvarchar(25)=N''
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @multiBranch char(1)=ISNULL((SELECT TOP(1) MltpBrnchs FROM dbo.OADM),N'N');
    SELECT a.PosID,a.PosCode,a.Dscription,a.Store,ISNULL(b.IsDflt,N'N') IsDflt,
           CASE WHEN @UserCode=N'' OR b.PosID IS NULL THEN N'N' ELSE N'Y' END Checked
    FROM dbo.APZ_RPOS a
    LEFT JOIN dbo.APZ_POS1 b ON a.PosID=b.PosID AND (@UserCode=N'' OR b.UserCode=@UserCode)
    JOIN dbo.APZ_STORE c ON a.Store=c.Code
    WHERE c.Locked=N'N' AND a.Inactive=N'N'
      AND (@UserCode=N'' OR @multiBranch<>N'Y' OR EXISTS(SELECT 1 FROM dbo.USR6 u WHERE u.UserCode=@UserCode AND u.BPLId=c.BPLId));
END;
GO
