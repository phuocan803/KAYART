# Hướng Dẫn Dùng COMMIT_CONVENTIONS.md
>
> **Hướng dẫn sử dụng file quy ước commit theo định dạng CONVENTIONS COMMITS**
>
## File Này Là Gì?

`COMMIT_CONVENTIONS.md` là tài liệu quy định cách viết commit message theo định dạng CONVENTIONS COMMITS.

## Khi Nào Cần Đọc?

### 1. **Lần Đầu Tham Gia Dự Án**

- Đọc toàn bộ file để hiểu quy ước
- Đặc biệt chú ý phần "Types" và "Scopes"

### 2. **Trước Khi Commit**

- Tham khảo nhanh phần "Ví Dụ Thực Tế"
- Kiểm tra format: `<type>(<scope>): <description>`

### 3. **Khi Không Chắc Chắn**

- Tìm ví dụ tương tự trong file
- Xem phần "Anti-Patterns" để tránh sai

## Cách Sử Dụng Nhanh

### Bước 1: Xác định Type

Hỏi bản thân: "Tôi đang làm gì?"

- Thêm tính năng → `feat`
- Sửa bug → `fix`
- Sửa tài liệu → `docs`
- Sửa UI → `style`
- Refactor → `refactor`

### Bước 2: Xác định Scope

Hỏi bản thân: "Phần nào bị ảnh hưởng?"

- Authentication → `auth`
- Drawing canvas → `canvas`
- Chat → `chat`
- Database → `db`

### Bước 3: Viết Description

- Ngắn gọn, rõ ràng
- Tiếng Anh
- Dùng động từ: "add", "fix", "update"

### Bước 4: Commit

```bash
git commit -m "feat(auth): add user registration "
```

## Checklist Nhanh

Trước khi commit, check:

- [ ] Type đúng? (`feat`, `fix`, `docs`...)
- [ ] Scope đúng? (`auth`, `canvas`, `chat`...)
- [ ] Description rõ ràng?
- [ ] Tiếng Anh ?

## Tips

### Tip 1: Bookmark Các Ví Dụ

Tìm phần "Ví Dụ Thực Tế" trong file, save lại để tham khảo nhanh.

### Tip 2: Copy Template

```
<type>(<scope>): <description>
```

Paste vào notepad, điền vào, rồi copy commit.

### Tip 3: Khi Rối

Xem file `GIT_QUICK_GUIDE.md` - phiên bản ngắn gọn hơn.

## Tìm Kiếm Nhanh

### Tìm Type phù hợp

Ctrl+F → tìm "Types"

### Tìm Scope phù hợp

Ctrl+F → tìm "Scopes"

### Tìm ví dụ

Ctrl+F → tìm "Ví Dụ Thực Tế"

## Cấu Trúc File

```
COMMIT_CONVENTIONS.md
├─ Quy Tắc Chung
├─ Types 
├─ Scopes
├─ Nguyên Tắc Viết Description
├─ Ví Dụ Thực Tế
│  ├─ Feature Commits
│  ├─ Fix Commits
│  ├─ Docs Commits
│  ├─ Style Commits
│  ├─ Refactor Commits
│  ├─ Performance Commits
│  └─ Chore Commits
├─ Anti-Patterns 
├─ Checklist
└─ Tools Hỗ Trợ
```

## Quick Reference

### Most Common Types

```bash
feat(auth): add user registration
fix(canvas): fix login with special characters in email
docs(readme): update instruction
style(ui): enhanced user interface
```

### Most Common Scopes

- `auth` - Authentication
- `canvas` - Drawing
- `chat` - Chat system
- `db` - Database
- `ui` - User interface

## Khi Cần Giúp Đỡ

1. **Đọc lại file** `COMMIT_CONVENTIONS.md`
2. **Xem file ngắn** `GIT_QUICK_GUIDE.md`
3. **Nhắn Zalo hỏi nhóm** Sẽ có hỗ trợ nè!!!
4. **Tham khảo** [Conventional Commits](https://www.conventionalcommits.org/)

## Tóm Tắt

1. Đọc file lần đầu để hiểu quy ước
2. Tham khảo nhanh khi commit
3. Follow format: `<type>(<scope>): <description>`
4. Xem ví dụ khi không chắc
5. Check checklist trước khi commit
