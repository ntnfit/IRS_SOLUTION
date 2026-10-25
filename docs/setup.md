# Thiết lập và chạy local

## Prerequisites

- Windows và Visual Studio 2022 Build Tools hoặc Visual Studio có .NET Framework 4.6.2 Developer Pack.
- SQL Server 2022 hoặc bản tương thích và `sqlcmd`.
- IIS Express.
- SAP Business One client/DI API tương thích.
- Runtime và assembly có license của DevExpress 20.1 và SAP Crystal Reports.

Các assembly thương mại không nằm trong repository. Đặt dependency client tại `client/lib/` và dependency
server tại `server/lib/`; danh sách chính xác được khai báo bằng `HintPath` trong các file `.csproj`.

## Build

```powershell
dotnet restore client/ApzonIRS.sln
dotnet build client/ApzonIRS.sln --no-restore

dotnet restore server/ApiIRS.sln
dotnet build server/ApiIRS.sln --no-restore
```

Nếu máy không có Developer Pack, xem [troubleshooting](troubleshooting.md#msb3644-net-framework-462).

## Chạy API dev

1. Dựng database theo [`database.md`](database.md).
2. Lấy bộ RSA và passphrase từ kho bí mật nội bộ, rồi đặt vào `IRS_RSA_PRIVATE_KEY`,
   `IRS_RSA_PUBLIC_KEY` và `IRS_RSA_PASSWORD_PHRASE`. Không ghi các giá trị này vào file trong repository.
3. Chạy `server/run-dev.ps1` một lần để tạo `.dev-site/Web.config` từ template đã loại credential.
4. Dùng `server/tools/IrsConfigTool` để điền các giá trị cấu hình; công cụ tự mã hóa các key mà API yêu cầu.
   Với SQL Server, `DatabaseSystemType` là `0`.
5. Khởi động lại `server/run-dev.ps1` và kiểm tra URL được script in ra.

`run-dev.ps1` dùng port cố định `18011` theo mặc định và từ chối chạy nếu port đã có owner, tránh tạo server trùng.

## Chạy client

Build output chính là `client/src/Apzon.Forms/bin/Debug/net462/Apzon.Forms.exe`.
API URL được nhập/cấu hình ở màn hình đăng nhập. Database và domain được API trả về từ registry database,
không được hard-code trong client.

## Email tùy chọn

Luồng gửi email của SlackBot chỉ hoạt động khi có đủ `IRS_SMTP_SENDER`, `IRS_SMTP_PASSWORD` và
`IRS_SMTP_RECIPIENT`. Nếu không cấu hình, API trả thông báo thiếu SMTP thay vì dùng credential hard-code.
Thông báo Slack cần thêm `IRS_SLACK_WEBHOOK_URL`; URL webhook không được lưu trong source hoặc config đã commit.
Controller Firebase legacy đọc `IRS_FIREBASE_SERVER_KEY` từ kho bí mật thay vì source. Endpoint legacy hiện trả
HTTP 404 và cần được chuyển sang FCM HTTP v1/service account trước khi dùng lại; mọi key từng public phải revoke.
