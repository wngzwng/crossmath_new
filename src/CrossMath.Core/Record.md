```shell
# 添加 logger

# 核心日志接口
dotnet add package Microsoft.Extensions.Logging

# 日志提供者（至少需要一个提供者才能输出日志）
dotnet add package Microsoft.Extensions.Logging.Console      # 控制台日志
dotnet add package Microsoft.Extensions.Logging.Debug      # 调试输出日志

# 可选的其他日志提供者
dotnet add package Microsoft.Extensions.Logging.EventLog  # Windows 事件日志
dotnet add package Serilog.Extensions.Logging             # Serilog 日志提供者
dotnet add package NLog.Extensions.Logging                # NLog 日志提供者
```

### 全局难度全1面板
#### 记忆深度

特殊玩法：含有一个 ？ 
面板的备选答案不全部提供出来，而是有个格子为？，但还是要保证面板的全局唯一