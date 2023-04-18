﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using project3.IRepository;
using project3.Models;
using project3.Views.Shared.Components.SearchBar;
using Microsoft.AspNetCore.Mvc.Rendering;
using project3.Data;
using project3.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace project3.Controllers
{
    public class CompanyController : Controller
    {
        private readonly IJobRepository _jobRepository;
        private readonly FelixDbContext _context;
        private readonly IApplicationRepository _applicationRepository;
        private readonly ICompanyRepository _companyRepository;

        public CompanyController(IJobRepository jobRepository, FelixDbContext context, IApplicationRepository applicationRepository, ICompanyRepository companyRepository)
        {
            _jobRepository = jobRepository;
           _context = context;
            _applicationRepository = applicationRepository;
            _companyRepository = companyRepository;
        }

        public IActionResult Dashboard()
        {
            

            CompanyViewModel companyViewModel = new CompanyViewModel();
            companyViewModel.applications = _applicationRepository.GetAll();
            companyViewModel.jobs = _jobRepository.GetAll();

            return View(companyViewModel);
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
        public async Task<IActionResult> add_job(job job,IFormFile job_image)
        {
            var newFileName = "";
            if(job_image != null)
            {
                newFileName = Convert.ToString(Guid.NewGuid().ToString());
                var concatFileName = String.Concat(newFileName, ".png");
                var filePath = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot", "JobImages")).Root+ $@"\{concatFileName}";

                using (FileStream fs = System.IO.File.Create(filePath))
                {
                    await job_image.CopyToAsync(fs);
                    fs.Flush();
                }
            }
            else
            {
                newFileName = "default";
            }

            if (ModelState.IsValid)
            {
                job.job_image = String.Concat(newFileName, ".png");
                _jobRepository.Add(job);
                return RedirectToAction("Dashboard");
            }
            
            return View("add_job"); 
        }

        public  IActionResult edit_job(int id)
        {
            return View( _jobRepository.GetById(id));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult edit_job(job job, IFormFile ? job_image)
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

        public async Task<IActionResult> Delete(int id)
        {
            return View(  _jobRepository.GetById(id));
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> delete_job(int id)
        {
            var jobDetails =  _jobRepository.GetById(id);
            if (jobDetails != null)
                _jobRepository.Delete(jobDetails);
            
            return RedirectToAction("job_list");

        }


        public IActionResult delete_all()
        {
            _applicationRepository.delete_all("deleteallapps");
            return RedirectToAction("Dashboard");
        }


        public IActionResult addjob()
        {
            _jobRepository.add_job("addjob");
            return RedirectToAction("job_list");
        }



        public IActionResult company_login()
        {
            return View();
        }
        public IActionResult company_register()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> company_register(company company, IFormFile company_image)
        {
            var newFileName = "";
            if (company_image != null)
            {
                newFileName = Convert.ToString(Guid.NewGuid().ToString());
                var concatFileName = String.Concat(newFileName, ".png");
                var filePath = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot", "CompanyImages")).Root + $@"\{concatFileName}";

                using (FileStream fs = System.IO.File.Create(filePath))
                {
                    await company_image.CopyToAsync(fs);
                    fs.Flush();
                }
            }
            else
            {
                newFileName = "Tea.jpg";
            }
            ModelState["company_image"].ValidationState = ModelValidationState.Valid;

            if (ModelState.IsValid)
            {
                var CuserV = await _companyRepository.GetByEmailAsync(company.company_email);
                if (CuserV == null)
                {
                    company.company_image = newFileName;
                    _companyRepository.Add(company);
                    return RedirectToAction("CompanyLogin", "Access");
                }
                ViewData["ValidateMessage"] = "Sorry! - This Email is already in use!";
                return View();
            }

            return View();
        }
        public async Task<IActionResult> job_list(string sortExpression = "", string SearchText = "", int pg = 1, int pageSize = 10, int FilterText=0)
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

            IEnumerable<job> jobs;
            if (SearchText != "" && SearchText != null)
            {
                jobs =  _jobRepository.SearchByName(SearchText);
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
            SPager SearchPager = new SPager(recsCount, pg, pageSize) { Action = "application_list", Controller = "Company", SearchText = SearchText };
            ViewBag.SearchPager = SearchPager;

            //var data = jobs.Skip(recSkip).Take(pageSize).ToList();
            //this.ViewBag.Pager = pager;
            this.ViewBag.PageSizes = GetPageSizes(pageSize);


            return View(data);
        }



        public async Task<IActionResult> application_list(DateTime dateTime ,  int pg = 1, int pageSize = 10)
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
            //var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;

            List<Application> data = app.Skip(recSkip).Take(pageSize).ToList();
            SPager SearchPager = new SPager(recsCount, pg, pageSize) { Action = "application_list", Controller = "Company" };
            ViewBag.SearchPager = SearchPager;

            //var data = jobs.Skip(recSkip).Take(pageSize).ToList();
            //this.ViewBag.Pager = pager;
            this.ViewBag.PageSizes = GetPageSizes(pageSize);


            return View(data);
        }

        public async Task<IActionResult> LogOut()
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("CompanyLogin", "Access");
        }

        public IActionResult job_rep(int id)
        {
            return View(_jobRepository.GetById(id));
        }

        public IActionResult DeleteApp(int id)
        {
            return View(_applicationRepository.GetById(id));
        }
        [HttpPost, ActionName("DeleteApp")]

        public async Task<IActionResult> delete_app(int id)
        {
            var application = _applicationRepository.GetById(id);
            if (application != null)
                _applicationRepository.Delete(application);

            return RedirectToAction("application_list");
        }

    }
}
