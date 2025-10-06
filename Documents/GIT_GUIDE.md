# Hướng Dẫn Sử Dụng Git
>
> **Hướng dẫn nhanh cho team KayArt**
>
## Quy Tắc 5 Bước

### 1. Pull Main Trước Khi Làm

```bash
git checkout main
git pull origin main
git checkout feature/your-branch
git merge main
```

### 2️. Commit Khi Hoàn Thành

- Hoàn thành 1 chức năng
- Sửa xong 1 bug
- Code đã test, chạy được
- KHÔNG commit khi làm dở

### 3️. Viết Message Chuẩn

```bash
<type>(<scope>): <short description (English)>
```

### 4️. Push Vào Branch Của Mình

```bash
git push origin feature/your-branch
```

### 5️. Tạo Pull Request → Review → Merge

- Không push trực tiếp vào main
- Phải có approve từ team

---

## Conventional Commits

>
> **Giới thiệu Git theo format Conventional Commits**
>

### Format

```
<type>(<scope>): <description>
```

### Types

| Type | Dùng Khi | Ví Dụ |
|------|----------|-------|
| `feat` | Thêm tính năng | `feat(auth): add JWT login` |
| `fix` | Sửa bug | `fix(canvas): fix drawing sync issue` |
| `docs` | Sửa tài liệu | `docs(readme): update installation guide` |
| `style` | Sửa UI/UX | `style(login): improve login UI` |
| `refactor` | Refactor code | `refactor(db): split UserRepository` |
| `perf` | Tối ưu performance | `perf(redis): optimize cache lookup` |
| `test` | Thêm test | `test(auth): add unit tests for login` |
| `chore` | Maintenance | `chore(deps): update packages` |

### Scopes

`auth` `canvas` `chat` `collab` `db` `ui` `network` `redis` `ai` `config`
---

## Ví Dụ Tốt

```bash
feat(auth): add user registration
fix(canvas): fix drawing sync issue when multiple users
docs(readme): add database installation guide
style(login): improve login form UI
refactor(db): split UserRepository out of Manager
perf(redis): optimize cache lookup with pipeline
```

## Ví Dụ Tệ

```bash
update                    # Quá chung chung
fix bug                   # Không rõ bug gì
them tinh nang            # Không dấu
WIP                       # Work in progress
asdf                      # Vô nghĩa
```

---

## Checklist Trước Commit

- [ ] Code đã test và chạy được
- [ ] Không có lỗi build
- [ ] Message theo format `type(scope): description (English)`
- [ ] Tiếng Anh
- [ ] Ngắn gọn, rõ ràng

---

## Workflow Nhanh

```bash
# 1. Pull main
git checkout main && git pull origin main
# 2. Chuyển branch
git checkout feature/your-branch
# 3. Merge main vào branch
git merge main
# 4. Code...
# 5. Commit
git add .
git commit -m "feat(auth): "
# 6. Push
git push origin feature/your-branch
# 7. Tạo Pull Request trên GitHub
```

---

## Tips

1. **Commit nhỏ, commit thường** - Dễ review
2. **1 commit = 1 chức năng** - Không commit dồn
3. **Test trước khi commit** - Đảm bảo code chạy
4. **Message rõ ràng** - Người khác đọc hiểu ngay
5. **Pull thường xuyên** - Tránh conflict lớn

---

## Tham Khảo

- [Conventional Commits](https://www.conventionalcommits.org/)
- File chi tiết: `COMMIT_CONVENTIONS.md`
