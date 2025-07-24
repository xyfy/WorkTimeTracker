#!/bin/bash

# GitHub Actions 工作流验证脚本

echo "🔍 验证 GitHub Actions 工作流..."
echo "=================================="

WORKFLOW_DIR=".github/workflows"
ERROR_COUNT=0

# 检查工作流目录
if [ ! -d "$WORKFLOW_DIR" ]; then
    echo "❌ 工作流目录不存在: $WORKFLOW_DIR"
    exit 1
fi

# 验证每个工作流文件
for workflow in "$WORKFLOW_DIR"/*.yml; do
    if [ -f "$workflow" ]; then
        filename=$(basename "$workflow")
        echo ""
        echo "📁 检查工作流: $filename"
        
        # 检查文件是否为空
        if [ ! -s "$workflow" ]; then
            echo "❌ 文件为空"
            ((ERROR_COUNT++))
            continue
        fi
        
        # 基本YAML语法检查
        if grep -q "^name:" "$workflow" && grep -q "^on:" "$workflow" && grep -q "^jobs:" "$workflow"; then
            echo "✅ 基本结构正确"
        else
            echo "❌ 缺少必需的顶级字段 (name, on, jobs)"
            ((ERROR_COUNT++))
        fi
        
        # 检查缩进
        if grep -qP "^\t" "$workflow"; then
            echo "⚠️  发现制表符，建议使用空格缩进"
        fi
        
        # 检查常见问题
        if grep -q "uses: actions/" "$workflow"; then
            echo "✅ 使用官方Actions"
        fi
        
        if grep -q "runs-on:" "$workflow"; then
            echo "✅ 指定了运行环境"
        fi
        
        # 统计信息
        lines=$(wc -l < "$workflow")
        size=$(wc -c < "$workflow")
        echo "📊 文件信息: $lines 行, $size 字节"
        
    fi
done

echo ""
echo "=================================="
if [ $ERROR_COUNT -eq 0 ]; then
    echo "✅ 所有工作流验证通过！"
    echo ""
    echo "📋 工作流摘要:"
    echo "- CI/CD Pipeline: 自动编译、测试、发布"
    echo "- PR Validation: Pull Request 验证"
    echo "- Release Build: 发布构建"
    echo "- Maintenance: 定期维护任务"
    echo "- Code Quality: 代码质量检查"
    echo ""
    echo "🚀 GitHub Actions 已配置完成！"
else
    echo "❌ 发现 $ERROR_COUNT 个错误，请检查修复"
    exit 1
fi
