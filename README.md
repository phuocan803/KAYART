# KayArt - Collaborative Drawing Application

## Tổng Quan

KayArt là ứng dụng vẽ và cộng tác nghệ thuật thời gian thực, cho phép nhiều người dùng vẽ cùng lúc trên một canvas.

## Tính Năng

## Góc Nhìn Người Dùng

- Cho phép đăng nhập, đăng ký, khôi phục mật khẩu của tài khoản
- Tạo và tham gia phòng với bản vẽ mới
- Tham gia phòng có sẵn
- Hiển thị bạn bè và danh sách người dùng trong cùng một phòng
- Cho phép trò chuyện trực tuyến trong phòng
- Sử dụng các công cụ cơ bản và nâng cao để vẽ và thiết kế.
- Đồng bộ nét vẽ khi người dùng mới tham gia vào phòng có sẵn
- Xuất bản vẽ và lưu vào máy người dùng.

## Góc nhìn hệ thống

- Ứng dụng AI hỗ trợ vẽ và chỉnh sửa hình ảnh
- Tính năng real-time trong phòng vẽ
- Cơ chế xác thực bảo mật với JWT, Firebase và reCAPTCHA
- Lưu trữ dữ liệu bằng SQL Server kết hợp Redis để tối ưu cache
- Quản lý secret an toàn thông qua Vault/Google Secret Manager

## Kiến Trúc

- **Client**: .NET 8.0
- **Server**: .NET 8.0
- **Database**: SQL Server hoặc SQL Local
- **Cache**: Redis
- **AI**:
- **Verify**: JWT, Firebase và reCAPTCHA
- **Secret Management**: Vault/Google Secret Manager

## Cài Đặt

### Yêu Cầu

- .NET 8.0 SDK
- SQL Server (LocalDB hoặc SQL Server)
- Visual Studio 2022 (recommended)

### Các Bước

1. Clone repository
2. Cài đặt NuGet packages
3. Cấu hình database connection trong [Server/appsettings.json]
4. Chạy database scripts trong thư mục `Database/`
5. Chạy Server trước, sau đó chạy Client

## Documentation

Coming soon...

## Nhóm Phát Triển
