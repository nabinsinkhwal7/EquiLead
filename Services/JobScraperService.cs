using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace EquidCMS.Services
{
    public class JobScraperService
    {
        private readonly HttpClient _httpClient;

        public JobScraperService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<JobDataDto> ScrapeJobDetails(string url)
        {
            var uri = new Uri(url);
            var domain = uri.Host.ToLower();

            try
            {
                var html = await _httpClient.GetStringAsync(url);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                if (domain.Contains("devnetjobs.org"))
                {
                    return ScrapeDevNetJobs(htmlDoc, url);
                }
                else if (domain.Contains("ngobox.org"))
                {
                    return ScrapeNgoBox(htmlDoc, url);
                }
                else if (domain.Contains("oraclecloud.com"))
                {
                    return ScrapeUndpOracleCloudJob(htmlDoc,url);
                }
                else if (domain.Contains("linkedin.com"))
                {
                    return ScrapeLinkedInJob(htmlDoc, url);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private JobDataDto ScrapeDevNetJobs(HtmlDocument htmlDoc, string url)
        {
            var jobData = new JobDataDto
            {
                SourceUrl = url,
                ScrapeDate = DateTime.Now
            };

            // Extract basic job info
            jobData.Title = htmlDoc.DocumentNode.SelectSingleNode("//span[@id='ctl00_ContentPlaceHolder1_JD1_lblJobTitle']")?.InnerText.Trim();
            jobData.CompanyName = htmlDoc.DocumentNode.SelectSingleNode("//span[@id='ctl00_ContentPlaceHolder1_JD1_lblCompanyName']")?.InnerText.Trim();

            // Extract location (combines location and country)
            var location = htmlDoc.DocumentNode.SelectSingleNode("//span[@id='ctl00_ContentPlaceHolder1_JD1_lblLocation']")?.InnerText.Trim();
            var country = htmlDoc.DocumentNode.SelectSingleNode("//span[@id='ctl00_ContentPlaceHolder1_JD1_lblCountry']")?.InnerText.Trim();
            jobData.Location = $"{location} {country}".Trim();

            // Extract deadline (from multiple possible locations)
            jobData.DeadlineText = htmlDoc.DocumentNode.SelectSingleNode("//span[@id='ctl00_ContentPlaceHolder1_JD1_lblPostedDate']")?.InnerText.Trim()
                                ?? htmlDoc.DocumentNode.SelectSingleNode("//span[@id='ctl00_ContentPlaceHolder1_JD1_lblPostedDateBtm']")?.InnerText.Trim();

            // Parse the deadline date
            jobData.Deadline = ParseDevNetJobsDate(jobData.DeadlineText);

            // Extract salary (from description)
            var descriptionNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='ctl00_ContentPlaceHolder1_JD1_tdDesc']");
            if (descriptionNode != null)
            {
                var descriptionHtml = descriptionNode.InnerHtml;
                jobData.Description = HtmlEntity.DeEntitize(descriptionNode.InnerText.Trim());

                // Extract salary if mentioned
                var salaryMatch = Regex.Match(descriptionHtml, @"<strong>Salary:</strong>([^<]+)", RegexOptions.IgnoreCase);
                if (salaryMatch.Success)
                {
                    jobData.Salary = salaryMatch.Groups[1].Value.Trim();
                }

                // Extract sectors
                var sectors = new List<string>();
                for (int i = 1; i <= 3; i++)
                {
                    var sector = htmlDoc.DocumentNode.SelectSingleNode($"//span[@id='ctl00_ContentPlaceHolder1_JD1_lblSector{i}']")?.InnerText.Trim();
                    if (!string.IsNullOrEmpty(sector))
                    {
                        sectors.Add(sector);
                    }
                }
                if (sectors.Count > 0)
                {
                    jobData.Description += $"\n\nRelevant Sectors: {string.Join(", ", sectors)}";
                }
            }

            return jobData;
        }

        private JobDataDto ScrapeNgoBox(HtmlDocument htmlDoc, string url)
        {
            var mainNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class,'row_section')]");
            var sideNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class,'card-block')]");

            if (mainNode == null || sideNode == null)
                return null;

            var rawMainHtml = mainNode.InnerHtml;
            rawMainHtml = rawMainHtml.Replace("</strong>", "");

            var jobData = new JobDataDto
            {
                Title = ExtractField(rawMainHtml, "Position Title"),
                CompanyName = ExtractField(rawMainHtml, "Organization"),
                Location = ExtractField(rawMainHtml, "Location"),
                DeadlineText = ExtractSideField(sideNode, "Apply By"),
                Salary = ExtractList(mainNode, "Compensation"),
                Description = ExtractParagraphAfterHeader(mainNode, "Key Responsibilities"),
                EngagementType = ExtractField(rawMainHtml, "Engagement Type"),
                RoleOverview = ExtractParagraphAfterHeader(mainNode, "About the Role"),
                Experience = ExtractParagraphAfterHeader(mainNode, "Required Qualifications &amp; Experience"),
                SourceUrl = url,
                ScrapeDate = DateTime.Now
            };

            var (minSalary, maxSalary) = ExtractSalaryNumbers(jobData.Salary);
            jobData.MinSaliry = minSalary ?? 0;
            jobData.MaxSaliry = maxSalary ?? 0;
            jobData.Deadline = ParseApplicationDeadline(jobData.DeadlineText);

            return jobData;
        }
        private JobDataDto ScrapeUndpOracleCloudJob(HtmlDocument htmlDoc, string url)
        {
            var jobData = new JobDataDto
            {
                SourceUrl = url,
                ScrapeDate = DateTime.Now
            };

            // Extract title
            var titleNode = htmlDoc.DocumentNode.SelectSingleNode("//h1[contains(@class, 'job-details__title')]");
            if (titleNode != null)
            {
                jobData.Title = titleNode.InnerText.Trim();
            }

            // Extract organization (UNDP is hardcoded in the HTML)
            jobData.CompanyName = "UNDP";

            // Extract location
            var locationNode = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='job-meta__title job-meta__title--locations']/following-sibling::span//span[contains(@class, 'job-meta__pin-item')]");
            if (locationNode != null)
            {
                jobData.Location = locationNode.InnerText.Trim();
            }

            // Extract deadline
            var deadlineNode = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='job-meta__title' and contains(text(), 'Apply Before')]/following-sibling::span");
            if (deadlineNode != null)
            {
                jobData.DeadlineText = deadlineNode.InnerText.Trim();
                jobData.Deadline = ParseApplicationDeadline(jobData.DeadlineText);
            }

            // Extract full job description
            var descriptionNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'job-details__description-content')]");
            if (descriptionNode != null)
            {
                jobData.Description = descriptionNode.InnerHtml;

                // Try to extract specific sections from the description
                var backgroundNode = descriptionNode.SelectSingleNode(".//strong[contains(text(), 'Background')]/following-sibling::p");
                if (backgroundNode != null)
                {
                    jobData.RoleOverview = backgroundNode.InnerHtml;
                }

                var responsibilitiesNode = descriptionNode.SelectSingleNode(".//strong[contains(text(), 'Duties and Responsibilities')]/following-sibling::ul");
                if (responsibilitiesNode != null)
                {
                    jobData.Description = responsibilitiesNode.InnerHtml;
                }
            }

            // Extract experience requirements
            var experienceNode = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='job-meta__title' and contains(text(), 'Education & Work Experience')]/following-sibling::span");
            if (experienceNode != null)
            {
                jobData.Experience = experienceNode.InnerText.Trim();
            }

            // Extract engagement type (Job Schedule)
            var engagementNode = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='job-meta__title' and contains(text(), 'Job Schedule')]/following-sibling::span");
            if (engagementNode != null)
            {
                jobData.EngagementType = engagementNode.InnerText.Trim();
            }

            // Extract salary (Grade)
            var gradeNode = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='job-meta__title' and contains(text(), 'Grade')]/following-sibling::span");
            if (gradeNode != null)
            {
                jobData.Salary = gradeNode.InnerText.Trim();
                var (minSalary, maxSalary) = ExtractSalaryNumbers(jobData.Salary);
                jobData.MinSaliry = minSalary ?? 0;
                jobData.MaxSaliry = maxSalary ?? 0;
            }

            return jobData;
        }
        private JobDataDto ScrapeLinkedInJob(HtmlDocument htmlDoc, string url)
        {
            var jobData = new JobDataDto
            {
                SourceUrl = url,
                ScrapeDate = DateTime.Now
            };

            try
            {
                // 1. Extract Job Title (from <h1> containing <a>)
                jobData.Title = htmlDoc.DocumentNode
                    .SelectSingleNode("//h1[contains(@class,'t-24')]/a")?.InnerText?.Trim();

                // 2. Extract Company Name (from company profile anchor)
                jobData.CompanyName = htmlDoc.DocumentNode
                    .SelectSingleNode("//a[contains(@href, '/company/') and contains(@href, '/life')]")?.InnerText?.Trim();

                // 3. Extract Location
                jobData.Location = htmlDoc.DocumentNode
                    .SelectSingleNode("//span[contains(text(), 'India') or contains(@class,'tvm__text')][1]")?.InnerText?.Trim();

                // 4. Extract Posted Date Text
                jobData.DeadlineText = htmlDoc.DocumentNode
                    .SelectSingleNode("//span[contains(text(), 'day') or contains(text(),'week') or contains(text(),'month')]")
                    ?.InnerText?.Trim();

                // 5. Parse Posted Date Text to actual DateTime if possible
                jobData.Deadline = ParseLinkedInPostedTime(jobData.DeadlineText);

                // 6. Extract Job Description
                var descriptionNode = htmlDoc.DocumentNode
                    .SelectSingleNode("//div[contains(@class, 'jobs-box__html-content') or @id='job-details']");

                if (descriptionNode != null)
                {
                    jobData.Description = HtmlEntity.DeEntitize(descriptionNode.InnerText.Trim());
                }

                return jobData;
            }
            catch
            {
                return null;
            }
        }


        private DateTime? ParseLinkedInPostedTime(string postedText)
        {
            if (string.IsNullOrEmpty(postedText)) return null;

            var now = DateTime.Now;
            var match = Regex.Match(postedText, @"(\d+)\s+(minute|hour|day|week|month|year)s?", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var value = int.Parse(match.Groups[1].Value);
                var unit = match.Groups[2].Value.ToLower();

                return unit switch
                {
                    "minute" => now.AddMinutes(-value),
                    "hour" => now.AddHours(-value),
                    "day" => now.AddDays(-value),
                    "week" => now.AddDays(-7 * value),
                    "month" => now.AddMonths(-value),
                    "year" => now.AddYears(-value),
                    _ => null
                };
            }

            return null;
        }


        private DateTime? ParseDevNetJobsDate(string dateText)
        {
            if (string.IsNullOrWhiteSpace(dateText))
                return null;

            try
            {
                // Try parsing common date formats on devnetjobs.org
                if (DateTime.TryParseExact(dateText, "dd MMM yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out var result))
                {
                    return result;
                }

                return DateTime.Parse(dateText);
            }
            catch
            {
                return null;
            }
        }

        private string ExtractField(string html, string label)
        {
            var match = Regex.Match(html, $"{Regex.Escape(label)}:?\\s*(.*?)<br>", RegexOptions.IgnoreCase);
            return match.Success ? HtmlEntity.DeEntitize(match.Groups[1].Value.Trim()) : "Not found";
        }

        private string ExtractParagraphAfterHeader(HtmlNode root, string headerText)
        {
            var header = root.SelectSingleNode($""".//h3[contains(., '{headerText}')]""");

            if (header == null) return "Not found";

            var sb = new System.Text.StringBuilder();
            var current = header.NextSibling;

            while (current != null)
            {
                if (current.Name == "h3") break;

                if (current.NodeType == HtmlNodeType.Element)
                {
                    if (current.Name == "ul" || current.Name == "ol")
                    {
                        foreach (var li in current.SelectNodes(".//li") ?? Enumerable.Empty<HtmlNode>())
                        {
                            sb.AppendLine("• " + HtmlEntity.DeEntitize(li.InnerText.Trim()));
                        }
                    }
                    else if (current.Name == "p")
                    {
                        sb.AppendLine(HtmlEntity.DeEntitize(current.InnerText.Trim()));
                    }
                }

                current = current.NextSibling;
            }

            return sb.ToString().Trim();
        }

        private string ExtractList(HtmlNode root, string label)
        {
            var ul = root.SelectSingleNode($".//strong[contains(text(), '{label}')]/ancestor::p/following-sibling::ul");
            if (ul == null) return "Not found";
            return string.Join("\n", ul.SelectNodes(".//li")?.Select(li => HtmlEntity.DeEntitize(li.InnerText.Trim())) ?? []);
        }

        private string ExtractSideField(HtmlNode node, string label)
        {
            var strong = node.SelectSingleNode($".//strong[contains(text(), '{label}')]");
            if (strong == null) return "Not found";

            var text = strong.ParentNode.InnerText;
            return Regex.Replace(text, $"{label}:?", "", RegexOptions.IgnoreCase).Trim();
        }

        private (decimal? MinSalary, decimal? MaxSalary) ExtractSalaryNumbers(string compensationText)
        {
            var matches = Regex.Matches(compensationText, @"INR\s([\d,]+)/day");

            if (matches.Count >= 2)
            {
                var amount1 = decimal.Parse(matches[0].Groups[1].Value.Replace(",", ""));
                var amount2 = decimal.Parse(matches[1].Groups[1].Value.Replace(",", ""));

                return (Math.Min(amount1, amount2), Math.Max(amount1, amount2));
            }
            else if (matches.Count == 1)
            {
                var amount = decimal.Parse(matches[0].Groups[1].Value.Replace(",", ""));
                return (amount, amount);
            }

            return (null, null);
        }

        private DateTime? ParseApplicationDeadline(string dateText)
        {
            if (string.IsNullOrWhiteSpace(dateText) || dateText == "Not found")
                return null;

            dateText = Regex.Replace(dateText, @"Apply By:\s*", "", RegexOptions.IgnoreCase).Trim();

            try
            {
                if (DateTime.TryParseExact(dateText, "dd MMM yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out var result))
                {
                    return result;
                }

                return DateTime.Parse(dateText);
            }
            catch
            {
                return null;
            }
        }
    }

    public class JobDataDto
    {
        public string Title { get; set; }
        public string CompanyName { get; set; }
        public int? CompanyId { get; set; }
        public string Location { get; set; }
        public string EngagementType { get; set; }
        public string Description { get; set; }
        public string Summary { get; set; }
        public string Salary { get; set; }
        public decimal MinSaliry { get; set; }
        public decimal MaxSaliry { get; set; }
        public DateTime? Deadline { get; set; }
        public string RoleOverview { get; set; }
        public string DeadlineText { get; set; }
        public string SourceUrl { get; set; }
        public string Experience { get; set; }
        public DateTime ScrapeDate { get; set; }
    }
}