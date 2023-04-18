using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using project3.Data;
using project3.IRepository;
using project3.Models;
using project3.ViewModels;
using project3.Views.Shared.Components.SearchBar;

namespace project3.Controllers
{
    public class UserController : Controller
    {
        private readonly IJobRepository _jobRepository;
        private readonly IUserRepository _userRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly FelixDbContext _flaxDbContext;
        public UserController(FelixDbContext felixDbContext,IJobRepository jobRepository, IUserRepository userRepository, IApplicationRepository applicationRepository, IMessageRepository messageRepository)
        {
            _flaxDbContext= felixDbContext;
            _jobRepository = jobRepository;
            _userRepository = userRepository;
            _applicationRepository = applicationRepository;
            _messageRepository = messageRepository;
            
        }

        public async Task<IActionResult> Index(string SearchText = "", int pg = 1, int pageSize = 3)
        {
            IEnumerable<job> jobs;
            if (SearchText != "" && SearchText != null)
            {
                jobs = _jobRepository.SearchByName(SearchText);
            }
            else
            {
                jobs = _jobRepository.GetAll();
            }




            if (pg < 1)
                pg = 1;

            int recsCount = jobs.Count();
            //var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;

            List<job> data = jobs.Skip(recSkip).Take(pageSize).ToList();
            SPager SearchPager = new SPager(recsCount, pg, pageSize) { Action = "index", Controller = "User", SearchText = SearchText };
            ViewBag.SearchPager = SearchPager;

            //var data = jobs.Skip(recSkip).Take(pageSize).ToList();
            //this.ViewBag.Pager = pager;
            this.ViewBag.PageSizes = GetPageSizes(pageSize);


            return View(data);
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
        public IActionResult about()
        {
            return View();
        }
        public IActionResult contact()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult contact(CreateMessageViewModel messageVewModel)
        {
            if (ModelState.IsValid)
            {
                Message message = new Message
                {
                    name = messageVewModel.name,
                    email = messageVewModel.email,
                    message = messageVewModel.message
                    
                };
                if (message.message != null && message.email !=null)
                {
                    _messageRepository.Add(message);
                    return RedirectToAction("Index");
                }
                ViewData["Msg"] = "Sorry! - You must enter an email and a message!";
                return View();
            }
            return View("contact");

        }
        public IActionResult privacy()
        {
            return View();
        }
        public async Task<IActionResult> UserDetails(string name)
        {
            user user = await _userRepository.GetByNameAsync(name);
            return View(user);
        }
        //public IActionResult user_login()
        //{
        //    return View();
        //}
        public IActionResult user_register()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> user_register(user user)
        {
            if (ModelState.IsValid)
            {
                var userV = await _userRepository.GetByEmailAsync(user.user_email);
                if (userV == null)
                {
                    _userRepository.Add(user);
                    return RedirectToAction("Login", "Access");
                }
                ViewData["ValidateMessage"] = "Sorry! - This Email is already in use!";
                return View();
            }

            return View();
        }
        public IActionResult JobDetails(int id)
        {
            ApplicationViewModel viewModel = new ApplicationViewModel();
            var job = _jobRepository.GetById(id);
            viewModel.job = job;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JobDetails(CreateApplicationModelView applicationViewModel, IFormFile CV)
        {
            var newFileName = "";
            if (CV != null)
            {
                newFileName = Convert.ToString(Guid.NewGuid().ToString());
                var concatFileName = String.Concat(newFileName, ".pdf");
                var filePath = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot", "CvPDF")).Root + $@"\{concatFileName}";

                using (FileStream fs = System.IO.File.Create(filePath))
                {
                    await CV.CopyToAsync(fs);
                    fs.Flush();
                }
            }

            ModelState["CV"].ValidationState = ModelValidationState.Valid;

            if (ModelState.IsValid)
            {
                var application = new Application
                {
                    job_id = applicationViewModel.job_id,
                    user_id = applicationViewModel.user_id,
                    company_id = applicationViewModel.company_id,
                    date_of_app = applicationViewModel.date_of_app,
                    CV =  String.Concat(newFileName, "pdf")

            };
                if (application.CV != null)
                {
                    _applicationRepository.Add(application);
                    return RedirectToAction("Index");
                }
                
                return RedirectToAction("JobDetails");
            }
            

            return RedirectToAction("JobDetails");
        }
        public async Task<IActionResult> LogOut()
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Access");
        }
    }
}
