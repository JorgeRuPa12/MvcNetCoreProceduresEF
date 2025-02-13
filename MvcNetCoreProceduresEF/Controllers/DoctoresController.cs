using Microsoft.AspNetCore.Mvc;
using MvcNetCoreProceduresEF.Models;
using MvcNetCoreProceduresEF.Repositories;

namespace MvcNetCoreProceduresEF.Controllers
{
    public class DoctoresController : Controller
    {
        private RepositoryDoctores repo;
        public DoctoresController(RepositoryDoctores repo)
        {
            this.repo = repo;
        }
        public async Task<IActionResult> Index()
        {
            @ViewData["ESPECIALIDADES"] =  await this.repo.GetEspecialidadesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string especialidad, int salario)
        {
            @ViewData["ESPECIALIDADES"] =  await this.repo.GetEspecialidadesAsync();
            List<Doctor> doctores = await this.repo.GetDoctoresEspecialidad(especialidad, salario);
            return View(doctores);
        }
    }
}
