# License

IRS giữ nguyên enforcement license: client import file, API kiểm tra và SQL Server lưu trạng thái license.
Không có bypass hoặc nhánh “luôn hợp lệ” trong gói bàn giao.

## Vị trí source

- Client UI: [`../client/src/Apzon.Forms/Apzon.Forms.Administrator/frm_LicenseManager.cs`](../client/src/Apzon.Forms/Apzon.Forms.Administrator/frm_LicenseManager.cs).
- API controller: [`../server/src/Apzon.Api/Apzon.Api.Controllers.Administrator/LicenseManagerController.cs`](../server/src/Apzon.Api/Apzon.Api.Controllers.Administrator/LicenseManagerController.cs).
- Repository: [`../server/src/Apzon.Services/Apzon.Services.Repository.Administrator/LicenseManagerRepository.cs`](../server/src/Apzon.Services/Apzon.Services.Repository.Administrator/LicenseManagerRepository.cs).
- SQL Server save procedure: [`database/APZ_LICENSEMANAGER_SAVELIC.sql`](database/APZ_LICENSEMANAGER_SAVELIC.sql).
- Công cụ config/QC: [`../server/tools/IrsConfigTool`](../server/tools/IrsConfigTool/).

## QC license local

Sau khi build server, `IrsConfigTool` hỗ trợ `hardware-key`, `create-qc-license` và `verify-license`.
QC license được ràng hardware key, version và ngày hết hạn; chỉ dùng cho kiểm thử local. File sinh ra có đuôi
`.lic` và bị `.gitignore` loại khỏi Git.

Trước khi chạy công cụ, nạp `IRS_RSA_PRIVATE_KEY`, `IRS_RSA_PUBLIC_KEY` và `IRS_RSA_PASSWORD_PHRASE` từ
kho bí mật nội bộ vào biến môi trường của process. Repository public không chứa và không sinh thay bộ key này.

```powershell
server/tools/IrsConfigTool/bin/Debug/net462/IrsConfigTool.exe hardware-key
server/tools/IrsConfigTool/bin/Debug/net462/IrsConfigTool.exe create-qc-license .local/irs-qc.lic
server/tools/IrsConfigTool/bin/Debug/net462/IrsConfigTool.exe verify-license .local/irs-qc.lic
```

Import file qua màn hình License Manager. Procedure SQL phải được triển khai trước. Production license phải đi
qua quy trình phê duyệt của chủ sở hữu hệ thống; không commit private key, license đã cấp hoặc hardware key.
