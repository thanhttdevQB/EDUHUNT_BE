using EDUHUNT_BE.Data;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenAI.Chat;
using System.ClientModel;
using EDUHUNT_BE.DTOs;
using EDUHUNT_BE.Interfaces;
using EDUHUNT_BE.Models;
using System.Text;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;

public class Url
{
    public string URL { get; set; }
    public int ParentPage { get; set; }
    public string CssSelector { get; set; }
}

public class ScholarshipRepository : IScholarship
{
    private readonly AppDbContext _dbContext;
    private readonly string _apiKey;
    private readonly ISurvey _surveyRepository;
    private readonly IScholarshipInfoes _scholarshipInfoes;

    public ScholarshipRepository(AppDbContext dbContext, IConfiguration configuration, ISurvey surveyRepository, IScholarshipInfoes scholarshipInfoes)
    {
        _dbContext = dbContext;
        _apiKey = configuration["ChatGPT:Key"];
        _surveyRepository = surveyRepository;
        _scholarshipInfoes = scholarshipInfoes;
    }

    public async Task<List<ScholarshipInfo>> GetRecommendedScholarships(string userId)
    {
        var userSurvey = await _surveyRepository.GetByUserId(userId);
        if (userSurvey == null)
        {
            return new List<ScholarshipInfo>();
        }
        var allScholarships = await _scholarshipInfoes.GetScholarshipInfo();
        var rankedScholarships = await RankScholarshipsWithChatGPT(userSurvey, allScholarships);
        int topN = 5;
        return rankedScholarships.Take(topN).ToList();
    }

    private async Task<List<ScholarshipInfo>> RankScholarshipsWithChatGPT(SurveyDto userSurvey, IEnumerable<ScholarshipInfo> allScholarships)
    {
        const int batchSize = 50;
        var batches = allScholarships.Select((s, i) => new { Scholarship = s, Index = i })
                                     .GroupBy(x => x.Index / batchSize)
                                     .Select(g => g.Select(x => x.Scholarship).ToList())
                                     .ToList();

        var resultList = new ConcurrentBag<List<ScholarshipInfo>>();

        await Parallel.ForEachAsync(batches, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, async (batch, token) =>
        {
            var result = await ProcessBatch(userSurvey, batch);
            resultList.Add(result);
        });

        return resultList.SelectMany(x => x).ToList();
    }

    private async Task<List<ScholarshipInfo>> ProcessBatch(SurveyDto userSurvey, List<ScholarshipInfo> scholarshipBatch)
    {

        string prompt = PreparePromptForChatGPT(userSurvey, scholarshipBatch);
        ChatClient client = new(model: "gpt-4-1106-preview", new ApiKeyCredential(_apiKey));
        ChatCompletion completion = await client.CompleteChatAsync(prompt);
        string response = completion.ToString();
        var result = ParseChatGPTResponse(response, scholarshipBatch);

        return result;
    }

    private string PreparePromptForChatGPT(SurveyDto userSurvey, List<ScholarshipInfo> scholarships)
    {
        StringBuilder promptBuilder = new StringBuilder();
        promptBuilder.AppendLine("Based on the following user survey and list of scholarships, please rank the scholarships from most suitable to least suitable. Return the result as a comma-separated list of scholarship IDs in order of suitability.");

        promptBuilder.AppendLine("\nUser Survey:");
        foreach (var answer in userSurvey.SurveyAnswers)
        {
            promptBuilder.AppendLine($"- {answer.Question}: {answer.Answer}");
        }

        promptBuilder.AppendLine("\nScholarships:");
        foreach (var scholarship in scholarships)
        {
            promptBuilder.AppendLine($"- ID: {scholarship.Id}");
            promptBuilder.AppendLine($"  Title: {scholarship.Title}");
            promptBuilder.AppendLine($"  Budget: {scholarship.Budget}");
            promptBuilder.AppendLine($"  Location: {scholarship.Location}");
            promptBuilder.AppendLine($"  School: {scholarship.SchoolName}");
            promptBuilder.AppendLine($"  Level: {scholarship.Level}");
            promptBuilder.AppendLine();
        }

        return promptBuilder.ToString();
    }

    private List<ScholarshipInfo> ParseChatGPTResponse(string response, List<ScholarshipInfo> scholarshipBatch)
    {
        var scholarshipIds = response.Split(',').Select(id => id.Trim());
        var scholarshipDict = scholarshipBatch.ToDictionary(s => s.Id.ToString());
        return scholarshipIds
            .Where(id => scholarshipDict.ContainsKey(id))
            .Select(id => scholarshipDict[id])
            .ToList();
    }

    public async Task<List<ScholarshipDto>> GetScholarships()
    {
        List<Url> scholarshipLinks = new List<Url>
        {
            new Url { URL = "https://www.scholarshipportal.com/", ParentPage = 1, CssSelector = "main" },
            new Url { URL = "https://www.rmit.edu.vn/study-at-rmit/scholarships/international-student-scholarships", ParentPage = 1, CssSelector = ".body-gridcontent.responsivegrid" }
        };

        using (var driver = new ChromeDriver())
        {
            SearchGoogle(driver);
            var scholarships = await Scrapingdata(driver, scholarshipLinks);

            // Add logic to save scholarships to the database if needed
            // ...

            return scholarships;
        }


        // Add logic to save scholarships to the database if needed
        // ...

    }
    //public async Task<List<ScholarshipDto>> GetScholarships()
    //{
    //    using (var driver = new ChromeDriver())
    //    {
    //        var scholarships = await _scrapingService.ScrapeScholarships();

    //        // Add logic to save scholarships to the database if needed
    //        // ...

    //        return scholarships;
    //    }
    //}






    public Task AddScholarship(ScholarshipDto scholarship)
    {
        // Implement logic to add scholarship to your data source (e.g., database)
        // ...

        // Return a completed task since there's no asynchronous operation
        return Task.CompletedTask;
    }

    private List<ScholarshipDto> ParseHtml(string html)
    {
        // Implement logic to parse the HTML and extract scholarship information
        // ...

        // For illustration purposes, return an empty list
        return new List<ScholarshipDto>();
    }

    private async Task<ScholarshipDto> UseOpenAIPrompt(string html, string url)
    {
        bool isFailed = false;
        while (true)
        {
            try
            {
                Console.WriteLine($"This is oke: {url}");
                ScholarshipDto newscholarship = new ScholarshipDto();
                string prompt = $"I'm a software developer. I need you to get the data from a scholarship page by reading the html code of the page: {html}." +
                    " Please read those, get for me the budget, title, location, universityName, level, and categories of the scholarship in this page." +
                    " Return it as a string follow the format: key: value; key: value,...; " +
                    " the keys have to be: budget, title, location, universityName, level, categories. Categories should include specific fields or special identifiers that help recognize the scholarship." +
                    " Not answer anything else so that I can easily manipulate the information.";

                /* var openAI = new OpenAIAPI("");
                var chat = openAI.Chat.CreateConversation();
                chat.Model = OpenAI_API.Models.Model.GPT4_Turbo;
                chat.AppendUserInput(prompt);
                string completions = await chat.GetResponseFromChatbotAsync();*/

                ChatClient client = new(model: "gpt-4-1106-preview", new ApiKeyCredential(_apiKey));
                ChatCompletion completion = client.CompleteChat(prompt);
                string completions = completion.ToString();

                Console.WriteLine("==========================this is GPT answer==========================");
                Console.WriteLine(completions);

                var budget = "";
                var title = "";
                var location = "";
                var university = "";
                var level = "";
                var categories = new List<string>();

                if (completions is not null)
                {
                    budget = Content(completions, "budget");
                    title = Content(completions, "title");
                    location = Content(completions, "location");
                    university = Content(completions, "university");
                    level = Content(completions, "level");
                    categories = Content(completions, "categories").Split(',').Select(c => c.Trim()).ToList();

                    Console.WriteLine("==========================this is Thanh answer==========================");
                    Console.WriteLine(budget);
                    Console.WriteLine(title);
                    Console.WriteLine(location);
                    Console.WriteLine(university);
                    Console.WriteLine(level);
                    Console.WriteLine(string.Join(", ", categories));
                }

                newscholarship = new ScholarshipDto
                {
                    Title = title,
                    Budget = budget,
                    Location = location,
                    School_name = university,
                    Level = level,
                    Url = url,
                    Id = Guid.NewGuid(),
                };

                // Save the ScholarshipInfo to the database
                var scholarshipInfo = new ScholarshipInfo
                {
                    Id = newscholarship.Id,
                    Title = newscholarship.Title,
                    Budget = newscholarship.Budget,
                    Location = newscholarship.Location,
                    SchoolName = newscholarship.School_name,
                    Level = newscholarship.Level,
                    Url = newscholarship.Url,
                    AuthorId = null,
                    IsInSite = false,
                    IsApproved = true,
                };

                _dbContext.ScholarshipInfos.Add(scholarshipInfo);
                await _dbContext.SaveChangesAsync();

                // Save categories to the database
                await SaveCategoriesToDatabase(categories, newscholarship.Id);

                return newscholarship;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UseOpenAIPromrt: {url} - {ex.Message}");
                if (isFailed) return new ScholarshipDto();
                await Task.Delay(60000);
                isFailed = true;
            }
        }
    }

    private async Task SaveCategoriesToDatabase(List<string> categories, Guid scholarshipId)
    {
        // Check if the ScholarshipId exists in the ScholarshipInfos table
        var scholarshipExists = await _dbContext.ScholarshipInfos.AnyAsync(s => s.Id == scholarshipId);
        if (!scholarshipExists)
        {
            throw new Exception("ScholarshipId does not exist in the ScholarshipInfos table.");
        }

        foreach (var categoryName in categories)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);
            if (category == null)
            {
                category = new Category { Name = categoryName };
                _dbContext.Categories.Add(category);
                await _dbContext.SaveChangesAsync();
            }

            var scholarshipCategory = new ScholarshipCategory
            {
                ScholarshipId = scholarshipId,
                CategoryId = category.Id
            };
            _dbContext.ScholarshipCategories.Add(scholarshipCategory);
        }
        await _dbContext.SaveChangesAsync();
    }

    private void SearchGoogle(ChromeDriver driver)
    {

        try
        {
            driver.Navigate().GoToUrl("https://www.google.com/");
            IWebElement textarea = driver.FindElement(By.ClassName("gLFyf"));
            //fill in textarea that " scholarshipportal "
            textarea.SendKeys("scholarshipportal");
            //enter
            textarea.SendKeys(Keys.Enter);
            //wait for 2 seconds
            Thread.Sleep(2000);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
    private async Task<List<ScholarshipDto>> Scrapingdata(ChromeDriver driver, List<Url> scholarshipLinks)
    {
        var chromeOptions = new ChromeOptions();
        chromeOptions.AddArgument("--ignore-certificate-errors-spki-list");
        chromeOptions.AddArgument("--ignore-certificate-errors");

        List <ScholarshipDto> scholarships = new List<ScholarshipDto>();
        try
        {
            List<string> scholarshipDivs = new List<string>();

            foreach(Url url in scholarshipLinks)
            {

            }

            foreach (Url urls in scholarshipLinks)
            {
                scholarshipDivs = (await ProcessData(driver, urls.URL, urls.ParentPage, urls.CssSelector));
                scholarships.AddRange(await ScrapingProcessedData(driver, scholarshipDivs, urls.CssSelector));
            }

            // For illustration purposes, return an empty list
            return scholarships;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return new List<ScholarshipDto>();
        }
        finally
        {
            // This will only close the main window, not the additional tabs
            driver.Quit();
        }
    }

    private string Content(string completions, string title)
    {
        var index = completions.IndexOf(title);
        var mainIndex = completions.IndexOf(':', index) + 1;
        var length = completions.IndexOf(';', index) - mainIndex;

        return completions.Substring(mainIndex, length);
    }

    private async Task<List<ScholarshipDto>> ScrapingProcessedData(ChromeDriver driver, List<string> scholarshipDivs, string css)
    {
        List<ScholarshipDto> scholarships = new List<ScholarshipDto>();
        var count = 0;
        foreach (var scholarshipDiv in scholarshipDivs)
        {
            var lastDivs = scholarshipDivs.Last();
            var scholarshipUrl = scholarshipDiv;

            ((IJavaScriptExecutor)driver).ExecuteScript($"window.open('{scholarshipUrl}', '_blank');");

            driver.SwitchTo().Window(driver.WindowHandles[^1]);

            var mainElement = driver.FindElement(By.CssSelector(css));
            Console.WriteLine( $"css: {css} {scholarshipUrl}");
            var scholarship = await UseOpenAIPrompt(mainElement.GetAttribute("outerHTML"), scholarshipUrl);

            scholarships.Add(
                    new ScholarshipDto
                    {
                        Budget = scholarship.Budget,
                        Title = scholarship.Title,
                        Level = scholarship.Level,
                        School_name = scholarship.School_name,
                        Url = scholarship.Url,
                        Location = scholarship.Location
                    }
                );

            count++;
            if (count == 3 && lastDivs != scholarshipDiv)
            {
                await Task.Delay(60000);
                count = 0;
            }

            driver.Close();

            driver.SwitchTo().Window(driver.WindowHandles[0]);
        }
        return scholarships;
    }
    
    private async Task<List<string>> ProcessData(ChromeDriver driver, string urls, int numberOfParentPage, string css)
    {
        List<string> scholarshipDivs = new List<string>();
        string scholarshipUrls = urls;

        driver.Navigate().GoToUrl(scholarshipUrls);

        await Task.Delay(1000);

        // Find all div elements with class "flex md:w-1/3"

        Uri uri = new Uri(scholarshipUrls);
        string domain = uri.Host;
        string completions = "";

        string html = driver.FindElement(By.CssSelector(css)).GetAttribute("outerHTML");

        while (true)
        {
            try
            {
                string prompt = "I'm a software developer." +
                $" I need you to get the data from a scholarship page by reading the html code of the page: {html}." +
                " Please read those pages contain the list of links to scholarship." +
                $" Get for me all the url of the scholarship (add the domain {domain}) contained in this page." +
                " Return it as a string follow the format: https://url1, https://url2,...." +
                " Not answer anything else" +
                " so that I can easily to manipulate the information.";

                /*var openAI = new OpenAIAPI("");
                var chat = openAI.Chat.CreateConversation();
                chat.Model = OpenAI_API.Models.Model.GPT4_Turbo;
                chat.AppendUserInput(prompt);
                completions = await chat.GetResponseFromChatbotAsync();*/

                ChatClient client = new(model: "gpt-4-1106-preview", new ApiKeyCredential(_apiKey));
                ChatCompletion completion = client.CompleteChat(prompt);
                completions = completion.ToString();
                break;
            }
            catch
            {
                await Task.Delay(60000);
            }
        }

        string[] temp = completions.Split(", ");
        foreach (string scholarship in temp)
        {
            scholarshipDivs.Add(scholarship);
        }

        // This is used to scratch data with multiple link
        /*while (numberOfParentPage - 1 > 0)
        {
            List<string> tempScholarshipDivs = scholarshipDivs;
            List<string> tempScholarshipDivsUndone = [];
            while (tempScholarshipDivs.Count != 0)
            {
                int countLinks = tempScholarshipDivs.Count;
                for (int i = 0; i < countLinks; i++)
                {
                    try
                    {
                        string scholarUrl = tempScholarshipDivs[i];
                        driver.Navigate().GoToUrl(scholarUrl);

                        await Task.Delay(1000);

                        // Find all div elements with class "flex md:w-1/3"
                        var scholarHtml = driver.FindElement(By.CssSelector(css)).GetAttribute("outerHTML");

                        Uri scholaruri = new Uri(scholarshipUrls);
                        string scholardomain = scholaruri.Host;

                        count++;

                        string scholarprompt = "I'm a software developer." +
                            $" I need you to get the data from a scholarship page by reading the html code of the page: {html}." +
                            " Please read those pages contain the list of links to scholarship." +
                            $" Get for me all the url of the scholarship (add the domain {domain}) contained in this page." +
                            " Return it as a string follow the format: https://url1, https://url2,...." +
                            " Not answer anything else" +
                            " so that I can easily to manipulate the information.";

                        *//*var scholaropenAI = new OpenAIAPI("");
                        var scholarchat = openAI.Chat.CreateConversation();
                        chat.Model = OpenAI_API.Models.Model.AdaTextEmbedding;
                        chat.AppendUserInput(prompt);
                        var scholarcompletions = await chat.GetResponseFromChatbotAsync();*//*

                        ChatClient clients = new(model: "gpt-4-1106-preview", new ApiKeyCredential(""));
                        ChatCompletion completionss = clients.CompleteChat(scholarprompt);
                        string scholarcompletions = completionss.ToString();
                        Console.WriteLine($"{i} times, scholarprompt: {scholarprompt} answer: {scholarcompletions}");

                        string[] temps = completions.Split(", ");
                        foreach (string scholarship in temps)
                        {
                            tempScholarshipDivsUndone.Add(scholarship);
                        }
                    }
                    catch (Exception ex)
                    {
                        i--;
                        await Task.Delay(6000);
                    }
                }
                tempScholarshipDivs = tempScholarshipDivsUndone;
                tempScholarshipDivsUndone = [];
            }
            scholarshipDivs = tempScholarshipDivs;
            numberOfParentPage--;
        }*/

        return scholarshipDivs;
    }
}

