# SmartWork 后端项目搭建说明

## 本次已完成

根据 `SmartWork-Project-Design.md`，已先搭建后端 Clean Architecture 基础架构，暂未处理前端。

项目结构：

```text
SmartWork
├── src
│   ├── SmartWork.API
│   ├── SmartWork.Application
│   ├── SmartWork.Domain
│   ├── SmartWork.Infrastructure
│   └── SmartWork.Shared
├── Directory.Build.props
├── global.json
└── SmartWork.sln
```

当前包含：

- `SmartWork.Domain`：核心实体、枚举、通用 `BaseEntity`。
- `SmartWork.Application`：应用层接口、认证 DTO、FluentValidation 校验器。
- `SmartWork.Infrastructure`：EF Core、Identity、JWT、Redis、Hangfire 的基础注册。
- `SmartWork.API`：Web API 入口、Serilog、Scalar/OpenAPI、健康检查、认证接口、Workspace 示例接口。
- `SmartWork.Shared`：通用 `Result` 返回模型。

## 已放入的核心能力

- Clean Architecture 分层。
- SQL Server + EF Core Code First 基础。
- ASP.NET Core Identity 用户体系。
- JWT Access Token 与 Refresh Token 数据结构。
- FluentValidation 请求参数校验。
- Serilog 请求日志与文件日志。
- Redis 连接注册。
- Hangfire 后台任务服务与开发环境 Dashboard。
- Scalar API 文档入口。

## 目前已有接口

- `GET /health`：健康检查。
- `POST /api/auth/register`：用户注册。
- `POST /api/auth/login`：用户登录。
- `POST /api/auth/refresh`：刷新令牌（令牌旋转，旧 refresh 立即失效）。
- `POST /api/auth/logout`：撤销当前刷新令牌。
- `GET /api/auth/me`：获取当前用户资料。
- `POST /api/auth/change-password`：修改密码（改密后撤销该用户所有刷新令牌）。
- `GET /api/workspaces`：获取当前用户工作空间。
- `POST /api/workspaces`：创建工作空间。


## 构建验证

已在本机执行 `dotnet build SmartWork.sln`，结果：构建成功，0 个错误。

当前仍有 NuGet 安全警告，来自间接依赖：

- `System.Security.Cryptography.Xml 9.0.0`：由后端依赖链带入。
- `Newtonsoft.Json 11.0.1`：由 API 文档相关依赖链带入。

后续可以通过升级对应上游包或替换相关包版本处理。

## 后续建议步骤

1. 安装或确认本机 `.NET 10 SDK`。
2. 还原 NuGet 包并执行构建：`dotnet restore`、`dotnet build`。
3. 准备 SQL Server 和 Redis，可先用 Docker Compose。
4. 创建第一版数据库迁移：`dotnet ef migrations add InitialCreate -p src/SmartWork.Infrastructure -s src/SmartWork.API`。
5. 执行数据库更新：`dotnet ef database update -p src/SmartWork.Infrastructure -s src/SmartWork.API`。
6. 补齐 Refresh Token 刷新、退出登录、Token 撤销逻辑。
7. 补齐 RBAC 权限模型：Role、Permission、RolePermission、UserRole 初始化。
8. 继续实现 Workspace、Document、Task、File、Notification、AuditLog 等核心业务模块。
9. 增加 Application 单元测试、API 集成测试、Infrastructure 数据访问测试。
10. 增加 Docker Compose：SmartWork.API、SQL Server、Redis。

## 注意事项

- `appsettings.json` 中的数据库密码和 JWT SigningKey 当前是开发占位值，正式开发前需要改成自己的本地配置。
- 当前骨架以“基础架构可扩展”为目标，业务逻辑只实现了认证和 Workspace 的最小入口。
- 如果 NuGet 包版本和本机 SDK 不匹配，优先统一到本机安装的 `.NET 10 SDK` 对应版本。

## 编码约定

- 后续 C# 代码尽量不使用 `var`，优先使用明确类型，方便阅读和学习。
- 只有在匿名类型、极复杂泛型或显式类型会明显降低可读性时，才考虑例外。
- 使用最新的 C# 语法（项目当前为 C# 14 / .NET 10），优先采用：`file-scoped namespace`、`collection-expression`（如 `[]`、`..`）、`primary constructor`、`record` / `record struct`、`required` 成员、`init` 只读属性、`pattern matching`（含 `is`/`switch` 表达式与属性模式）、`target-typed new()`、`global using`、`nullable reference type`。在保证可读性的前提下，用新语法替代等价的旧写法。


## Account
- "userName": "Wei",
- "password": "Aa123456!"
- "email": "2570016347@qq.com",