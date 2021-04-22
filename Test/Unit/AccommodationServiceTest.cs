﻿using AintBnB.BusinessLogic.CustomExceptions;
using AintBnB.BusinessLogic.Imp;
using AintBnB.Core.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using static AintBnB.BusinessLogic.Helpers.Authentication;

namespace Test.Unit
{
    [TestFixture]
    public class AccommodationServiceTest : TestBase
    {
        [SetUp]
        public async Task SetUp()
        {
            await SetupDatabaseForTestingAsync();
            SetupTestClasses();
            await CreateDummyUsersAsync();
        }

        [TearDown]
        public async Task TearDown()
        {
            await DisposeAsync();
        }

        [Test]
        public async Task CreateAccommodationAsync_ShouldReturn_NewAccommodationWhenCustomerCreatesOwnedByTheCustomer()
        {
            LoggedInAs = userCustomer1;

            var acc = await accommodationService.CreateAccommodationAsync(userCustomer1, adr1, 50, 2, 2.3, "mmm mmm", 600, 2, new List<byte[]>(), 100);

            Assert.AreEqual(1, acc.Id);
            Assert.AreEqual(userCustomer1.Id, acc.Owner.Id);
            Assert.AreEqual(adr1, acc.Address);
        }

        [Test]
        public async Task CreateAccommodationAsync_ShouldReturn_NewAccommodationWhenAdminCreatesOnBehalfOfCustomer()
        {
            LoggedInAs = userAdmin;

            var acc = await accommodationService.CreateAccommodationAsync(userCustomer1, adr1, 50, 2, 2.3, "mmm mmm", 600, 2, new List<byte[]>(), 100);

            Assert.AreEqual(1, acc.Id);
            Assert.AreEqual(userCustomer1.Id, acc.Owner.Id);
            Assert.AreEqual(adr1, acc.Address);
        }

        [Test]
        public async Task CreateAccommodationAsync_ShouldReturn_NewAccommodationWhenEmployeeCreatesOnBehalfOfCustomer()
        {
            LoggedInAs = userEmployee1;

            var acc = await accommodationService.CreateAccommodationAsync(userCustomer1, adr1, 50, 2, 2.3, "mmm mmm", 600, 2, new List<byte[]>(), 100);

            Assert.AreEqual(1, acc.Id);
            Assert.AreEqual(userCustomer1.Id, acc.Owner.Id);
            Assert.AreEqual(adr1, acc.Address);
        }

        [Test]
        public void CreateAccommodationAsync_ShouldFail_IfDaysToCreateScheduleForIsLessThanOne()
        {
            LoggedInAs = userCustomer1;

            var ex = Assert.ThrowsAsync<ParameterException>(async ()
                => await accommodationService.CreateAccommodationAsync(userCustomer2, adr1, 50, 2, 2.3, "mmm mmm", 600, 2, new List<byte[]>(), 0));

            Assert.AreEqual("Days to create the schedule for cannot be less than one!", ex.Message);
        }

        [Test]
        public void CreateAccommodationAsync_ShouldFail_IfOwnerIsNotCustomer()
        {
            LoggedInAs = userAdmin;

            var ex = Assert.ThrowsAsync<AccessException>(async ()
                => await accommodationService.CreateAccommodationAsync(userAdmin, adr1, 50, 2, 2.3, "mmm mmm", 600, 2, new List<byte[]>(), 100));

            Assert.AreEqual(ex.Message, $"Must be performed by a customer with ID {userAdmin.Id}, or by admin or an employee on behalf of a customer with ID {userAdmin.Id}!");

            LoggedInAs = userEmployee1;

            ex = Assert.ThrowsAsync<AccessException>(async ()
                => await accommodationService.CreateAccommodationAsync(userEmployee1, adr2, 50, 2, 2.3, "mmm mmm", 600, 2, new List<byte[]>(), 100));

            Assert.AreEqual(ex.Message, $"Must be performed by a customer with ID {userEmployee1.Id}, or by admin or an employee on behalf of a customer with ID {userEmployee1.Id}!");
        }

        [Test]
        public void CreateAccommodationAsync_ShouldFail_IfCustomerTriesToCreateAccommodationForAnotherUser()
        {
            LoggedInAs = userCustomer1;

            var ex = Assert.ThrowsAsync<AccessException>(async ()
                => await accommodationService.CreateAccommodationAsync(userCustomer2, adr1, 50, 2, 2.3, "mmm mmm", 600, 2, new List<byte[]>(), 100));

            Assert.AreEqual(ex.Message, $"Must be performed by a customer with ID {userCustomer2.Id}, or by admin or an employee on behalf of a customer with ID {userCustomer2.Id}!");
        }

        [Test]
        [TestCase(0, "ss", "3f", "23", "ss", "Fredrikstad", "Norway", 10, "ss", 10, 1)]
        [TestCase(1, "", "3f", "23", "ss", "Fredrikstad", "Norway", 10, "ss", 10, 1)]
        [TestCase(1, "ss", "s", "23", "ss", "Fredrikstad", "Norway", 10, "ss", 10, 1)]
        [TestCase(1, "ss", "3f", "", "ss", "Fredrikstad", "Norway", 10, "ss", 10, 1)]
        [TestCase(1, "ss", "3f", "23", "", "Fredrikstad", "Norway", 10, "ss", 10, 1)]
        [TestCase(1, "ss", "3f", "23", "ss", "Fredrikstad", "Norway", 0, "ss", 10, 1)]
        [TestCase(1, "ss", "3f", "23", "ss", "Fredrikstad", "Norway", 10, "", 10, 1)]
        [TestCase(1, "ss", "3f", "23", "ss", "Fredrikstad", "Norway", 10, "ss", 0, 1)]
        [TestCase(1, "ss", "3f", "23", "ss", "Fredrikstad", "Norway", 10, "ss", 10, 0)]
        public void ValidateAccommodationAsync_ShouldFail_IfNotAllCriteriasMet(int ownerId, string street, string number, string zip, string area, string city, string country, int squareMeters, string description, int pricePerNight, int cancellationDeadlineInDays)
        {
            var owner = new User { Id = ownerId };
            var ad = new Address { Street = street, Number = number, Zip = zip, Area = area, City = city, Country = country };
            var acc = new Accommodation { Owner = owner, Address = ad, SquareMeters = squareMeters, Description = description, PricePerNight = pricePerNight, CancellationDeadlineInDays = cancellationDeadlineInDays };

            var ex = Assert.ThrowsAsync<ParameterException>(async ()
                => await accommodationService.ValidateAccommodationAsync(acc));
        }

        [Test]
        [TestCase(1, "ss", "3f", "23", "ss", "London", "Norway", 10, "ss", 10, 1)]
        [TestCase(1, "ss", "3f", "23", "ss", "Osloo", "Norway", 10, "ss", 10, 1)]
        [TestCase(1, "ss", "3f", "23", "ss", "Oslo", "Norrway", 10, "ss", 10, 1)]
        public void ValidateAccommodationAsync_ShouldFail_IfACityIsNotInACountry_OrCityOrCountryDoesNotExist(int ownerId, string street, string number, string zip, string area, string city, string country, int squareMeters, string description, int pricePerNight, int cancellationDeadlineInDays)
        {
            var owner = new User { Id = ownerId };
            var ad = new Address { Street = street, Number = number, Zip = zip, Area = area, City = city, Country = country };
            var acc = new Accommodation { Owner = owner, Address = ad, SquareMeters = squareMeters, Description = description, PricePerNight = pricePerNight, CancellationDeadlineInDays = cancellationDeadlineInDays };

            var ex = Assert.ThrowsAsync<GeographicalException>(async ()
                => await accommodationService.ValidateAccommodationAsync(acc));
        }

        [Test]
        [TestCase(1, "s", "1s", "23", "ss", "Fredrikstad", "Norway", 10, "ss", 10, 1)]
        [TestCase(1, "s--", "1s", "23", "ss", "Fredrikstad", "Norway", 10, "ss", 10, 1)]
        [TestCase(1, "ss", "s", "23", "ss", "Fredrikstad", "Norway", 10, "ss", 10, 1)]
        [TestCase(1, "ss", "s1", "23", "ss", "Fredrikstad", "Norway", 10, "ss", 10, 1)]
        [TestCase(1, "ss", "1-", "23", "ss", "Fredrikstad", "Norway", 10, "ss", 10, 1)]
        [TestCase(1, "ss", "1", "3", "ss", "Fredrikstad", "Norway", 10, "ss", 10, 1)]
        [TestCase(1, "ss", "1", "123456789ab", "ss", "Fredrikstad", "Norway", 10, "ss", 10, 1)]
        [TestCase(1, "ss", "1", "2-", "ss", "Fredrikstad", "Norway", 10, "ss", 10, 1)]
        public void ValidateAccommodationAsync_ShouldFail_IfRegexCheckerFailsOnStreetNumberZipOrArea(int ownerId, string street, string number, string zip, string area, string city, string country, int squareMeters, string description, int pricePerNight, int cancellationDeadlineInDays)
        {
            var owner = new User { Id = ownerId };
            var ad = new Address { Street = street, Number = number, Zip = zip, Area = area, City = city, Country = country };
            var acc = new Accommodation { Owner = owner, Address = ad, SquareMeters = squareMeters, Description = description, PricePerNight = pricePerNight, CancellationDeadlineInDays = cancellationDeadlineInDays };

            var ex = Assert.ThrowsAsync<ParameterException>(async ()
                => await accommodationService.ValidateAccommodationAsync(acc));
        }

        [Test]
        [TestCase(1, "s-s", "1s", "23", "ss", "Fredrikstad", "Norway", 10, "ss", 10, 1)]
        [TestCase(1, "1-s", "1s", "23", "ss", "Fredrikstad", "Norway", 10, "ss", 10, 1)]
        [TestCase(1, "1-s-2-s", "1s", "23", "ss", "Fredrikstad", "Norway", 10, "ss", 10, 1)]
        [TestCase(1, "1 s 2 s", "1s", "23", "ss", "Fredrikstad", "Norway", 10, "ss", 10, 1)]
        [TestCase(1, "1 s-2 s", "1s", "23", "ss", "Fredrikstad", "Norway", 10, "ss", 10, 1)]
        [TestCase(1, "ss", "1s", "23", "ss", "Fredrikstad", "Norway", 10, "ss", 10, 1)]
        public void ValidateAccommodationAsync_ShouldSucceed_IfStreetIsLongerThan2Characters_WithOnlyLettersNumbers_WithOneSpaceOrDashBetweenThem(int ownerId, string street, string number, string zip, string area, string city, string country, int squareMeters, string description, int pricePerNight, int cancellationDeadlineInDays)
        {
            var owner = new User { Id = ownerId };
            var ad = new Address { Street = street, Number = number, Zip = zip, Area = area, City = city, Country = country };
            var acc = new Accommodation { Owner = owner, Address = ad, SquareMeters = squareMeters, Description = description, PricePerNight = pricePerNight, CancellationDeadlineInDays = cancellationDeadlineInDays };

            Assert.DoesNotThrowAsync(async () => await accommodationService.ValidateAccommodationAsync(acc));
        }

        [Test]
        [TestCase(1, "s-s", "123", "23", "ss", "Fredrikstad", "Norway", 10, "ss", 10, 1)]
        [TestCase(1, "s-s", "2", "23", "ss", "Fredrikstad", "Norway", 10, "ss", 10, 1)]
        public void ValidateAccommodationAsync_ShouldSucceed_IfNumberStartsWithNumberLargerThan0_WithOneOptionalLetterAtTheEnd(int ownerId, string street, string number, string zip, string area, string city, string country, int squareMeters, string description, int pricePerNight, int cancellationDeadlineInDays)
        {
            var owner = new User { Id = ownerId };
            var ad = new Address { Street = street, Number = number, Zip = zip, Area = area, City = city, Country = country };
            var acc = new Accommodation { Owner = owner, Address = ad, SquareMeters = squareMeters, Description = description, PricePerNight = pricePerNight, CancellationDeadlineInDays = cancellationDeadlineInDays };

            Assert.DoesNotThrowAsync(async () => await accommodationService.ValidateAccommodationAsync(acc));
        }

        [Test]
        [TestCase(1, "s-s", "2", "12345-6789", "ss", "Fredrikstad", "Norway", 10, "ss", 10, 1)]
        [TestCase(1, "s-s", "2", "abcd-6789", "ss", "Fredrikstad", "Norway", 10, "ss", 10, 1)]
        [TestCase(1, "s-s", "2", "ab", "ss", "Fredrikstad", "Norway", 10, "ss", 10, 1)]
        [TestCase(1, "s-s", "2", "1-2", "ss", "Fredrikstad", "Norway", 10, "ss", 10, 1)]
        [TestCase(1, "s-s", "2", "1 2-s", "ss", "Fredrikstad", "Norway", 10, "ss", 10, 1)]
        [TestCase(1, "s-s", "2", "1 2 s", "ss", "Fredrikstad", "Norway", 10, "ss", 10, 1)]
        public void ValidateAccommodationAsync_ShouldSucceed_IfZipIsBetween2And10Characters_WithOnlyLettersAndNumbersWithSpaceOrDashBetweenThem(int ownerId, string street, string number, string zip, string area, string city, string country, int squareMeters, string description, int pricePerNight, int cancellationDeadlineInDays)
        {
            var owner = new User { Id = ownerId };
            var ad = new Address { Street = street, Number = number, Zip = zip, Area = area, City = city, Country = country };
            var acc = new Accommodation { Owner = owner, Address = ad, SquareMeters = squareMeters, Description = description, PricePerNight = pricePerNight, CancellationDeadlineInDays = cancellationDeadlineInDays };

            Assert.DoesNotThrowAsync(async () => await accommodationService.ValidateAccommodationAsync(acc));
        }

        [Test]
        public void CreateScheduleForXAmountOfDays_Should_CreateScheduleForCorrectAmountOfDays()
        {
            var acc = new Accommodation();

            var days = 100;

            Assert.Null(acc.Schedule);

            var result = typeof(AccommodationService)
                .GetMethod("CreateScheduleForXAmountOfDays", BindingFlags.NonPublic | BindingFlags.Instance);
            result.Invoke(accommodationService, new object[] { acc, days });

            Assert.AreEqual(acc.Schedule.Count, days);
        }

        [Test]
        public void AddDaysToDateAndAddToSchedule_Shoudl_AddCorrectDatesToSchedule()
        {
            var dateAndStatus = new SortedDictionary<string, bool>();

            var td = DateTime.Today;

            var daysToAddToSchedule = 10;

            Assert.AreEqual(0, dateAndStatus.Count);

            var result = typeof(AccommodationService)
                .GetMethod("AddDaysToDateAndAddToSchedule", BindingFlags.NonPublic | BindingFlags.Instance);
            result.Invoke(accommodationService, new object[] { daysToAddToSchedule, td, dateAndStatus });

            Assert.AreEqual(td.ToString("yyyy-MM-dd"), dateAndStatus.Keys.First());

            Assert.AreEqual(td.AddDays(daysToAddToSchedule - 1).ToString("yyyy-MM-dd"), dateAndStatus.Keys.Last());
        }

        [Test]
        public async Task GetAccommodationAsync_ShouldReturn_CorrectAccommodationIfAnyoneLoggedIn()
        {
            LoggedInAs = userCustomer2;

            await CreateDummyAccommodationAsync();

            var acc = await accommodationService.GetAccommodationAsync(1);

            Assert.AreEqual(accommodation1.Id, acc.Id);
            Assert.AreEqual(accommodation1.Owner.Id, acc.Owner.Id);
            Assert.AreEqual(accommodation1.Address.Street, acc.Address.Street);
            Assert.AreEqual(accommodation1.ToString(), acc.ToString());
        }

        [Test]
        public async Task GetAccommodationAsync_ShouldFail_IfNoOneLoggedIn()
        {
            LoggedInAs = null;

            await CreateDummyAccommodationAsync();

            var ex = Assert.ThrowsAsync<LoginException>(async ()
                => await accommodationService.GetAccommodationAsync(1));

            Assert.AreEqual("Not logged in!", ex.Message);
        }

        [Test]
        public async Task GetAccommodationAsync_ShouldFail_IfNoAccommodationsWithTheIdExists()
        {
            LoggedInAs = userAdmin;

            await CreateDummyAccommodationAsync();

            var ex = Assert.ThrowsAsync<IdNotFoundException>(async ()
                => await accommodationService.GetAccommodationAsync(1000));

            Assert.AreEqual("Accommodation with ID 1000 not found!", ex.Message);
        }

        [Test]
        public async Task GetAllAccommodationsAsync_ShouldReturn_CorrectAccommodationIfAnyoneLoggedInAndDatabaseHasAccommodations()
        {
            LoggedInAs = userCustomer2;

            await CreateDummyAccommodationAsync();

            var all = await accommodationService.GetAllAccommodationsAsync();

            Assert.AreEqual(4, all.Count);
        }

        [Test]
        public void GetAllAccommodationsAsync_ShouldFail_IfNoAccommodationsInDatabase()
        {
            LoggedInAs = userCustomer1;

            var ex = Assert.ThrowsAsync<NoneFoundInDatabaseTableException>(async ()
                => await accommodationService.GetAllAccommodationsAsync());

            Assert.AreEqual("No accommodations found!", ex.Message);
        }

        [Test]
        public async Task GetAllAccommodationsAsync_ShouldFail_IfNoOneLoggedIn()
        {
            LoggedInAs = null;

            await CreateDummyAccommodationAsync();

            var ex = Assert.ThrowsAsync<LoginException>(async ()
                => await accommodationService.GetAllAccommodationsAsync());

            Assert.AreEqual("Not logged in!", ex.Message);
        }

        [Test]
        public async Task GetAllOwnedAccommodationsAsync_ShouldReturn_AllTheAccommodationsOwnedByAUser()
        {
            LoggedInAs = userCustomer2;

            await CreateDummyAccommodationAsync();

            var all = await accommodationService.GetAllOwnedAccommodationsAsync(userCustomer2.Id);

            Assert.AreEqual(2, all.Count);
            Assert.AreEqual(userCustomer2.Id, all[0].Owner.Id);
            Assert.AreEqual(userCustomer2.ToString(), all[0].Owner.ToString());
            Assert.AreEqual(userCustomer2.Id, all[1].Owner.Id);
            Assert.AreEqual(userCustomer2.ToString(), all[1].Owner.ToString());
        }

        [Test]
        public async Task GetAllOwnedAccommodationsAsync_ShouldFail_IfTheUserHasNoAccommodations()
        {
            LoggedInAs = userAdmin;

            await CreateDummyAccommodationAsync();

            var ex = Assert.ThrowsAsync<NoneFoundInDatabaseTableException>(async ()
                => await accommodationService.GetAllOwnedAccommodationsAsync(1));

            Assert.AreEqual("User with Id 1 doesn't have any accommodations!", ex.Message);
        }

        [Test]
        public async Task UpdateAccommodationAsync_ShouldSucceed_IfChangingSquareMetersDescriptionPricePerNightOrCancellationDeadlineInDays()
        {
            await CreateDummyAccommodationAsync();

            LoggedInAs = accommodation1.Owner;

            Accommodation upd = new Accommodation { SquareMeters = 1, Description = "new", PricePerNight = 1, CancellationDeadlineInDays = 11 };

            Assert.AreNotEqual(accommodation1.SquareMeters, upd.SquareMeters);
            Assert.AreNotEqual(accommodation1.Description, upd.Description);
            Assert.AreNotEqual(accommodation1.PricePerNight, upd.PricePerNight);
            Assert.AreNotEqual(accommodation1.CancellationDeadlineInDays, upd.CancellationDeadlineInDays);

            await accommodationService.UpdateAccommodationAsync(accommodation1.Id, upd);

            Assert.AreEqual(accommodation1.SquareMeters, upd.SquareMeters);
            Assert.AreEqual(accommodation1.Description, upd.Description);
            Assert.AreEqual(accommodation1.PricePerNight, upd.PricePerNight);
            Assert.AreEqual(accommodation1.CancellationDeadlineInDays, upd.CancellationDeadlineInDays);
        }

        [Test]
        public async Task UpdateAccommodationAsync_ShouldFail_IfNotDoneByACustomerThatDoesNotOwnTheAccommodation()
        {
            await CreateDummyAccommodationAsync();

            LoggedInAs = userCustomer2;

            Accommodation upd = new Accommodation { SquareMeters = 1, Description = "new", PricePerNight = 1, CancellationDeadlineInDays = 11 };

            var ex = Assert.ThrowsAsync<AccessException>(async ()
                => await accommodationService.UpdateAccommodationAsync(accommodation1.Id, upd));

            Assert.AreEqual($"Must be performed by a customer with ID {accommodation1.Owner.Id}, or by admin or an employee on behalf of a customer with ID {accommodation1.Owner.Id}!", ex.Message);
        }

        [Test]
        public async Task UpdateAccommodationAsync_ShouldSucceed_IfDoneByEmployeeOnBehalfOfTheOwner()
        {
            await CreateDummyAccommodationAsync();

            LoggedInAs = accommodation1.Owner;

            Accommodation upd = new Accommodation { SquareMeters = 1, Description = "new", PricePerNight = 1, CancellationDeadlineInDays = 11 };

            Assert.AreNotEqual(accommodation1.SquareMeters, upd.SquareMeters);
            Assert.AreNotEqual(accommodation1.Description, upd.Description);
            Assert.AreNotEqual(accommodation1.PricePerNight, upd.PricePerNight);
            Assert.AreNotEqual(accommodation1.CancellationDeadlineInDays, upd.CancellationDeadlineInDays);

            await accommodationService.UpdateAccommodationAsync(accommodation1.Id, upd);

            Assert.AreEqual(accommodation1.SquareMeters, upd.SquareMeters);
            Assert.AreEqual(accommodation1.Description, upd.Description);
            Assert.AreEqual(accommodation1.PricePerNight, upd.PricePerNight);
            Assert.AreEqual(accommodation1.CancellationDeadlineInDays, upd.CancellationDeadlineInDays);
        }

        [Test]
        public async Task UpdateAccommodationAsync_ShouldSucceed_IfDoneByAdminOnBehalfOfTheOwner()
        {
            LoggedInAs = userAdmin;

            await CreateDummyAccommodationAsync();

            Accommodation upd = new Accommodation { SquareMeters = 1, Description = "new", PricePerNight = 1, CancellationDeadlineInDays = 11 };

            Assert.AreNotEqual(accommodation1.SquareMeters, upd.SquareMeters);
            Assert.AreNotEqual(accommodation1.Description, upd.Description);
            Assert.AreNotEqual(accommodation1.PricePerNight, upd.PricePerNight);
            Assert.AreNotEqual(accommodation1.CancellationDeadlineInDays, upd.CancellationDeadlineInDays);

            await accommodationService.UpdateAccommodationAsync(accommodation1.Id, upd);

            Assert.AreEqual(accommodation1.SquareMeters, upd.SquareMeters);
            Assert.AreEqual(accommodation1.Description, upd.Description);
            Assert.AreEqual(accommodation1.PricePerNight, upd.PricePerNight);
            Assert.AreEqual(accommodation1.CancellationDeadlineInDays, upd.CancellationDeadlineInDays);
        }

        [Test]
        public async Task ExpandScheduleOfAccommodationWithXAmountOfDaysAsync_ShouldSucceed()
        {
            await CreateDummyAccommodationAsync();

            LoggedInAs = accommodation1.Owner;

            int exandByDays = 10;

            int newSize = accommodation1.Schedule.Count + exandByDays;

            Assert.AreNotEqual(accommodation1.Schedule.Count, newSize);

            await accommodationService.ExpandScheduleOfAccommodationWithXAmountOfDaysAsync(accommodation1.Id, exandByDays);

            Assert.AreEqual(accommodation1.Schedule.Count, newSize);
        }

        [Test]
        public async Task ExpandScheduleOfAccommodationWithXAmountOfDaysAsync_ShouldFail_IfDoneByACustomerThatDoesNotOwnTheAccommodation()
        {
            await CreateDummyAccommodationAsync();

            LoggedInAs = accommodation3.Owner;

            var ex = Assert.ThrowsAsync<AccessException>(async ()
                => await accommodationService.ExpandScheduleOfAccommodationWithXAmountOfDaysAsync(accommodation1.Id, 10));

            Assert.AreEqual($"Must be performed by a customer with ID {accommodation1.Owner.Id}, or by admin or an employee on behalf of a customer with ID {accommodation1.Owner.Id}!", ex.Message);
        }

        [Test]
        public async Task ExpandScheduleOfAccommodationWithXAmountOfDaysAsync_ShouldFail_IfDaysToExapndByIsLessThanOne()
        {
            await CreateDummyAccommodationAsync();

            LoggedInAs = accommodation1.Owner;

            var ex = Assert.ThrowsAsync<ParameterException>(async ()
                => await accommodationService.ExpandScheduleOfAccommodationWithXAmountOfDaysAsync(accommodation1.Id, 0));

            Assert.AreEqual("Days cannot be less than one!", ex.Message);
        }

        [Test]
        public async Task ExpandScheduleOfAccommodationWithXAmountOfDaysAsync_ShouldFail_IfNoAccommodationWithTheIdExist()
        {
            await CreateDummyAccommodationAsync();

            LoggedInAs = accommodation1.Owner;

            var ex = Assert.ThrowsAsync<IdNotFoundException>(async ()
                => await accommodationService.ExpandScheduleOfAccommodationWithXAmountOfDaysAsync(100, 10));

            Assert.AreEqual("Accommodation with ID 100 not found!", ex.Message);
        }

        [Test]
        public async Task FindAvailableAsync_ShouldReturn_AllAvailableAccommodationsInACityIfAvailable()
        {
            await CreateDummyAccommodationAsync();

            LoggedInAs = accommodation1.Owner;

            Assert.AreEqual(accommodation1.Address.City, accommodation2.Address.City);
            Assert.AreEqual(accommodation1.Address.Country, accommodation2.Address.Country);
            Assert.AreNotEqual(accommodation1.Address.City, accommodation3.Address.City);

            var av = await accommodationService.FindAvailableAsync(accommodation1.Address.Country, accommodation1.Address.City, DateTime.Today.ToString("yyyy-MM-dd"), 5);

            Assert.True(av.Contains(accommodation1));
            Assert.True(av.Contains(accommodation2));
            Assert.False(av.Contains(accommodation3));
        }

        [Test]
        public async Task FindAvailableAsync_ShouldFail_IfNoDatesAvailable()
        {
            await CreateDummyAccommodationAsync();

            await CreateDummyBookingAsync();

            LoggedInAs = accommodation1.Owner;

            var ex = Assert.ThrowsAsync<DateException>(async ()
                => await accommodationService.FindAvailableAsync(accommodation1.Address.Country, accommodation1.Address.City, DateTime.Today.AddDays(6).ToString("yyyy-MM-dd"), 15));

            Assert.AreEqual($"No available accommodations found in {accommodation1.Address.Country}, {accommodation1.Address.City} from {DateTime.Today.AddDays(6).ToString("yyyy-MM-dd")} for {15} nights", ex.Message);
        }

        [Test]
        public async Task SortListOfAccommodations_ShouldReturn_ListWithCorrectSorting()
        {
            await CreateDummyAccommodationAsync();

            LoggedInAs = accommodation1.Owner;

            var av = await accommodationService.FindAvailableAsync(accommodation1.Address.Country, accommodation1.Address.City, DateTime.Today.ToString("yyyy-MM-dd"), 5);

            Assert.True(accommodation1.SquareMeters < accommodation2.SquareMeters);

            Assert.AreEqual(accommodation1.Id, av[0].Id);

            Assert.AreEqual(accommodation2.Id, accommodationService.SortListOfAccommodations(av, "Size", "Descending")[0].Id);

            Assert.AreEqual(accommodation1.Id, accommodationService.SortListOfAccommodations(av, "Size", "Ascending")[0].Id);


            Assert.True(accommodation1.PricePerNight < accommodation2.PricePerNight);

            Assert.AreEqual(accommodation2.Id, accommodationService.SortListOfAccommodations(av, "Price", "Descending")[0].Id);

            Assert.AreEqual(accommodation1.Id, accommodationService.SortListOfAccommodations(av, "Price", "Ascending")[0].Id);


            Assert.True(accommodation1.KilometersFromCenter < accommodation2.KilometersFromCenter);

            Assert.AreEqual(accommodation2.Id, accommodationService.SortListOfAccommodations(av, "Distance", "Descending")[0].Id);

            Assert.AreEqual(accommodation1.Id, accommodationService.SortListOfAccommodations(av, "Distance", "Ascending")[0].Id);


            accommodation1.AmountOfRatings = 1;
            accommodation1.AverageRating = 2;
            accommodation2.AmountOfRatings = 1;
            accommodation2.AverageRating = 5;

            Assert.AreEqual(accommodation2.Id, accommodationService.SortListOfAccommodations(av, "Rating", "Descending")[0].Id);

            Assert.AreEqual(accommodation1.Id, accommodationService.SortListOfAccommodations(av, "Rating", "Ascending")[0].Id);
        }
    }
}