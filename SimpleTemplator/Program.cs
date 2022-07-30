using Markdig;
using Markdig.Syntax;

var projectPath = "/home/milleniumbug/dokumenty/PROJEKTY/InDevelopment/website";//Directory.GetCurrentDirectory();
var distPath = Path.Combine(projectPath, "dist");
var srcPath = Path.Combine(projectPath, "src");
var markdownPipeline = new MarkdownPipelineBuilder()
    .UseAdvancedExtensions()
    .Build();

if (args.ElementAtOrDefault(0) == "init")
{
    
}
else// if(args.ElementAtOrDefault(0) == "build")
{
    try
    {
        Directory.Delete(distPath, recursive: true);
    }
    catch (DirectoryNotFoundException)
    {
        // if the directory is not found, success
    }

    Directory.CreateDirectory(distPath);
    
    var template = File.ReadAllText(Path.Combine(srcPath, "_template.html"));
    foreach (var fileInfo in new DirectoryInfo(srcPath).EnumerateFiles("*", SearchOption.AllDirectories))
    {
        var relativePath = fileInfo.FullName.Remove(0, srcPath.Length + 1);
        var targetPath = Path.Combine(distPath, relativePath);
        Console.WriteLine(relativePath);
        if (fileInfo.Name.StartsWith("_", StringComparison.Ordinal))
        {
            continue;
        }
        else if (fileInfo.Extension == ".md")
        {
            var root = RootIndirect(relativePath);
            var main = Markdown.ToHtml(File.ReadAllText(fileInfo.FullName), markdownPipeline);
            string title;
            if (root == ".")
            {
                title = "";
            }
            else if (Path.GetFileNameWithoutExtension(fileInfo.Name) != "index")
            {
                title = Path.GetFileNameWithoutExtension(fileInfo.Name);
            }
            else
            {
                title = Path.GetFileNameWithoutExtension(fileInfo.Directory?.Name) ?? "";
            }
                
            var targetDirectoryPath = Path.GetDirectoryName(targetPath);
            if (targetDirectoryPath != null)
            {
                Directory.CreateDirectory(targetDirectoryPath);
            }
            
            var menu = Markdown.ToHtml(File.ReadAllText(Path.Combine(srcPath, "_menu.md")).Replace("$ROOT$", root), markdownPipeline);

            File.WriteAllText(
                Path.ChangeExtension(targetPath, ".html"),
                template
                    .Replace("$MAIN$", main)
                    .Replace("$TITLE$", title)
                    .Replace("$MENU$", menu));
        }
        else 
        {
            File.Copy(fileInfo.FullName, targetPath);
        }
    }
}



string RootIndirect(string p)
{
    var t = new List<string>();
    string? dir = p;
    while ((dir = Path.GetDirectoryName(dir)) != "")
    {
        t.Add("..");
    }

    if (t.Count == 0)
    {
        return ".";
    }
    return string.Join(Path.DirectorySeparatorChar.ToString(), t);
}