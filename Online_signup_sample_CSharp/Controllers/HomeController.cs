/* This file is part of Sign-up Page Sample.

The Sign-up Page Sample is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

The Sign-up Page Sample is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with The Sign-up Page Sample.  If not, see <http://www.gnu.org/licenses/>. */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Online_signup_sample_CSharp.Models;

namespace Online_signup_sample_CSharp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Param()
        {
            // Receive parameters from index.cshtml.
            string firstName = String.Format(Request.Form["firstName"].ToString());
            string lastName = String.Format(Request.Form["lastName"].ToString());
            string mobileNum = String.Format(Request.Form["mobileNumber"].ToString());

            SubscriptionCommand command = new SubscriptionCommand();

            ViewBag.Message = command.Execute(firstName, lastName, mobileNum);

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
