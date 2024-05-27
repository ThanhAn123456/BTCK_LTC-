# BTCK_LTC#

## Mục lục

- [Giới thiệu](#giới-thiệu)
- [Chức năng chính](#chức-năng-chính)
- [Yêu cầu hệ thống](#yêu-cầu-hệ-thống)
- [Cài đặt](#cài-đặt)
- [Cấu hình cơ sở dữ liệu](#cấu-hình-cơ-sở-dữ-liệu)
- [Tính năng](#tính-năng)
- [Cấu trúc dự án](#cấu-trúc-dự-án)
- [Đóng góp](#đóng-góp)
- [Liên hệ](#liên-hệ)

## Giới thiệu

Nguyễn Thành An

MSV: 21115053120201

Lớp học phần: 223LTC04

Đây là dự án quản lý nội dung công ty đơn giản sử dụng ASP.NET Core MVC và SQL Server theo phương pháp Database First.

## Chức năng chính

- Quản lý thông tin Công ty, Phòng ban, Chức vụ, Nhân viên, Bài đăng, Danh mục (Thêm, Sửa, Xóa, Xem)
- Tìm kiếm và lọc Công ty, Phòng ban, Chức vụ, Nhân viên, Bài đăng, Danh mục theo nhiều điều kiện khác nhau
- Upload ảnh, bài đăng
- Phân quyền theo chức vụ

## Yêu cầu hệ thống

- .NET Core SDK 3.1 trở lên
- SQL Server 2016 trở lên
- Visual Studio 2019 hoặc bất kỳ IDE nào hỗ trợ .NET Core
- Trình duyệt web hiện đại (Chrome, Firefox, Edge)

## Các package cần cài đặt

Dưới đây là danh sách các package NuGet cần cài đặt cho dự án:

- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.EntityFrameworkCore.Tools`
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
- `Microsoft.VisualStudio.Web.CodeGeneration.Design`
- `System.IdentityModel.Tokens.Jwt`
- `X.PagedList.Mvc.Core`

Để cài đặt các package này, bạn có thể sử dụng lệnh sau trong Package Manager Console hoặc thông qua giao diện quản lý NuGet trong Visual Studio:

```bash
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.Tools
...
```
## Cài đặt

1. **Clone repository:**

    ```bash
    git clone https://github.com/ThanhAn123456/BTCK_LTC-.git
    cd BTCK_LTC#
    ```

2. **Khôi phục các gói NuGet:**

    ```bash
    dotnet restore
    ```

3. **Cấu hình chuỗi kết nối đến SQL Server trong file `appsettings.json`:**

    ```json
    "ConnectionStrings": {
      "QuanLyBaiDangCongTyConnection": "Data Source=YourServerName;Initial Catalog=YourDatabaseName;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False"
    }
    ```

4. **Tạo mô hình từ cơ sở dữ liệu hiện có:**

    Trong Visual Studio, mở Package Manager Console và chạy lệnh sau:

    ```bash
    Scaffold-DbContext "Data Source=YourServerName;Initial Catalog=YourDatabaseName;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models
    ```
Lệnh này sẽ tạo các lớp mô hình và ngữ cảnh (DbContext) dựa trên cơ sở dữ liệu hiện có.

5. **Chạy ứng dụng:**

    ```bash
    dotnet run
    ```

6. **Mở trình duyệt và truy cập:**

    ```
    http://localhost:your_port
    ```

## Cấu hình cơ sở dữ liệu

Đảm bảo rằng SQL Server của bạn đang chạy và bạn đã tạo một cơ sở dữ liệu với các bảng cần thiết. Cập nhật thông tin kết nối trong `appsettings.json` với thông tin cụ thể của bạn.

## Tính năng
   #### Phân quyền
  -Có Login, logout
  
  -Có phân quyền truy cập các trang theo chức vụ
   #### Trang chủ
  -Hiển thị các bài đăng

  -Cho phép tìm kiếm, lọc bài đăng
  
  -Nhấn vào các bài đăng để xem chi tiết 
  #### Bài đăng
  -Quản lý bài đăng gồm thêm, xóa, sửa.
  
  -Tìm kiếm và lọc bài đăng

  -Có upload ảnh và thêm nội dung bài đăng bằng thư viện Ckeditor
  #### Công ty
  -Quản lý công ty gồm thêm, xóa, sửa.
  
  -Tìm kiếm công ty
  #### Phòng ban
  -Quản lý phòng ban gồm thêm, xóa, sửa.
  
  -Tìm kiếm phòng ban
  #### Chức vụ
  -Quản lý chức vụ gồm thêm, xóa, sửa.
  
  -Tìm kiếm chức vụ
  #### Danh mục
  -Quản lý danh mục gồm thêm, xóa, sửa.
  
  -Tìm kiếm danh mục
  #### Nhân viên
  -Quản lý nhân viên gồm thêm, xóa, sửa.
  
  -Tìm kiếm nhân viên

## Cấu trúc dự án

- **Controllers:** Chứa các controller điều khiển luồng dữ liệu và xử lý yêu cầu từ người dùng.
- **Models:** Chứa các lớp mô hình đại diện cho dữ liệu của ứng dụng, được tạo từ cơ sở dữ liệu bằng cách sử dụng Database First.
- **Views:** Chứa các file giao diện (Razor Pages) để hiển thị dữ liệu cho người dùng.
- **wwwroot:** Chứa các file tĩnh như CSS, JavaScript và hình ảnh.

## Đóng góp

Chào mừng các đóng góp từ cộng đồng! Nếu bạn có ý tưởng, phát hiện lỗi hoặc cải tiến, hãy mở một issue hoặc tạo một pull request.

## Liên hệ

Nếu có bất kỳ câu hỏi hoặc ý kiến đóng góp nào, vui lòng liên hệ:

- **Email:** vacnguyen16092003@gmail.com
- **Số điện thoại:** 0335399907
- **Facebook:** https://www.facebook.com/profile.php?id=100022935258438
