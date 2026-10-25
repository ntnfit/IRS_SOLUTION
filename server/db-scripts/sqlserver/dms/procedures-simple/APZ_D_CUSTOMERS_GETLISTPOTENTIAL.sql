USE [SBODemoAU];
GO
CREATE OR ALTER PROCEDURE dbo.[APZ_D_Customers_GetListPotential]
    @userCode nvarchar(50) = ''
AS
BEGIN
    SET NOCOUNT ON;
SELECT A.[CardCode], A.[CardName], A.[Address], A.[Cellular], A.[CardType], A.[Notes], B.[E_MailL], A.[IntrntSite], B.[Cellolar], B.[BirthDate], B.[Name] AS [CntPrsName]
		, B.[CntctCode], ISNULL(D.[Street], '') + ' ' + ISNULL(D.[City], '') + ' ' + ISNULL(D.[Country], '') AS [ShipToAddress]
		, ISNULL(E.[Street], '') + ' ' + ISNULL(E.[City], '') + ' ' + ISNULL(E.[Country], '') AS [BillToAddress], A.[BillToDef], A.[ShipToDef], A.[U_Latitude], A.[U_Longitude]
		, a.[U_Province] AS [TinhTP]
		,ISNULL((select string_agg(cast(y.[EmpId] as nvarchar), ',') from APZ_D_STR2 y where y.[CardCode] = a.[CardCode]),M.[empID]) as [saleEmployees], A.[SlpCode]
	 from  OCRD A LEFT JOIN OCPR B ON A.[CardCode] = B.[CardCode] AND A.[CntctPrsn] = B.[Name]
					LEFT JOIN CRD1 D ON A.[CardCode] = D.[CardCode] AND A.[ShipToDef] = D.[Address] AND d.[AdresType] = 'S'
					LEFT JOIN CRD1 E ON A.[CardCode] = E.[CardCode] AND A.[BillToDef] = E.[Address] AND E.[AdresType] = 'B'
					LEFT JOIN OHEM M ON A.[UserSign] = M.[userId]
		where A.[U_SubType]='S' and A.[CardType]='L';
END;
GO
