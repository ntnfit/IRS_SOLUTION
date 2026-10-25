USE [SBODemoAU];
GO

CREATE OR ALTER PROCEDURE dbo.APZ_D_Delivery_SearchDeliveryOrder
    @docEntry int = 0,
    @userSign nvarchar(50) = N'',
    @fromDate datetime = NULL,
    @toDate datetime = NULL,
    @status nvarchar(20) = N''
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @structure table
    (
        ID int IDENTITY(1,1) NOT NULL,
        ColumnName nvarchar(50) PRIMARY KEY,
        Caption nvarchar(100), DataType varchar(50), SubType varchar(100),
        Width int, GroupIndex int, Visible tinyint, FormatType nvarchar(20),
        DisplayFormat nvarchar(20), ReadOnly tinyint, AllowEdit tinyint,
        AllowMove tinyint, AllowSize tinyint, AllowSort tinyint, Fixed varchar(1),
        BackColor nvarchar(20), ForeColor nvarchar(20), SummaryType varchar(10),
        GroupSummaryType varchar(10), GroupSummaryShowInFooter tinyint,
        SummaryFormat nvarchar(50), Expression nvarchar(4000), Parent tinyint,
        [Key] tinyint, PivotArea nvarchar(20)
    );

    INSERT @structure
        (ColumnName,Caption,DataType,SubType,Width,GroupIndex,Visible,FormatType,DisplayFormat,ReadOnly,AllowEdit,AllowMove,AllowSize,AllowSort,Fixed,BackColor)
    VALUES
        (N'DocEntry',N'DocEntry','int','',75,-1,1,NULL,NULL,1,1,1,1,0,'L','silver'),
        (N'BaseEntry',N'BaseEntry','int','',75,-1,0,NULL,NULL,1,1,1,1,0,'L',''),
        (N'VisOrder',N'VisOrder','int','',75,-1,0,NULL,NULL,1,1,1,1,0,'L',''),
        (N'DOEntry',N'DOEntry','int','',100,-1,0,NULL,NULL,0,1,1,1,0,'',NULL),
        (N'CardCode',N'Mã khách hàng','linkedcflobject','2,Apz_Cfl_LoadBusinessPartner @cardType=''C'' ',100,-1,1,NULL,NULL,0,1,1,1,0,'',NULL),
        (N'CardName',N'Tên Khách hàng','linkedobject','2 #CardCode',200,-1,1,NULL,NULL,0,1,1,1,0,'',NULL),
        (N'ContactPerson',N'Người liên hệ','nvarchar','',75,-1,1,NULL,NULL,0,1,1,1,0,'',NULL),
        (N'Phone',N'Phone','nvarchar','',75,-1,1,NULL,NULL,0,1,1,1,0,'',NULL),
        (N'UserSign',N'Nhân viên GH','cfl','APZ_CFL_Users',75,-1,1,NULL,NULL,0,1,1,1,0,'',NULL),
        (N'ScheduledDate',N'Ngày cần giao','datetime','',75,-1,1,'datetime','',0,1,1,1,0,'',NULL),
        (N'ScheduledTime',N'Giờ cần giao','time','',75,-1,1,'','',0,1,1,1,0,'',NULL),
        (N'ShipToCode',N'Mã địa chỉ','cfl','APZ_CFL_ShipToCode @CardCode',75,-1,1,'','',0,1,1,1,0,'',NULL),
        (N'ShipToAddress',N'Địa chỉ','nvarchar','',75,-1,1,NULL,NULL,0,1,1,1,0,'',NULL),
        (N'Amount',N'Tiền phải thu','numeric','',75,-1,1,'numeric',NULL,0,1,1,1,0,'',NULL),
        (N'Remark',N'Ghi chú cho NVGH','nvarchar','',75,-1,1,NULL,NULL,0,1,1,1,0,'',NULL),
        (N'LateTime',N'Thời gian GH muộn','datetime','',75,-1,1,NULL,NULL,1,1,1,1,0,'',NULL),
        (N'LateReason',N'Lý do muộn','nvarchar','',75,-1,1,NULL,NULL,1,1,1,1,0,'',NULL),
        (N'ActualAmount',N'Tiền thu thực tế','numeric','',100,-1,1,'numeric',NULL,1,1,1,1,0,'',NULL),
        (N'AddressConfirm',N'Địa chỉ GH thực tế','nvarchar','',100,-1,1,NULL,NULL,1,1,1,1,0,'',NULL),
        (N'Latitude',N'Latitude','nvarchar','',200,-1,0,NULL,NULL,1,1,1,1,0,'',NULL),
        (N'Longitude',N'Longitude','nvarchar','',75,-1,0,NULL,NULL,1,1,1,1,0,'',NULL),
        (N'StartTime',N'Thời gian bắt đầu GH','datetime','',75,-1,1,'','',1,1,1,1,0,'',NULL),
        (N'ConfirmTime',N'Thời gian kết thúc GH','datetime','',75,-1,1,'','',1,1,1,1,0,'',NULL),
        (N'ConfirmStatus',N'Kết quả GH','ValidValue','',100,-1,1,NULL,NULL,1,1,1,1,0,'',NULL),
        (N'Feedback',N'KH phản hồi','nvarchar','',75,-1,1,'','',1,1,1,1,0,'',NULL),
        (N'ConfirmNote',N'Ghi chú GH','nvarchar','',75,-1,1,'','',1,1,1,1,0,'',NULL),
        (N'Status',N'Status','validvalue','',45,-1,0,NULL,NULL,1,1,1,1,0,'',NULL);

    SELECT * FROM @structure ORDER BY ID;
    SELECT * FROM dbo.APZ_D_ODLO
    WHERE (@docEntry=0 OR DocEntry=@docEntry)
      AND (@userSign=N'' OR CONVERT(nvarchar(50),UserSign)=@userSign)
      AND (@status=N'' OR Status=@status)
      AND (@fromDate IS NULL OR ScheduledDate>=@fromDate)
      AND (@toDate IS NULL OR ScheduledDate<DATEADD(day,1,@toDate));
    SELECT ColumnName,[Value],[Description],BackColor,ForeColor
    FROM (VALUES
      (N'Status',N'Waiting',N'Waiting',CAST(NULL AS nvarchar(20)),CAST(NULL AS nvarchar(20))),
      (N'Status',N'Processing',N'Processing',NULL,NULL),(N'Status',N'Done',N'Done',NULL,NULL),
      (N'ConfirmStatus',N'A',N'Giao hàng thành công',NULL,NULL),
      (N'ConfirmStatus',N'P',N'Giao hàng 1 phần',NULL,NULL),
      (N'ConfirmStatus',N'F',N'Không giao được hàng',NULL,NULL)
    ) v(ColumnName,[Value],[Description],BackColor,ForeColor);
END;
GO
