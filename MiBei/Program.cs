using HtmlAgilityPack;
using System.Net;
using System.Diagnostics;

// 获取最新的节点网页
string url = "https://www.mibei77.com/search/label/jiedian"; // 设置获取的 url

HttpClient client = new HttpClient();
string html = await client.GetStringAsync(url);

HtmlDocument doc = new HtmlDocument();
doc.LoadHtml(html);

// 获取所有符合条件的article标签
HtmlNodeCollection articleNodes = doc.DocumentNode.SelectNodes("//article[contains(@class, 'blog-post')]");
if (articleNodes != null && articleNodes.Count > 0)
{
    // 获取第一个article标签
    HtmlNode firstArticleNode = articleNodes[0];

    // 获取<div class="entry-header">标签中的<h2 class="entry-title">的a标签
    HtmlNode aNode = firstArticleNode.SelectSingleNode(".//div[contains(@class, 'entry-header')]/h2[contains(@class, 'entry-title')]/a");
    if (aNode != null && aNode.Attributes["href"] != null) // 设置当有 href 内容时的逻辑
    {
        string link = aNode.Attributes["href"].Value; // 获取 href 的值，即网页链接

        string linkTextFilePath = "link.txt"; // 设置要写入的文件
        File.WriteAllText(linkTextFilePath, link); // 写入到文件里

        Console.WriteLine("链接已写入文件：" + linkTextFilePath); // 显示链接写入输出
    }
}
else
{
    Console.WriteLine("未找到符合条件的article标签"); // 当没有article 标签时显示未找到
}


//获取节点网页中的链接
string currentDirectory = Environment.CurrentDirectory; // 获取软件本体地址
string linkFilePath = Path.Combine(currentDirectory, "link.txt"); // 拼接 link.txt 的地址

try
{
    // 读取链接文件内容
    string link = File.ReadAllText(linkFilePath);

    // 发起 HTTP 请求并获取网页内容
    WebClient webClient = new WebClient(); // 启动 WebClient
    string htmlContent = webClient.DownloadString(link);

    // 使用 HtmlAgilityPack 解析 HTML
    var htmlDocument = new HtmlDocument();
    htmlDocument.LoadHtml(htmlContent);

    // 查找目标 <p> 标签
    HtmlNode targetPTag = null;
    var pTags = htmlDocument.DocumentNode.SelectNodes("//p");
    if (pTags != null)
    {
        foreach (HtmlNode pTag in pTags)
        {
            if (pTag.InnerHtml.Contains("v2ray订阅链接,不需要开代理"))
            {
                targetPTag = pTag;
                break;
            }
        }
    }

    // 输出目标 <p> 标签后面一个 <p> 标签内的内容
    if (targetPTag != null && targetPTag.NextSibling != null && targetPTag.NextSibling.Name.Equals("p", StringComparison.OrdinalIgnoreCase))
    {
        string result = targetPTag.NextSibling.InnerHtml;
        // Console.WriteLine(result);

        // 写入文件
        string nodeFilePath = "node.txt"; // 设置 node.txt 的地址
        File.WriteAllText(nodeFilePath, result); // 把获取到的更新链接 (result) 写入到 node.txt

        Console.WriteLine("链接已写入文件：" + nodeFilePath);

        Console.WriteLine("最新的更新链接是：" + result);
    }
    else
    {
        Console.WriteLine("未找到指定内容或后续标签不是 <p> 标签。");
    }
}
catch (Exception ex)
{
    Console.WriteLine("发生错误：" + ex.Message); // 显示错误信息，一般用不到
}


// 下载node.txt中的服务器
string softwareDirectory = Environment.CurrentDirectory; // 获取软件本体地址
string nodeTextFilePath = Path.Combine(softwareDirectory, "node.txt"); // 设置 node.txt 文件地址
string nodeLinkFilePath = Path.Combine(softwareDirectory, "node_link.txt"); // 设置 node_link.txt 文件地址

if (File.Exists(nodeTextFilePath)) // 检测 node.txt 文件存在
{
    string fileContent = File.ReadAllText(nodeTextFilePath); // 读取 node.txt 文件地址
    Uri uriResult;
    bool isUrl = Uri.TryCreate(fileContent, UriKind.Absolute, out uriResult) &&
                 (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    if (isUrl)
    {
        DownloadFile(fileContent, nodeLinkFilePath);
    }
    else
    {
        Console.WriteLine("node.txt文件中的内容不是有效的URL链接。"); // 几乎用不到，只是备用而已
    }
}
else
{
    Console.WriteLine("找不到node.txt文件。");
}

string v2rayN = "start v2rayN.exe"; // 设置启动 v2rayN 的 Shell 命令
// 执行命令
Process.Start(new ProcessStartInfo("cmd.exe", $"/C {v2rayN}") { CreateNoWindow = true });

string node = "node.txt"; // 设置打开 node.txt 的命令，锁定 Windows 本地原生 notepad 记事本打开
// 执行命令
Process.Start(new ProcessStartInfo("notepad.exe", $"{node}"));

// 退出程序
Console.WriteLine("执行完毕，按下 Enter 关闭程序");
Console.ReadLine(); // 暂停程序



static void DownloadFile(string url, string fileName)
{
    using (WebClient client = new WebClient())
    {
        try
        {
            client.DownloadFile(url, fileName);
            Console.WriteLine("文件下载成功！");
        }
        catch (Exception ex)
        {
            Console.WriteLine("下载文件时发生错误：" + ex.Message);
        }
    }
}
