
Create procedure APZ_PeriodAndVolumeDiscounts_GetItems
(in fCode nvarchar(50) default '',
in tCode nvarchar(50) default '',
in fVendor nvarchar(50) default '',
in tVendor nvarchar(50) default '') 
As 
Begin 
select "ItemCode" from OSPP  where 1=1 and
(IFNULL(:fCode, 'N') = 'N' or "ItemCode" = :fCode) and
(IFNULL(:tCode,'N') ='N' or "ItemCode"=:tCode) and
(IFNULL(:fVendor,'N') = 'N' or "CardCode"=:fVendor)and
(IFNULL(:tVendor, null) = null or "CardCode" >= :tVendor) ;
end;




