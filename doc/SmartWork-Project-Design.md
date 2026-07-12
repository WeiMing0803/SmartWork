# SmartWork 项目设计文档

## 1. 项目介绍

### 项目名称

SmartWork

### 项目定位

SmartWork 是一个结合 **Notion + Jira** 思想的企业级协作管理平台。

目标：

* 知识管理
* 文档协作
* 项目任务管理
* 团队协作
* 文件管理
* 权限控制

核心理念：

> 使用 Notion 的知识管理能力 + Jira 的项目管理能力，打造一个轻量级团队工作平台。

---

# 2. 技术栈设计

# 2.1 后端技术栈

## 核心框架

| 技术           | 版本 | 用途        |
| ------------ | -- | --------- |
| .NET         | 10 | Web API开发 |
| ASP.NET Core | 10 | 后端接口框架    |
| C#           | 14 | 主要开发语言    |

---

## 数据访问层

### Entity Framework Core

用途：

* ORM
* Code First开发
* 数据库迁移

数据库：

```
SQL Server
```

开发方式：

```
Entity Class

↓

EF Core Migration

↓

SQL Server Database
```

---

## 身份认证

### ASP.NET Core Identity

用途：

* 用户管理
* 用户注册
* 用户登录
* 密码管理
* RBAC权限

数据库：

```
AspNetUsers

AspNetRoles

AspNetUserRoles

AspNetUserClaims

```

认证方式：

```
JWT Access Token

+

Refresh Token
```

认证流程：

```
Vue

↓

用户名密码

↓

.NET API

↓

Identity验证

↓

生成JWT

↓

返回Token

```

---

## 日志系统

### Serilog

用途：

* 请求日志
* 异常日志
* 用户操作日志

记录：

```
API访问

错误异常

用户行为

系统事件

```

---

## 缓存

### Redis

用途：

* 用户信息缓存
* 热点数据缓存
* Token管理
* 临时数据存储

例如：

```
User Profile

Workspace List

Document Cache

```

---

## 后台任务

### Hangfire

用途：

执行后台任务：

例如：

* 定时清理数据
* 发送通知
* 生成统计数据

任务：

```
Scheduler

↓

Background Worker

↓

Database Update

```

---

## 数据验证

### FluentValidation

用途：

请求参数验证。

例如：

用户注册：

```
Email不能为空

密码长度>=8

用户名唯一
```

---

## API文档

### Scalar

用途：

* API测试
* 接口文档
* API交互式调试
* OpenAPI 可视化展示

---

# 2.2 前端技术栈

## Vue3

用途：

前端框架。

---

## TypeScript

用途：

* 类型安全
* 提高大型项目维护性

---

## Vite

用途：

前端构建工具。

---

## UI组件库

### Element Plus

用途：

后台管理系统UI组件。

包含：

* Table
* Form
* Dialog
* Menu
* Pagination

---

## 状态管理

### Pinia

用途：

管理：

* 用户状态
* Token
* Workspace信息

---

## 路由

### Vue Router

用途：

页面导航。

例如：

```
/dashboard

/document

/task

/member

```

---

## HTTP请求

### Axios

用途：

调用.NET API。

功能：

* Token自动携带
* 请求拦截
* 错误处理

---

# 2.3 部署技术

## Docker

用途：

容器化。

镜像：

```
Frontend Image

Backend Image

Redis Image

SQL Server Image

```

---

## Kubernetes

用途：

生产环境部署。

部署结构：

```
                Nginx Ingress

                      |

        ----------------------------

        |                          |

 Vue Container             .NET API Container


                      |

              ----------------

              |              |

          SQL Server       Redis

```

---

# 3. 系统功能设计

# 3.1 用户中心

## 功能

* 用户注册
* 用户登录
* 修改密码
* JWT认证
* Refresh Token
* 用户资料

数据库：

```
User

Role

Permission

```

---

# 3.2 Dashboard 首页

功能：

## 欢迎区域

显示：

* 用户信息
* 日期
* 工作空间

---

## 快捷操作

按钮：

```
新建文档

创建任务

上传文件

邀请成员

```

---

## 数据统计

展示：

```
文档数量

任务数量

成员数量

文件数量

```

---

## 最近动态

显示：

```
谁创建了文档

谁完成了任务

谁上传了文件

```

---

# 3.3 Workspace 工作空间

类似 Notion Space。

功能：

* 创建空间
* 删除空间
* 修改空间
* 邀请成员

结构：

```
Workspace

    |

    |---- Document

    |---- Task

    |---- File

    |---- Member

```

数据库：

```
Workspace

WorkspaceMember

```

---

# 3.4 Document 文档管理

类似 Notion。

功能：

## 文档列表

支持：

* 搜索
* 分类
* 标签

---

## 文档编辑

支持：

* Markdown
* 图片
* 代码块
* 链接

---

## 文档历史

保存：

```
版本号

修改人

修改时间

```

数据库：

```
Document

DocumentTag

DocumentHistory

```

---

# 3.5 Task 任务管理

类似 Jira。

功能：

## 看板模式

状态：

```
TODO

IN_PROGRESS

REVIEW

DONE

```

---

## 创建任务

字段：

```
任务名称

描述

负责人

优先级

截止日期

状态

```

---

## 任务详情

包含：

* 评论
* 附件
* 操作记录

数据库：

```
Task

TaskComment

TaskAttachment

```

---

# 3.6 文件管理

类似 Google Drive。

功能：

* 上传文件
* 下载文件
* 删除文件
* 文件分类

存储：

开发：

```
Local Storage
```

生产：

```
AWS S3
```

数据库：

```
File

```

---

# 3.7 成员管理

功能：

* 查看成员
* 邀请成员
* 修改角色

角色：

```
Admin

Manager

Member

Guest

```

---

# 3.8 日历管理

功能：

展示：

* 任务截止时间
* 工作安排
* 项目计划

---

# 3.9 通知中心

功能：

通知：

```
任务完成

评论提醒

成员邀请

系统消息

```

数据库：

```
Notification

```

---

# 3.10 搜索中心

全局搜索：

搜索：

```
文档

任务

文件

成员

```

---

# 3.11 系统设置

功能：

## 用户设置

* 修改头像
* 修改昵称

## 安全设置

* 修改密码
* Token管理

## 系统设置

* 主题
* 通知配置

---

# 3.12 管理后台

功能：

## 用户管理

* 查看用户
* 禁用用户
* 修改角色

## 权限管理

配置：

```
Role

Permission

```

## 系统日志

查看：

```
登录日志

操作日志

异常日志

```

---

# 4. 数据库核心实体

```
User

Role

Permission


Workspace

WorkspaceMember


Document

DocumentTag

DocumentHistory


Task

TaskComment


File


Notification


AuditLog

```

---

# 5. 后端项目结构

推荐 Clean Architecture：

```
SmartWork

src

├── SmartWork.API

├── SmartWork.Application

├── SmartWork.Domain

├── SmartWork.Infrastructure

└── SmartWork.Shared

```

---

# 6. 前端页面结构

```
src

├── views

│
├── Dashboard

├── Workspace

├── Document

├── Task

├── File

├── Member

├── Calendar

├── Setting


├── components

├── router

├── stores

├── api

└── utils

```

---

# 7. 项目开发阶段

## Phase 1 基础系统

完成：

* 用户注册登录
* JWT认证
* 权限管理
* Dashboard

---

## Phase 2 核心业务

完成：

* Workspace
* Document
* Task

---

## Phase 3 企业能力

增加：

* Redis
* Serilog
* Hangfire
* AuditLog
* 文件上传

---

## Phase 4 部署

完成：

* Docker
* Kubernetes
* CI/CD

---

# 项目最终目标

打造一个：

> 基于 .NET 10 + Vue3 + Kubernetes 的企业级知识管理与项目协作平台。

技术能力覆盖：

* Web API设计
* 身份认证
* RBAC权限
* ORM设计
* 缓存
* 日志
* 后台任务
* 容器化部署
* 云原生架构
