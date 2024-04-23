using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

public class DynamicRoslynCompiler
{
    public static List<PortableExecutableReference> CollectUnityCompileEnvDll()
    {
        var ret = new List<PortableExecutableReference>();
        
        return ret;
    }

    public static void CompileAndRun(string code)
    {
        // 解析代码为语法树
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);

        var refFrameWork = MetadataReference.CreateFromFile(typeof(Framework.Facade).GetTypeInfo().Assembly.Location);
        PortableExecutableReference refNetStandard21 =
            MetadataReference.CreateFromFile("F:\\_UnityWorkSpace\\UnityInstall\\2023.1.17f1\\Editor\\Data\\NetStandard\\ref\\2.1.0\\netstandard.dll");
        // 设置编译选项
        CSharpCompilation compilation = CSharpCompilation.Create(
            "DynamicAssembly",
            new[] { syntaxTree },
            new[]
            {
                MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).GetTypeInfo().Assembly.Location),
                refFrameWork,
                refNetStandard21,
            },
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        // 编译代码
        using (var ms = new MemoryStream())
        {
            EmitResult result = compilation.Emit(ms);

            if (!result.Success)
            {
                // 输出编译错误
                var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);
                foreach (var diagnostic in failures)
                {
                    Debug.LogError($"compile Error : {diagnostic.Id}: {diagnostic.GetMessage()}");
                }

                return;
            }

            ms.Seek(0, SeekOrigin.Begin);

            // 加载编译后的程序集
            Assembly assembly = Assembly.Load(ms.ToArray());

            // 使用反射执行编译后的代码
            Type type = assembly.GetType("DynamicNamespace.DynamicClass");
            MethodInfo method = type.GetMethod("DynamicMethod");
            var ret = method.Invoke(null, null);
            Debug.LogError($"ret is {ret}");
        }
    }
}


public class DynamicExecuteCode : MonoBehaviour
{
    static void DoExecute()
    {
        var text = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Scripts/DynamicExecuteCode/TemplateCode.cs");
        string code = text.text;
        DynamicRoslynCompiler.CompileAndRun(code);
    }

    void Update()
    {
        DoExecute();
    }
}