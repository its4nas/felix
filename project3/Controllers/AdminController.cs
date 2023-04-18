﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using project3.IRepository;
using project3.Models;
using project3.Views.Shared.Components.SearchBar;
using project3.ViewModels;

namespace project3.Controllers
{
    public class AdminController : Controller
    {
        private readonly IJobRepository _jobRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;

        public AdminController(IJobRepository jobRepository,ICompanyRepository companyRepository,
                               IApplicationRepository applicationRepository, IUserRepository userRepository,
                               IMessageRepository messageRepository)
        {
            _jobRepository = jobRepository;
            _companyRepository = companyRepository;
            _applicationRepository = applicationRepository;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        public async Task<IActionResult> Dashboard()
        {

            AdminViewModel adminViewModel = new AdminViewModel();
            adminViewModel.job = _jobRepository.GetAll();
            adminViewModel.company = _companyRepository.GetAll();
            adminViewModel.applications = _applicationRepository.GetAll();
            adminViewModel.messages = _messageRepository.GetAll();

            return View(adminViewModel);

        }
        private List<SelectListItem> GetPageSizes(int selectedPageSize = 10)
        {
            var pagesSizes = new List<SelectListItem>();
            if (selectedPageSize == 5)
            {
                pagesSizes.Add(new SelectListItem("5", "5", true));
            }
            else
            {
                pagesSizes.Add(new SelectListItem("5", "5"));
            }
            for (int lp = 10; lp <= 100; lp += 10)
            {
                if (lp == selectedPageSize)
                { pagesSizes.Add(new SelectListItem(lp.ToString(), lp.ToString(), true)); }
                else
                { pagesSizes.Add(new SelectListItem(lp.ToString(), lp.ToString())); }
            }

            return pagesSizes;
        }

        public IActionResult add_job()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> add_job(job job, IFormFile job_image)
        {
            var newFileName = "";
            if (job_image != null)
            {
                newFileName = Convert.ToString(Guid.NewGuid().ToString());
                var concatFileName = String.Concat(newFileName, ".png");
                var filePath = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot", "JobImages")).Root + $@"\{concatFileName}";

                using (FileStream fs = System.IO.File.Create(filePath))
                {
                    await job_image.CopyToAsync(fs);
                    fs.Flush();
                }
            }

            if (ModelState.IsValid)
            {
                job.job_image = String.Concat(newFileName, ".png");
                _jobRepository.Add(job);
                return RedirectToAction("Dashboard");
            }

            return View("add_job");
        }

        public IActionResult edit_job(int id)
        {
            return View( _jobRepository.GetById(id));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> edit_job(job job, IFormFile job_image)
        {
            var newFileName = "";
            if (job_image != null)
            {
                newFileName = Convert.ToString(Guid.NewGuid().ToString());
                var concatFileName = String.Concat(newFileName, ".png");
                var filePath = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot", "JobImages")).Root + $@"\{concatFileName}";

                using (FileStream fs = System.IO.File.Create(filePath))
                {
                    job_image.CopyToAsync(fs);
                    fs.Flush();
                }

                newFileName = String.Concat(newFileName, ".png");
            }
            else
            {
                newFileName = String.Concat(job.job_image);
            }

            job.job_image = String.Concat(newFileName);
            _jobRepository.Update(job);
            return RedirectToAction("Dashboard");

        }


        public IActionResult delete_all()
        {
            _applicationRepository.delete_all("deleteallapps");
            return RedirectToAction("Dashboard");
        }


        public IActionResult DeleteJob(int id)
        {
            return View( _jobRepository.GetById(id));

        }
        [HttpPost, ActionName("DeleteJob")]
        public async Task<IActionResult> delete_job(int id)
        {
            var jobDetails = _jobRepository.GetById(id);
            if (jobDetails != null)
            {
                var apps = _applicationRepository.GetByJobId(jobDetails.job_id);
                if (apps != null)
                {
                    ViewData["Msg"] = "Must delete applications on this job first.";
                    return View(jobDetails);
                }

            }
            _jobRepository.Delete(jobDetails);

            return RedirectToAction("job_list");

        }

        public async Task<IActionResult> DeleteCompany(int id)
        {
            return View(_companyRepository.GetById(id));
        }
        [HttpPost, ActionName("DeleteCompany")]
        public async Task<IActionResult> Delete_company(int id)
        {
            var companyDetails = _companyRepository.GetById(id);
            if (companyDetails != null)
            {
                var jobs = _jobRepository.GetByCompanyId(id);
                var apps = _applicationRepository.GetByCompanyId(id);
                if (jobs != null)
                {
                    ViewData["MSG"] = "Must delete company jobs first.";
                    return View(companyDetails);
                }
                if (apps != null)
                {
                    ViewData["MSG2"] = "Must delete company applications first.";
                    return View(companyDetails);
                }
            }
                _companyRepository.Delete(companyDetails);

            return RedirectToAction("company_list");

        }

        public async Task<IActionResult> job_list(string sortExpression = "", string SearchText = "", int pg = 1, int pageSize = 5, int FilterText = 0)
        {
            ViewData["SortPerName"] = "job_name";
            ViewData["SortPerId"] = "job_id";
            ViewData["SortPerCity"] = "city";

            SortOrder sortOrder;
            string sortproperty;

            switch (sortExpression.ToLower())
            {
                case "job_name_desc":
                    sortOrder = SortOrder.Descending;
                    sortproperty = "job_name";
                    ViewData["SortIconName"] = "ion ion-md-arrow-round-up";
                    ViewData["SortPerName"] = "job_name";
                    break;
                case "job_id":
                    sortOrder = SortOrder.Ascending;
                    sortproperty = "job_name";
                    ViewData["SortIconName"] = "ion ion-md-arrow-round-down";
                    ViewData["SortPerName"] = "job_name_desc";
                    break;
                case "job_id_desc":
                    sortOrder = SortOrder.Descending;
                    sortproperty = "job_id";
                    ViewData["SortIconId"] = "ion ion-md-arrow-round-up";
                    ViewData["SortPerId"] = "job_id";
                    break;
                case "city":
                    sortOrder = SortOrder.Ascending;
                    sortproperty = "city";
                    ViewData["SortIconCity"] = "ion ion-md-arrow-round-down";
                    ViewData["SortPerCity"] = "city_desc";
                    break;
                case "city_desc":
                    sortOrder = SortOrder.Descending;
                    sortproperty = "city";
                    ViewData["SortIconCity"] = "ion ion-md-arrow-round-up";
                    ViewData["SortPerCity"] = "city";
                    break;

                default:
                    sortOrder = SortOrder.Ascending;
                    sortproperty = "job_id";
                    ViewData["SortIconId"] = "ion ion-md-arrow-round-down";
                    ViewData["SortPerId"] = "job_id_desc";
                    break;
            }

            IEnumerable<job> jobs;
            if (SearchText != "" && SearchText != null)
            {
                jobs = _jobRepository.SearchByName(SearchText);
            }
            else
            {
                if (FilterText != 0 && FilterText != null)
                {
                    jobs = _jobRepository.SearchByCategory(FilterText);
                }
                else
                {
                    jobs = _jobRepository.GetAll(sortproperty, sortOrder);

                }

            }
            



            if (pg < 1)
                pg = 1;

            int recsCount = jobs.Count();
            //var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;

            List<job> data = jobs.Skip(recSkip).Take(pageSize).ToList();
            SPager SearchPager = new SPager(recsCount, pg, pageSize) { Action = "job_list", Controller = "Admin", SearchText = SearchText };
            ViewBag.SearchPager = SearchPager;

            //var data = jobs.Skip(recSkip).Take(pageSize).ToList();
            //this.ViewBag.Pager = pager;
            this.ViewBag.PageSizes = GetPageSizes(pageSize);


            return View(data);
        }


        public IActionResult add_company()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> add_company(company company, IFormFile company_image)
        {
            var newFileName = "";
            if (company_image != null)
            {
                newFileName = Convert.ToString(Guid.NewGuid().ToString());
                var concatFileName = String.Concat(newFileName, ".png");
                var filePath = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot", "JobImages")).Root + $@"\{concatFileName}";

                using (FileStream fs = System.IO.File.Create(filePath))
                {
                    await company_image.CopyToAsync(fs);
                    fs.Flush();
                }
            }

                company.company_image = String.Concat(newFileName, ".png");
                _companyRepository.Add(company);
                return RedirectToAction("Dashboard");

        }
        public IActionResult edit_company(int id)
        {
            return View(_companyRepository.GetById(id));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult edit_company(company company, IFormFile? company_image)
        {
            var newFileName = "";
            if (company_image != null)
            {
                newFileName = Convert.ToString(Guid.NewGuid().ToString());
                var concatFileName = String.Concat(newFileName, ".png");
                var filePath = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot", "JobImages")).Root + $@"\{concatFileName}";

                using (FileStream fs = System.IO.File.Create(filePath))
                {
                    company_image.CopyToAsync(fs);
                    fs.Flush();
                }

                newFileName = String.Concat(newFileName, ".png");
            }
            else
            {
                newFileName = String.Concat(company.company_image);
            }

            company.company_image = String.Concat(newFileName);
            _companyRepository.Update(company);
            return RedirectToAction("Dashboard");

        }

        public IActionResult company_list(string sortExpression = "", string SearchText = "", int pg = 1, int pageSize = 5)
        {
            ViewData["SortPerName"] = "job_name";
            ViewData["SortPerId"] = "job_id";
            ViewData["SortPerCity"] = "city";

            SortOrder sortOrder;
            string sortproperty;

            switch (sortExpression.ToLower())
            {
                case "job_name_desc":
                    sortOrder = SortOrder.Descending;
                    sortproperty = "job_name";
                    ViewData["SortIconName"] = "ion ion-md-arrow-round-up";
                    ViewData["SortPerName"] = "job_name";
                    break;
                case "job_id":
                    sortOrder = SortOrder.Ascending;
                    sortproperty = "job_id";
                    ViewData["SortIconId"] = "ion ion-md-arrow-round-down";
                    ViewData["SortPerId"] = "job_id_desc";
                    break;
                case "job_id_desc":
                    sortOrder = SortOrder.Descending;
                    sortproperty = "job_id";
                    ViewData["SortIconId"] = "ion ion-md-arrow-round-up";
                    ViewData["SortPerId"] = "job_id";
                    break;
                case "city":
                    sortOrder = SortOrder.Ascending;
                    sortproperty = "city";
                    ViewData["SortIconCity"] = "ion ion-md-arrow-round-down";
                    ViewData["SortPerCity"] = "city_desc";
                    break;
                case "city_desc":
                    sortOrder = SortOrder.Descending;
                    sortproperty = "city";
                    ViewData["SortIconCity"] = "ion ion-md-arrow-round-up";
                    ViewData["SortPerCity"] = "city";
                    break;

                default:
                    sortOrder = SortOrder.Ascending;
                    sortproperty = "job_name";
                    ViewData["SortIconName"] = "ion ion-md-arrow-round-down";
                    ViewData["SortPerName"] = "job_name_desc";
                    break;
            }

            IEnumerable<company> companies;
            if (SearchText != "" && SearchText != null)
            {
                companies = _companyRepository.SearchByName(SearchText);
            }
            else
            {
                companies = _companyRepository.GetAll(sortproperty, sortOrder);

            }



            if (pg < 1)
                pg = 1;

            int recsCount = companies.Count();
            int recSkip = (pg - 1) * pageSize;

            List<company> data = companies.Skip(recSkip).Take(pageSize).ToList();
            SPager SearchPager = new SPager(recsCount, pg, pageSize) { Action = "company_list", Controller = "Admin", SearchText = SearchText };
            ViewBag.SearchPager = SearchPager;

            this.ViewBag.PageSizes = GetPageSizes(pageSize);


            return View(data);
        }



        public async Task<IActionResult> application_list(DateTime dateTime, int pg = 1, int pageSize = 5)
        {
            

            IEnumerable<Application> app;
            if (dateTime != default)
            {
                app = _applicationRepository.SearchByDate(dateTime);
            }
            else
            {
                app = _applicationRepository.GetAll();
            }


            if (pg < 1)
                pg = 1;

            int recsCount = app.Count();
            int recSkip = (pg - 1) * pageSize;

            List<Application> data = app.Skip(recSkip).Take(pageSize).ToList();
            SPager SearchPager = new SPager(recsCount, pg, pageSize) { Action = "application_list", Controller = "Admin" };
            ViewBag.SearchPager = SearchPager;

            this.ViewBag.PageSizes = GetPageSizes(pageSize);


            return View(data);
        }

        public IActionResult Message()
        {
            var msg = _messageRepository.GetAll();
            return View(msg);
        }
        public IActionResult DeleteUser(int id)
        {
            return View(_userRepository.GetById(id));
        }
        [HttpPost, ActionName("DeleteUser")]
        public async Task<IActionResult> delete_user(int id)
        {
            var userDetails = _userRepository.GetById(id);
            if (userDetails != null)
            {
                var apps = _applicationRepository.GetByUserId(id);
                if(apps!= null)
                {
                    ViewData["MSG"] = "Must delete relevent user applications first.";
                    return View(userDetails);
                }
            }
                _userRepository.Delete(userDetails);

            return RedirectToAction("user_list");
        }

        public IActionResult user_rep(int id)
        {
            return View(_userRepository.GetById(id));
        }
        public IActionResult job_rep(int id)
        {
            return View(_jobRepository.GetById(id));
        }





        public IActionResult add_user()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> add_user(user user, IFormFile user_image)
        {
            if (user.user_email != null)
            {
                var newFileName = "";
                if (user_image != null)
                {
                    newFileName = Convert.ToString(Guid.NewGuid().ToString());
                    var concatFileName = String.Concat(newFileName, ".png");
                    var filePath = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(),
                        "wwwroot", "JobImages")).Root + $@"\{concatFileName}";

                    using (FileStream fs = System.IO.File.Create(filePath))
                    {
                        await user_image.CopyToAsync(fs);
                        fs.Flush();
                    }
                }

                user.user_image = String.Concat(newFileName, ".png");
                _userRepository.Add(user);
                return RedirectToAction("Dashboard");
            }
            ViewData["Message"] = "Email Cannot be Empty";
            return View();

        }

        public IActionResult edit_user(int id)
        {
            return View(_userRepository.GetById(id));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> edit_user(user user, IFormFile user_image)
        {
            var newFileName = "";
            if (user_image != null)
            {
                newFileName = Convert.ToString(Guid.NewGuid().ToString());
                var concatFileName = String.Concat(newFileName, ".png");
                var filePath = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot", "JobImages")).Root + $@"\{concatFileName}";

                using (FileStream fs = System.IO.File.Create(filePath))
                {
                    user_image.CopyToAsync(fs);
                    fs.Flush();
                }

                newFileName = String.Concat(newFileName, ".png");
            }
            else
            {
                newFileName = String.Concat(user.user_image);
            }

            
            user.user_image = String.Concat(newFileName);
            _userRepository.Update(user);
            return RedirectToAction("Dashboard");

        }
        public async Task<IActionResult> user_list(string SearchText = "", int pg = 1, int pageSize = 5)
        {
            IEnumerable<user> users;
            if (SearchText != "" && SearchText != null)
            {
                users = _userRepository.GetByName(SearchText);
            }
            else
            {
                users = await _userRepository.GetAll();

            }



            if (pg < 1)
                pg = 1;

            int recsCount = users.Count();
            int recSkip = (pg - 1) * pageSize;

            List<user> data = users.Skip(recSkip).Take(pageSize).ToList();
            SPager SearchPager = new SPager(recsCount, pg, pageSize) { Action = "user_list", Controller = "Admin", SearchText = SearchText };
            ViewBag.SearchPager = SearchPager;

            this.ViewBag.PageSizes = GetPageSizes(pageSize);


            return View(data);
        }


        public IActionResult DeleteApp(int id)
        {
            return View(_applicationRepository.GetById(id));
        }
        [HttpPost, ActionName("DeleteApp")]
        public async Task<IActionResult> delete_app(int id)
        {
            
            _applicationRepository.Delete(_applicationRepository.GetById(id));

            return RedirectToAction("application_list");
        }

    }
}
