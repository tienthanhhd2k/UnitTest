using System.Collections.Generic;
using System.Linq;
using dotnetcoremvc2.Controllers;
using dotnetcoremvc2.Interface;
using dotnetcoremvc2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace UnitTest;

public class Tests
{
    private Mock<ILogger<PeopleController>> _logger;
    private Mock<IPeople> _peopleMock;
    public static List<PersonModel> _people = new List<PersonModel>(){ 
            new PersonModel{
                Id = 1,
                FirstName = "Do",
                LastName = "Thanh",
                Address = "Hai Duong",
                Gender = "Male"
            },

            new PersonModel{
                Id = 2,
                FirstName = "Nguyen",
                LastName  = "Binh",
                Address = "Ha Noi",
                Gender = "Male"
            },

            new PersonModel{
                Id = 3,
                FirstName = "Tran",
                LastName = "Nam",
                Address = "Hai Phong",
                Gender = "Male"
            }
            };
    [SetUp]
    public void Setup()
    {
        _logger = new Mock<ILogger<PeopleController>>();
        _peopleMock = new Mock<IPeople>();
    }

    [Test]
    public void Test1()
    {
        //SetUP
        _peopleMock.Setup(x=>x.List()).Returns(_people);
        //Arrage
        var controller = new PeopleController(_logger.Object, _peopleMock.Object);
        var expectedCount = _people.Count;

        //Act
        var result = controller.Index();

        //Assert
        Assert.IsInstanceOf<ViewResult>(result, "Test");

        var view = (ViewResult)result;
        Assert.IsAssignableFrom<List<PersonModel>>(view.ViewData.Model, "Invalid view data model");
        
        var model = view.ViewData.Model as List<PersonModel>;
        Assert.IsNotNull(model, "view data model must not be null");
        Assert.AreEqual(expectedCount, model?.Count, "Model count is not equal to expected count");

    }

    [Test]
    public void Create()
    {
        //Arrage
        var model = new PersonModel
        {
            Id = 1,
                FirstName = "Do",
                LastName = "Thanh",
                Address = "Hai Duong",
                Gender = "Male"
        };
        _peopleMock.Setup(x => x.Create(model))
        .Callback<PersonModel>((PersonModel p) =>
        { 
            _people.Add(p);
        });
        var controller = new PeopleController(_logger.Object, _peopleMock.Object);

        var expected = _people.Count +1;
        

        //Act
        var result = controller.Create(model);

        //Assert
        Assert.IsInstanceOf<RedirectToActionResult>(result,"Invalid return type");

        var view = (RedirectToActionResult)result;
        Assert.AreEqual("Index", view.ActionName,"Invalid action name");

        var acutal = _people.Count;
        Assert.AreEqual(expected, acutal,"Error");
        
        Assert.AreEqual(model,_people.Last(),"Not equals");

    }
    [TearDown]
    public void TearDown()
    {
        _logger = null;
        _peopleMock = null;
    }
}