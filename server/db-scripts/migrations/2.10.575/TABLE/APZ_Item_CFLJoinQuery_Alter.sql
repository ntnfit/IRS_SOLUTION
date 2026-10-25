
alter table APZ_Item_CFLJoinQuery add primary key ("Alias");

GO

delete from APZ_Item_CFLJoinQuery where "Alias" in ('f','g','k','l','a1','p','t1','v');

GO

insert into APZ_Item_CFLJoinQuery values ('f', 'LEFT JOIN OITB f ON A."ItmsGrpCod" = f."ItmsGrpCod"');

GO

insert into APZ_Item_CFLJoinQuery values ('g', 'LEFT JOIN OVTG g ON A."VatGourpSa" = g."Code"');

GO

insert into APZ_Item_CFLJoinQuery values ('k', 'LEFT JOIN OVTG k ON A."VatGroupPu" = k."Code"');

GO

insert into APZ_Item_CFLJoinQuery values ('l', 'LEFT JOIN OITT l ON l."Code" = A."ItemCode"');

GO

insert into APZ_Item_CFLJoinQuery values ('a1', 'left join UGP1 a1 on a."UgpEntry" = a1."UgpEntry" and a1."UomEntry" = a."IUoMEntry"');

GO

insert into APZ_Item_CFLJoinQuery values ('p', 'LEFT JOIN #tblPrice p ON a."ItemCode"=p."ItemCode" AND p."CardCode"=:CardCode');

GO

insert into APZ_Item_CFLJoinQuery values ('t1', 'Left Join (SELECT "ItemCode", "WhsCode", "OnHand", "IsCommited", "OnOrder" FROM OITW) t1 on a."ItemCode" = t1."ItemCode" and (CASE WHEN  a."DfltWH" <>'''' THEN a."DfltWH" else :WhsDefault end) = t1."WhsCode"');