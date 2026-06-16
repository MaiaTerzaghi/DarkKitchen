using DarkKitchen.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Test.Repositories;

[TestClass]
public sealed class RepositoryTest
{
    private readonly DbContext _context = DbContextBuilder.BuildTestDbContext();
    private readonly Repository<EntityTest> _repository;

    private const string SomeName = "Some Name";
    private const string Alpha = "Alpha";
    private const string Beta = "Beta";
    private const string EntityOne = "Entity One";
    private const string EntityTwo = "Entity Two";
    private const string UpdatedName = "Updated Name";
    private const string Entity3 = "Entity 3";
    private const string Entity4 = "Entity 4";

    public RepositoryTest()
    {
        _repository = new Repository<EntityTest>(_context);
    }

    [TestInitialize]
    public void Initialize()
    {
        _context.Database.EnsureCreated();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
    }

    [TestMethod]
    public void Add_ValidEntity_ReturnsSavedEntityWithId()
    {
        var entity = new EntityTest(SomeName);

        var result = _repository.Add(entity);

        Assert.IsNotNull(result);
        Assert.AreNotEqual(0, result.Id);
        Assert.AreEqual(SomeName, result.Name);
    }

    [TestMethod]
    public void Get_ExistingEntity_ReturnsEntity()
    {
        var entity = new EntityTest(SomeName);

        _context.Add(entity);
        _context.SaveChanges();

        var result = _repository.Get(e => e.Id == entity.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(entity.Id, result.Id);
    }

    [TestMethod]
    public void Update_ExistingEntity_ReturnsUpdatedEntity()
    {
        var entity = new EntityTest(SomeName);

        _context.Add(entity);
        _context.SaveChanges();

        entity.Name = UpdatedName;
        var result = _repository.Update(entity);

        Assert.IsNotNull(result);
        Assert.AreEqual(UpdatedName, result.Name);
    }

    [TestMethod]
    public void GetAll_WithoutFilters_ReturnsAllEntities()
    {
        _context.Add(new EntityTest(EntityOne));
        _context.Add(new EntityTest(EntityTwo));
        _context.SaveChanges();

        var (items, totalCount) = _repository.GetAll();

        Assert.AreEqual(2, items.Count);
        Assert.AreEqual(2, totalCount);
    }

    [TestMethod]
    public void GetAll_WithPredicate_ReturnsFilteredEntities()
    {
        _context.Add(new EntityTest(EntityOne));
        _context.Add(new EntityTest(EntityTwo));
        _context.SaveChanges();

        var (items, totalCount) = _repository.GetAll(predicate: e => e.Name == EntityOne);

        Assert.AreEqual(1, items.Count);
        Assert.AreEqual(1, totalCount);
        Assert.AreEqual(EntityOne, items[0].Name);
    }

    [TestMethod]
    public void GetAll_WithOrderBy_ReturnsOrderedEntities()
    {
        _context.Add(new EntityTest(Beta));
        _context.Add(new EntityTest(Alpha));
        _context.SaveChanges();

        var (items, totalCount) = _repository.GetAll(orderBy: e => e.Name);

        Assert.AreEqual(2, items.Count);
        Assert.AreEqual(Alpha, items[0].Name);
        Assert.AreEqual(Beta, items[1].Name);
    }

    [TestMethod]
    public void GetAll_WithOrderByDescending_ReturnsOrderedEntitiesDescending()
    {
        _context.Add(new EntityTest(Alpha));
        _context.Add(new EntityTest(Beta));
        _context.SaveChanges();

        var (items, totalCount) = _repository.GetAll(orderBy: e => e.Name, descending: true);

        Assert.AreEqual(2, items.Count);
        Assert.AreEqual(Beta, items[0].Name);
        Assert.AreEqual(Alpha, items[1].Name);
    }

    [TestMethod]
    public void GetAll_WithPagination_ReturnsCorrectPage()
    {
        for(var i = 1; i <= 5; i++)
        {
            _context.Add(new EntityTest($"Entity {i}"));
        }

        _context.SaveChanges();

        var (items, totalCount) = _repository.GetAll(page: 2, pageSize: 2);

        Assert.AreEqual(2, items.Count);
        Assert.AreEqual(5, totalCount);
        Assert.AreEqual(Entity3, items[0].Name);
        Assert.AreEqual(Entity4, items[1].Name);
    }

    [TestMethod]
    public void Delete_ExistingEntity_RemovesFromDatabase()
    {
        var entity = new EntityTest(SomeName);

        _context.Add(entity);
        _context.SaveChanges();

        _repository.Delete(entity);

        var result = _repository.Get(e => e.Id == entity.Id);
        Assert.IsNull(result);
    }
}
