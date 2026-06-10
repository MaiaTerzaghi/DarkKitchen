using DarkKitchen.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Test.Repositories;

[TestClass]
public sealed class RepositoryTest
{
    private readonly DbContext _context = DbContextBuilder.BuildTestDbContext();
    private readonly Repository<EntityTest> _repository;

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
        var entity = new EntityTest("Some Name");

        var result = _repository.Add(entity);

        Assert.IsNotNull(result);
        Assert.AreNotEqual(0, result.Id);
        Assert.AreEqual("Some Name", result.Name);
    }

    [TestMethod]
    public void Get_ExistingEntity_ReturnsEntity()
    {
        var entity = new EntityTest("Some Name");

        _context.Add(entity);
        _context.SaveChanges();

        var result = _repository.Get(e => e.Id == entity.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(entity.Id, result.Id);
    }

    [TestMethod]
    public void Update_ExistingEntity_ReturnsUpdatedEntity()
    {
        var entity = new EntityTest("Some Name");

        _context.Add(entity);
        _context.SaveChanges();

        entity.Name = "Updated Name";
        var result = _repository.Update(entity);

        Assert.IsNotNull(result);
        Assert.AreEqual("Updated Name", result.Name);
    }

    [TestMethod]
    public void GetAll_WithoutFilters_ReturnsAllEntities()
    {
        _context.Add(new EntityTest("Entity One"));
        _context.Add(new EntityTest("Entity Two"));
        _context.SaveChanges();

        var (items, totalCount) = _repository.GetAll();

        Assert.AreEqual(2, items.Count);
        Assert.AreEqual(2, totalCount);
    }

    [TestMethod]
    public void GetAll_WithPredicate_ReturnsFilteredEntities()
    {
        _context.Add(new EntityTest("Entity One"));
        _context.Add(new EntityTest("Entity Two"));
        _context.SaveChanges();

        var (items, totalCount) = _repository.GetAll(predicate: e => e.Name == "Entity One");

        Assert.AreEqual(1, items.Count);
        Assert.AreEqual(1, totalCount);
        Assert.AreEqual("Entity One", items[0].Name);
    }

    [TestMethod]
    public void GetAll_WithOrderBy_ReturnsOrderedEntities()
    {
        _context.Add(new EntityTest("Beta"));
        _context.Add(new EntityTest("Alpha"));
        _context.SaveChanges();

        var (items, totalCount) = _repository.GetAll(orderBy: e => e.Name);

        Assert.AreEqual(2, items.Count);
        Assert.AreEqual("Alpha", items[0].Name);
        Assert.AreEqual("Beta", items[1].Name);
    }

    [TestMethod]
    public void GetAll_WithOrderByDescending_ReturnsOrderedEntitiesDescending()
    {
        _context.Add(new EntityTest("Alpha"));
        _context.Add(new EntityTest("Beta"));
        _context.SaveChanges();

        var (items, totalCount) = _repository.GetAll(orderBy: e => e.Name, descending: true);

        Assert.AreEqual(2, items.Count);
        Assert.AreEqual("Beta", items[0].Name);
        Assert.AreEqual("Alpha", items[1].Name);
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
        Assert.AreEqual("Entity 3", items[0].Name);
        Assert.AreEqual("Entity 4", items[1].Name);
    }

    [TestMethod]
    public void Delete_ExistingEntity_RemovesFromDatabase()
    {
        var entity = new EntityTest("Some Name");

        _context.Add(entity);
        _context.SaveChanges();

        _repository.Delete(entity);

        var result = _repository.Get(e => e.Id == entity.Id);
        Assert.IsNull(result);
    }
}
