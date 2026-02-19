using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

Console.WriteLine("UI Tester starting...");
var baseUrl = new Uri("http://localhost:5019");
var handler = new HttpClientHandler { AllowAutoRedirect = false, UseCookies = true, CookieContainer = new CookieContainer() };
using var client = new HttpClient(handler) { BaseAddress = baseUrl };
try
{
    var get = await client.GetAsync("/Account/Register");
    var html = await get.Content.ReadAsStringAsync();
    var m = Regex.Match(html, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");
    if (!m.Success) { Console.WriteLine("Could not find antiforgery token on register page"); return 1; }
    var token = WebUtility.HtmlDecode(m.Groups[1].Value);
    var email = $"test.user.{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}@example.test";
    var form = new List<KeyValuePair<string,string>>() {
        new("__RequestVerificationToken", token),
        new("Input.FirstName","Test"),
        new("Input.LastName","User"),
        new("Input.Email", email),
        new("Input.Password","P@ssw0rd123!"),
        new("Input.ConfirmPassword","P@ssw0rd123!"),
        new("Input.DateOfBirth", DateTime.UtcNow.AddYears(-30).ToString("yyyy-MM-dd")),
        new("Input.PhoneNumber","555-0100"),
        new("Input.Address","123 Test St")
    };
    var content = new FormUrlEncodedContent(form);
    var post = await client.PostAsync("/Account/Register", content);
    Console.WriteLine($"POST status: {(int)post.StatusCode} {post.StatusCode}");
    if ((int)post.StatusCode == 302)
    {
        Console.WriteLine($"Registered redirect: {post.Headers.Location}");
        try {
            System.IO.File.WriteAllText(Path.Combine("Tools","last_registered_email.txt"), email);
            Console.WriteLine($"Wrote registered email to Tools/last_registered_email.txt: {email}");
        } catch (Exception ex) {
            Console.WriteLine($"Failed writing registered email file: {ex.Message}");
        }
    }
    else
    {
        var body = await post.Content.ReadAsStringAsync();
        Console.WriteLine("Registration response body:");
        Console.WriteLine(body);
    }

    // Follow to home and check signed-in state
    var home = await client.GetAsync("/");
    var homeHtml = await home.Content.ReadAsStringAsync();
    if (homeHtml.Contains(email) || Regex.IsMatch(homeHtml, "Logout|Sign out|Sign Out", RegexOptions.IgnoreCase))
    {
        Console.WriteLine($"SIGNED_IN: {email}");
    }
    else
    {
        Console.WriteLine("SIGNED_IN_NOT_DETECTED");
        if (Regex.IsMatch(homeHtml, "Logout|Sign out|Sign Out", RegexOptions.IgnoreCase)) Console.WriteLine("Logout link present (likely signed in)");
        else Console.WriteLine("No logout link detected");

        // Attempt to login programmatically
        Console.WriteLine("Attempting login...");
        var loginGet = await client.GetAsync("/Account/Login");
        var loginHtml = await loginGet.Content.ReadAsStringAsync();
        var lm = Regex.Match(loginHtml, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");
        var loginToken = lm.Success ? WebUtility.HtmlDecode(lm.Groups[1].Value) : string.Empty;
        var loginForm = new List<KeyValuePair<string,string>>() {
            new("__RequestVerificationToken", loginToken),
            new("Input.Email", email),
            new("Input.Password", "P@ssw0rd123!"),
            new("Input.RememberMe","false")
        };
        var loginContent = new FormUrlEncodedContent(loginForm);
        var loginPost = await client.PostAsync("/Account/Login", loginContent);
        Console.WriteLine($"LOGIN POST status: {(int)loginPost.StatusCode} {loginPost.StatusCode}");
        if ((int)loginPost.StatusCode == 302) Console.WriteLine($"LOGIN redirect: {loginPost.Headers.Location}");
        var home2 = await client.GetAsync("/");
        var home2Html = await home2.Content.ReadAsStringAsync();
        if (home2Html.Contains(email) || Regex.IsMatch(home2Html, "Logout|Sign out|Sign Out", RegexOptions.IgnoreCase))
        {
            Console.WriteLine($"SIGNED_IN_AFTER_LOGIN: {email}");
        }
        else
        {
            Console.WriteLine("SIGNED_IN_NOT_DETECTED_AFTER_LOGIN");
        }

        // Try to access patient booking pages
        var bookPage = await client.GetAsync("/patient/book-appointment");
        Console.WriteLine($"GET /patient/book-appointment -> {(int)bookPage.StatusCode} {bookPage.StatusCode}");
        var historyPage = await client.GetAsync("/patient/appointment-history");
        Console.WriteLine($"GET /patient/appointment-history -> {(int)historyPage.StatusCode} {historyPage.StatusCode}");
    }
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine("Exception during UI test:");
    Console.WriteLine(ex);
    return 2;
}