namespace MauiApp.Models;

/// <summary>
/// 뉴스 기사 데이터
/// </summary>
public class NewsArticle
{
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public DateTime PublishedTime { get; set; }
    public string Category { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    public string TimeAgo
    {
        get
        {
            var timeSpan = DateTime.Now - PublishedTime;
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes}분 전";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours}시간 전";
            return $"{(int)timeSpan.TotalDays}일 전";
        }
    }
}

/// <summary>
/// 주식 정보
/// </summary>
public class StockInfo
{
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Change { get; set; }
    public decimal ChangePercent { get; set; }
    public long Volume { get; set; }

    public bool IsPositive => Change >= 0;
    public string ChangeColor => IsPositive ? "Green" : "Red";
    public string ChangeSign => IsPositive ? "+" : "";
}

/// <summary>
/// 전자책 내용
/// </summary>
public class EBookContent
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Chapter { get; set; } = string.Empty;
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public string Content { get; set; } = string.Empty;

    public string Progress => $"{PageNumber}/{TotalPages}";
    public double ProgressPercent => TotalPages > 0 ? (double)PageNumber / TotalPages * 100 : 0;
}

/// <summary>
/// 일정 정보
/// </summary>
public class CalendarEvent
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Color Color { get; set; } = Colors.Blue;

    public string TimeRange => $"{StartTime:HH:mm} - {EndTime:HH:mm}";
    public string Duration
    {
        get
        {
            var duration = EndTime - StartTime;
            if (duration.TotalMinutes < 60)
                return $"{duration.TotalMinutes}분";
            return $"{duration.TotalHours:F1}시간";
        }
    }
}

/// <summary>
/// 샘플 데이터 생성기
/// </summary>
public static class DisguiseSampleData
{
    /// <summary>
    /// 샘플 뉴스 기사 생성
    /// </summary>
    public static List<NewsArticle> GetSampleNews()
    {
        return new List<NewsArticle>
        {
            new NewsArticle
            {
                Title = "글로벌 기술 기업들, AI 개발에 대규모 투자 발표",
                Summary = "주요 기술 기업들이 인공지능 연구 개발에 수조원 규모의 투자를 발표하며 AI 경쟁이 심화되고 있습니다. 전문가들은 이번 투자가 산업 전반에 큰 변화를 가져올 것으로 전망하고 있습니다.",
                Source = "테크뉴스",
                PublishedTime = DateTime.Now.AddMinutes(-15),
                Category = "IT/과학"
            },
            new NewsArticle
            {
                Title = "국내 증시, 외국인 매수세에 상승 마감",
                Summary = "코스피가 외국인 투자자들의 적극적인 매수에 힘입어 1% 이상 상승하며 장을 마감했습니다. 특히 반도체와 배터리 관련 종목들이 강세를 보였습니다.",
                Source = "경제일보",
                PublishedTime = DateTime.Now.AddHours(-2),
                Category = "경제"
            },
            new NewsArticle
            {
                Title = "새로운 재생에너지 정책 발표, 2030년까지 탄소중립 목표",
                Summary = "정부가 재생에너지 확대와 탄소 배출 감축을 위한 새로운 정책을 발표했습니다. 태양광과 풍력 발전 설비를 대폭 확충하고, 전기차 보급을 가속화할 계획입니다.",
                Source = "환경뉴스",
                PublishedTime = DateTime.Now.AddHours(-4),
                Category = "사회"
            },
            new NewsArticle
            {
                Title = "프리미어리그, 주말 흥미진진한 경기 펼쳐져",
                Summary = "영국 프리미어리그에서 주말 동안 여러 박빙의 경기가 펼쳐졌습니다. 상위권 팀들 간의 순위 경쟁이 더욱 치열해지고 있어 남은 시즌이 주목됩니다.",
                Source = "스포츠투데이",
                PublishedTime = DateTime.Now.AddHours(-6),
                Category = "스포츠"
            },
            new NewsArticle
            {
                Title = "봄 여행 시즌 대비 관광지 안전 점검 강화",
                Summary = "다가오는 봄 여행 시즌을 맞아 전국 주요 관광지에서 안전 점검이 진행되고 있습니다. 특히 산악 지역과 해안가의 안전 시설 보강 작업이 집중적으로 이뤄지고 있습니다.",
                Source = "여행매거진",
                PublishedTime = DateTime.Now.AddHours(-8),
                Category = "여행"
            }
        };
    }

    /// <summary>
    /// 샘플 주식 정보 생성
    /// </summary>
    public static List<StockInfo> GetSampleStocks()
    {
        var random = new Random();
        return new List<StockInfo>
        {
            new StockInfo
            {
                Symbol = "AAPL",
                Name = "Apple Inc.",
                Price = 175.25m,
                Change = 2.35m,
                ChangePercent = 1.36m,
                Volume = 54890000
            },
            new StockInfo
            {
                Symbol = "MSFT",
                Name = "Microsoft Corp.",
                Price = 380.50m,
                Change = -1.20m,
                ChangePercent = -0.31m,
                Volume = 23450000
            },
            new StockInfo
            {
                Symbol = "GOOGL",
                Name = "Alphabet Inc.",
                Price = 142.80m,
                Change = 3.45m,
                ChangePercent = 2.47m,
                Volume = 31200000
            },
            new StockInfo
            {
                Symbol = "TSLA",
                Name = "Tesla Inc.",
                Price = 242.90m,
                Change = -4.60m,
                ChangePercent = -1.86m,
                Volume = 89100000
            },
            new StockInfo
            {
                Symbol = "AMZN",
                Name = "Amazon.com Inc.",
                Price = 178.35m,
                Change = 1.85m,
                ChangePercent = 1.05m,
                Volume = 42700000
            },
            new StockInfo
            {
                Symbol = "NVDA",
                Name = "NVIDIA Corp.",
                Price = 920.75m,
                Change = 15.25m,
                ChangePercent = 1.68m,
                Volume = 67800000
            }
        };
    }

    /// <summary>
    /// 샘플 전자책 내용 생성
    /// </summary>
    public static EBookContent GetSampleEBook()
    {
        return new EBookContent
        {
            Title = "성공하는 사람들의 7가지 습관",
            Author = "스티븐 코비",
            Chapter = "제3장: 중요한 일을 먼저 하라",
            PageNumber = 142,
            TotalPages = 380,
            Content = @"시간 관리의 핵심은 '중요한 일'과 '긴급한 일'을 구분하는 것입니다. 많은 사람들이 긴급한 일에 쫓겨 살면서 정작 중요한 일들을 미루는 경우가 많습니다.

중요하지만 긴급하지 않은 일들, 예를 들어 운동, 독서, 인간관계 구축, 장기 계획 수립 등은 우리 삶의 질을 크게 향상시킵니다. 하지만 이런 일들은 당장 해야 할 이유가 없어 보이기 때문에 쉽게 미뤄지곤 합니다.

성공하는 사람들은 이러한 '제2사분면'의 활동에 시간을 투자합니다. 그들은 긴급한 일에 반응하기보다는, 중요한 일을 먼저 계획하고 실행합니다.

매일 아침, 오늘 해야 할 일 중에서 가장 중요한 일 세 가지를 정하고, 그 일들을 우선적으로 처리하는 습관을 들이십시오. 처음에는 어렵겠지만, 점차 삶의 균형을 찾고 더 큰 성과를 이룰 수 있을 것입니다.

중요한 것은 '바쁨'이 아니라 '생산성'입니다. 진정으로 중요한 일에 집중할 때, 우리는 비로소 의미 있는 성과를 만들어낼 수 있습니다."
        };
    }

    /// <summary>
    /// 샘플 일정 생성
    /// </summary>
    public static List<CalendarEvent> GetSampleEvents()
    {
        var today = DateTime.Today;
        return new List<CalendarEvent>
        {
            new CalendarEvent
            {
                StartTime = today.AddHours(9),
                EndTime = today.AddHours(10),
                Title = "팀 회의",
                Location = "2층 회의실 A",
                Description = "주간 진행 상황 공유 및 다음 주 계획 논의",
                Color = Colors.Blue
            },
            new CalendarEvent
            {
                StartTime = today.AddHours(11),
                EndTime = today.AddHours(12),
                Title = "프로젝트 검토",
                Location = "온라인 (Zoom)",
                Description = "신규 프로젝트 제안서 검토 및 피드백",
                Color = Colors.Green
            },
            new CalendarEvent
            {
                StartTime = today.AddHours(14),
                EndTime = today.AddHours(15.5),
                Title = "고객 미팅",
                Location = "강남 본사 10층",
                Description = "Q1 실적 보고 및 Q2 계획 발표",
                Color = Colors.Orange
            },
            new CalendarEvent
            {
                StartTime = today.AddHours(16),
                EndTime = today.AddHours(17),
                Title = "교육 세미나",
                Location = "3층 대강당",
                Description = "AI와 머신러닝 기초 교육",
                Color = Colors.Purple
            }
        };
    }
}

