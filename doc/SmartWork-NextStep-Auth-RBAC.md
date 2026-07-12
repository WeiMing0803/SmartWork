# SmartWork 下一步计划：认证闭环 + RBAC 基础

## Context
后端 Clean Architecture 骨架与全部实体的外键配置已完成并落库（`FixDocumentHistoryFk` 迁移已 `database update`）。当前认证只实现了 `register` / `login`，缺刷新、登出、当前用户、改密；JWT 里没有 Role Claim，无法做 `[Authorize(Roles=...)]`；Controller 直接用 DbContext 且手写校验样板代码重复。本计划补齐认证闭环并引入 RBAC 基础，为后续 Workspace/Document/Task 业务模块铺路。

对应设计文档 Phase 1「基础系统」剩余部分 + Phase 2 入口。

## 现状关键文件
- `src/SmartWork.Infrastructure/Auth/AuthService.cs`：已有 Register/Login，签发 Access Token + 落库 RefreshToken（SHA256 哈希）。
- `src/SmartWork.Application/Auth/AuthDtos.cs`：已有 `RefreshTokenRequest` DTO 但未实现。
- `src/SmartWork.Domain/Entities/RefreshToken.cs`：已有 `RevokedAt` / `IsActive`。
- `src/SmartWork.API/Controllers/AuthController.cs`：每个 action 手写 `validator.ValidateAsync` 样板。
- `src/SmartWork.Infrastructure/Identity/ApplicationRole.cs`：已有 `ApplicationRole : IdentityRole<Guid>`，但未 seed、未签入 JWT。

## 实施步骤

### 1. 补齐 `IAuthService` / `AuthService`
在 `IAuthService` 新增：
- `Task<Result<AuthResponse>> RefreshAsync(RefreshTokenRequest request, CancellationToken ct)`
- `Task<Result> LogoutAsync(string refreshToken, CancellationToken ct)`
- `Task<Result<UserProfile>> GetProfileAsync(Guid userId, CancellationToken ct)`
- `Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken ct)`

`AuthService` 实现要点：
- **Refresh**：用 `AccessToken` 解出 `sub`(userId)（`JwtSecurityTokenHandler.ReadJwtToken`，不验签只取 claim），按 `HashToken(refreshToken)` 查 `RefreshTokens`，校验 `UserId 匹配 && IsActive`；通过后撤销旧 token（`RevokedAt = now`）并签发新对（旋转）。失败统一返回"无效的刷新令牌"。
- **Logout**：按 hash 查到未撤销的 token，置 `RevokedAt`。
- **GetProfile**：返回 `UserProfile(UserId, Email, UserName, DisplayName, AvatarUrl)`。
- **ChangePassword**：`UserManager.ChangePasswordAsync`（需要当前密码）；成功后撤销该用户所有 RefreshToken（强制重新登录）。
- 抽出 `CreateAuthResponseAsync` 已存在，复用。

### 2. JWT 加入 Role Claim
- `AuthService.CreateAccessToken`：查 `UserManager.GetRolesAsync(user)`，为每个 role 加 `Claim(ClaimTypes.Role, role)`。
- `Infrastructure/DependencyInjection.cs` 的 `AddJwtBearer`：`TokenValidationParameters.RoleClaimType = ClaimTypes.Role`。
- `Application/Common/Interfaces/ICurrentUserService`：加 `IReadOnlyCollection<string> Roles`，`CurrentUserService` 从 `ClaimTypes.Role` 读取。

### 3. 新增 DTO 与 Validator
- `AuthDtos.cs` 新增：`ChangePasswordRequest(CurrentPassword, NewPassword)`、`UserProfile(...)`。
- 新增 `ChangePasswordRequestValidator`：新密码 ≥8、与旧密码不同。

### 4. Controller 补接口 + 去样板
`AuthController` 新增：
- `POST /api/auth/refresh` → `RefreshAsync`
- `POST /api/auth/logout` → `LogoutAsync`（`[Authorize]`，从 body 取 refreshToken）
- `GET /api/auth/me` → `GetProfileAsync`（`[Authorize]`，用 `ICurrentUserService.UserId`）
- `POST /api/auth/change-password` → `ChangePasswordAsync`（`[Authorize]`）

去样板：引入一个轻量的 `ValidationFilter<T>` 或 `EndpointFilter`，让 `[HttpPost]` 自动跑 FluentValidation，失败返回 400。若改动过大可先保留手写样式，仅在新接口沿用——**优先小改动**，本步可只做"统一 `Result → IActionResult` 的扩展方法 `ToActionResult`"。

### 5. RBAC Seed
- `Infrastructure/Persistence/DbInitializer.cs`（新）：`InitializeAsync` 在应用启动时确保存在 `Admin / Manager / Member / Guest` 四个 `ApplicationRole`。
- `Program.cs`：开发环境调用 `DbInitializer.InitializeAsync`（用 `app.Services.CreateScope()`）。
- 暂不做细粒度 Permission 表（设计文档提到的 `Permission/RolePermission` 留到 Phase 3）。

### 6. 全局异常处理
- `API/Middleware/ExceptionHandlingMiddleware.cs`（新）：捕获未处理异常，返回统一 `{ errors }`，Serilog 记录。`Program.cs` 最前注册。

### 7. 迁移与验证
- 上述改动不涉及实体结构变化（Role 表 Identity 已建），**无需新迁移**。若 `DbInitializer` 仅插入角色行，也不需要迁移。
- 验证见下。

## 不在本计划范围
- Workspace CRUD 下沉到 Application 层（下一步单独做）。
- 细粒度 Permission 表、AuditLog 自动写入、Notification 来源关联。
- 前端。

## 验证
1. `dotnet build SmartWork.sln` —— 0 错误。
2. `dotnet run --project src/SmartWork.API`，打开 Scalar（`/scalar`）按顺序调：
   - `POST /api/auth/register` → 拿到 accessToken/refreshToken。
   - `GET /api/auth/me`（带 Bearer）→ 返回用户资料。
   - `POST /api/auth/refresh` → 用旧 accessToken + refreshToken 换新对；再用旧 refreshToken 刷新应失败（已旋转）。
   - `POST /api/auth/change-password` → 改密成功；旧 accessToken 仍在有效期内但 refresh 已全部撤销，刷新应失败。
   - `POST /api/auth/login` 用新密码登录成功。
   - `POST /api/auth/logout` → 撤销当前 refresh；再刷新应失败。
3. 检查数据库 `AspNetRoles` 有 4 行；`RefreshTokens` 表 `RevokedAt` 在登出/旋转/改密后被正确写入。
