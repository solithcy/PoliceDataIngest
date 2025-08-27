using Microsoft.EntityFrameworkCore;
using Moq;
using PoliceDataIngest.Context;
using PoliceDataIngest.Model;
using PoliceDataIngest.Services;

namespace Testing;

public class Tests
{
    private FileInfo _file;
    private ParseService _parse;
    [SetUp]
    public void Setup()
    {
    }

    [Test, Order(1)]
    public async Task Check_That_DownloadZip_Downloads_Correctly()
    {
        _file = await ApiService.DownloadZip(0, 0);
        Assert.That(_file.Exists);
    }

    [Test, Order(2)]
    public void Check_That_ParseService_Inits()
    {
        _parse = new ParseService(_file, 2025, 01, true);
    }

    [Test, Order(3)]
    public void Check_That_GetCrime_Parses_Correctly()
    {
        var c = _parse.GetCrimes().ToList();
        Assert.That(c, Is.Not.Empty);
        foreach (var crime in c.Take(10))
        {
            Assert.That(crime.CrimeType, Is.Not.EqualTo(""));
        }
    }

    [Test]
    public void Check_That_ReadPopulations_Parses_Correctly()
    {
        var pop = PopService.ReadPopulations();
        Assert.That(pop, Is.Not.Empty);
        foreach (var p in pop.Take(10))
        {
            Assert.That(p.Population, Is.GreaterThan(0));
        }
    }

    [Test, Order(4)]
    public void Check_That_AddCrimeData_Completes()
    {
        var (factory, ctx) = GetMockDbObjects();
        
        ctx.Setup(ctx => ctx.GetExistingHashSet(It.IsAny<DbSet<CrimeArea>>())).ReturnsAsync([]);
        ctx.Setup(ctx => ctx.QuickPushCrimeAreas(It.IsAny<List<CrimeArea>>())).Returns(Task.CompletedTask);
        
        Assert.DoesNotThrowAsync(()=>Program.AddCrimeData(factory.Object));
        
        ctx.Verify(ctx => ctx.QuickPushCrimeAreas(It.IsAny<List<CrimeArea>>()), Times.AtLeastOnce);
    }

    [Test]
    public void Check_That_AddPopulationData_Completes()
    {
        var (factory, ctx) = GetMockDbObjects();

        ctx.Setup(ctx => ctx.GetExistingHashSet(It.IsAny<DbSet<PopulationArea>>())).ReturnsAsync([]);
        ctx.Setup(ctx => ctx.QuickPushPopAreas(It.IsAny<List<PopulationArea>>())).Returns(Task.CompletedTask);
        
        Assert.DoesNotThrowAsync(()=>Program.AddPopulationData(factory.Object));
        
        ctx.Verify(ctx => ctx.QuickPushPopAreas(It.IsAny<List<PopulationArea>>()), Times.AtLeastOnce);
    }

    private (Mock<PoliceDbContextFactory>, Mock<PoliceDbContext>) GetMockDbObjects()
    {
        var mockFactory = new Mock<PoliceDbContextFactory>();
        var mockContext = new Mock<PoliceDbContext>();
        mockFactory.Setup(factory => factory.CreateDbContext(It.IsAny<string[]>()))
            .Returns(mockContext.Object);

        return (mockFactory, mockContext);
    }
}