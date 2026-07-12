# SmartWork 后端

SmartWork 是一个基于 **.NET 10 + Clean Architecture** 的智能工作协同平台后端，提供用户认证授权（JWT + Refresh Token + RBAC）、工作空间、文档管理、任务协作、文件资产、通知与审计等能力。

## 技术栈

| 分类 | 技术 |
|---|---|
| 运行时 | .NET 10 (ASP.NET Core) |
| 语言 | C# 14 / Nullable enable |
| 架构 | Clean Architecture（Domain → Application → Infrastructure → API） |
| ORM | Entity Framework Core 10 + SQL Server |
| 认证 | ASP.NET Core Identity + JWT Bearer |
| 校验 | FluentValidation |
| 日志 | Serilog（Console / File） |
| 后台任务 | Hangfire（SQL Server 存储） |
| 缓存 | StackExchange.Redis（可选） |
| API 文档 | OpenAPI + Scalar |

## 项目结构

```
SmartWork/
├── src/
│   ├── SmartWork.Domain/            领域层：实体、值对象、领域契约（不依赖任何层）
│   ├── SmartWork.Application/       应用层：用例契约、DTO、验证器、接口（依赖 Domain）
│   ├── SmartWork.Infrastructure/    基础设施层：EF Core、Identity、JWT、Hangfire、Redis（依赖 Application、Domain）
│   ├── SmartWork.API/               表现层：控制器、中间件、DI 编排（依赖三层）
│   └── SmartWork.Shared/            共享层：通用结果类型（Result）
├── doc/                             项目文档
│   ├── SmartWork-Project-Design.md        项目设计
│   ├── SmartWork-Backend-Setup.md         后端搭建说明
│   └── SmartWork-NextStep-Auth-RBAC.md    下一步：认证与 RBAC 规划
├── Directory.Build.props            统一编译属性（net10.0 / C# 14 / Nullable）
├── global.json                      SDK 版本约束
└── SmartWork.sln
```

### 分层依赖方向

```
API  →  Infrastructure  →  Application  →  Domain
                                        ↘
                                          Shared（被各层引用）
```

- **Domain**：纯领域模型，零外部依赖。
- **Application**：定义「做什么」——接口（`IXxxService`）、DTO、FluentValidation 验证器。
- **Infrastructure**：定义「怎么做」——服务实现、`ApplicationDbContext`、Identity、JWT 签发、外部客户端。
- **API**：控制器、中间件、`Program.cs` 编排 DI 与中间件管道。

## 领域实体

`SmartWork.Domain/Entities/`：

| 实体 | 说明 |
|---|---|
| `Workspace` / `WorkspaceMember` | 工作空间及其成员关系 |
| `Document` / `DocumentTag` / `DocumentHistory` | 文档、标签、变更历史 |
| `TaskItem` / `TaskComment` | 任务与任务评论 |
| `FileAsset` | 文件资产 |
| `Notification` | 通知 |
| `AuditLog` | 操作审计日志（表已建，写入逻辑待实现） |
| `RefreshToken` | 刷新令牌（哈希存储） |

所有实体继承 `BaseEntity`（含 `Id`、`CreatedAt` 等公共字段）。

## 认证机制

- **登录**：`POST /api/auth/login` 校验密码后签发 `AccessToken` + `RefreshToken`。
- **AccessToken**：JWT，默认有效期 **60 分钟**（`Jwt:AccessTokenMinutes`）。
- **RefreshToken**：64 字节随机串哈希存库，默认有效期 **14 天**（`Jwt:RefreshTokenDays`），一次性使用、自动旋转。
- **刷新**：`POST /api/auth/refresh` 需同时携带旧的 `AccessToken`（仅解析取 `sub`，不验期）与 `RefreshToken`，成功返回新的一对。
- **角色**：基于 ASP.NET Core Identity 的 `ApplicationRole`，开发环境由 `DbInitializer` 初始化默认角色。

## 快速开始

### 环境要求

- .NET 10 SDK（见 `global.json`，`rollForward: latestFeature`）
- SQL Server（开发默认用 LocalDB：`(localdb)\MSSQLLocalDB`）
- （可选）Redis：`localhost:6379`

### 配置

主要配置在 `src/SmartWork.API/appsettings.json`：

- `ConnectionStrings:DefaultConnection` —— 数据库连接串
- `ConnectionStrings:Redis` —— Redis 连接（留空则不启用）
- `Jwt:SigningKey` / `Issuer` / `Audience` / `AccessTokenMinutes` / `RefreshTokenDays` —— JWT 参数
- `Serilog` —— 日志级别与输出目标（Console / File）

### 数据库迁移

```bash
cd src/SmartWork.API
dotnet ef database update
```

### 运行

```bash
cd src/SmartWork.API
dotnet run
```

启动后：

- 根路径 `/` 在开发环境自动跳转到 Scalar 文档页
- **API 文档**：`https://localhost:<端口>/scalar`
- **OpenAPI JSON**：`https://localhost:<端口>/openapi/v1.json`
- **健康检查**：`https://localhost:<端口>/health`
- **Hangfire 仪表盘**（开发环境）：`https://localhost:<端口>/jobs`

### 信任开发证书（首次）

```bash
dotnet dev-certs https --trust
```

## API 概览

认证相关端点（`/api/auth`）：

| 方法 | 路径 | 鉴权 | 说明 |
|---|---|---|---|
| POST | `/api/auth/register` | 匿名 | 注册 |
| POST | `/api/auth/login` | 匿名 | 登录，返回 token 对 |
| POST | `/api/auth/refresh` | 匿名 | 刷新 token |
| POST | `/api/auth/logout` | Bearer | 登出，撤销 refresh token |
| GET  | `/api/auth/me` | Bearer | 获取当前用户资料 |
| POST | `/api/auth/change-password` | Bearer | 修改密码（改后吊销所有 refresh token） |

## 日志

Serilog 默认输出到控制台；文件 sink 在 `appsettings.json` 中以注释形式保留为开关，取消注释即可按天滚动写入 `logs/smartwork-YYYYMMDD.log`。

## 相关文档

- [项目设计](doc/SmartWork-Project-Design.md)
- [后端搭建说明](doc/SmartWork-Backend-Setup.md)
- [下一步：认证与 RBAC 规划](doc/SmartWork-NextStep-Auth-RBAC.md)
