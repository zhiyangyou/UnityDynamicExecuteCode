using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class DynamicRoslynCompiler
{
    private static string[] _activeScriptCompilationDefines = null;

    private static string[] ActiveScriptCompilationDefines
    {
        get
        {
            if (_activeScriptCompilationDefines == null)
            {
                _activeScriptCompilationDefines = EditorUserBuildSettings.activeScriptCompilationDefines;
            }

            return _activeScriptCompilationDefines;
        }
    }

    private static List<string> _depDllPaths = null;

    private static List<string> GetCurDomainRefs(List<string> excludeAssyNames)
    {
        if (_depDllPaths != null && _depDllPaths.Count > 0)
        {
            return _depDllPaths;
        }

        _depDllPaths = new List<string>();
        foreach (var assembly in AppDomain.CurrentDomain
                     .GetAssemblies() //TODO: PERF: just need to load once and cache? or get assembly based on changed file only?
                     .Where(a => excludeAssyNames.All(assyName => !a.FullName.StartsWith(assyName))))
        {
            try
            {
                if (string.IsNullOrEmpty(assembly.Location)) // 动态程序集会触发这个
                {
                    continue;
                }

                _depDllPaths.Add(assembly.Location);
            }
            catch (Exception)
            {
                Debug.LogError(
                    $"Unable to add a reference to assembly as unable to get location or null: {assembly.FullName} when hot-reloading, this is likely dynamic assembly and won't cause issues");
            }
        }

        return _depDllPaths;
    }

    private static PortableExecutableReference[] GetCurDomainExecutableRefs(List<string> execludeDllPaths)
    {
        return GetCurDomainRefs(execludeDllPaths)
            .Select(dllPath => PortableExecutableReference.CreateFromFile(dllPath))
            .ToArray();
    }

    public static void CompileAndRun(string code)
    {
        const string asmName = "DynamicAssembly";
        List<string> listExcludeAsmName = new List<string>()
        {
            asmName
        };
        // 宏定义
        var parseSyntaxTree = new CSharpParseOptions();
        parseSyntaxTree.WithPreprocessorSymbols(ActiveScriptCompilationDefines);

        // 解析代码为语法树
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code, parseSyntaxTree);

        var compileOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        var dllRefs = GetCurDomainExecutableRefs(listExcludeAsmName);


        SyntaxTree[] syntaxTrees = new[] { syntaxTree };

        // 设置编译选项
        CSharpCompilation compilation = CSharpCompilation.Create(asmName, syntaxTrees, dllRefs, compileOptions);

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
            Debug.Log($"dynamic call ret is {ret}");
        }
    }
}


public class DynamicExecuteCode : MonoBehaviour
{
    static void CostTime(Action ac, string name = "")
    {
        var sw = new Stopwatch();
        sw.Start();
        try
        {
            ac();
        }
        catch (Exception e)
        {
            Debug.LogError($" Do {name} error ");
            Debug.LogException(e);
        }
        finally
        {
            sw.Stop();
            Debug.Log($"Do {name} cost time: {sw.ElapsedMilliseconds}ms");
        }
    }

    static string GetRealAssetPath(string assetPath)
    {
        var basePath = Application.dataPath.Replace("Assets", "");
        return Path.Combine(basePath, assetPath);
    }

    static void DoExecute()
    {
        string filePath = GetRealAssetPath("Assets/Scripts/DynamicExecuteCode/TemplateCode.cs");
        string code = "";
        CostTime(() => { code = File.ReadAllText(filePath); }, $"加载代码文件:{filePath}");
        CostTime(() => { DynamicRoslynCompiler.CompileAndRun(code); }, $"动态编译:{filePath}");
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(200, 200, 200, 200), "动态执行代码"))
        {
            DoExecute();
        }
    }

    void Update()
    {
        // Debug.LogError("  333  ");
    }
}