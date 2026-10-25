# Troubleshooting

## MSB3644: .NET Framework 4.6.2

Cài .NET Framework 4.6.2 Developer Pack. Nếu reference assemblies đã có trong NuGet cache, có thể truyền
`TargetFrameworkRootPath` trỏ tới thư mục `Microsoft.NETFramework.ReferenceAssemblies.net462` khi build.
Không retarget solution chỉ để né lỗi toolchain.

## Login không có domain/database

Kiểm tra API đã chạy, `IRS-COMMON` đã được tạo và company database đã đăng ký bằng
[`server/tools/dev-db`](../server/tools/dev-db/). Client không tự dò SQL Server.

## DISERVERSERVICE.ASMX không lắng nghe

Đây là endpoint SAP DI Server, không phải ApiIRS. Nếu môi trường dùng Service Layer/DI API thay cho DI Server,
cấu hình đúng service type/address bằng `IrsConfigTool`; không trỏ endpoint này về ApiIRS.

## Crystal Report trắng hoặc không load

Xác nhận Crystal runtime đúng kiến trúc tiến trình, assembly nằm trong `client/lib/`, template `.rpt` tồn tại
trong thư mục company và procedure datasource đã được triển khai. POS templates thuộc
[`client/src/Apzon.Forms/Report`](../client/src/Apzon.Forms/Report/); server dev publish chúng qua `run-dev.ps1`.

## Port 18011 đang bận

Xác định PID đang giữ port và tái sử dụng hoặc dừng đúng process của project. Không tăng port tùy ý vì client
có thể vẫn trỏ tới endpoint cũ.
