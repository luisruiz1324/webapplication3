using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using TimeClock.Services;
using TimeClock.Domain;
using TimeClock.Controllers.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using TimeClock.Extensions;
using System.Security.Principal;
using System.Security.Claims;

namespace UnitTests.ControllerTests
{
    public class ShiftControllerTests : Controller
    {
        private List<Shift> shifts = new List<Shift>
        {
            new Shift
            {
                Id = 1,
                UserId = "abc123",
                TotalHours = 0,
                IsActive = true,
            },
            new Shift
            {
                Id = 2,
                UserId = "abc456",
                TotalHours = 0,
                IsActive = true,
            }
        };

        public ShiftControllerTests() {



        }

        [Fact]
        public async void GetAllWorkShifts_Test()
        {
            var _shiftService = new Mock<IShiftService>();
            _shiftService.Setup(x => x.GetShiftsAsync(It.IsAny<string>(), It.IsAny<bool?>())).Returns(Task.FromResult(shifts));
            ShiftController shiftController = new ShiftController(_shiftService.Object);
            var result = await shiftController.GetAll(null, null);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void GetAllWorkShiftsNoResults_Test()
        {
            var _shiftService = new Mock<IShiftService>();
            shifts = new List<Shift>();
            _shiftService.Setup(x => x.GetShiftsAsync(It.IsAny<string>(), It.IsAny<bool?>())).Returns(Task.FromResult(shifts));
            ShiftController shiftController = new ShiftController(_shiftService.Object);
            var result = await shiftController.GetAll(null, null);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async void GetWorkShiftsByUserId_Test()
        {
            var _shiftService = new Mock<IShiftService>();
            _shiftService.Setup(x => x.GetShiftsByUserIdAsync(It.IsAny<string>())).Returns(Task.FromResult(shifts));
            ShiftController shiftController = new ShiftController(_shiftService.Object);
            var identity = new GenericIdentity("test", "test");
            identity.AddClaim(new Claim("id", "abc123"));
            var contextUser = new ClaimsPrincipal(identity);
 
            shiftController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = contextUser }
            };

            var result = await shiftController.GetShiftsByUserId();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void GetWorkShiftsByUserIdNoShifts_Test()
        {
            var _shiftService = new Mock<IShiftService>();
            _shiftService.Setup(x => x.GetShiftsByUserIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new List<Shift>()));
            ShiftController shiftController = new ShiftController(_shiftService.Object);
            var identity = new GenericIdentity("test", "test");
            identity.AddClaim(new Claim("id", "unregisteredUser"));
            var contextUser = new ClaimsPrincipal(identity);

            shiftController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = contextUser }
            };

            var result = await shiftController.GetShiftsByUserId();
            Assert.IsType<NotFoundObjectResult>(result);
        }



    }
}
